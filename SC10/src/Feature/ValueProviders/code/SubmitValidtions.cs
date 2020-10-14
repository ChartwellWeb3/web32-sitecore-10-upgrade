using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.ExperienceForms.Models;
using Sitecore.ExperienceForms.Processing;

namespace Chartwell.Feature.ValueProviders
{
  public class CustomSubmitValidation : BaseSubmitValidation
  {
    public CustomSubmitValidation(ISubmitActionData submitActionData) : base(submitActionData)
    {
    }
  }

  public class ProfSubmitValidation : BaseSubmitValidation
  {
    public ProfSubmitValidation(ISubmitActionData submitActionData) : base(submitActionData)
    {
    }

    protected override bool Execute(string data, FormSubmitContext formSubmitContext)
    {
      if (isEmailExist)
      {
        return false;
      }
      else
      {
        return true;
      }
    }
  }
}