using Chartwell.Foundation.Models;
using Chartwell.Foundation.utility;
using Sitecore.Data;
using Sitecore.Globalization;
using SitecoreOLP.OP;
using SitecoreOLP.OP.DA;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;

namespace Chartwell.Feature.RegionalContact.Controllers
{

  public class RegionalContactController : Controller
  {
    readonly string constring = ConfigurationManager.ConnectionStrings["SitecoreOLP"].ToString();
    readonly ChartwellUtiles util = new ChartwellUtiles();

    // GET: RegionalContact
    public ActionResult Index()
    {
      RegionalContactModel contact = new RegionalContactModel();

      TimeOfDayForVisit(contact);

      var ParentPropertyItemID = Sitecore.Context.Item.ID;
      var PropertyItem = util.PropertyDetails(ParentPropertyItemID).Where(x => x.Language == Sitecore.Context.Language.Name).FirstOrDefault().GetItem();

      contact.PropertyPhoneNo = PropertyItem.Fields["RegionalPhoneNumber"].Value;
      contact.ItemLanguage = PropertyItem.Language.Name;
      contact.ContactItemID = PropertyItem.ID.ToString();

      return View(contact);
    }

    private static void TimeOfDayForVisit(RegionalContactModel contact)
    {
      contact.TimeOfDayOfVisitList = new SelectList(new[]
{
            new SelectListItem { Text = Translate.Text("AnyTime"), Value = Translate.Text("AnyTime"), Selected =true },
            new SelectListItem { Text = Translate.Text("Morning"), Value = Translate.Text("Morning") },
            new SelectListItem { Text = Translate.Text("Afternoon"), Value = Translate.Text("Afternoon") },
            new SelectListItem { Text = Translate.Text("Evening"), Value = Translate.Text("Evening") }
        }, "Text", "Value");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]

    public ActionResult Index(RegionalContactModel contact)
    {
      var lang = Sitecore.Context.Language;

      var contextItem = util.PropertyDetails(new ID(Guid.Parse(contact.ContactItemID))).Where(x => x.Language == Sitecore.Context.Language.Name).FirstOrDefault().GetItem();

      contact.PropertyName = contextItem.Fields["Title"].Value; 
      contact.PropertyPhoneNo = contextItem.Fields["RegionalPhoneNumber"].Value;

      contact.EmailSubjectLine = contact.VisitDate.HasValue ? $"{Translate.Text("ContactUsPVEmailSubject")} {contact.PropertyName.ToTitleCase()}" :
                                                              $"{Translate.Text("ContactUsEmailSubject")} {contact.PropertyName.ToTitleCase()}";

      if (ModelState.IsValid)
      {

        PersonDT person = new PersonDT();
        try
        {
          person.FirstName = contact.FirstName.Trim();
          person.LastName = !string.IsNullOrEmpty(contact.LastName) ? contact.LastName.Trim() : string.Empty;
          person.PhoneFaxNumber = contact.PhoneNo = !string.IsNullOrEmpty(contact.ContactPhoneNo) ? contact.ContactPhoneNo.Trim() : string.Empty;
          person.EmailAddress = contact.EmailAddress.Trim();
          person.ContactMeForSubscription = Convert.ToBoolean(contact.ConsentToConnect);
          person.Questions = !string.IsNullOrEmpty(contact.Question) ? contact.Question.Trim() : string.Empty;
          person.YardiID = "22222";
          person.PropertyName = contact.PropertyName;
          person.NonContactUsFormName = contact.PropertyName;
          person.FormTypeName = Translate.Text("PropertyContactUs");
          person.PVDate = contact.VisitDate;
          person.PVTime = contact.VisitDate.HasValue ? contact.TimeOfDayForVisit : string.Empty;
          person.EmailSubjectLine = contact.EmailSubjectLine;
          person.ContactLanguage = contact.ItemLanguage == "en" ? "English" : "French";

          StringBuilder LogEntry = new StringBuilder();
          LogEntry.AppendLine("[INFO] : Contact Form Submitted successfully");
          LogEntry.AppendLine("Language : (" + contact.ItemLanguage + ") " + " Form Name : " + contact.PropertyName);
          LogEntry.AppendLine();
          ContactFormLog(LogEntry.ToString());

        }
        catch (Exception ex)
        {
          StringBuilder msg = new StringBuilder();
          msg.Append("Exception Type: ");
          msg.AppendLine(ex.GetType().ToString());
          msg.AppendLine("Exception: " + ex.Message);
          msg.AppendLine("Stack Trace: ");
          if (ex.StackTrace != null)
          {
            msg.AppendLine(ex.StackTrace);
            msg.AppendLine();
          }
          msg.AppendLine("Language : (" + contact.ItemLanguage + ") " + " Form Name : " + contact.PropertyName);
          ContactFormLog(msg.ToString());
          Response.Write("Internal Error");
          contact.SendEmailError = true;
          return PartialView("ContactMsg", contact);
        };

        PersonDA Person = new PersonDA(constring);
        Person.GeneralInsertContactUs(person);
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
        return PartialView("RegionalContactMsg", contact);
      }
      else
      {
        TimeOfDayForVisit(contact);
        return View(contact);
      }
    }

