using Sitecore.ExperienceForms.Mvc.Models.Validation;

namespace Chartwell.Feature.ValueProviders
{
  public class EmailExistValidation : BaseValidation
  {
    public EmailExistValidation(ValidationDataModel validationItem) : base(validationItem)
    {
    }
  }

  public class AlreadyRegisteredValidator : BaseValidation
  {
    public AlreadyRegisteredValidator(ValidationDataModel validationItem) : base(validationItem)
    {

    }
  }
}