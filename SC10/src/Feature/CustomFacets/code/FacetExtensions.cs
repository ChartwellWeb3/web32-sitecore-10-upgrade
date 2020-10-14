using Sitecore.XConnect.Collection.Model;

namespace Chartwell.Feature.CustomFacets
{
  public static class FacetExtensions
  {
    #region OOTB facets
    
    /// <summary>
    /// Retrieve PersonalInformation Title facet value
    /// </summary>
    /// <param name="facet"></param>
    /// <returns></returns>
    public static string GetTitleFacetValue(this PersonalInformation facet)
    {
      return facet.Title;
    }

    #endregion

    #region Custom facets
    /// <summary>
    /// Retrieve CustomExtension Subscribe facet value
    /// </summary>
    /// <param name="facet"></param>
    /// <returns></returns>
    public static string GetSubscribeFacetValue(this ContactFacetExtension facet)
    {
      return facet.Subscribe;
    }

    public static string GetUnSubscribeFacetValue(this ContactFacetExtension facet)
    {
      return facet.Unsubscribe;
    }
    public static string GetSourceFacetValue(this ContactFacetExtension facet)
    {
      return facet.Source;
    }
    public static string GetContactPageFacetValue(this ContactFacetExtension facet)
    {
      return facet.ContactPage;
    }
    public static string GetConsentFacetValue(this ContactFacetExtension facet)
    {
      return facet.Consent;
    }
    public static string GetSalesFacetValue(this ContactFacetExtension facet)
    {
      return facet.Sales;
    }
    public static string GetLanguageFacetValue(this ContactFacetExtension facet)
    {
      return facet.Language;
    }
    public static string GetTypeFacetValue(this ContactFacetExtension facet)
    {
      return facet.Type;
    }
    public static string GetTagFacetValue(this ContactFacetExtension facet)
    {
      return facet.Tag;
    }
    public static string GetPropertyFacetValue(this ContactFacetExtension facet)
    {
      return facet.Property;
    }
    public static string GetPhoneFacetValue(this ContactFacetExtension facet)
    {
      return facet.Phone;
    }
    #endregion
  }
}