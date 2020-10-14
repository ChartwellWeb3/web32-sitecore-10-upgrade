using Sitecore.XConnect;
using Sitecore.XConnect.Schema;
using System;

namespace Chartwell.Feature.CustomFacets
{
  /// <summary>
  /// Custom facet extension: contactext, for list manager import and exm insert token
  /// </summary>
  [Serializable]
  [FacetKey(DefaultFacetKey)]
  public class ContactFacetExtension : Facet
  {
    public ContactFacetExtension()
    {

    }

    public const string DefaultFacetKey = "ContactExt";

    public string Source { get; set; }
    public string ContactPage { get; set; }
    public string Consent { get; set; }
    public string Sales { get; set; }
    public string Language { get; set; }
    public string Type { get; set; }
    public string Tag { get; set; }
    public string Subscribe { get; set; }
    public string Unsubscribe { get; set; }
    public string Property { get; set; }
    public string Phone { get; set; }
  }
  
  /// <summary>
  /// To extend model, add properties. Execute unloaded console application and place .json in both xconnect locations. Build a new model for each new custom FacetKey.
  /// </summary>
  public class ContactFacetExtModel
  {
    public static XdbModel Model { get; } = BuildModel();

    private static XdbModel BuildModel()
    {
      XdbModelBuilder modelBuilder = new XdbModelBuilder("ContactFacetExtModel", new XdbModelVersion(0, 3));

      modelBuilder.ReferenceModel(Sitecore.XConnect.Collection.Model.CollectionModel.Model);
      modelBuilder.DefineFacet<Contact, ContactFacetExtension>(ContactFacetExtension.DefaultFacetKey);

      return modelBuilder.BuildModel();
    }
  }
}