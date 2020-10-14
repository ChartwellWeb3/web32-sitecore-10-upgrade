using Chartwell.Foundation.CustomAnnotations;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Sitecore.Data.Items;

namespace Chartwell.Foundation.Models
{
  public class RegionalContactModel
  {
    public string PropertyName { get; set; }

    public string PhoneNo { get; set; }

    [LocalizedDisplayName("FirstName")]
    [LocalizedRequired(ErrorMessage = "FirstNameValidationMsg")]
    [SpecialChar]
    [LocalizedMaxLengthTextBox]
    public string FirstName { get; set; }

    [LocalizedDisplayName("LastName")]
    [SpecialChar]
    [LocalizedMaxLengthTextBox]
    public string LastName { get; set; }

    [LocalizedDisplayName("PhoneNo")]
    [RegularExpression(@"^\(?([0-9\s]{3})\)?[-. ]?([0-9\s]{3})[-. ]?([0-9\s]{4})$")]
    public string ContactPhoneNo { get; set; }

    [LocalizedDisplayName("Email")]
    [LocalizedRequired(ErrorMessage = "EmailValidationMsg")]
    [EmailAddressCheck]
    public string EmailAddress { get; set; }

    [LocalizedDisplayName("ConsentToConnect")]
    [LocalizedRequired(ErrorMessage = "ConsentToConnectValidationMsg")]
    public bool? ConsentToConnect { get; set; }

    [LocalizedDisplayName("Question")]
    [LocalizedMaxLengthTextArea]
    [SpecialChar]
    public string Question { get; set; }

    public string ContactPropertyName { get; set; }

    public DateTime? VisitDate { get; set; }

    public string TimeOfDayForVisit { get; set; }

    public IEnumerable<SelectListItem> TimeOfDayOfVisitList { get; set; }

    public string PropertyPhoneNo { get; set; }

    public string City { get; set; }

    public bool SendEmailError { get; set; }

    public string EmailSubjectLine { get; set; }

    public string ItemLanguage { get; set; }

    public Language ContextLanguage { get; set; }

    public string NonContactUsFormName { get; set; }

    public string DisplayEmailFirstName { get; set; }

    public string DisplayEmailLastName { get; set; }

    public string DisplayEmailPhoneNo { get; set; }
    public string DisplayEmailId { get; set; }

    public string DisplayEmailQuestion { get; set; }

    public string DisplayEmailVisitDate { get; set; }

    public string DisplayEmailVisitTime { get; set; }

    public string DisplayEmailConsent { get; set; }

    public string ContactUsConfirmMsg1 { get; set; }

    public string ContactUsConfirmMsg2 { get; set; }

    public string PVConfirmationMsg { get; set; }

    public string YardiID { get; set; }

    public string ContactItemID { get; set; }

    public Item ContextItem { get; set; }

  }
}