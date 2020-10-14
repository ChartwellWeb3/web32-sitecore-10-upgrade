using Sitecore.DependencyInjection;
using Sitecore.ExperienceForms.Data;
using Sitecore.ExperienceForms.Mvc.Models.Validation;
using Sitecore.ExperienceForms.Mvc.Models.Validation.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Web.Mvc;
using Sitecore.ExperienceForms.Mvc.Models.Fields;
using System.ComponentModel.DataAnnotations;

namespace Chartwell.Feature.ValueProviders
{
  public class BaseValidation : ValidationElement<RegularExpressionParameters>
  {
    private IFormDataProvider _dataProvider;
    public static bool IsEmailExistCheck { get; set; }

    protected virtual IFormDataProvider FormDataProvider
    {
      get
      {
        IFormDataProvider formDataProvider = this._dataProvider;
        if (formDataProvider == null)
        {
          IFormDataProvider service = ServiceLocator.ServiceProvider.GetService<IFormDataProvider>();
          IFormDataProvider formDataProvider1 = service;
          this._dataProvider = service;
          formDataProvider = formDataProvider1;
        }
        return formDataProvider;
      }
    }

    public override IEnumerable<ModelClientValidationRule> ClientValidationRules
    {
      get
      {
        if (string.IsNullOrEmpty(this.RegularExpression))
        {
          yield break;
        }
      }
    }

    /// <summary>
    /// Form's ItemId - Set in validation item
    /// </summary>
    protected virtual string RegularExpression
    {
      get;
      set;
    }

    protected virtual string Title
    {
      get;
      set;
    }


    public BaseValidation(ValidationDataModel validationItem) : base(validationItem)
    {

    }
    public override void Initialize(object validationModel)
    {
      object regularExpression;
      base.Initialize(validationModel);
      StringInputViewModel stringInputViewModel = validationModel as StringInputViewModel;
      if (stringInputViewModel != null)
      {
        Title = stringInputViewModel.Title;
      }
      RegularExpressionParameters parameters = Parameters;
      if (parameters != null)
      {
        regularExpression = parameters.RegularExpression;
      }
      else
      {
        regularExpression = null;
      }
      if (regularExpression == null)
      {
        regularExpression = string.Empty;
      }
      RegularExpression = (string)regularExpression;
    }

    public bool IsEmailExist(object email, string regExp)
    {
      var formId = Guid.Parse(regExp);
      var data = FormDataProvider.GetEntries(formId, null, null);
      foreach (var item in data)
      {
        var emailValue = item.Fields.Where(x => x.FieldName == "Email").FirstOrDefault();
        if (emailValue != null && emailValue.Value.ToLower() == email.ToString().ToLower())
        {
          return true;
        }
      }
      return false;
    }

    public override ValidationResult Validate(object value)
    {
      var isExist = IsEmailExist(value, this.RegularExpression);
      if (isExist)
      {
        IsEmailExistCheck = true;
        return new ValidationResult(this.FormatMessage(new object[] { this.Title }));
      }
      IsEmailExistCheck = false;
      return ValidationResult.Success;
    }
  } 
  
}