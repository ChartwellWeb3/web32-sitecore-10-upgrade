using Chartwell.Foundation.CustomAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Sitecore.Data.Items;
using Sitecore.Data;

namespace Chartwell.Foundation.Models
{
  public class CorporateContactModel
  {
    public List<CorporateEnquirySubject> SubjectList { get; set; }

    public List<CorporateContactModel> PropertyList { get; set; }

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
    public string PhoneNo { get; set; }

    [LocalizedDisplayName("Email")]
    [LocalizedRequired(ErrorMessage = "EmailValidationMsg")]
    [EmailAddressCheck]
    public string EMailAddress { get; set; }

    [LocalizedDisplayName("Question")]
    [LocalizedMaxLengthTextArea]
    [SpecialChar]
    public string Questions { get; set; }

    public string PropertyID { get; set; }
    [LocalizedDisplayName("ResidenceOfInterest")]
    public string ResidenceOfInterest { get; set; }

    public string PropertyName { get; set; }

    [LocalizedDisplayName("Subject")]
    public string Subject { get; set; }

    [LocalizedDisplayName("CorpConsentToConnect")]
    [LocalizedRequired(ErrorMessage = "ConsentToConnectValidationMsg")]
    public bool? ConsentToConnect { get; set; }

    public string CorporateEnquirySubjectEmail { get; set; }

    public bool SendEmailError { get; set; }
    public string ItemLanguage { get; set; }

    public string DisplayEmailFirstName { get; set; }

    public string DisplayEmailLastName { get; set; }

    public string DisplayEmailPhoneNo { get; set; }
    public string DisplayEmailId { get; set; }

    public string DisplayEmailQuestion { get; set; }

    public string DisplayEmailConsent { get; set; }

    public string ContactUsConfirmMsg1 { get; set; }

    public string ContactUsConfirmMsg2 { get; set; }

    public string ContactUsConfirmMsg3 { get; set; }

    public string CorporateEnquirySubjectLine { get; set; }

    public string CorporateEnquirySubject { get; set; }

    public ID ContextItemID { get; set; }

    public Item ContextItem { get; set; }

  }
}
