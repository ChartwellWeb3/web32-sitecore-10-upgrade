using Chartwell.Foundation.Models;
using Chartwell.Foundation.utility;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data;
using Sitecore.Data.Managers;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;

namespace Chartwell.Feature.CorporateContact.Controllers
{
  public class CorporateContactController : Controller
  {
    string constring = ConfigurationManager.ConnectionStrings["SitecoreOLP"].ToString();
    readonly ChartwellUtiles util = new ChartwellUtiles();

    // GET: CorporateContact
    public ActionResult Index()
    {
      var t = Request.Url.Host;

      var PropertyItem = util.PropertyDetails(Sitecore.Context.Item.ID).Where(x => x.Language == Sitecore.Context.Language.Name).FirstOrDefault().GetItem();
      
      SqlDataReader reader = PopulateDropDownItems();

      CorporateContactModel corporateModel = new CorporateContactModel
      {
        ItemLanguage = PropertyItem.Language.Name,
        ContextItemID = Sitecore.Context.Item.ID
      };

      corporateModel.SubjectList = GetCorpSubjectDetails(reader, corporateModel, corporateModel.ItemLanguage).ToList();
      corporateModel.PropertyList = GetCorpPropertyDetails(corporateModel, corporateModel.ItemLanguage).OrderBy(o => o.ResidenceOfInterest).ToList();

      return PartialView(corporateModel);
    }

    private static List<CorporateContactModel> GetCorpPropertyDetails(CorporateContactModel corporateModel, string lang)
    {
      var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext();
      List<CorporateContactModel> propertyList = new List<CorporateContactModel>();

      var results = context.GetQueryable<SearchResultItem>()
           .Where(x => x.TemplateName == "PropertyPage")
           .Where(x => x.Name != "__Standard Values")
           .Where(x => x.Language == corporateModel.ItemLanguage)
           .OrderBy(o => o.Name).ToList();

      foreach (var property in results)
      {
        CorporateContactModel contactProp = new CorporateContactModel();

        contactProp.PropertyID = !string.IsNullOrEmpty(property.GetField("property id").Value) ?
        property.GetField("property id").Value : string.Empty;
        contactProp.ResidenceOfInterest = !string.IsNullOrEmpty(property.GetField("property name").Value) ?
        property.GetField("property name").Value : string.Empty;

        propertyList.Add(contactProp);
      }
      return propertyList.ToList();
    }

    private static List<CorporateEnquirySubject> GetCorpSubjectDetails(SqlDataReader reader, CorporateContactModel corporateModel, string lang)
    {
      List<CorporateEnquirySubject> subjectList = new List<CorporateEnquirySubject>();

      while (reader.Read())
      {
        CorporateEnquirySubject subject = new CorporateEnquirySubject
        {
          CorporateEnquirySubjectID = Convert.ToInt32(reader["CorporateEnquirySubjectID"]),
          CorporateEnquirySubjectName = corporateModel.ItemLanguage == "en" ? reader["CorporateEnquirySubjectName"].ToString() : reader["CorporateEnquirySubjectNameFr"].ToString(),
          CorporateEnquirySubjectEmailDistribution = reader["CorporateEnquirySubjectEmailDistribution"].ToString(),
        };
        subjectList.Add(subject);
      }
      return subjectList.ToList();
    }

    public ActionResult CorporateOfficeInfo()
    {
      return View();
    }

    public ActionResult SubmitMsg()
    {
      return PartialView();
    }
    public CorporateEnquirySubject GetCorpSubDetails(string SubjectID)
    {
      SqlConnection conn = new SqlConnection(constring);
      SqlCommand cmd = new SqlCommand();
      SqlDataReader reader;

      cmd.Parameters.AddWithValue("SubjectID", SubjectID);
      cmd.CommandText = @"Select * from CorporateEnquirySubject where CorporateEnquirySubjectID = @SubjectID";
      cmd.CommandType = CommandType.Text;
      cmd.Connection = conn;
      conn.Open();
      reader = cmd.ExecuteReader();

      var lang = Sitecore.Context.Language;
      CorporateEnquirySubject subject = new CorporateEnquirySubject();

      while (reader.Read())
      {
        {
          subject.CorporateEnquirySubjectName = lang.Name.ToString() == "en" ? reader["CorporateEnquirySubjectName"].ToString() : reader["CorporateEnquirySubjectNameFr"].ToString();
          subject.CorporateEnquirySubjectEmailDistribution = reader["CorporateEnquirySubjectEmailDistribution"].ToString();
        };
      }

      return subject;
    }

