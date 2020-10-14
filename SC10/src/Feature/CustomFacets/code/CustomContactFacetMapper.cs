using Sitecore.ListManagement.Import;
using Sitecore.ListManagement.XConnect.Web.Import;
using Sitecore.XConnect;

namespace Chartwell.Feature.CustomFacets
{
  /// <summary>
  /// Custom facet mapper for contactext facet
  /// </summary>
  public class CustomContactFacetMapper : IFacetMapper
  {
    public CustomContactFacetMapper()
      : this(ContactFacetExtension.DefaultFacetKey)
    {
    }
    public CustomContactFacetMapper(string facetName)
    {
      this.FacetName = facetName;
    }
    public string FacetName { get; }
    public MappingResult Map(
      string facetKey,
      Facet source,
      ContactMappingInfo mappings,
      string[] data)
    {
      if (facetKey != this.FacetName)
      {
        return new NoMatch(facetKey);
      }
      var cF = source as ContactFacetExtension ?? new ContactFacetExtension();
      var subscribe = mappings.GetValue(nameof(cF.Subscribe), data);
      var property = mappings.GetValue(nameof(cF.Property), data);
      var language = mappings.GetValue(nameof(cF.Language), data);
      var sales = mappings.GetValue(nameof(cF.Sales), data);
      var sourceField = mappings.GetValue(nameof(cF.Source), data);
      var tag = mappings.GetValue(nameof(cF.Tag), data);
      var unsubscribe = mappings.GetValue(nameof(cF.Unsubscribe), data);
      var contactPage = mappings.GetValue(nameof(cF.ContactPage), data);
      var consent = mappings.GetValue(nameof(cF.Consent), data);
      var type = mappings.GetValue(nameof(cF.Type), data);
      var phone = mappings.GetValue(nameof(cF.Phone), data);

      if (!string.IsNullOrWhiteSpace(property)) { cF.Property = property; }
      if (!string.IsNullOrWhiteSpace(subscribe)) { cF.Subscribe = subscribe; }
      if (!string.IsNullOrWhiteSpace(language)) { cF.Language = language; }
      if (!string.IsNullOrWhiteSpace(sales)) { cF.Sales = sales; }
      if (!string.IsNullOrWhiteSpace(sourceField)) { cF.Source = sourceField; }
      if (!string.IsNullOrWhiteSpace(tag)) { cF.Tag = tag; }
      if (!string.IsNullOrWhiteSpace(unsubscribe)) { cF.Unsubscribe = unsubscribe; }
      if (!string.IsNullOrWhiteSpace(contactPage)) { cF.ContactPage = contactPage; }
      if (!string.IsNullOrWhiteSpace(consent)) { cF.Consent = consent; }
      if (!string.IsNullOrWhiteSpace(type)) { cF.Type = type; }
      if (!string.IsNullOrWhiteSpace(phone)) { cF.Phone = phone; }

      return new FacetMapped(facetKey, cF);
    }
  }
}