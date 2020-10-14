using Chartwell.Foundation.Models;
using Chartwell.Foundation.utility;
using Sitecore;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Data.Templates;
using Sitecore.Globalization;
using SitecoreOLP.OP;
using SitecoreOLP.OP.DA;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;

namespace Chartwell.Feature.RegionalContact.Controllers
{
  public class GeneralContactController : Controller
  {
    private readonly string constring = ConfigurationManager.ConnectionStrings["SitecoreOLP"].ToString();
    readonly ChartwellUtiles chartwellUtiles = new ChartwellUtiles();

    public ActionResult Index()
    {
      GeneralContactModel generalContactModel = new GeneralContactModel();
      TimeOfDayOfVisitDropDown(generalContactModel);

      ID iD = Context.Item.ID;
      List<SearchResultItem> list = (from x in chartwellUtiles.PropertyDetails(iD)
                                     where x.Language == Context.Language.Name
                                     select x).ToList();
      Item item = list[0].GetItem();
      generalContactModel.ItemLanguage = item.Language.Name;
      generalContactModel.ContactItemID = item.ID.ToString();

      //PrepareLabelsForEmail(generalContactModel);
      return View(generalContactModel);
    }

    private string PropertyDetails(Item item)
    {
      Template template = TemplateManager.GetTemplate(item);
      string text2;
      if (template.Name != "Standard template" && template.Name != "SearchResultTemplate")
      {
        if (template.Name == "Static Pages")
        {
          text2 = item.DisplayName;
        }
        else if (!(template.Name == "Blog Post"))
        {
          text2 = item.DisplayName;
        }
        else
        {
          text2 = template.Name + " - " + item.DisplayName;
        }
      }
      else
      {
        char[] splitChars = new char[] { '?', '=' };

        var text6 = Request.UrlReferrer.Query.Split('&').FirstOrDefault().Split(splitChars).Where(x => !string.IsNullOrEmpty(x)).ToList();
        if (text6.Contains("propertyname"))
        {
          text2 = "Search Results for " + Request.QueryString[0].ToTitleCase();
        }
        else if (text6.Contains("postalcode"))
        {
          text2 = Translate.Text("RetirementHomesNear") + " " + Request.QueryString[0].ToUpper();
        }
        else
        {
          text2 = Translate.Text("Retirement Homes in and around") + " " + text6[1].ToTitleCase();
        }
      }
      return text2;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Index(GeneralContactModel contact)
    {
      if (base.ModelState.IsValid)
      {
        contact.YardiID = "22222";

        List<SearchResultItem> contextItem = (from x in chartwellUtiles.PropertyDetails(new ID(Guid.Parse(contact.ContactItemID)))
                                              where x.Language == Context.Language.Name
                                              select x).ToList();
        contact.ContextItem = contextItem.FirstOrDefault().GetItem();
        contact.EmailSubjectLine = contact.VisitDate.HasValue ? $"{Translate.Text("ContactUsPVEmailSubject")} {PropertyDetails(contact.ContextItem)}" :
                                                                $"{Translate.Text("ContactUsEmailSubject")} {PropertyDetails(contact.ContextItem)}";

        contact.PropertyName = PropertyDetails(contact.ContextItem);

        PersonDT personDT = new PersonDT();
        try
        {
          personDT.FirstName = contact.FirstName.Trim();
          personDT.LastName = !string.IsNullOrEmpty(contact.LastName) ? contact.LastName.Trim() : string.Empty;
          string text3 = personDT.PhoneFaxNumber = contact.PhoneNo = (!string.IsNullOrEmpty(contact.ContactPhoneNo)) ? contact.ContactPhoneNo.Trim() : string.Empty;
          personDT.EmailAddress = contact.EmailAddress.Trim();
          personDT.ContactMeForSubscription = System.Convert.ToBoolean(contact.ConsentToConnect);
          personDT.Questions = !string.IsNullOrEmpty(contact.Question) ? contact.Question.Trim() : string.Empty;
          personDT.YardiID = contact.YardiID;
          personDT.PropertyName = !string.IsNullOrEmpty(contact.PropertyName) ? contact.PropertyName.Trim().Replace("-", " ").ToTitleCase() : string.Empty;
          personDT.CityName = !string.IsNullOrEmpty(contact.ContactCity) ? contact.ContactCity : string.Empty;
          personDT.PVDate = contact.VisitDate;
          personDT.PVTime = contact.VisitDate.HasValue ? contact.TimeOfDayForVisit : string.Empty;
          personDT.EmailSubjectLine = contact.EmailSubjectLine.Replace("-", " ");
          personDT.ContactLanguage = contact.ItemLanguage == "en" ? "English" : "French";
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.AppendLine("[INFO] : Contact Form Submitted successfully");
          stringBuilder.AppendLine("Language : (" + contact.ItemLanguage + ")  Form Name : " + contact.PropertyName);
          stringBuilder.AppendLine();
          ContactFormLog(stringBuilder.ToString());
        }
        catch (Exception ex)
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
          stringBuilder2.AppendLine("Language : (" + contact.ItemLanguage + ")  Form Name : " + contact.NonContactUsFormName);
          ContactFormLog(stringBuilder2.ToString());
          base.Response.Write("Internal Error");
          contact.SendEmailError = true;
          return PartialView("ContactMsg", contact);
        }
        PersonDA personDA = new PersonDA(constring);
        personDA.GeneralInsertContactUs(personDT);
        try
        {
          SendMail(contact);
        }
        catch (SmtpException ex2)
        {
          string str = Translate.Text("EmailServerErrorMsg1") + "<br>";
          str = str + Translate.Text(ex2.Message.ToTitleCase().Replace(" ", "").Replace(".", "")) + "<br>";
          str += "<br>";
          str += Translate.Text("EmailServerErrorRefresh1");
          str = str + " <a href=" + base.Request.UrlReferrer + ">" + Translate.Text("EmailServerErrorRefresh2") + "</a>";
          str = str + " " + Translate.Text("EmailServerErrorRefresh3");
          base.Response.Write(str);
          contact.SendEmailError = true;
        }
        return PartialView("GeneralContactMsg", contact);
      }
      TimeOfDayOfVisitDropDown(contact);
      return View(contact);
    }