    /// <summary>
    /// WorkItem#35 - Replace SqlDataReader with ExecuteNonQuery() method
    /// Make use of the "Using" statements for command and connection objects. This
    /// ensure that the command and connection objects are closed after execution
    /// Remove code that is redundant/commented
    /// Add try/catch blocks
    /// </summary>
    /// <param name="contact"></param>
    /// <returns></returns>

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Index(CorporateContactModel contact)
    {
      if (ModelState.IsValid)
      {
        try
        {
          CorporateEnquirySubject details = GetCorpSubDetails(contact.Subject);
          contact.CorporateEnquirySubjectEmail = details.CorporateEnquirySubjectEmailDistribution;

          var propname = GetCorpPropertyDetails(contact, contact.ItemLanguage).Where(x => x.PropertyID == contact.ResidenceOfInterest).FirstOrDefault();
          contact.PropertyName = propname.ResidenceOfInterest;

          contact.CorporateEnquirySubjectLine = Translate.Text("CorporatEmailSubject") + " " + contact.PropertyName.ToTitleCase();
          contact.ContactUsConfirmMsg1 = Translate.Text("ContactUsConfirmMsg1");
          contact.ContactUsConfirmMsg2 = Translate.Text("ContactUsConfirmMsg2");
          contact.ContactUsConfirmMsg3 = Translate.Text("ContactUsConfirmMsg3");

          using (SqlConnection conn = new SqlConnection(constring))
          {
            using (SqlCommand cmd = new SqlCommand("sp_InsertCorporateEnquiry", conn))
            {
              cmd.CommandType = CommandType.StoredProcedure;
              cmd.Parameters.AddWithValue("@FirstName", contact.FirstName.Trim());
              cmd.Parameters.AddWithValue("@LastName", !string.IsNullOrEmpty(contact.LastName) ? contact.LastName.Trim() : string.Empty);
              cmd.Parameters.AddWithValue("@PhoneNo", !string.IsNullOrEmpty(contact.PhoneNo) ? contact.PhoneNo.Trim() : string.Empty);
              cmd.Parameters.AddWithValue("@EMailAddress", contact.EMailAddress.Trim());
              cmd.Parameters.AddWithValue("@Question", !string.IsNullOrEmpty(contact.Questions) ? contact.Questions.Trim() : string.Empty);
              cmd.Parameters.AddWithValue("@YardiID", Int32.Parse(contact.ResidenceOfInterest));
              cmd.Parameters.AddWithValue("@PropertyName", contact.PropertyName);
              cmd.Parameters.AddWithValue("@Consent", contact.ConsentToConnect);
              cmd.Parameters.AddWithValue("@ContactLanguage", contact.ItemLanguage == "en" ? "English" : "French");
              cmd.Parameters.AddWithValue("@EmailSubjectLine", contact.CorporateEnquirySubjectLine);
              cmd.Parameters.AddWithValue("@CorporateEnquirySubject", details.CorporateEnquirySubjectName);
              conn.Open();
              int retVal = cmd.ExecuteNonQuery();
            }
          }
          StringBuilder LogEntry = new StringBuilder();
          LogEntry.AppendLine("[INFO] : Corporate Enquiry Contact Form Submitted successfully");
          LogEntry.AppendLine("[INFO] : Language : (" + contact.ItemLanguage + ")  Form Name : " + contact.PropertyName);
          LogEntry.AppendLine("[INFO] : Corporate Enquiry Subject: " + details.CorporateEnquirySubjectName);
          ContactFormLog(LogEntry.ToString());
        }
        catch (SqlException ex)
        {
          StringBuilder stringBuilder2 = new StringBuilder();
          stringBuilder2.Append("Exception Type: ");
          stringBuilder2.AppendLine(ex.GetType().ToString());
          stringBuilder2.AppendLine("Exception: " + ex.Message);
          stringBuilder2.AppendLine("Stack Trace: ");
          if (ex.StackTrace != null)
          {
            stringBuilder2.AppendLine(ex.StackTrace);
            stringBuilder2.AppendLine();
          }
          stringBuilder2.AppendLine("Language : (" + contact.ItemLanguage + ")  Form Name : " + contact.PropertyName);
          ContactFormLog(stringBuilder2.ToString());
          base.Response.Write("Internal Error");
          contact.SendEmailError = true;
          return PartialView("SubmitMsg", contact);
        }

        try
        {
          SendMail(contact);
        }
        catch (SmtpException ex)
        {
          string msg = Translate.Text("EmailServerErrorMsg1") + "<br>";
          msg += Translate.Text(ex.Message.ToTitleCase().Replace(" ", "").Replace(".", "")) + "<br>";
          msg += "<br>";
          msg += Translate.Text("EmailServerErrorRefresh1");
          msg += " " + "<a href=" + Request.UrlReferrer + ">" + Translate.Text("EmailServerErrorRefresh2") + "</a>";
          msg += " " + Translate.Text("EmailServerErrorRefresh3");
          Response.Write(msg);
          contact.SendEmailError = true;
        }
        return PartialView("SubmitMsg", contact);
      }
      else
      {
        if (!Request.IsAjaxRequest())
        {
          SqlDataReader reader = PopulateDropDownItems();
          contact.SubjectList = GetCorpSubjectDetails(reader, contact).ToList();
          contact.PropertyList = GetCorpPropertyDetails(contact).OrderBy(o => o.ResidenceOfInterest).ToList();
          ID t = new ID("{B439ACB8-6B04-4683-AA55-5CC5FDE30DCE}");

          var ParentPropertyItemID = t;
          var ParentPropertyItem = util.PropertyDetails(ParentPropertyItemID).Where(x => x.Language == Sitecore.Context.Language.Name).ToList();
          var PropertyItem = ParentPropertyItem[0].GetItem();

          Sitecore.Context.Item = ItemManager.GetItem(PropertyItem.ID, PropertyItem.Language, Sitecore.Data.Version.Latest, Sitecore.Context.Database);

          return PartialView("Index", contact);
        }
        else
        {
          return View(contact);
        }
      }
    }

