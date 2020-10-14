using Chartwell.Foundation.Models;
using Chartwell.Foundation.utility;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;

namespace Chartwell.Feature.QuestionnaireForm.Controllers
{
  public class QuestionnaireController : Controller
  {
    ChartwellUtiles _c;

    public QuestionnaireController()
    {
      _c = new ChartwellUtiles();
    }

    public ActionResult Index()
    {
      return View();
    }

    public bool SendMail(QuestionnaireModel contact)
    {
      try
      {
        var host = System.Web.HttpContext.Current.Request.Url.Host;

        var serverAddress = host.Equals("chartwell.com") || host.Equals("devqa.chartwell.com") ? new MailAddress(ConfigurationManager.AppSettings["ContactUsEmailFrom"].ToString()) :
                                                                                                new MailAddress("kevinchirayath@gmail.com");
        var pwd = host.Equals("chartwell.com") || host.Equals("devqa.chartwell.com") ? ConfigurationManager.AppSettings["ContactUsEmailPass"].ToString() : "ahokwsajilqhfldt";
        var smtpHost = host.Equals("chartwell.com") || host.Equals("devqa.chartwell.com") ? ConfigurationManager.AppSettings["SMTPHOSTNAME"].ToString() :
                                                                                           ConfigurationManager.AppSettings["LOCALSMTPHOSTNAME"].ToString();

        var emailItem = _c.GetEndUserMessageDetails(contact.FormName);

        var mainBody = _c.GetEmailMainBody(contact.Score, emailItem);
        var subject = contact.FirstName + emailItem.Fields["Subject"].Value;
        var contactMessage = contact.FirstName + emailItem.Fields["ContactMessage"].Value;
        var legalDisclaimer = emailItem.Fields["LegalDisclaimer"].Value.Replace("[email]", contact.FromEmail);

        using (MailMessage EndUserMM = new MailMessage())
        {
          EndUserMM.From = new MailAddress(contact.FromEmail);
          EndUserMM.To.Add(new MailAddress(contact.ToEmail));
          EndUserMM.Sender = new MailAddress(serverAddress.Address);

          EndUserMM.Subject = subject;
          EndUserMM.Body = contactMessage + "<br />" + mainBody + legalDisclaimer;
          EndUserMM.IsBodyHtml = true;

          using (SmtpClient smtp = new SmtpClient())
          {
            smtp.Host = smtpHost;
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = new NetworkCredential(serverAddress.Address, pwd);
            smtp.Send(EndUserMM);
          };
        }
        return true;
      }
      catch (SmtpFailedRecipientException)
      {
        return false;
      }
    }
  }
}