    private void ContactFormLog(string ContactFormMessage)
    {
      string text = Request.PhysicalApplicationPath + "App_Data\\ContactFormsLog\\" + DateTime.Now.ToString("MMM") + DateTime.Now.Year.ToString();
      if (!Directory.Exists(text))
      {
        Directory.CreateDirectory(text);
      }
      string path = "~/App_Data/ContactFormsLog/" + DateTime.Now.ToString("MMM") + DateTime.Now.Year.ToString() + "/ContactFormsLogEntries - " + DateTime.Now.ToShortDateString().Replace("/", "-") + ".txt";
      path = base.HttpContext.Server.MapPath(path);
      StreamWriter streamWriter = new StreamWriter(path, append: true);
      streamWriter.WriteLine("********** {0} **********", DateTime.Now);
      streamWriter.Write(ContactFormMessage);
      streamWriter.Close();
    }

    private void SendMail(GeneralContactModel contact)
    {
      StringBuilder stringBuilder = new StringBuilder();
      string text = Translate.Text("ContactUsEmailFrom");

      string empty = (!(contact.FirstName.Trim().ToLower() == "test")) ? Translate.Text("EmailCommonContactUs") : "patelshirin@gmail.com";
      string password = Translate.Text("ContactUsEmailPass");

      PrepareEmail(contact, stringBuilder);

      #region Email send to C2C / Local
      if (!Request.Url.Host.Equals("chartwell.com"))
      {
        var fAddress = new MailAddress(Translate.Text("ContactUsEmailFrom"));
        var tAddress = new MailAddress(contact.EmailAddress);
        string pwd = Translate.Text("ContactUsEmailPass");
        using (MailMessage C2CMM = new MailMessage(fAddress.Address, tAddress.Address))
        {
          C2CMM.Subject = contact.EmailSubjectLine.Replace("-", " ");
          C2CMM.Body = stringBuilder.ToString();
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
        using (MailMessage C2CMM = new MailMessage(text, empty))
        {
          C2CMM.Subject = contact.EmailSubjectLine.Replace("-", " ");
          C2CMM.Body = stringBuilder.ToString();
          using (SmtpClient smtp = new SmtpClient())
          {
            smtp.Host = Translate.Text("SMTPHOSTNAME");
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = new NetworkCredential(text, password);
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
          EndUserMM.Body = chartwellUtiles.GetEndUserMessageDetails().Fields["End User Email Content"].Value;
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
        using (MailMessage EndUserMM = new MailMessage(text, contact.EmailAddress.ToString()))
        {
          EndUserMM.Subject = Translate.Text("EndUserEmailSubject");
          EndUserMM.Body = chartwellUtiles.GetEndUserMessageDetails().Fields["End User Email Content"].Value;
          EndUserMM.IsBodyHtml = true;
          using (SmtpClient smtp = new SmtpClient())
          {
            smtp.Host = Translate.Text("SMTPHOSTNAME");
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = new NetworkCredential(text, password);
            smtp.Send(EndUserMM);
          };
        }
      }
      #endregion Email send to End User / Local
    }
    private static void PrepareEmail(GeneralContactModel contact, StringBuilder body1)
    {
      body1.Append(Translate.Text("FirstName") + ": " + contact.FirstName + Environment.NewLine);
      body1.Append(Translate.Text("LastName") + ": " + contact.LastName + Environment.NewLine);
      body1.Append(Translate.Text("PhoneNo") + ": " + contact.ContactPhoneNo + Environment.NewLine);
      body1.Append(Translate.Text("Email") + ": " + contact.EmailAddress + Environment.NewLine);
      body1.Append(Translate.Text("ContactCity") + ": " + contact.ContactCity + Environment.NewLine);
      body1.Append(Translate.Text("Question") + ": " + contact.Question + Environment.NewLine);
      string str = (!System.Convert.ToBoolean(contact.ConsentToConnect)) ? ((contact.ItemLanguage == "fr") ? "Non" : "No") : ((contact.ItemLanguage == "en") ? "Yes" : "Oui");
      body1.Append(Translate.Text("ConsentToConnect") + ": " + str + Environment.NewLine);
      if (contact.VisitDate.HasValue)
      {
        body1.Append(Translate.Text("PreferredDateForPV") + ": " + contact.VisitDate.Value.ToShortDateString() + Environment.NewLine);
        body1.Append(Translate.Text("Tour") + ": " + contact.TimeOfDayForVisit);
      }
    }

    private static void TimeOfDayOfVisitDropDown(GeneralContactModel contact)
    {
      contact.TimeOfDayOfVisitList = new SelectList(new SelectListItem[4]
      {
            new SelectListItem
            {
                Text = Translate.Text("AnyTime"),
                Value = Translate.Text("AnyTime"),
                Selected = true
            },
            new SelectListItem
            {
                Text = Translate.Text("Morning"),
                Value = Translate.Text("Morning")
            },
            new SelectListItem
            {
                Text = Translate.Text("Afternoon"),
                Value = Translate.Text("Afternoon")
            },
            new SelectListItem
            {
                Text = Translate.Text("Evening"),
                Value = Translate.Text("Evening")
            }
      }, "Text", "Value");
    }

    public ActionResult GeneralContactMsg()
    {
      return PartialView();
    }
  }
}