    private void SendMail(CorporateContactModel contact)
    {
      StringBuilder emailBody = new StringBuilder();

      var fromAddress = Translate.Text("CorporateEmailFrom");
      string toAddress = (!(contact.FirstName.Trim().ToLower() == "test")) ? contact.CorporateEnquirySubjectEmail : "patelshirin@gmail.com";

      string fromPassword = Translate.Text("CorporateEmailPassword");
      string subject = Translate.Text("CorporatEmailSubject") + " " + contact.PropertyName.ToTitleCase();

      PrepareEmail(contact, emailBody);

      #region Email send to C2C / Local
      if (!Request.Url.Host.Equals("chartwell.com"))
      {
        var fAddress = new MailAddress(Translate.Text("ContactUsEmailFrom"));
        var tAddress = new MailAddress(contact.EMailAddress);
        string pwd = Translate.Text("ContactUsEmailPass");
        using (MailMessage C2CMM = new MailMessage(fAddress.Address, tAddress.Address))
        {
          C2CMM.Subject = subject;
          C2CMM.Body = emailBody.ToString();
          using (SmtpClient smtp = new SmtpClient())
          {
            smtp.Host = Translate.Text("LOCALSMTPHOSTNAME");
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = new NetworkCredential(fAddress.Address, pwd);
            smtp.Send(C2CMM);
          };
        }
      }
      else
      {
        using (MailMessage C2CMM = new MailMessage(fromAddress, toAddress))
        {
          C2CMM.Subject = subject;
          C2CMM.Body = emailBody.ToString();
          using (SmtpClient smtp = new SmtpClient())
          {
            smtp.Host = Translate.Text("SMTPHOSTNAME");
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = new NetworkCredential(fromAddress, fromPassword);
            smtp.Send(C2CMM);
          };
        }
      }
      #endregion Email send to C2C / Local

      #region Email send to End User / Local
      if (!Request.Url.Host.Equals("chartwell.com"))
      {
        var fAddress = new MailAddress(Translate.Text("ContactUsEmailFrom"));
        var tAddress = new MailAddress(contact.EMailAddress);
        string pwd = Translate.Text("ContactUsEmailPass");
        using (MailMessage EndUserMM = new MailMessage(fAddress.Address, tAddress.Address))
        {
          EndUserMM.Subject = Translate.Text("EndUserEmailSubject");
          EndUserMM.Body = util.GetEndUserMessageDetails().Fields["End User Email Content"].Value;
          EndUserMM.IsBodyHtml = true;
          using (SmtpClient smtp = new SmtpClient())
          {
            smtp.Host = Translate.Text("LOCALSMTPHOSTNAME");
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = new NetworkCredential(fAddress.Address, pwd);
            smtp.Send(EndUserMM);
          };
        }
      }
      else
      {
        using (MailMessage EndUserMM = new MailMessage(fromAddress, contact.EMailAddress.ToString()))
        {
          EndUserMM.Subject = Translate.Text("EndUserEmailSubject");
          EndUserMM.Body = util.GetEndUserMessageDetails().Fields["End User Email Content"].Value;
          EndUserMM.IsBodyHtml = true;
          using (SmtpClient smtp = new SmtpClient())
          {
            smtp.Host = Translate.Text("SMTPHOSTNAME");
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = new NetworkCredential(fromAddress, fromPassword);
            smtp.Send(EndUserMM);
          };
        }
      }
      #endregion Email send to End User / Local
    }

