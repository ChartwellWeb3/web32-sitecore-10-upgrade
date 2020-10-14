using Chartwell.Feature.QuestionnaireForm.Controllers;
using Chartwell.Foundation.Models;
using Sitecore.ExperienceForms.Models;
using Sitecore.ExperienceForms.Processing;
using Sitecore.ExperienceForms.Processing.Actions;
using System.Linq;

namespace Chartwell.Feature.ValueProviders.QuestionnaireForm
{
  public class QuestionnaireEmailSubmit : SubmitActionBase<string>
  {
    private readonly QuestionnaireController _questionnaireController;
    private bool _emailSent = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogSubmit"/> class.
    /// </summary>
    /// <param name="submitActionData">The submit action data.</param>
    public QuestionnaireEmailSubmit(ISubmitActionData submitActionData) : base(submitActionData)
    {
      _questionnaireController = new QuestionnaireController();
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
      if (formSubmitContext != null)
      {
        var language = Sitecore.Context.Language;
        var toEmail = formSubmitContext.Fields.FirstOrDefault(x => x.Name == "ToEmail");
        var fromEmail = formSubmitContext.Fields.FirstOrDefault(x => x.Name == "FromEmail");
        var score = formSubmitContext.Fields.FirstOrDefault(x => x.Name == "recommendationScore");
        var formName = formSubmitContext.Fields.FirstOrDefault(x => x.Name == "FormName");
        var message = formSubmitContext.Fields.FirstOrDefault(x => x.Name == "Message");

        var property = toEmail.GetType().GetProperty("Value");
        var messageValue = CheckPropertyValue(message);


        _emailSent = _questionnaireController.SendMail(
          new QuestionnaireModel
          {
            FromEmail = property.GetValue(fromEmail).ToString(),
            Message = messageValue,
            ToEmail = property.GetValue(toEmail).ToString(),
            Score = int.Parse(property.GetValue(score).ToString()),
            ItemLanguage = language.ToString(),
            FormName = property.GetValue(formName).ToString()
          });
      }

      if (_emailSent)
        return true;

      return false;
    }

    private static string CheckPropertyValue(IViewModel obj)
    {
      var valueField = obj.GetType().GetProperty("Value");
      if (valueField.GetValue(obj) != null)
        return valueField.GetValue(obj).ToString();

      return "";
    }
  }
}