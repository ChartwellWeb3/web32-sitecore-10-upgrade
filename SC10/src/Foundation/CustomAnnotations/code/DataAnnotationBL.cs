using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Chartwell.Foundation.CustomAnnotations
{
  public class DataAnnotationBL
  {

  }

  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  public class EmailAddressCheckAttribute : DataTypeAttribute, IClientValidatable
  {
    private static Regex _regex = new Regex(@"^[\w-\._\+%]+@(?:[\w-]+\.)+[\w]{2,6}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public EmailAddressCheckAttribute()
        : base(DataType.EmailAddress)
    {
      ErrorMessage = "{0}";
    }


    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    {
      yield return new ModelClientValidationRule
      {
        ValidationType = "email",
        ErrorMessage = FormatErrorMessage(Translate.Text("EmailInvalidError"))
      };
    }

    public override bool IsValid(object value)
    {
      if (value == null)
      {
        return true;
      }

      string valueAsString = value as string;

      return valueAsString != null && _regex.Match(valueAsString).Length > 0;
    }
  }

  public class LocalizedMaxLengthTextBoxAttribute : ValidationAttribute, IClientValidatable
  {
    public int MaxLength { get; set; }
    public LocalizedMaxLengthTextBoxAttribute()
    {
      if (MaxLength == 0)
        MaxLength = 50;
    }
    public override string FormatErrorMessage(string name)
    {
      return Translate.Text(base.FormatErrorMessage(name));
    }

    IEnumerable<ModelClientValidationRule> IClientValidatable.GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    {
      return new[] { new ModelClientValidationMaxLengthRule(Translate.Text("CharacterLimitFirstLastNameMsg"), MaxLength) };
    }
    public override bool IsValid(object value)
    {
      if (value == null)
      {
        return true;
      }

      string valueAsString = value as string;

      return valueAsString != null && valueAsString.Length > 0;
    }
  }

  public class LocalizedMaxLengthTextAreaAttribute : ValidationAttribute, IClientValidatable
  {
    public int MaxLength { get; set; }

    public override string FormatErrorMessage(string name)
    {
      return Translate.Text(base.FormatErrorMessage(name));
    }

    public LocalizedMaxLengthTextAreaAttribute()
    {
      if (MaxLength == 0)
        MaxLength = 1000;
    }

    IEnumerable<ModelClientValidationRule> IClientValidatable.GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    {
      return new[] { new ModelClientValidationMaxLengthRule(Translate.Text("CharacterLimitQuestionMsg"), MaxLength) };

    }

    public override bool IsValid(object value)
    {
      if (value == null)
      {
        return true;
      }

      string valueAsString = value as string;

      return valueAsString != null && valueAsString.Length > 0;
    }
  }

  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  public class SpecialCharAttribute : DataTypeAttribute, IClientValidatable
  {
    private static Regex _regex = new Regex(@"[^<>:/]*", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public SpecialCharAttribute()
        : base(DataType.Text) { }


    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    {
      return new[] { new ModelClientValidationRegexRule(Translate.Text("SpecialCharValidationMsg"), _regex.ToString()) };
    }

    public override bool IsValid(object value)
    {
      if (value == null)
      {
        return true;
      }

      string valueAsString = value as string;

      return valueAsString != null && _regex.Match(valueAsString).Length > 0;
    }
  }

  /// <summary>
  /// Overrides Required attribute to read and display validation error messages from the Sitecore dictionary
  /// </summary>
  public class LocalizedRequiredAttribute : RequiredAttribute, IClientValidatable
  {
    public override string FormatErrorMessage(string name)
    {
      return Translate.Text(base.FormatErrorMessage(name));
    }

    IEnumerable<ModelClientValidationRule> IClientValidatable.GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    {
      return new[] { new ModelClientValidationRequiredRule(Translate.Text(ErrorMessage)) };
    }
  }

  /// <summary>
  /// Overrides DisplayName attribute to read and display the column title from the Sitecore dictionary
  /// </summary>
  public class LocalizedDisplayNameAttribute : DisplayNameAttribute
  {
    private readonly string resourceName;
    public LocalizedDisplayNameAttribute(string resourceName) : base()
    {
      this.resourceName = resourceName;
    }
    public override string DisplayName
    {
      get
      {
        return Translate.Text(resourceName);
      }
    }
  }
}