    private void SendMail(RegionalContactModel contact)
    {
      StringBuilder emailBody = new StringBuilder();

      string fromAddress = Translate.Text("ContactUsEmailFrom");
      string fromPassword = Translate.Text("ContactUsEmailPass");

      string toAddress = (!(contact.FirstName.Trim().ToLower() == "test")) ? Translate.Text("EmailCommonContactUs") : "patelshirin@gmail.com";

      PrepareEmail(contact, emailBody);

      #region Email send to C2C / Local
      if (!Request.Url.Host.Equals("chartwell.com"))
      {
        var fAddress = new MailAddress(Translate.Text("ContactUsEmailFrom"));
        var tAddress = new MailAddress(contact.EmailAddress);
        using (MailMessage C2CMM = new MailMessage(fAddress.Address, tAddress.Address))
        {
          C2CMM.Subject = contact.EmailSubjectLine;
          C2CMM.Body = emailBody.ToString();
          using (SmtpClient smtp = new SmtpClient())
          {
            smtp.Host = Translate.Text("LOCALSMTPHOSTNAME");
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = new NetworkCredential(fAddress.Address, fromPassword);
            smtp.Send(C2CMM);
          };
        }
      }
      else
      {
        using (MailMessage C2CMM = new MailMessage(fromAddress, toAddress))
        {
          C2CMM.Subject = contact.EmailSubjectLine;
          C2CMM.Body = emailBody.ToString();
          string pwd = Translate.Text("ContactUsEmailPass");
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
        var tAddress = new MailAddress(contact.EmailAddress);
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
            smtp.Credentials = new NetworkCredential(fAddress.Address, fromPassword);
            smtp.Send(EndUserMM);
          };
        }
      }
      else
      {
        using (MailMessage EndUserMM = new MailMessage(fromAddress, contact.EmailAddress.ToString()))
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

    private void ContactFormLog(string ContactFormMessage)
    {
      string dirName = Request.PhysicalApplicationPath + "App_Data\\ContactFormsLog\\" + DateTime.Now.ToString("MMM") + DateTime.Now.Year.ToString();
      if (!Directory.Exists(dirName))
      {
        Directory.CreateDirectory(dirName);
      }

      string logFile = "~/App_Data/ContactFormsLog/" + DateTime.Now.ToString("MMM")
                                                     + DateTime.Now.Year.ToString()
                                                     + "/"
                                                     + "ContactFormsLogEntries - "
                                                     + DateTime.Now.ToShortDateString().Replace("/", "-") + ".txt";

      logFile = HttpContext.Server.MapPath(logFile);

      // Open the log file for append and write the log  
      StreamWriter sw = new StreamWriter(logFile, true);
      sw.WriteLine("********** {0} **********", DateTime.Now);
      sw.Write(ContactFormMessage);

      sw.Close();
    }

    private static void PrepareEmail(RegionalContactModel contact, StringBuilder body1)
    {
      body1.Append(Translate.Text("FirstName") + ": " + contact.FirstName + Environment.NewLine);
      body1.Append(Translate.Text("LastName") + ": " + contact.LastName + Environment.NewLine);
      body1.Append(Translate.Text("PhoneNo") + ": " + contact.PhoneNo + Environment.NewLine);
      body1.Append(Translate.Text("Email") + ": " + contact.EmailAddress + Environment.NewLine);
      body1.Append(Translate.Text("Question") + ": " + contact.Question + Environment.NewLine);
      string Consenttxt = Convert.ToBoolean(contact.ConsentToConnect) ? (contact.ItemLanguage == "en" ? "Yes" : "Oui") : (contact.ItemLanguage == "fr" ? "Non" : "No");
      body1.Append(Translate.Text("ConsentToConnect") + ": " + Consenttxt + Environment.NewLine);

      if (contact.VisitDate.HasValue)
      {
        body1.Append(Translate.Text("PreferredDateForPV") + ": " + contact.VisitDate.Value.ToShortDateString() + Environment.NewLine);
        body1.Append(Translate.Text("Tour") + ": " + contact.TimeOfDayForVisit);
      }
    }
  }
}