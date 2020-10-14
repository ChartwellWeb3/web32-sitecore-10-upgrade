using Newtonsoft.Json;
using Sitecore.ExperienceForms.Models;
using Sitecore.ExperienceForms.Processing;
using Sitecore.ExperienceForms.Processing.Actions;
using System.Web;

namespace Chartwell.Feature.ValueProviders
{
  public class BaseSubmitValidation : SubmitActionBase<string>
  {
    protected readonly bool isEmailExist = EmailExistValidation.IsEmailExistCheck;
    /// <summary>
    /// Initializes a new instance of the <see cref="LogSubmit"/> class.
    /// </summary>
    /// <param name="submitActionData">The submit action data.</param>
    public BaseSubmitValidation(ISubmitActionData submitActionData) : base(submitActionData)
    {
    }

    /// <summary>
    /// Tries to convert the specified <paramref name="value" /> to an instance of the specified target type.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="target">The target object.</param>
    /// <returns>
    /// true if <paramref name="value" /> was converted successfully; otherwise, false.
    /// </returns>
    protected override bool TryParse(string value, out string target)
    {
      if (!string.IsNullOrEmpty(value))
      {
        target = JsonConvert.DeserializeObject<Parameters>(value).UserHasAccess;
        HttpContext.Current.Session["hasAccess"] = target;
      }
      target = string.Empty;
      return true;
    }
    
    /// <summary>
    /// Executes the action with the specified <paramref name="data" />.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="formSubmitContext">The form submit context.</param>
    /// <returns>
    ///   <c>true</c> if the action is executed correctly; otherwise <c>false</c>
    /// </returns>
    protected override bool Execute(string data, FormSubmitContext formSubmitContext)
    {
      if (isEmailExist)
      {
        return true;
      }
      else
      {
        return false;
      }
    }
  }
  public class Parameters
  {
    public string UserHasAccess { get; set; }
  }
}