    private static void PrepareEmail(CorporateContactModel contact, StringBuilder body1)
    {
      body1.Append(Translate.Text("FirstName") + ": " + contact.FirstName + Environment.NewLine);
      body1.Append(Translate.Text("LastName") + ": " + contact.LastName + Environment.NewLine);
      body1.Append(Translate.Text("PhoneNo") + ": " + contact.PhoneNo + Environment.NewLine);
      body1.Append(Translate.Text("Email") + ": " + contact.EMailAddress + Environment.NewLine);
      body1.Append(Translate.Text("Questions") + ": " + contact.Questions + Environment.NewLine);
      string Consenttxt = Convert.ToBoolean(contact.ConsentToConnect) ? (contact.ItemLanguage == "en" ? "Yes" : "Oui") : (contact.ItemLanguage == "fr" ? "Non" : "No");
      body1.Append(Translate.Text("ConsentToConnect") + ": " + Consenttxt + Environment.NewLine);
    }

    private SqlDataReader PopulateDropDownItems()
    {
      SqlConnection conn = new SqlConnection(constring);
      SqlCommand cmd = new SqlCommand();
      SqlDataReader reader;

      cmd.CommandText = @"Select * from CorporateEnquirySubject where CorporateEnquirySubjectID != 1";

      cmd.CommandType = CommandType.Text;

      cmd.Connection = conn;

      conn.Open();

      reader = cmd.ExecuteReader();
      return reader;
    }

    private List<CorporateContactModel> GetCorpPropertyDetails(CorporateContactModel corporateModel)
    {
      var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext();
      List<CorporateContactModel> propertyList = new List<CorporateContactModel>();

      var results = context.GetQueryable<SearchResultItem>()
           .Where(x => x.TemplateName == "PropertyPage")
           .Where(x => x.Name != "__Standard Values")
           .Where(x => x.Language == corporateModel.ItemLanguage)
           .OrderBy(o => o.Name).ToList();

      foreach (var property in results)
      {
        CorporateContactModel contactProp = new CorporateContactModel
        {
          PropertyID = !string.IsNullOrEmpty(property.GetField("property id").Value) ?
        property.GetField("property id").Value : string.Empty,
          ResidenceOfInterest = !string.IsNullOrEmpty(property.GetField("property name").Value) ?
        property.GetField("property name").Value : string.Empty
        };

        propertyList.Add(contactProp);
      }
      return propertyList.ToList();
    }

    private List<CorporateEnquirySubject> GetCorpSubjectDetails(SqlDataReader reader, CorporateContactModel corporateModel)
    {
      List<CorporateEnquirySubject> subjectList = new List<CorporateEnquirySubject>();

      while (reader.Read())
      {
        CorporateEnquirySubject subject = new CorporateEnquirySubject
        {
          CorporateEnquirySubjectID = Convert.ToInt32(reader["CorporateEnquirySubjectID"]),
          CorporateEnquirySubjectName = corporateModel.ItemLanguage == "en" ? reader["CorporateEnquirySubjectName"].ToString() : reader["CorporateEnquirySubjectNameFr"].ToString(),
          CorporateEnquirySubjectEmailDistribution = reader["CorporateEnquirySubjectEmailDistribution"].ToString(),
        };
        subjectList.Add(subject);
      }
      return subjectList.ToList();
    }

    public void ContactFormLog(string ContactFormMessage)
    {
      string text = Request.PhysicalApplicationPath + "App_Data\\ContactFormsLog\\" + DateTime.Now.ToString("MMM") + DateTime.Now.Year.ToString();
      if (!Directory.Exists(text))
      {
        Directory.CreateDirectory(text);
      }
      string path = "~/App_Data/ContactFormsLog/" + DateTime.Now.ToString("MMM") + DateTime.Now.Year.ToString() + "/ContactFormsLogEntries - " + DateTime.Now.ToShortDateString().Replace("/", "-") + ".txt";
      path = HttpContext.Server.MapPath(path);
      StreamWriter streamWriter = new StreamWriter(path, append: true);
      streamWriter.WriteLine();
      streamWriter.WriteLine("********** {0} **********", DateTime.Now);
      streamWriter.Write(ContactFormMessage);
      streamWriter.Close();
    }
  }
}