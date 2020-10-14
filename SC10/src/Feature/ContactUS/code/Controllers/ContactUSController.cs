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

namespace Chartwell.Feature.ContactUs.Controllers
{

  public class ContactUSController : Controller
  {
    readonly string constring = ConfigurationManager.ConnectionStrings["SitecoreOLP"].ToString();
    readonly ChartwellUtiles util = new ChartwellUtiles();

    // GET: ContactUS
    public ActionResult Index()
    {
      ContactUsModel contact = new ContactUsModel();
      TimeOfDayOfVisitDropDown(contact);
      //PrepareLabelsForEmail(contact);
      ID ParentPropertyItemID = Context.Item.ID;
      PopulateContactUsModelFromContext(contact, ParentPropertyItemID);
      return View(contact);
    }

    private void PopulateContactUsModelFromContext(ContactUsModel contact, ID ParentPropertyItemID)
    {
      Item PropertyItem = (from x in util.PropertyDetails(ParentPropertyItemID)
                           where x.Language == Context.Language.Name
                           select x).FirstOrDefault().GetItem();
      Template templateType = TemplateManager.GetTemplate(PropertyItem);
      if (templateType.Name != "Standard template")
      {
        if (templateType.Name == "SplitterPage")
        {
          contact.PropertyPhoneNo = PropertyItem.Fields["SplitterPhone"].Value;
          contact.ContactItemID = PropertyItem.ID.ToString();

          string text3 = contact.PropertyName = (contact.NonContactUsFormName = PropertyItem.Name.ToTitleCase());
          List<SearchResultItem> propertyDetailsResults = util.GetYardiForCommunity(PropertyItem.DisplayName);
          List<ContactUsModel> propertyDetails = (from c in propertyDetailsResults
                                                  select new ContactUsModel
                                                  {
                                                    YardiID = c.GetItem().Fields["Property ID"].Value,
                                                    PropertyName = c.GetItem().Fields["Property Name"].Value
                                                  }).ToList();
          contact.YardiID = propertyDetails[0].YardiID;
          contact.PropertyType = "PropertyContactUsFormSubmit";
          contact.ItemLanguage = PropertyItem.Language.Name.ToString();
        }
        else
        {
          Item contactUsPropertyItemPath = (from x in util.PropertyDetails(ParentPropertyItemID)
                                            where x.Language == PropertyItem.Language.Name
                                            select x).FirstOrDefault().GetItem().Parent;
          contact.PropertyPhoneNo = util.GetPhoneNumber(contactUsPropertyItemPath);
          int propertyID = System.Convert.ToInt32(contactUsPropertyItemPath.Fields["Property ID"].Value);
          contact.YardiID = contactUsPropertyItemPath.Fields["Property ID"].Value;
          contact.ContactItemID = contactUsPropertyItemPath.ID.ToString();

          string text3 = contact.PropertyName = (contact.NonContactUsFormName = PropertyItem.Parent.Name.ToTitleCase());
          string strPropertyType = (from x in util.PropertyDetails(new ID(contactUsPropertyItemPath.Fields["property type"].ToString()))
                                    where x.Language == PropertyItem.Language.Name
                                    select x).FirstOrDefault().GetItem().Name;
          if (strPropertyType.Equals("LTC"))
          {
            contact.PropertyType = "LTC_ContactFormSubmit";
          }
          else
          {
            contact.PropertyType = "PropertyContactUsFormSubmit";
          }
          contact.ItemLanguage = contactUsPropertyItemPath.Language.Name.ToString();
          contact.ChartwellEmail = util.GetEmail(contactUsPropertyItemPath);
          contact.ContextLanguage = contactUsPropertyItemPath.Language;
        }
      }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Index(ContactUsModel contact)
    {
      string emailSubject2 = string.Empty;

      var ItemTemplateName = util.PropertyDetails(new ID(contact.ContactItemID)).FirstOrDefault().GetItem().TemplateName;

      contact.PropertyName = (ItemTemplateName == "SplitterPage") ? util.PropertyDetails(new ID(contact.ContactItemID)).FirstOrDefault().GetItem().Fields["Title"].Value.ToLower() :
                                                                    util.PropertyDetails(new ID(contact.ContactItemID)).FirstOrDefault().GetItem().Fields["Property Name"].Value.ToLower();
      contact.ChartwellEmail = util.GetEmail(util.PropertyDetails(new ID(contact.ContactItemID)).FirstOrDefault().GetItem());

      
      if (ItemTemplateName == "SplitterPage")
      {
        contact.YardiID = util.GetYardiForCommunity(contact.PropertyName)[0].GetItem().Fields["Property ID"].Value;
      }
      else
      {
        contact.YardiID = util.PropertyDetails(new ID(contact.ContactItemID)).FirstOrDefault().GetItem().Fields["Property ID"].Value;
      }

      if (string.IsNullOrEmpty(contact.PropertyName))
      {
        contact.PropertyName = contact.NonContactUsFormName;
      }

      var ModelContact = util.PropertyDetails(new ID(contact.ContactItemID)).Where(x => x.Language == Context.Language.Name).FirstOrDefault();
      contact.PropertyName = ModelContact.GetItem().Name;

      emailSubject2 = (!contact.VisitDate.HasValue) ? Translate.Text("ContactUsEmailSubject") + " " + contact.PropertyName.ToTitleCase() : 
                                                      Translate.Text("ContactUsPVEmailSubject") + " " + contact.PropertyName.ToTitleCase();

      if (base.ModelState.IsValid)
      {
        PersonDT person = new PersonDT();
        try
        {
          person.FirstName = contact.FirstName.Trim();
          person.LastName = ((!string.IsNullOrEmpty(contact.LastName)) ? contact.LastName.Trim() : string.Empty);
          person.PhoneNumber = (!string.IsNullOrEmpty(contact.ContactPhoneNo)) ? contact.ContactPhoneNo.Trim() : string.Empty;
          person.EmailAddress = contact.EmailAddress.Trim();
          person.ContactMeForSubscription = System.Convert.ToBoolean(contact.ConsentToConnect);
          person.Questions = ((!string.IsNullOrEmpty(contact.Question)) ? contact.Question.Trim() : string.Empty);
          person.YardiID = contact.YardiID;
          person.PropertyName = contact.PropertyName;
          person.PVDate = contact.VisitDate;
          person.PVTime = (contact.VisitDate.HasValue ? contact.TimeOfDayForVisit : string.Empty);
          person.EmailSubjectLine = emailSubject2;
          person.ContactLanguage = ((contact.ItemLanguage == "en") ? "English" : "French");
          StringBuilder LogEntry = new StringBuilder();
          LogEntry.Append("[INFO] : Contact Form Submitted successfully");
          LogEntry.AppendLine("Language : (" + contact.ItemLanguage + ")  Form Name : " + contact.PropertyName);
          ContactFormLog(LogEntry.ToString());
        }
        catch (Exception ex2)
        {
          StringBuilder msg5 = new StringBuilder();
          msg5.Append("Exception Type: ");
          msg5.AppendLine(ex2.GetType().ToString());
          msg5.AppendLine("Exception: " + ex2.Message);
          msg5.AppendLine("Stack Trace: ");
          if (ex2.StackTrace != null)
          {
            msg5.AppendLine(ex2.StackTrace);
          }
          msg5.AppendLine("Language : (" + contact.ItemLanguage + ")  Form Name : " + contact.PropertyName);
          ContactFormLog(msg5.ToString());
          base.Response.Write("Internal Error");
          contact.SendEmailError = true;
          return PartialView("ContactMsg", contact);
        }
        PersonDA Person = new PersonDA(constring);
        Person.InsertContactUs(person);
        try
        {
          SendMail(contact);
        }
        catch (SmtpException ex)
        {
          string msg4 = Translate.Text("EmailServerErrorMsg1") + "<br>";
          msg4 += Translate.Text("EmailServerErrorRefresh1");
          msg4 = msg4 + " <a href=" + base.Request.UrlReferrer + ">" + Translate.Text("EmailServerErrorRefresh2") + "</a>";
          msg4 = msg4 + " " + Translate.Text("EmailServerErrorRefresh3");
          base.Response.Write(msg4);
          contact.SendEmailError = true;
          StringBuilder EmailServerErrorLogMsg = new StringBuilder();
          EmailServerErrorLogMsg.AppendLine("Email Server error");
          EmailServerErrorLogMsg.AppendLine(ex.Message);
          ContactFormLog(EmailServerErrorLogMsg.ToString());
        }
        contact.ContextLanguage = Context.Language;
        return PartialView("ContactMsg", contact);
      }
      TimeOfDayOfVisitDropDown(contact);
      return View(contact);
    }

    private void ContactFormLog(string ContactFormMessage)
    {
      string dirName = Request.PhysicalApplicationPath + "App_Data\\ContactFormsLog\\" + DateTime.Now.ToString("MMM") + DateTime.Now.Year.ToString();
      if (!Directory.Exists(dirName))
      {
        Directory.CreateDirectory(dirName);
      }
      string logFile2 = "~/App_Data/ContactFormsLog/" + DateTime.Now.ToString("MMM") + DateTime.Now.Year.ToString() + "/ContactFormsLogEntries - " + DateTime.Now.ToShortDateString().Replace("/", "-") + ".txt";
      logFile2 = base.HttpContext.Server.MapPath(logFile2);
      StreamWriter sw = new StreamWriter(logFile2, append: true);
      sw.WriteLine("********** {0} **********", DateTime.Now);
      sw.Write(ContactFormMessage);
      sw.Close();
    }

    private static void TimeOfDayOfVisitDropDown(ContactUsModel contact)
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

    private void SendMail(ContactUsModel contact)
    {
      StringBuilder emailBody = new StringBuilder();
      string fromAddress = Translate.Text("ContactUsEmailFrom");  
      string toAddress = (contact.FirstName.Trim().ToLower() == "test") ? contact.EmailAddress : ((!string.IsNullOrEmpty(contact.ChartwellEmail)) ? contact.ChartwellEmail : Translate.Text("EmailCommonContactUs")); // ConfigurationManager.AppSettings["EMAILCOMMONCONTACTUS"].ToString());
      string fromPassword = Translate.Text("ContactUsEmailPass"); 
      string subject = (!contact.VisitDate.HasValue) ? Translate.Text("ContactUsEmailSubject") + " " + contact.PropertyName.ToTitleCase() : 
                                                       Translate.Text("ContactUsPVEmailSubject") + " " + contact.PropertyName.ToTitleCase();

      PrepareEmail(contact, emailBody);

      #region Email send to C2C / Local
      if (!Request.Url.Host.Equals("chartwell.com"))
      {
        var fAddress = new MailAddress(Translate.Text("ContactUsEmailFrom")); 
        var tAddress = new MailAddress(contact.EmailAddress);
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
            smtp.Credentials = new NetworkCredential(fAddress.Address, pwd);
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

    private static void PrepareEmail(ContactUsModel contact, StringBuilder body1)
    {
      body1.Append(Translate.Text("FirstName") + ": " + contact.FirstName + Environment.NewLine);
      body1.Append(Translate.Text("LastName") + ": " + contact.LastName + Environment.NewLine);
      body1.Append(Translate.Text("PhoneNo") + ": " + contact.ContactPhoneNo + Environment.NewLine);
      body1.Append(Translate.Text("Email") + ": " + contact.EmailAddress + Environment.NewLine);
      body1.Append(Translate.Text("Questions") + ": " + contact.Question + Environment.NewLine);
      string Consenttxt = (!System.Convert.ToBoolean(contact.ConsentToConnect)) ? ((contact.ItemLanguage == "fr") ? "Non" : "No") : ((contact.ItemLanguage == "en") ? "Yes" : "Oui");
      body1.Append(Translate.Text("ConsentToConnect") + ": " + Consenttxt + Environment.NewLine);

      if (contact.VisitDate.HasValue)
      {
        body1.Append(Translate.Text("PreferredDateForPV") + ": " + contact.VisitDate.Value.ToShortDateString() + Environment.NewLine);
        body1.Append(Translate.Text("Tour") + ": " + contact.TimeOfDayForVisit);
      }

    }

    public ActionResult ContactMsg()
    {
      return PartialView();
    }
  }
}