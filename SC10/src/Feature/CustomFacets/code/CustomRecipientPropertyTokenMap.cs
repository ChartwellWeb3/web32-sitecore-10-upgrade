using Sitecore.Modules.EmailCampaign.Core.Personalization;
using Sitecore.XConnect.Collection.Model;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// Implementing and mapping EXM custom and OOTB insert tokens 
/// </summary>
namespace Chartwell.Feature.CustomFacets
{
  public class CustomRecipientPropertyTokenMap : DefaultRecipientPropertyTokenMap
  {
    protected static readonly MethodInfo GetTitleValue =
        typeof(FacetExtensions).GetMethod(nameof(FacetExtensions.GetTitleFacetValue), new[] { typeof(PersonalInformation) });
    
    protected static readonly MethodInfo GetSubscribeValue =
            typeof(FacetExtensions).GetMethod(nameof(FacetExtensions.GetSubscribeFacetValue), new[] { typeof(ContactFacetExtension) });

    protected static readonly MethodInfo GetUnSubscribeFacetValue =
            typeof(FacetExtensions).GetMethod(nameof(FacetExtensions.GetUnSubscribeFacetValue), new[] { typeof(ContactFacetExtension) });

    protected static readonly MethodInfo GetSourceFacetValue =
            typeof(FacetExtensions).GetMethod(nameof(FacetExtensions.GetSourceFacetValue), new[] { typeof(ContactFacetExtension) });

    protected static readonly MethodInfo GetContactPageFacetValue =
            typeof(FacetExtensions).GetMethod(nameof(FacetExtensions.GetContactPageFacetValue), new[] { typeof(ContactFacetExtension) });

    protected static readonly MethodInfo GetConsentFacetValue =
            typeof(FacetExtensions).GetMethod(nameof(FacetExtensions.GetConsentFacetValue), new[] { typeof(ContactFacetExtension) });

    protected static readonly MethodInfo GetSalesFacetValue =
            typeof(FacetExtensions).GetMethod(nameof(FacetExtensions.GetSalesFacetValue), new[] { typeof(ContactFacetExtension) });

    protected static readonly MethodInfo GetLanguageFacetValue =
            typeof(FacetExtensions).GetMethod(nameof(FacetExtensions.GetLanguageFacetValue), new[] { typeof(ContactFacetExtension) });

    protected static readonly MethodInfo GetTypeFacetValue =
            typeof(FacetExtensions).GetMethod(nameof(FacetExtensions.GetTypeFacetValue), new[] { typeof(ContactFacetExtension) });

    protected static readonly MethodInfo GetTagFacetValue =
            typeof(FacetExtensions).GetMethod(nameof(FacetExtensions.GetTagFacetValue), new[] { typeof(ContactFacetExtension) });

    protected static readonly MethodInfo GetPropertyFacetValue =
            typeof(FacetExtensions).GetMethod(nameof(FacetExtensions.GetPropertyFacetValue), new[] { typeof(ContactFacetExtension) });

    protected static readonly MethodInfo GetPhoneFacetValue =
            typeof(FacetExtensions).GetMethod(nameof(FacetExtensions.GetPhoneFacetValue), new[] { typeof(ContactFacetExtension) });

    public CustomRecipientPropertyTokenMap()
    {

    }
    static CustomRecipientPropertyTokenMap()
    { 
      if (TokenBindings == null)
      {
        TokenBindings = new Dictionary<Token, RecipientPropertyTokenBinding>();
      }

      var list = new List<RecipientPropertyTokenBinding>()
      {
        RecipientPropertyTokenBinding.Build<PersonalInformation>(new Token("title"), null, GetTitleValue),
        RecipientPropertyTokenBinding.Build<ContactFacetExtension>(new Token("property"), null, GetPropertyFacetValue),
        RecipientPropertyTokenBinding.Build<ContactFacetExtension>(new Token("language"), null, GetLanguageFacetValue),
        RecipientPropertyTokenBinding.Build<ContactFacetExtension>(new Token("sales"), null, GetSalesFacetValue),
        RecipientPropertyTokenBinding.Build<ContactFacetExtension>(new Token("source"), null, GetSourceFacetValue),
        RecipientPropertyTokenBinding.Build<ContactFacetExtension>(new Token("tag"), null, GetTagFacetValue),
        RecipientPropertyTokenBinding.Build<ContactFacetExtension>(new Token("unsubscribe"), null, GetUnSubscribeFacetValue),
        RecipientPropertyTokenBinding.Build<ContactFacetExtension>(new Token("contactPage"), null, GetContactPageFacetValue),
        RecipientPropertyTokenBinding.Build<ContactFacetExtension>(new Token("subscribe"), null, GetSubscribeValue),
        RecipientPropertyTokenBinding.Build<ContactFacetExtension>(new Token("consent"), null, GetConsentFacetValue),
        RecipientPropertyTokenBinding.Build<ContactFacetExtension>(new Token("type"), null, GetTypeFacetValue),
        RecipientPropertyTokenBinding.Build<ContactFacetExtension>(new Token("phonenumber"), null, GetPhoneFacetValue)
      };

      foreach (var token in list)
      {
        TokenBindings.Add(token.Token, token);
      }
    }
  }
}