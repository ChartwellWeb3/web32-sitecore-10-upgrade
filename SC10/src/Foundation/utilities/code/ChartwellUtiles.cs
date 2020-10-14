using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Sitecore;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.ContentSearch.Linq;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Globalization;
using Sitecore.Links;
using Sitecore.Resources.Media;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Sitecore.Data.Templates;
using Chartwell.Foundation.Models;
using Sitecore.CES.GeoIp.Core.Model;
using Sitecore.CES.GeoIp.Core.Lookups;
using System.Threading.Tasks;
using Sitecore.ContentSearch.Utilities;
using Sitecore.Links.UrlBuilders;

namespace Chartwell.Foundation.utility
{
  public class ChartwellUtiles
  {
    private readonly QueryHelpers _qh;
    private readonly string constr = ConfigurationManager.ConnectionStrings["experienceforms"].ToString();

    public ChartwellUtiles()
    {
      _qh = new QueryHelpers();
    }
    /// <summary>
    /// For retrieving isUnity phone number from Primary contact of the properties
    /// </summary>
    /// <returns></returns>
    public String GetPhoneNumber(Item item)
    {
      string itemPath = item.Paths.Path.ToString().ToLower();
      //  itemPath = itemPath.Replace(Sitecore.Context.Data.Site.RootPath.ToLower(), "");
      //   itemPath = itemPath.Replace(Sitecore.Context.Data.Site.StartItem.ToLower(), "");
      string phonenumber = "";
      foreach (Item root in item.Children)
      {
        if (root.Name.Contains("primarycontact"))
        {
          phonenumber = root.Fields["Phone"].ToString();
        }
        if (phonenumber == "" && root.Name.Contains("landlinecontact")) //only get landlineinformation when there is no unity number in the property
        {
          phonenumber = root.Fields["Phone"].ToString();
        }
      }

      return SetPhoneNumberWithDashes(phonenumber.Trim());
    }

    public String GetEmail(Item item)
    {
      string itemPath = item.Paths.Path.ToString().ToLower();
      //  itemPath = itemPath.Replace(Sitecore.Context.Data.Site.RootPath.ToLower(), "");
      //   itemPath = itemPath.Replace(Sitecore.Context.Data.Site.StartItem.ToLower(), "");
      string email = "";
      foreach (Item root in item.Children)
      {
        if (root.Name.Contains("primarycontact"))
        {
          email = root.Fields["Email"].ToString();
        }
        if (email == "" && root.Name.Contains("landlinecontact")) //only get landlineinformation when there is no unity number in the property
        {
          email = root.Fields["Email"].ToString();
        }
      }

      return email.Trim();
    }

    public string GetDictionaryItem(string Label, string Language)
    {
      List<SearchResultItem> matches;

      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        var predicate = PredicateBuilder.True<SearchResultItem>();

        predicate = predicate.And(p => p.Path.StartsWith("/sitecore/system/Dictionary"));
        predicate = predicate.And(p => p.Name == Label);

        matches = context.GetQueryable<SearchResultItem>().Where(predicate).ToList();
      }

      string dictionaryPhrase = matches.Where(x => x.Language == Language).Select(x => x.Fields["phrase"]).FirstOrDefault().ToString();
      //return matches;
      return dictionaryPhrase;
    }

    public List<SearchResultItem> GetDictionaryItem(string Label)
    {
      List<SearchResultItem> matches;

      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        var predicate = PredicateBuilder.True<SearchResultItem>();

        predicate = predicate.And(p => p.Path.StartsWith("/sitecore/system/Dictionary"));
        predicate = predicate.And(p => p.Name == Label);

        matches = context.GetQueryable<SearchResultItem>().Where(predicate).ToList();
      }

      var dictionaryPhrase = matches.ToList();
      //return matches;
      return dictionaryPhrase;
    }

    public string GetExternalUrl(string StaticItem, string Language)
    {
      SearchResultItem matches;

      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        var predicate = PredicateBuilder.True<SearchResultItem>();

        predicate = predicate.And(p => p.TemplateName.Equals("Static Pages"));
        predicate = predicate.And(p => p.Name.Equals(StaticItem));
        predicate = predicate.And(p => p.Language.Equals(Language));

        matches = context.GetQueryable<SearchResultItem>().Where(predicate).ToList().FirstOrDefault();
      }

      var externalUrl = matches.GetItem().Fields["ExternalUrl"].Value;
      //return matches;
      return externalUrl;
    }


    public List<SearchResultItem> PropertyDetails(ID itemID)
    {
      List<SearchResultItem> CitySearchDistanceResults = null;
      try
      {
        using (var propcontext = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
        {
          CitySearchDistanceResults = propcontext.GetQueryable<SearchResultItem>()
           .Where(x => x.ItemId == itemID)
           //.Where(x => x.Language == Sitecore.Context.Language.Name)
           .ToList();
        }

        return CitySearchDistanceResults;
      }
      catch (Exception)
      {

        throw;
      }

    }

    public string GetRedirectUrl(Language language)
    {
      string searchParamQueryString = string.Empty;
      string queryStringValue = string.Empty;
      if (!string.IsNullOrEmpty(HttpContext.Current.Request.Url.Query))
      {

        if (HttpContext.Current.Request.QueryString.Keys[0].ToLower().Equals("city") || HttpContext.Current.Request.QueryString.Keys[0].ToLower().Equals("nom-de-la-ville"))
        {
          var queryStringValueList = SwitchSelectedCity(language.Name, HttpContext.Current.Request.QueryString.GetValues(0).FirstOrDefault().ToLower())
                                     .Where(l => l.Language == language.Name).ToList();

          queryStringValue = ValidCityForSearch(queryStringValueList);

          searchParamQueryString = "/" + HttpUtility.UrlDecode(HttpContext.Current.Request.Url.Query.ToLower())
                                                          .Replace(HttpUtility.UrlDecode(HttpContext.Current.Request.QueryString.Keys[0].ToLower()) + "=", Translate.TextByLanguage("city", language).ToLower() + "=")
                                                          .Replace(HttpUtility.UrlDecode(HttpContext.Current.Request.QueryString.GetValues(0).FirstOrDefault().ToLower()), queryStringValue);
        }
        else if (HttpContext.Current.Request.QueryString.Keys[0].ToLower().Equals("region") || HttpUtility.UrlDecode(HttpContext.Current.Request.QueryString.Keys[0]).ToLower().Equals("région"))
        {
          var queryStringValueList = SwitchSelectedCity(language.Name, HttpContext.Current.Request.QueryString.GetValues(0).FirstOrDefault().ToLower())
                           .Where(l => l.Language == language.Name).ToList();

          queryStringValue = ValidCityForSearch(queryStringValueList);

          searchParamQueryString = "/" + HttpUtility.UrlDecode(HttpContext.Current.Request.Url.Query.ToLower())
                                                    .Replace(HttpUtility.UrlDecode(HttpContext.Current.Request.QueryString.Keys[0].ToLower()) + "=", Translate.TextByLanguage("Region", language).ToLower() + "=")
                                                    .Replace(HttpUtility.UrlDecode(HttpContext.Current.Request.QueryString.GetValues(0).FirstOrDefault().ToLower()), queryStringValue);
        }
        else
        {
          searchParamQueryString = "/" + HttpContext.Current.Request.Url.Query;
        }
      }

      return searchParamQueryString;
    }

    private static string ValidCityForSearch(List<SearchResultItem> queryStringValueList)
    {
      string queryStringValue;
      if (queryStringValueList != null && queryStringValueList.Count > 0)
      {
        queryStringValue = queryStringValueList.FirstOrDefault().GetItem().Fields["City Name"].Value.ToLower().Replace(" ", "-");
      }
      else
      {
        queryStringValue = HttpContext.Current.Request.QueryString.GetValues(0).FirstOrDefault().ToLower().Replace(" ", "-");
      }

      return queryStringValue;
    }

    public List<SearchResultItem> GetYardiForCommunity(string CommunityName)
    {
      List<SearchResultItem> source = null;
      using (IProviderSearchContext providerSearchContext = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        source = (from x in providerSearchContext.GetQueryable<SearchResultItem>()
                  where x.TemplateName == "PropertyPage"
                  where x.Name != "__Standard Values"
                  where x["Property ID"] != "99999"
                  where x["SplitterURL"].Equals(CommunityName)
                  select x).ToList();
      }
      return (from x in source
              where x["name"].Contains("retirement residence") || x["name"].Contains("residence pour retraites")
              select x).ToList();
    }

    public IEnumerable<SearchResultItem> SearchSelectedCity(string language, string searchCriteria)
    {
      IEnumerable<SearchResultItem> results = null;
      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        results = context.GetQueryable<SearchResultItem>()
         .Where(x => x.TemplateName == "City")
         .Where(x => x.Language == language)
         //.Where(x => x["City Name"] == searchCriteria)
         .Where(x => x.Name == searchCriteria.RemoveDiacritics().Replace(" ", ""))
         .OrderBy(o => o.Name).ToList();
      }
      return results.ToList();
    }

    public SearchResultItem GetRegionalLandingPageUrl(string city, string lang)
    {
      IEnumerable<SearchResultItem> results = null;
      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        results = context.GetQueryable<SearchResultItem>()
         .Where(x => x.TemplateName == "RegionalPropertiesPage")
         .Where(x => x.Language == lang)

         .Where(x => x.Name.StartsWith("retirement-living") && x.Name.Contains(city)).ToList();
      }
      return results.FirstOrDefault();
    }

    public Item GetItemByTemplate(string template)
    {
      var predicate = PredicateBuilder.True<SearchResultItem>();
      predicate = predicate.And(x => x.Path == template);
      predicate = predicate.And(x => x.Language == Context.Language.Name);

      var query = _qh.SingleSearchResultQuery(predicate);
      return query;
    }

    public Item GetItem(string ItemName)
    {
      IEnumerable<SearchResultItem> results = null;
      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        var predicate = PredicateBuilder.True<SearchResultItem>();
        predicate = predicate.And(x => x.TemplateName == "RegionalPropertiesPage");
        //predicate = predicate.And(x => x.Name.Contains(ItemName));
        results = context.GetQueryable<SearchResultItem>().Where(predicate).ToList();
      }
      return results.Where(x => x.GetItem().DisplayName.Equals(ItemName)).FirstOrDefault().GetItem();
    }

    public string GetMediaItemRedirectUrl(string ItemName)
    {
      IEnumerable<SearchResultItem> results = null;
      //using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      //{
      //  var predicate = PredicateBuilder.True<SearchResultItem>();
      //  predicate = predicate.And(x => x.Path.StartsWith("/sitecore/content/URLRedirect/StaticPages redirects"));
      //  predicate = predicate.And(x => x.TemplateName == "Pdf");
      //  predicate = predicate.Or(x => x.TemplateName == "Jpeg");
      //  predicate = predicate.And(x => x.Name.Contains(ItemName));
      //  results = context.GetQueryable<SearchResultItem>().Where(predicate).ToList();
      //}
      using (IProviderSearchContext context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        Expression<Func<SearchResultItem, bool>> predicate = PredicateBuilder.True<SearchResultItem>();
        predicate = predicate.And((SearchResultItem p) => p.TemplateName == "URL-Mapping");
        predicate = predicate.And((SearchResultItem p) => p["dnn url"].Equals(ItemName, StringComparison.InvariantCultureIgnoreCase));
        results = context.GetQueryable<SearchResultItem>().Where(predicate).ToList();
      }

      //var url = results.FirstOrDefault().GetItem();
      if (results.Count() > 0 && results != null)
      {
        return results.FirstOrDefault().GetItem().Fields["new url"].Value;
      }
      else
      {
        return string.Empty;
      }
    }

    public IEnumerable<SearchResultItem> SearchSelectedCityWithSpaces(string language, string searchCriteria)
    {
      IEnumerable<SearchResultItem> results = null;

      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        results = context.GetQueryable<SearchResultItem>()
         .Where(x => x.TemplateName == "City")
         .Where(x => x.Language == language)
         .Where(x => x.Name == searchCriteria.RemoveDiacritics().TrimPunctuation().ToLower())
         .OrderBy(o => o.Name).ToList();
      }
      return results.ToList();
    }

    public IEnumerable<SearchResultItem> SwitchSelectedCity(string language, string searchCriteria)
    {
      IEnumerable<SearchResultItem> results = null;
      List<SearchResultItem> matches;

      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        var predicate = PredicateBuilder.True<SearchResultItem>();

        predicate = predicate.And(p => p.Path.StartsWith("/sitecore/content/Chartwell/Project/Content Shared Folder/City"));
        predicate = predicate.And(p => p["City Name"] == searchCriteria.Replace("-", " "));
        predicate = predicate.Or(p => p["City Name"] == searchCriteria.Replace(" ", "-"));

        matches = context.GetQueryable<SearchResultItem>().Where(predicate).ToList();
      }

      string selectedItem = "";
      ID selectedItemID = new ID();
      foreach (SearchResultItem item in matches)
      {
        if (item.GetItem().Fields["City Name"].Value.ToLower() == searchCriteria.Replace("-", " ").ToLower() ||
          item.GetItem().Fields["City Name"].Value.ToLower() == searchCriteria.Replace(" ", "-").ToLower())
        {
          selectedItem = item.Name;
          selectedItemID = item.ItemId;
          break;
        }
      }
      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        results = context.GetQueryable<SearchResultItem>()
         .Where(x => x.TemplateName == "City")
         .Where(x => x.ItemId == selectedItemID)
         .OrderBy(o => o.Name).ToList();
      }

      return results.ToList();
    }

    public string FormattedAddress(Item PropertyItem, string ProvinceName)
    {
      string PropertyFormattedAddress = "";
      if (PropertyItem.Language.ToString() == "en")
      {
        PropertyFormattedAddress = PropertyItem.Fields["Street name and number"].ToString() + ", "
                    + CityName(PropertyItem.Language.Name, PropertyItem)  //PropertyItem.Fields["Selected City"].Value 
                    + ", " + ProvinceName + " "
                    + PropertyItem.Fields["Postal code"].ToString();
      }
      else
      {
        string str = PropertyItem.Fields["Postal code"].ToString();
        string postalcode = "  " + str;
        PropertyFormattedAddress = PropertyItem.Fields["Street name and number"].ToString() + ", " +
                                   CityName(PropertyItem.Language.Name, PropertyItem) + // PropertyItem.Fields["Selected City"].Value + 
                                   " (" +
                                   ProvinceName + ")" + postalcode; //to add extra space
      }
      return PropertyFormattedAddress;
    }

    public static string Indent(int count)
    {
      return "&nbsp;&nbsp;";
    }

    public string SetPhoneNumberWithDashes(string phoneNumberValue)
    {

      string phone = RemoveSpecialCharacters(phoneNumberValue);
      //Check if it is null or contains any non-digits


      //Check if it is in the format : ###-###-####
      if (!Regex.IsMatch(phone, @"\d{3}\-\d{3}\-\d{4}"))
      {
        if (phone.Length == 10)
          return string.Format("{0:###-###-####}", double.Parse(phone));
        else
          return phoneNumberValue;
      }




      //Otherwise return the empty string
      return string.Empty;
    }

    public string RemoveSpecialCharacters(string str)
    {
      StringBuilder sb = new StringBuilder();
      foreach (char c in str)
      {
        if ((c >= '0' && c <= '9'))
        {
          sb.Append(c);
        }
      }
      return sb.ToString();
    }

    public Language SetLanguageFromUrl(string sanitizedUrl)
    {
      if (sanitizedUrl.Contains("résultat-de-la-recherche"))
        sanitizedUrl = "/fr" + sanitizedUrl;
      else
        sanitizedUrl = "/en" + sanitizedUrl;

      var LangFromUrl = sanitizedUrl.Split('/').Where(x => !string.IsNullOrEmpty(x)).Where(x => x.Equals("en") || x.Equals("fr")).FirstOrDefault();
      Language.TryParse(string.IsNullOrEmpty(LangFromUrl) ? "en" : LangFromUrl, out Language LanguageFromUrl);
      Context.SetLanguage(LanguageFromUrl, true);
      return Context.Language;
    }

    public List<SearchResultItem> GetItemForUrl(string url)
    {
      List<SearchResultItem> matches;
      var tUrl = url.Split('/').Where(x => !string.IsNullOrEmpty(x)).Last();
      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        var predicate = PredicateBuilder.True<SearchResultItem>();
        predicate = predicate.And(p => p.TemplateName.Equals("SplitterPage"));
        matches = context.GetQueryable<SearchResultItem>().Where(predicate).ToList();
      }

      var SearchResultItem = matches.Where(x => x.GetItem().DisplayName.ToLower().Equals(tUrl)).ToList();
      return SearchResultItem;
    }

    public SearchResultItem GetItemForUrl(string url, string urlLang)
    {
      List<SearchResultItem> matches;
      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        var predicate = PredicateBuilder.True<SearchResultItem>();
        predicate = predicate.And(p => p.TemplateName.Equals("PropertyPage") &&
        (p.Name.Equals(url.Replace("-", " ").Replace("'", "").RemoveDiacritics()) || p.Name.Contains(url.Replace("-", " ").Replace("'", "").RemoveDiacritics())));
        predicate = predicate.Or(p => p.TemplateName.Equals("SplitterPage"));
        predicate = predicate.Or(p => p.TemplateName.Equals("URL-Mapping") && p["DNN URL"].Contains(url));
        predicate = predicate.Or(p => p.TemplateName.Equals("RegionalPropertiesPage"));
        predicate = predicate.And(p => !p.Name.Equals("__Standard Values"));
        predicate = predicate.And(p => p.Language == urlLang);
        matches = context.GetQueryable<SearchResultItem>().Where(predicate).ToList();
      }

      var SearchResultItem = matches.FirstOrDefault(f => f.GetItem()["DNN URL"].Contains(url)
      || f.Name.Equals(url.Replace("-", " ").Replace("'", "").RemoveDiacritics())
      || f.Name.Contains(url.Replace("-", " ").Replace("'", "").RemoveDiacritics())
      || f.GetItem().DisplayName.ToLower().Equals(url)
      || f.GetItem().DisplayName.Equals(url)
      );

      if (SearchResultItem != null)
        return SearchResultItem;
      else
        return null;
    }

    public List<Item> GetTopNavParentItems(string node)
    {
      List<SearchResultItem> matches;
      var NodeItem = node == "parent" ? "topnavorder" : "topnavchilditemsorder";
      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        var predicate = PredicateBuilder.True<SearchResultItem>();
        predicate = predicate.And(p => p.TemplateName.Equals("TopNavOrder"));
        predicate = predicate.And(p => !p.Name.Equals("__Standard Values"));
        predicate = predicate.And(p => p.Language.Equals(Context.Language.Name));
        matches = context.GetQueryable<SearchResultItem>().Where(predicate).ToList();
      }

      var TopNavParentItems = matches.Select(s => s.GetField(NodeItem).Value.Split('|')).ToList()[0]
                                       .Select(e => PropertyDetails(new ID(e)).Where(l => l.Language == Context.Language.Name)
                                       .Select(n => n.GetItem()) //.DisplayName)
                                       .FirstOrDefault()).ToList();
      return TopNavParentItems;
    }

    public List<Item> LeftNavItems(List<ID> PrmLeftNavIDs, Item rootsite)
    {
      List<Item> matches = new List<Item>();
      foreach (ID id in PrmLeftNavIDs)
      {
        using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
        {
          var predicate = PredicateBuilder.True<SearchResultItem>();
          predicate = predicate.And(p => p.ItemId == id);
          predicate = predicate.And(p => p.Language == Context.Language.Name);

          var LeftNavItem = context.GetQueryable<SearchResultItem>().Where(predicate).FirstOrDefault().GetItem();
          matches.Add(LeftNavItem);
        }
      }

      List<Item> PropertyLeftNavItems = new List<Item>();
      foreach (Item LeftNavTemplateID in matches)
      {

        PropertyLeftNavItems.Add(rootsite.Children.Where(r => r.TemplateID == LeftNavTemplateID.TemplateID)
                            .FirstOrDefault());
      }
      if (PropertyLeftNavItems.Where(c => c == null).Count() > 0)
        PropertyLeftNavItems = PropertyLeftNavItems.Where(p => p != null).ToList();
      return PropertyLeftNavItems;
    }

    public List<SearchResultItem> GetStaticPageItem(string phrase)
    {
      List<SearchResultItem> matches;
      List<SearchResultItem> searchResultItems = null;
      var splitStaticPageItem = phrase.Split('/').Where(x => !string.IsNullOrEmpty(x)).Last();

      splitStaticPageItem = splitStaticPageItem.ToLower().Replace("/en/", "").Replace("/fr/", "").Replace("/", "").Replace("-", " ").ToLower();
      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        var predicate = PredicateBuilder.True<SearchResultItem>();
        predicate = predicate.And(p => p.TemplateName.Equals("Static Pages"));
        //predicate = predicate.Or(p => p.TemplateName.Equals("Static Panel"));
        predicate = predicate.And(p => !p.Name.Equals("__Standard Values"));
        //predicate = predicate.And(p => p.Language.Equals(Context.Language.Name));
        matches = context.GetQueryable<SearchResultItem>().Where(predicate).ToList();

        searchResultItems = matches.Where(x => x.GetItem().DisplayName.Replace("-", " ").ToLower().Contains(splitStaticPageItem) ||
                                               x.GetItem().DisplayName.ToLower().Contains(splitStaticPageItem.Replace(" ", "")) ||
                                               x.GetItem().Name.Replace("-", " ").ToLower().Equals(splitStaticPageItem) ||
                                               x.GetItem().Name.Replace("-", " ").ToLower().Contains(splitStaticPageItem)
                                               ).ToList();


        //searchResultItems = matches.Where(x => x.GetItem().DisplayName.Replace("-", " ").ToLower().Equals(splitStaticPageItem)
        //                    || x.GetItem().DisplayName.Replace("-", " ").ToLower().Contains(splitStaticPageItem)
        //                    || x.GetItem().DisplayName.ToLower().Contains(splitStaticPageItem.Replace(" ", ""))).ToList();

        if (searchResultItems != null && searchResultItems.Count() == 2)
        {
          searchResultItems = searchResultItems.Where(x => x.GetItem().DisplayName.Replace("-", " ").ToLower().Equals(splitStaticPageItem)).ToList();
        }
        else if (searchResultItems != null && searchResultItems.Count > 2)
        {
          var temp = searchResultItems.Where(i => i.GetItem().DisplayName.Equals(splitStaticPageItem)).ToList();
          searchResultItems = temp;
        }
        if (searchResultItems != null && searchResultItems.Count() == 0)
          searchResultItems = matches.Where(x => x.GetItem().DisplayName.Replace("-", " ").Replace(" ", "").ToLower().Equals(splitStaticPageItem)).ToList();
      }

      return searchResultItems;
    }

    public List<SearchResultItem> SetLangContextFromUrl(List<string> phrase)
    {
      List<SearchResultItem> matches;
      List<SearchResultItem> searchResultItems = null;

      var ContentItem = phrase.Count == 1 ? phrase[0] : phrase[1]; //.Split('/').Where(x => !string.IsNullOrEmpty(x)).Last();
      ContentItem = ContentItem.ToLower().Replace("/en/", "").Replace("/fr/", "").Replace("/", "").Replace("-", " ").ToLower();
      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        var predicate = PredicateBuilder.True<SearchResultItem>();
        predicate = predicate.And(p => p.Path.StartsWith("/sitecore/content/Chartwell/Project") &&
                                      !p.Path.StartsWith("/sitecore/content/Chartwell/Project/blog") &&
                                      !p.Path.StartsWith("/sitecore/content/Chartwell/Project/Data") &&
                                      !p.Path.StartsWith("/sitecore/content/Chartwell/Project/PropertySearch") &&
                                      !p.Path.StartsWith("/sitecore/content/Chartwell/Project/Content Shared Folder") &&
                                      !p.Path.StartsWith("/sitecore/content/Chartwell/Master-pages"));
        if (phrase.Count == 1)
          predicate = predicate.And(p => p.Content.Equals(phrase[0]) ||
                                         p.Content.StartsWith(phrase[0]) ||
                                         p.Content.Contains(phrase[0]));

        else
        {
          string lang = phrase[0].Equals("retirement-residences") || phrase[0].Equals("continuum-of-care") ? "en" : "fr";
          predicate = predicate.And(p => p.Content.Equals(phrase[1]) ||
                                         p.Content.StartsWith(phrase[1]) ||
                                         p.Content.Contains(phrase[1]));

          predicate = predicate.And(p => p.Language == lang);
        }

        matches = context.GetQueryable<SearchResultItem>().Where(predicate).ToList();

        searchResultItems = matches.Where(x => (!string.IsNullOrEmpty(x.GetItem().DisplayName) && x.GetItem().DisplayName.ToLower().Equals(ContentItem.Replace(" ", "-"))) ||
                                               (!string.IsNullOrEmpty(x.GetItem().DisplayName) && x.GetItem().DisplayName.ToLower().Contains(ContentItem.Replace(" ", "-"))) ||
                                               x.GetItem().Name.ToLower().Equals(ContentItem.Replace("-", " ").Replace("'", "")) ||
                                               x.GetItem().Name.ToLower().Contains(ContentItem.Replace("-", " ").Replace("'", ""))

                                               ||

                                               // Conditions for Static Pages
                                               (!string.IsNullOrEmpty(x.GetItem().DisplayName) && x.GetItem().DisplayName.ToLower().Equals(ContentItem.Replace("-", " "))) ||
                                               (!string.IsNullOrEmpty(x.GetItem().DisplayName) && x.GetItem().DisplayName.ToLower().Contains(ContentItem.Replace("-", " "))) ||
                                               x.GetItem().Name.ToLower().Equals(ContentItem.Replace(" ", "-").Replace("'", "")) ||
                                               x.GetItem().Name.ToLower().Contains(ContentItem.Replace(" ", "-").Replace("'", ""))
                                               ).ToList();


        if (searchResultItems != null && searchResultItems.Count() == 2)
        {
          searchResultItems = searchResultItems.Where(x => x.GetItem().DisplayName.ToLower().Equals(ContentItem.Replace(" ", "-")) ||
                                                           x.GetItem().DisplayName.ToLower().StartsWith(ContentItem.Replace(" ", "-")) ||
                                                           x.GetItem().Name.ToLower().Equals(ContentItem.Replace("-", " ").Replace("'", "")) ||
                                                           x.GetItem().Name.ToLower().Contains(ContentItem.Replace("-", " ").Replace("'", ""))

                                                           ||

                                                           // Conditions for Static Pages
                                                           x.GetItem().DisplayName.ToLower().Equals(ContentItem.Replace("-", " ")) ||
                                                           x.GetItem().DisplayName.ToLower().StartsWith(ContentItem.Replace("-", " ")) ||
                                                           x.GetItem().Name.ToLower().Equals(ContentItem.Replace(" ", "-").Replace("'", "")) ||
                                                           x.GetItem().Name.ToLower().Contains(ContentItem.Replace(" ", "-").Replace("'", ""))
                                                           ).ToList();
        }
        else if (searchResultItems != null && searchResultItems.Count > 2)
        {
          var temp = searchResultItems.Where(i => i.GetItem().DisplayName.ToLower().Equals(ContentItem.Replace(" ", "-")) ||
                                                  i.Name.ToLower().Contains(ContentItem.Replace("-", " ").Replace("'", ""))

                                                  ||

                                                  i.GetItem().DisplayName.ToLower().Equals(ContentItem.Replace("-", " ")) ||
                                                  i.Name.ToLower().Contains(ContentItem.Replace(" ", "-").Replace("'", ""))
                                                  ).ToList();
          searchResultItems = temp;
        }
        if (searchResultItems != null && searchResultItems.Count() == 0)
          searchResultItems = matches.Where(x => x.GetItem().DisplayName.Replace(" ", "-").ToLower().Equals(ContentItem) ||

                                                 x.GetItem().Name.ToLower().Equals(ContentItem.Replace(" ", "-").Replace("'", "")) ||
                                                 x.GetItem().DisplayName.Replace("-", " ").ToLower().Equals(ContentItem) ||
                                                 x.GetItem().Name.ToLower().Equals(ContentItem.Replace("-", " ").Replace("'", "")) ||
                                                 x.GetItem().TemplateName.Equals("PropertyPage") || x.GetItem().TemplateName.Equals("SplitterPage")
                                                 ).ToList();
      }

      return searchResultItems;
    }

    public Item GetUrlFromUrlMapping(string url)
    {
      List<SearchResultItem> matches;
      url = url.Replace("/en/", "/").Replace("/fr/", "/");
      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        var predicate = PredicateBuilder.True<SearchResultItem>();
        predicate = predicate.And(p => p.TemplateName.Equals("URL-Mapping"));
        predicate = predicate.And(p => p["dnn url"].Equals(url));

        matches = context.GetQueryable<SearchResultItem>().Where(predicate).ToList();

        var UrlRedirectItem = matches.Where(u => u.GetItem().Fields["dnn url"].Value.TrimEnd('/').Equals(url)).ToList();

        return UrlRedirectItem.Count != 0 ? UrlRedirectItem.FirstOrDefault().GetItem() : null;
      }
    }

    public Item GetItemTypeForItemFromUrl(string rawUrlOnLoad)
    {
      List<SearchResultItem> matches;
      rawUrlOnLoad = rawUrlOnLoad.Replace("/en/", "/").Replace("/fr/", "/");
      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        var predicate = PredicateBuilder.True<SearchResultItem>();
        predicate = predicate.And(p => p.Content.Contains(rawUrlOnLoad) && (p.TemplateName.Equals("PropertyPage") || p.TemplateName.Equals("Static Pages")));

        matches = context.GetQueryable<SearchResultItem>().Where(predicate).ToList().Where(i => i.GetItem().DisplayName.ToLower().Replace("-", " ").Replace("'", "").Replace("’", "").RemoveDiacritics()
                                                                                    .Contains(rawUrlOnLoad.ToLower().RemoveDiacritics().Replace("/", "").Replace("'", "").Replace("’", ""))).ToList();

        return matches.Count != 0 ? matches.FirstOrDefault().GetItem() : null;
      }
    }

    public List<SearchResultItem> DetermineRedirection(string url)
    {
      List<SearchResultItem> matches = null;
      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        var predicate = PredicateBuilder.True<SearchResultItem>();
        predicate = predicate.And(p => !p.Path.StartsWith("/sitecore/content/Chartwell/Project/Content Shared Folder"));
        predicate = predicate.And(p => !p.Path.StartsWith("/sitecore/system"));
        predicate = predicate.And(p => !p.TemplateName.Equals("Dictionary entry"));
        predicate = predicate.And(p => !p.TemplateName.Equals("Dictionary folder"));
        predicate = predicate.And(p => !p.TemplateName.Equals("Media folder"));
        predicate = predicate.And(p => !p.TemplateName.Equals("Blog Post"));
        predicate = predicate.And(p => !p.TemplateName.Equals("Blog Tag"));
        predicate = predicate.And(p => !p.TemplateName.Equals("Blog Category"));
        predicate = predicate.And(p => !p.TemplateName.Equals("Jpeg"));
        predicate = predicate.And(p => !p.TemplateName.Equals("Image"));
        predicate = predicate.And(p => !p.TemplateName.Equals("URL-Mapping"));
        predicate = predicate.And(p => !p.TemplateName.Equals("View rendering"));
        predicate = predicate.And(p => !p.TemplateName.Equals("Rendering Folder"));
        predicate = predicate.And(p => !p.TemplateName.Equals("PropertySearch"));
        predicate = predicate.And(p => !p.TemplateName.Equals("OutcomeMap"));
        predicate = predicate.And(p => !p.TemplateName.Equals("Pdf"));
        predicate = predicate.And(p => !p.TemplateName.Equals("QuestionnaireEmailTemplate"));
        predicate = predicate.And(p => !p.TemplateName.Equals("Template Section"));
        predicate = predicate.And(p => !p.TemplateName.Equals("Template"));
        predicate = predicate.And(p => !p.TemplateName.Equals("Folder"));
        predicate = predicate.And(p => !p.TemplateName.Equals("Template Field"));
        predicate = predicate.And(p => p.Content.Contains(url)
                                        || p.Content.Contains(url.Replace(" ", ""))
                                        || p.Name.Equals(url.Replace("-", " ").Replace("'", "").Replace("’", "").RemoveDiacritics())
                                        );

        matches = context.GetQueryable<SearchResultItem>().Where(predicate).ToList();

        return matches;
      }
    }

    public List<SearchResultItem> GetRedirectItem(string url)
    {
      List<SearchResultItem> matches = null;

      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        var predicate = PredicateBuilder.True<SearchResultItem>();
        predicate = predicate.And(p => !p.TemplateName.Equals("Dictionary entry"));
        predicate = predicate.And(p => !p.TemplateName.Equals("Media folder"));
        predicate = predicate.And(p => !p.TemplateName.Equals("Blog Post"));
        predicate = predicate.And(p => !p.TemplateName.Equals("Jpeg"));
        predicate = predicate.And(p => !p.TemplateName.Equals("Image"));
        predicate = predicate.And(p => !p.TemplateName.Equals("URL-Mapping"));
        predicate = predicate.And(p => !p.TemplateName.Equals("customPage"));
        predicate = predicate.And(p => !p.TemplateName.Equals("PropertySearch"));
        predicate = predicate.And(p => !p.TemplateName.Equals("Pdf"));
        predicate = predicate.And(p => p.Content.Contains(url));

        matches = context.GetQueryable<SearchResultItem>().Where(predicate).ToList();

        return matches;
      }
    }
    public Item GetBlogPost(string BlogPost, ref string OrigLang)
    {
      List<SearchResultItem> matches = null;
      //url = url.Substring(url.LastIndexOf("/")).Equals("/") ? (url.Substring(url.TrimEnd('/').LastIndexOf("/"))).Trim('/') : url.Substring(url.LastIndexOf("/")).Trim('/');
      var lang = BlogPost.Contains("/en/") ? "en" : BlogPost.Contains("/fr") ? "fr" : "";
      var url = BlogPost.Substring(BlogPost.LastIndexOf("/") + 1).ToLower();
      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        var predicate = PredicateBuilder.True<SearchResultItem>();
        predicate = predicate.And(p => !p.TemplateName.Equals("Jpeg"));
        predicate = predicate.And(p => !p.TemplateName.Equals("Image"));
        predicate = predicate.And(p => !p.TemplateName.Equals("Dictionary entry"));
        predicate = predicate.And(p => !p.TemplateName.Equals("Dictionary folder"));
        predicate = predicate.And(p => !p.TemplateName.Equals("Folder"));
        predicate = predicate.And(p => !p.TemplateName.Equals("XBlog Data"));
        predicate = predicate.And(p => !p.TemplateName.Equals("Media folder"));
        predicate = predicate.And(p => !p.TemplateName.Equals("Node"));
        predicate = predicate.And(p => !p.TemplateName.Equals("URL-Mapping"));
        predicate = predicate.And(p => !p.TemplateName.Equals("Static Pages"));
        predicate = predicate.And(p => !p.TemplateName.Equals("customPage"));
        predicate = predicate.And(p => !p.TemplateName.Equals("QuestionnaireEmailTemplate"));
        predicate = predicate.And(p => !p.TemplateName.Equals("Template Section"));
        predicate = predicate.And(p => !p.TemplateName.Equals("Template"));
        predicate = predicate.And(p => !p.TemplateName.Equals("Template Field"));

        //predicate = predicate.And(p => p.TemplateName.Equals("Blog Post"));
        //predicate = predicate.And(p => p.Name != "__Standard Values");
        predicate = predicate.And(p => p.Content.Contains(url.RemoveDiacritics()));

        matches = context.GetQueryable<SearchResultItem>().Where(predicate).OrderBy(o => o.Language).ToList();

        if (matches.Count == 0)
        {
          var fallbackpredicate = PredicateBuilder.True<SearchResultItem>();
          fallbackpredicate = fallbackpredicate.And(p => p.TemplateName.Equals("Blog Post"));
          fallbackpredicate = fallbackpredicate.And(p => p.Name != "__Standard Values");
          matches = context.GetQueryable<SearchResultItem>().Where(fallbackpredicate).OrderBy(o => o.Language).ToList();

          var blogList = matches.Where(b => b.GetItem().DisplayName.RemoveDiacritics().ToLower().Contains(url.RemoveDiacritics().ToLower())).ToList();
          matches = blogList;
        }

        Item UrlFromContent = null;

        if (matches.Count > 0)
        {
          List<SearchResultItem> BlogItem = new List<SearchResultItem>();
          if (BlogPost.ToLower().Equals("/blog") || BlogPost.ToLower().Equals("/blogue"))
          {
            BlogItem = matches.Where(i => i.GetItem().DisplayName.ToLower().Equals(url.ToLower())).ToList();
            UrlFromContent = BlogItem.FirstOrDefault().GetItem();
          }
          else if (HttpContext.Current.Request.RawUrl.ToString().Equals("/fr/blog"))
          {
            lang = "fr";
            BlogItem = PropertyDetails(matches.Where(n => n.Name.Equals("blog")).FirstOrDefault().GetItem().ID);
            UrlFromContent = BlogItem.Where(l => l.Language.Equals(lang)).FirstOrDefault().GetItem();
          }
          else if (HttpContext.Current.Request.RawUrl.ToString().Equals("/en/blogue"))
          {
            lang = "en";
            BlogItem = PropertyDetails(matches.Where(n => n.Name.Equals("blog")).FirstOrDefault().GetItem().ID);
            UrlFromContent = BlogItem.Where(l => l.Language.Equals(lang)).FirstOrDefault().GetItem();
          }
          else
          {
            {
              //BlogItem = PropertyDetails(matches.FirstOrDefault().GetItem().ID).OrderBy(bi => bi.Language).ToList();
              BlogItem = PropertyDetails(matches.Where(n => n.Name.Equals(url.RemoveDiacritics()
                                                                             .Replace("-", " ").Replace("'", "").Replace("’", "")) ||
                                                                              n.GetItem().DisplayName.RemoveDiacritics().Replace("'", "").Replace("’", "").Replace("-", " ").ToLower()
                                                                             .Equals(url.RemoveDiacritics().Replace("'", "").Replace("’", "").Replace("-", " ").ToLower()))
                                                                             .FirstOrDefault().GetItem().ID).OrderBy(bi => bi.Language).ToList();

              var origLang = BlogItem.Where(x => x.Name.Equals(url.Replace("-", " "))).ToList(); //.GetItem().Language.Name;
              if (origLang.Count != 0)
              {
                OrigLang = origLang.FirstOrDefault().GetItem().Language.Name;
                if (string.IsNullOrEmpty(lang)) lang = OrigLang;
              }
              else
              {
                OrigLang = BlogItem.Where(x => x.GetItem().DisplayName.RemoveDiacritics().Replace("'", "").Replace("’", "").ToLower()
                                   .Equals(url.RemoveDiacritics().Replace("'", "").Replace("’", "").ToLower())).FirstOrDefault().GetItem().Language.Name;
                if (string.IsNullOrEmpty(lang)) lang = OrigLang;
              }
              UrlFromContent = BlogItem.Where(l => l.Language.Equals(lang)).FirstOrDefault().GetItem();

            }
          }
        }
        return UrlFromContent;
      }
    }

    public SearchResultItem SetItemContextFromUrl(List<string> phrase, Language language)
    {
      List<SearchResultItem> matches;
      SearchResultItem searchResultItem = null;

      string splitStaticPageItem = string.Empty;
      if (phrase.Count == 1)
        splitStaticPageItem = phrase[0];
      else
      {
        splitStaticPageItem = phrase[1];
      }
      splitStaticPageItem = splitStaticPageItem.ToLower().Replace("/en/", "").Replace("/fr/", "").Replace("/", "").Replace("-", " ").ToLower();

      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        var predicate = PredicateBuilder.True<SearchResultItem>();
        predicate = predicate.And(p => p.Path.StartsWith("/sitecore/content/Chartwell/Project") &&
                                      !p.Path.StartsWith("/sitecore/content/Chartwell/Project/blog") &&
                                      !p.Path.StartsWith("/sitecore/content/Chartwell/Project/Data") &&
                                      !p.Path.StartsWith("/sitecore/content/Chartwell/Project/PropertySearch") &&
                                      !p.Path.StartsWith("/sitecore/content/Chartwell/Project/Content Shared Folder") &&
                                      !p.Path.StartsWith("/sitecore/content/Chartwell/Master-pages"));
        predicate = predicate.And(p => p.Name.Equals(splitStaticPageItem.Replace("'", "").Replace("-", " ")) ||
                                       p.Content.Equals(splitStaticPageItem.Replace("'", "").Replace("-", " ")) ||
                                       p.Content.StartsWith(splitStaticPageItem.Replace("'", "").Replace("-", " ")) ||
                                       p.Content.Contains(splitStaticPageItem.Replace("'", "").Replace("-", " ")) ||
                                       p["display name"].Contains(splitStaticPageItem.Replace(" ", "-"))

                                       ||

                                       // Conditions for Static Pages
                                       p.Name.Equals(splitStaticPageItem.Replace("'", "")) ||
                                       p.Content.Equals(splitStaticPageItem.Replace("'", "")) ||
                                       p.Content.StartsWith(splitStaticPageItem.Replace("'", "")) ||
                                       p.Content.Contains(splitStaticPageItem.Replace("'", ""))

                                       );

        matches = context.GetQueryable<SearchResultItem>().Where(predicate).ToList();

        var ItemID = matches.Where(x => x.Name.ToLower().Equals(splitStaticPageItem.Replace("'", "").Replace("-", " ")) ||
                                        x.Name.ToLower().Contains(splitStaticPageItem.Replace("-", " ").Replace("'", "")) ||
                                        x.GetItem().DisplayName.ToLower().Contains(splitStaticPageItem.Replace(" ", "-"))

                                        ||

                                        // Conditions for Static Pages
                                        x.Name.ToLower().Equals(splitStaticPageItem.Replace("'", "").Replace(" ", "-")) ||
                                        x.Name.ToLower().Contains(splitStaticPageItem.Replace(" ", "-").Replace("'", "")) ||
                                        x.GetItem().DisplayName.ToLower().Contains(splitStaticPageItem.Replace("-", " ")) &&
                                        x.Language.Equals(language))
                                        .FirstOrDefault().GetItem().ID;

        var LangSpecificUrl = PropertyDetails(ItemID).Where(l => l.Language.ToString().Equals(language.ToString())).FirstOrDefault();

        searchResultItem = LangSpecificUrl;
      }
      return searchResultItem;
    }

    public List<SearchResultItem> SetContextItem(List<string> phrase, Language langOnLoad, string TemplateName)
    {
      List<SearchResultItem> matches;
      List<SearchResultItem> searchResultItems = null;
      var splitStaticPageItem = phrase.Count == 1 ? phrase[0] : phrase[1]; // phrase.Split('/').Where(x => !string.IsNullOrEmpty(x)).Last();

      splitStaticPageItem = splitStaticPageItem.ToLower().Replace("/en/", "").Replace("/fr/", "").Replace("/", "").Replace("-", " ").RemoveDiacritics().ToLower();
      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        var predicate = PredicateBuilder.True<SearchResultItem>();
        predicate = predicate.And(p => p.TemplateName.Equals(TemplateName));
        predicate = predicate.And(p => !p.Name.Equals("__Standard Values"));
        matches = context.GetQueryable<SearchResultItem>().Where(predicate).ToList();

        searchResultItems = matches.Where(x => x.GetItem().DisplayName.ToLower().Equals(splitStaticPageItem.Replace(" ", "-")) ||
                                               x.GetItem().DisplayName.ToLower().Contains(splitStaticPageItem.Replace(" ", "-")) ||
                                               x.GetItem().Name.ToLower().Equals(splitStaticPageItem.Replace("-", " ").Replace("'", "")) ||
                                               x.GetItem().Name.ToLower().Contains(splitStaticPageItem.Replace("-", " ").Replace("'", ""))

                                               ||

                                               //Conditions for Static Pages
                                               x.GetItem().DisplayName.ToLower().Equals(splitStaticPageItem.Replace("-", " ")) ||
                                               x.GetItem().DisplayName.ToLower().Contains(splitStaticPageItem.Replace("-", " ")) ||
                                               x.GetItem().Name.ToLower().Equals(splitStaticPageItem.Replace(" ", "-").Replace("'", "")) ||
                                               x.GetItem().Name.ToLower().Contains(splitStaticPageItem.Replace(" ", "-").Replace("'", ""))
                                               ).ToList();


        if (searchResultItems != null && searchResultItems.Count() == 2)
        {
          searchResultItems = searchResultItems.Where(x => x.GetItem().Language.Equals(langOnLoad)).ToList();
        }
        else if (searchResultItems != null && searchResultItems.Count > 2)
        {
          var temp = searchResultItems.Where(i => i.GetItem().DisplayName.Equals(splitStaticPageItem.Replace(" ", "-")) ||
                                                  i.GetItem().Name.ToLower().Contains(splitStaticPageItem.Replace("-", " ").Replace("'", ""))

                                                  ||

                                                  //Conditions for Static Pages
                                                  i.GetItem().DisplayName.Equals(splitStaticPageItem.Replace("-", " ")) ||
                                                  i.GetItem().Name.ToLower().Contains(splitStaticPageItem.Replace(" ", "-").Replace("'", ""))
                                                  ).ToList();
          searchResultItems = temp;
        }
        if (searchResultItems != null && searchResultItems.Count() == 0)
          searchResultItems = matches.Where(x => x.GetItem().DisplayName.ToLower().Equals(splitStaticPageItem.Replace(" ", "-")) ||
                                                 x.GetItem().Name.ToLower().Contains(splitStaticPageItem.Replace("-", " ").Replace("'", ""))

                                                 ||

                                                 //Conditions for Static Pages
                                                 x.GetItem().DisplayName.ToLower().Equals(splitStaticPageItem.Replace("-", " ")) ||
                                                 x.GetItem().Name.ToLower().Contains(splitStaticPageItem.Replace("-", " ").Replace("'", ""))
                                                 ).ToList();
      }

      return searchResultItems;
    }

    public Item GetStaticItemLanguageFromUrl(string staticItemname, Language language)
    {
      List<SearchResultItem> matches;
      SearchResultItem searchResultItem = null;

      var splitStaticPageItem = staticItemname.Split('/').Where(x => !string.IsNullOrEmpty(x)).Last();
      splitStaticPageItem = splitStaticPageItem.ToLower().Replace("/en/", "").Replace("/fr/", "").Replace("/", "").Replace("-", " ").RemoveDiacritics().ToLower();

      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        var predicate = PredicateBuilder.True<SearchResultItem>();
        predicate = predicate.And(p => p.TemplateName.Equals("Static Pages"));
        predicate = predicate.And(p => !p.Name.Equals("__Standard Values"));
        matches = context.GetQueryable<SearchResultItem>().Where(predicate).ToList();

        var ItemID = matches.Where(x => x.GetItem().DisplayName.Replace("-", " ").ToLower().Contains(splitStaticPageItem) ||
                                              x.GetItem().DisplayName.ToLower().Contains(splitStaticPageItem.Replace(" ", "")))
                                               .FirstOrDefault().GetItem().ID;
        searchResultItem = matches.Where(i => i.ItemId.Equals(ItemID) && i.Language.ToString().Equals(language.ToString())).FirstOrDefault();
      }

      return searchResultItem.GetItem();
    }
    public List<SearchResultItem> GetStaticPageItemFrom404(string phrase)
    {
      List<SearchResultItem> matches;
      List<SearchResultItem> searchResultItems = null;
      var splitStaticPageItem = phrase.Split('/').Where(x => !string.IsNullOrEmpty(x)).Last();

      splitStaticPageItem = splitStaticPageItem.ToLower().Replace("/en/", "").Replace("/fr/", "").Replace("/", "").Replace("-", " ").ToLower();
      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        var predicate = PredicateBuilder.True<SearchResultItem>();
        predicate = predicate.And(p => p.TemplateName.Equals("Static Pages"));
        predicate = predicate.And(p => !p.Name.Equals("__Standard Values"));
        matches = context.GetQueryable<SearchResultItem>().Where(predicate).ToList();

        searchResultItems = matches.Where(x => x.GetItem().Name.Replace("-", " ").ToLower().Equals(splitStaticPageItem) ||
                                               x.GetItem().Name.Replace("-", " ").ToLower().Contains(splitStaticPageItem) ||
                                               x.GetItem().DisplayName.Replace("-", " ").ToLower().Contains(splitStaticPageItem) ||
                                               x.GetItem().DisplayName.ToLower().Contains(splitStaticPageItem.Replace(" ", ""))).ToList();

        if (searchResultItems != null && searchResultItems.Count() == 2)
        {
          searchResultItems = searchResultItems.Where(x => x.GetItem().DisplayName.Replace("-", " ").ToLower().Equals(splitStaticPageItem)).ToList();
        }
        else if (searchResultItems != null && searchResultItems.Count > 2)
        {
          var temp = searchResultItems.Where(i => i.GetItem().DisplayName.Equals(splitStaticPageItem)).ToList();
          searchResultItems = temp;
        }
        if (searchResultItems != null && searchResultItems.Count() == 0)

          searchResultItems = matches.Where(x => x.GetItem().DisplayName.Replace("-", " ").Replace(" ", "").ToLower().Equals(splitStaticPageItem)).ToList();
      }

      return searchResultItems;
    }

    public Item GetStaticPageAllChildItem(Item item)
    {
      Item child;
      child = item.GetChildren().FirstOrDefault();
      return child;
    }

    public bool LeftNavChildItem(Item item)
    {
      return item.Parent.Name != "retirement-residences" && item.Parent != null && item.Parent.Children.Count > 0;
    }

    public List<SearchResultItem> LeftNavChildItemTemplateName(string key)
    {
      List<SearchResultItem> matches;

      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        var predicate = PredicateBuilder.True<SearchResultItem>();
        predicate = predicate.And(p => p.Path.StartsWith("/sitecore/templates/User Defined/Chartwell/PropertyTemplates/PropertySubItems"));
        predicate = predicate.And(p => p.Name.Equals("__Standard Values"));
        predicate = predicate.And(p => !p.TemplateName.Equals("_BaseCommonLeftMenu"));
        matches = context.GetQueryable<SearchResultItem>().Where(predicate).ToList();
      }
      List<SearchResultItem> PropertySubItems = new List<SearchResultItem>();

      var tPropertySubItems = matches.Where(x => x.GetItem().DisplayName.ToLower().RemoveDiacritics().Replace("-", "").Replace(" ", "")
                                                            .Equals(key.RemoveDiacritics().Replace("-", "").Replace(" ", ""))).ToList();

      if (tPropertySubItems.Count == 0)
      {
        tPropertySubItems = matches.Where(x => x.GetItem().DisplayName.ToLower().RemoveDiacritics().Replace("-", "").Contains(key.RemoveDiacritics().Replace("-", "")) ||
        key.RemoveDiacritics().Replace("-", "").Contains(x.GetItem().DisplayName.ToLower().RemoveDiacritics().Replace("-", ""))).ToList();
      }

      if (tPropertySubItems.Count != 0)
      {
        PropertySubItems = matches.Where(x => x.ItemId.Equals(tPropertySubItems.FirstOrDefault().GetItem().ID)).ToList();
      }

      return PropertySubItems;
    }
    public Item GetSplitterPageItem(string url, string lang)
    {
      List<SearchResultItem> matches;
      Item SplitterPageItem = null;
      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        var predicate = PredicateBuilder.True<SearchResultItem>();
        predicate = predicate.And(p => p.TemplateName == "SplitterPage");
        predicate = predicate.And(p => p.Language.Equals(lang));
        predicate = predicate.And(p => p.Name.Contains(url));
        matches = context.GetQueryable<SearchResultItem>().Where(predicate).ToList();
      }
      if (matches.Count > 0)
        SplitterPageItem = matches[0].GetItem();

      return SplitterPageItem;

    }

    public Item GetPropertyFrom404(string url, string lang)
    {
      List<SearchResultItem> matches;
      Item PropertyPageItem = null;
      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        var predicate = PredicateBuilder.True<SearchResultItem>();
        predicate = predicate.And(p => p.TemplateName == "PropertyPage");
        predicate = predicate.And(p => p.Language.Equals(lang));
        predicate = predicate.And(p => p.Name.Equals(url.ToLower().RemoveDiacritics().Replace("-", " ").Replace("'", "").Replace("’", "")));
        matches = context.GetQueryable<SearchResultItem>().Where(predicate).ToList();
      }
      if (matches.Count > 0)
        PropertyPageItem = matches[0].GetItem();

      return PropertyPageItem;

    }

    public string GetPropertyUrl(Item PropertyItem)
    {
      string itemURL = LinkManager.GetItemUrl(PropertyItem, new ItemUrlBuilderOptions
      {
        UseDisplayName = true,
        LowercaseUrls = true,
        AlwaysIncludeServerUrl = false,
        LanguageEmbedding = LanguageEmbedding.Always,
        LanguageLocation = LanguageLocation.FilePath,
        Language = Context.Language //PropertyItem.Language 

      });

      return itemURL.Replace(" ", "-") + "/" + Translate.Text("overview");
    }

    public string GetitemUrl(Item item, Language language)
    {
      string url = LinkManager.GetItemUrl(Context.Database.GetItem(item.ID, LanguageManager.GetLanguage(language.ToString())), new ItemUrlBuilderOptions
      {
        UseDisplayName = true,
        LowercaseUrls = true,
        LanguageEmbedding = LanguageEmbedding.Always,
        LanguageLocation = LanguageLocation.FilePath,
        Language = language

      });
      url = url.Replace(" ", "-");
      return url;

    }

    public Item GetEndUserMessageDetails()
    {
      SearchResultItem results = null;
      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        var predicate = PredicateBuilder.True<SearchResultItem>();
        //predicate = predicate.And(x => x.Path.StartsWith("/sitecore/content/Chartwell/Project/EndUserEmailMessage/EndUserEmail"));
        predicate = predicate.And(x => x.TemplateName == "EndUserEmail");
        predicate = predicate.And(x => x.Language == Context.Language.Name);
        results = context.GetQueryable<SearchResultItem>().Where(predicate).FirstOrDefault(); // .ToList();
      }
      return results.GetItem();
    }

    /// <summary>
    /// Grab email template item by formname for questionnaire forms
    /// </summary>
    /// <param name="formName"></param>
    /// <returns></returns>
    public Item GetEndUserMessageDetails(string formName)
    {
      SearchResultItem results;

      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        var predicate = PredicateBuilder.True<SearchResultItem>();
        predicate = predicate.And(x => x.TemplateName == "QuestionnaireEmailTemplate");
        predicate = predicate.And(x => x.Name.Contains(formName));
        predicate = predicate.And(x => x.Language == Context.Language.Name);
        results = context.GetQueryable<SearchResultItem>().Where(predicate).FirstOrDefault();
      }
      return results.GetItem();
    }

    /// <summary>
    /// Get corresponding template by score
    /// </summary>
    /// <param name="score"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public string GetEmailMainBody(int score, Item item)
    {
      if (score <= 11 && score >= 0)
        return item.Fields["Score Category 1 Message"].Value;
      else if (score <= 18 && score >= 12)
        return item.Fields["Score Category 2 Message"].Value;
      else if (score >= 19)
        return item.Fields["Score Category 3 Message"].Value;

      return "";
    }

    public string PropertyType(string Language, Item SearchPropertyItem)
    {
      string name = PropertyDetails(new ID(SearchPropertyItem.Fields["property type"].ToString())).FirstOrDefault().GetItem()
          .Name;
      string text = SearchPropertyItem.Fields["property type"].ToString();
      string result = string.Empty;
      if (!string.IsNullOrEmpty(text))
      {
        ID itemID = new ID(text);
        List<SearchResultItem> list = (from x in PropertyDetails(itemID)
                                       where x.Language == Language
                                       select x).ToList();
        Item item = list[0].GetItem();
        result = item.Fields["property type"].ToString();
      }
      return result;
    }

    public string ProvinceName(string Language, Item SearchPropertyItem)
    {
      string text = SearchPropertyItem.Fields["Province"].ToString();
      string result = string.Empty;
      if (!string.IsNullOrEmpty(text))
      {
        ID itemID = new ID(text);
        List<SearchResultItem> list = (from x in PropertyDetails(itemID)
                                       where x.Language == Language
                                       select x).ToList();
        Item item = list[0].GetItem();
        result = item.Fields["Province Name"].ToString();
      }
      return result;
    }

    public string AbbrProvinceName(string Language, Item SearchPropertyItem)
    {
      string text = SearchPropertyItem.Fields["Province"].ToString();
      string result = string.Empty;
      if (!string.IsNullOrEmpty(text))
      {
        ID itemID = new ID(text);
        List<SearchResultItem> list = (from x in PropertyDetails(itemID)
                                       where x.Language == Language
                                       select x).ToList();
        Item item = list[0].GetItem();
        result = item.Fields["Province Name"].ToString();

        switch (result.ToLower())
        {
          case "ontario":
            result = "ON";
            break;
          case "quebec":
            result = "QC";
            break;
          case "british bolumbia":
            result = "BC";
            break;
          case "alberta":
            result = "AB";
            break;
        }

      }
      return result;
    }
    public HtmlString Carousel_BackGroundImageDisplay(Item item)
    {
      var carouselCnt = item.Fields["PropertyCarousel"].Value.Split('|').ToList().Count(c => !string.IsNullOrEmpty(c));
      var backGroundImageCnt = item.Fields["Thumbnail Photo"].Value.Split('|').ToList().Count(c => !string.IsNullOrEmpty(c));

      HtmlString imgTag = new HtmlString(string.Empty);

      if (!carouselCnt.Equals(0) && !backGroundImageCnt.Equals(0))
      {
        imgTag = new HtmlString(string.Empty);
      }
      else if (carouselCnt.Equals(0) && !backGroundImageCnt.Equals(0))
      {
        ImageField imageField = item.Fields["Thumbnail Photo"];
        MediaItem image = new MediaItem(imageField.MediaItem);
        string src = StringUtil.EnsurePrefix('/', MediaManager.GetMediaUrl(image));
        imgTag = new HtmlString(src);
      }
      else
      {
        imgTag = new HtmlString(string.Empty);
      }
      return imgTag;
    }
    public Item GetItemByStringId(string stringId)
    {
      var id = new ID(new Guid(stringId));
      var predicate = PredicateBuilder.True<SearchResultItem>();
      predicate = predicate.And(p => p.ItemId == id);
      predicate = predicate.And(p => p.Language == Context.Language.Name);

      return _qh.SingleSearchResultQuery(predicate);
    }

    public Item GetItemById(ID itemId)
    {
      var predicate = PredicateBuilder.True<SearchResultItem>();
      predicate = predicate.And(p => p.ItemId == itemId);
      predicate = predicate.And(p => p.Language == Context.Language.Name);

      return _qh.SingleSearchResultQuery(predicate);
    }

    public Item GetItemByIdNoLang(ID itemId)
    {
      var predicate = PredicateBuilder.True<SearchResultItem>();
      Item t;
      predicate = predicate.And(p => p.ItemId == itemId);
      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        t = context.GetQueryable<SearchResultItem>().Where(predicate).FirstOrDefault().GetItem();
      }
      return t;
      //return _qh.SingleSearchResultQuery(predicate);
    }

    public Item GetItemByPath(string path)
    {
      var predicate = PredicateBuilder.True<SearchResultItem>();
      predicate = predicate.And(p => p.Path == path);
      predicate = predicate.And(p => p.Language == Context.Language.Name);

      return _qh.SingleSearchResultQuery(predicate);
    }

    public List<Item> NewestResidencesList()
    {
      List<string> ProvinceItems = new List<string>();
      List<SearchResultItem> NewResidenceItems = new List<SearchResultItem>();

      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        var predicate = PredicateBuilder.True<SearchResultItem>();
        predicate = predicate.And(p => p.TemplateName == "NewResidencePage" && p.Name == "newest-properties");
        predicate = predicate.Or(p => p.TemplateName == "PropertyPage" && p["Is New Property"] == "1");
        predicate = predicate.And(p => p.Language == Context.Language.Name);

        //Get list of provinces
        ProvinceItems = context.GetQueryable<SearchResultItem>().Where(predicate)
                                   .FirstOrDefault().GetItem()
                                   .Fields["NewResidencesRegionOrder"].Value.Split('|').ToList();

        //Get list of properties with the flag [Is New Property] set to 1 (true)
        NewResidenceItems = context.GetQueryable<SearchResultItem>().Where(predicate).Where(n => n.TemplateName == "PropertyPage").ToList();
      }

      List<Item> SortedNewResidences = new List<Item>();
      ProvinceItems.ForEach(p => SortedNewResidences.AddRange(NewResidenceItems.Where(n => new ID(Guid.Parse(n["Province"])) == new ID(Guid.Parse(p)))
                                               .Select(i => i.GetItem())
                                               .ToList()));

      return SortedNewResidences;
    }

    public IEnumerable<SearchResultItem> CheckForSplitterPage(string language, string searchCriteria, ref bool CheckSplitPage)
    {
      IEnumerable<SearchResultItem> results = null;

      searchCriteria = searchCriteria.RemoveDiacritics().Replace("'", "").Replace("-", " ")
          .TrimPunctuation()
          .ToLower();
      using (IProviderSearchContext providerSearchContext = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {

        results = (from x in providerSearchContext.GetQueryable<SearchResultItem>()
                   where x.TemplateName == "PropertyPage"
                   where x.Language == language
                   where x["property ID"] != "99999"
                   select x into o
                   orderby o.Name
                   select o).ToList();
      }

      results = results.Where(x => x.GetItem().DisplayName.RemoveDiacritics().Replace("'", "").Replace("-", " ").TrimPunctuation().ToLower()
                                .Equals(searchCriteria) || x.Name.Contains(searchCriteria)).ToList();

      bool stringToBool = false;
      bool flag = false;
      flag = (from i in results
              where bool.TryParse(i["IsSplitter"], out stringToBool)
              select stringToBool into b
              where b.Equals(obj: true)
              select b).Count().Equals(results.Count());
      CheckSplitPage = false;
      if (flag)
      {
        results = (from x in results
                   where x.Language == Context.Language.Name
                   select x).Take(1);
        CheckSplitPage = true;
      }
      return (from x in results
              where x.Language == Context.Language.Name
              select x).ToList();
    }

    public bool UserRegistered(string Email)
    {
      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        var predicate = PredicateBuilder.True<SearchResultItem>();
        predicate = predicate.And(p => p.TemplateName == "Email");
        predicate = predicate.And(p => p.Name == "Email");

        var UserDetails = context.GetQueryable<SearchResultItem>().Where(predicate).ToList();
      }

      return false;
    }

    public Item GetActivitiesItem(string propertyItemPath)
    {
      var predicate = PredicateBuilder.True<SearchResultItem>();
      predicate = predicate.And(p => p.Path == propertyItemPath);
      predicate = predicate.And(p => p.Language == Context.Language.Name);

      return _qh.SingleSearchResultQuery(predicate);
    }

    public Item GetActivityTemplateItem(string activityItemPath)
    {
      var predicate = PredicateBuilder.True<SearchResultItem>();
      predicate = predicate.And(p => p.Path == activityItemPath);
      predicate = predicate.And(p => p.Language == Context.Language.Name);

      var item = _qh.SingleSearchResultQuery(predicate);
      return item;
    }

    public string GetBrochureUrl(Item brochure)
    {
      ImageField imageField = brochure.Fields["Property Brochure"];
      if (!string.IsNullOrEmpty(imageField.Value))
      {
        MediaItem item = GetItemByIdNoLang(new ID(new Guid(imageField.Value)));
        return (imageField != null) ?
            HashingUtils.ProtectAssetUrl(MediaManager.GetMediaUrl(item)) : string.Empty;
      }
      return string.Empty;
    }

    public string GetDiningUrl(Item d)
    {
      ImageField imageField = d.Fields["Menus"];
      MediaItem item = GetItemByIdNoLang(new ID(new Guid(imageField.Value)));
      return (imageField != null) ?
          HashingUtils.ProtectAssetUrl(MediaManager.GetMediaUrl(item)) : string.Empty;
    }

    public string GetActivitiesCalendarUrl(Item a)
    {
      ImageField imageField = a.Fields["Property Activity Calendar"];
      MediaItem item = GetItemByIdNoLang(new ID(new Guid(imageField.Value)));
      return (imageField != null) ?
          HashingUtils.ProtectAssetUrl(MediaManager.GetMediaUrl(item)) : string.Empty;
    }

    public List<Item> GetCarouselItems()
    {
      var predicate = PredicateBuilder.True<SearchResultItem>();
      predicate = predicate.And(p => p.TemplateName == "CarouselTemplate");
      predicate = predicate.And(p => p.Language == Context.Language.Name);

      var carouselItems = _qh.ListSearchResultQuery(predicate)
        .Select(x => x.GetItem()).ToList();

      var predicate2 = PredicateBuilder.True<SearchResultItem>();
      predicate2 = predicate2.And(p => p.TemplateName == "PropertyPage" && p["Is New Property"] == "1");
      predicate2 = predicate2.And(p => p.Language == Context.Language.Name);

      var carouselObjs = _qh.ListSearchResultQuery(predicate2);

      List<Item> SortedCarouselImgs = new List<Item>();
      foreach (var item in carouselItems)
      {
        SortedCarouselImgs.AddRange(carouselObjs.Where(n => n.ItemId == new ID(new Guid(item["Residence"].ToString())))
                                               .Select(i => i.GetItem())
                                               .ToList());
      }

      return SortedCarouselImgs;
    }

    public string GetImageUrl(Item SearchPropertyItem)
    {
      ImageField imageField = SearchPropertyItem.Fields["Thumbnail Photo"];
      string text = (imageField != null) ?
          HashingUtils.ProtectAssetUrl(MediaManager.GetMediaUrl(imageField.MediaItem, new MediaUrlBuilderOptions { MaxWidth = 1280 })) : string.Empty;
      return text.Replace("/sitecore/shell", "");
    }

    public bool IsEmailValid(string email)
    {
      var conn = new SqlConnection(constr);
      conn.Open();

      var command = new SqlCommand("select * from FieldData where Value = @email", conn);
      command.Parameters.Add(new SqlParameter("@email", email));
      var isEmail = command.ExecuteScalar();

      if (isEmail != null)
        return true;
      return false;
    }

    //public string GetCityName(Item item)
    //{
    //  var pred = PredicateBuilder.True<SearchResultItem>();
    //  pred = pred.And(x => x.TemplateName == "City");
    //  pred = pred.And(x => x.Language == Context.Language.Name);

    //  var cities = _qh.ListSearchResultQuery(pred).ToList();

    //  var city = cities.Where(x => x.ItemId == new ID(new Guid(item["City"].ToString())))
    //               .Select(x => x.GetItem()).FirstOrDefault();

    //  return city.Name;
    //}

    public string CityName(string Language, Item SearchPropertyItem)
    {
      string text = SearchPropertyItem.Fields["City"].ToString();
      string result = string.Empty;
      if (!string.IsNullOrEmpty(text))
      {
        ID itemID = new ID(text);
        var list = (from x in PropertyDetails(itemID)
                    where x.Language == Language
                    select x).FirstOrDefault();
        Item item = list.GetItem();
        result = item.Fields["City Name"].ToString();
      }
      return result;
    }


    public List<string> RegionGeoNames(string currLat, string currLng)
    {
      List<string> locDetails = new List<string>();
      var url = "https://secure.geonames.net/findNearbyPostalCodesJSON?lat=" + currLat + "&lng=" + currLng + "&maxRows=1&username=chartwellrr";
      WebRequest webrequest = WebRequest.Create(url);

      using (WebResponse wrs = webrequest.GetResponse())
      using (Stream stream = wrs.GetResponseStream())
      using (StreamReader reader = new StreamReader(stream))
      {
        var json = reader.ReadToEnd();
        JToken token = JObject.Parse(json);

        //IList<string> storeNames = token.SelectToken("postalCodes").Select(s => (string)s).ToList();

        string City = (string)token.SelectToken("postalCodes[0].adminName2");
        if (string.IsNullOrEmpty(City))
        {
          City = (string)token.SelectToken("postalCodes[0].placeName");
          if (string.IsNullOrEmpty(City))
            City = (string)token.SelectToken("postalCodes[0].adminName1");

        }
        locDetails.Add(City);
        string Province = (string)token.SelectToken("postalCodes[0].adminCode1");
        locDetails.Add(Province);
        string PostalCode = (string)token.SelectToken("postalCodes[0].postalCode");
        locDetails.Add(PostalCode);
        string CountryCode = (string)token.SelectToken("postalCodes[0].countryCode");
        locDetails.Add(CountryCode);

        return locDetails;
      }
    }


    public List<string> GeoNameLocation(string searchCity)
    {
      List<string> locDetails = new List<string>();
      var url = "https://secure.geonames.net/searchJSON?q=" + searchCity + "&maxRows=1&username=chartwellrr";
      WebRequest webrequest = WebRequest.Create(url);

      using (WebResponse wrs = webrequest.GetResponse())
      using (Stream stream = wrs.GetResponseStream())
      using (StreamReader reader = new StreamReader(stream))
      {
        var json = reader.ReadToEnd();
        JToken token = JObject.Parse(json);

        //IList<string> storeNames = token.SelectToken("postalCodes").Select(s => (string)s).ToList();

        locDetails.Add((string)token.SelectToken("geonames[0].lat"));
        locDetails.Add((string)token.SelectToken("geonames[0].lng"));

        return locDetails;
      }
    }

    public string ClosestResidenceDetails(string Lat, string Lng, string Language)
    {

      string strCoordinate = Lat + "," + Lng;
      string NearestResidenceUrl;

      using (IProviderSearchContext context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        Expression<Func<SearchResultItem, bool>> predicate = PredicateBuilder.True<SearchResultItem>();

        predicate = predicate.And(d => d.Language == Language);
        predicate = predicate.And(d => d.TemplateName == "PropertyPage");
        predicate = predicate.And(d => d["Property ID"] != "99999");
        predicate = predicate.And(d => d.Name != "__Standard Values");

        var Queryresults = context.GetQueryable<SearchResultItem>()
                          .Where(predicate)
                          .WithinRadius(s => s.Location, strCoordinate, 500)
                          .OrderByDistance(d => d.Location, strCoordinate).ToList()
                          .Where(i => GetItemById(new ID(i.GetItem().Fields["property type"].Value)).Name.Equals("RET")).Take(1).ToList()
                          .Select(u => new
                          {
                            ItemID = u.ItemId.ToString(),
                            PropertyID = u.GetField("property id").Value,
                            PropertyName = u.GetField("property name").Value,
                            PropertyItemUrl = GetPropertyUrl(u.GetItem()),
                            PropertyType = PropertyType(Language, u.GetItem()),
                            Distance = double.Parse(string.Format("{0:0.0}", Distance(double.Parse(Lat, CultureInfo.InvariantCulture), double.Parse(Lng, CultureInfo.InvariantCulture), u.Location.Latitude, u.Location.Longitude, 'K'))),
                          }).OrderBy(o => o.Distance).FirstOrDefault().ToString();
        

        //var Queryresults = context.GetQueryable<SearchResultItem>()
        //                          .Where(predicate)
        //                          .WithinRadius(s => s.Location, strCoordinate, 500)
        //                          .OrderByDistance(d => d.Location, strCoordinate).ToList()
        //                          .Where(i => GetItemById(new ID(i.GetItem().Fields["property type"].Value)).Name.Equals("RET")).Take(1)
        //                          .Select(s => new
        //                          {
        //                            ItemID = s.ItemId.ToString(),
        //                            PropertyID = s.GetField("property id").Value,
        //                            PropertyName = s.GetField("property name").Value,
        //                            PropertyItemUrl = GetPropertyUrl(s.GetItem()),
        //                            PropertyType = PropertyType(Language, s.GetItem())
        //                          }).FirstOrDefault().ToString();

        NearestResidenceUrl = Queryresults;

      }
      return NearestResidenceUrl;
    }

    public IEnumerable<SearchResultItem> ClosestResidenceDetails(string Lat, string Lng)
    {

      string strCoordinate = Lat + "," + Lng;

      using (IProviderSearchContext context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        Expression<Func<SearchResultItem, bool>> predicate = PredicateBuilder.True<SearchResultItem>();

        predicate = predicate.And(d => d.Language == Context.Language.Name);
        predicate = predicate.And(d => d.TemplateName == "PropertyPage");
        predicate = predicate.And(d => d["Property ID"] != "99999");
        predicate = predicate.And(d => d.Name != "__Standard Values");


        IEnumerable<SearchResultItem> Queryresults = context.GetQueryable<SearchResultItem>()
          .Where(predicate)
          .WithinRadius(s => s.Location, strCoordinate, 500)
          .OrderByDistance(d => d.Location, strCoordinate)
          .ToList();

        return Queryresults.ToList();
      }
    }
    public WhoIsInformation GetUserGeoIPDetails()
    {
      string UserIP = GetUSerIP(); //HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
      if (string.IsNullOrEmpty(UserIP))
      {
        UserIP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
      }

      var userLocation = LookupManager.GetWhoIsInformationByIp(UserIP);
      return userLocation;
    }

    public string GetUSerIP()
    {
      string UserIP = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
      if (string.IsNullOrEmpty(UserIP))
      {
        UserIP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
      }
      return UserIP;
    }

    public List<string> GetPageSEO(Item contextItem)
    {
      string strPageDescription = string.Empty;
      string strPageKeyword = string.Empty;
      string strPageTitle;
      string SEOCity = string.Empty;
      string SEOProvince = string.Empty;

      List<string> lstPageSEO = new List<string>();

      string strPropertyType = string.Empty;
      if (contextItem.Name.Equals("Home"))
      {
        strPageTitle = "Chartwell Retirement Residences-Canada's Largest Senior Housing Choice";
        strPageDescription = "Chartwell offers a range of senior living options across Canada including independent and assisted living retirement homes, memory care, long term care and extended care residences. We offer our residents a safe and rewarding lifestyle in a seniors housing community that they are proud to call home.";
        if (Context.Language.Name != "en")
        {
          strPageTitle = "Résidences pour retraités Chartwell - Le plus grand choix de logements pour personnes âgées au Canada";
          strPageDescription = "Chartwell offre une vaste gamme d’hébergements pour retraités à l'échelle du Canada, des résidences pour personnes autonomes et semi-autonomes sans oublier des centres de soins de longue durée. Nous offrons à nos résidents un milieu de vie sécuritaire et valorisant qu'ils sont heureux d'appeler leur chez-soi.";
        }
      }
      else if (contextItem.TemplateName.Equals("SearchResultTemplate"))
      {
        if (Context.Language.Name == "en")
        {
          strPageTitle = "Chartwell Retirement Residences - SearchResults";
        }
        else
        {
          strPageTitle = "Chartwell résidences pour retraités - Résultat de la recherche";
        }

      }
      else
      {
        Template itemTemplate = TemplateManager.GetTemplate(contextItem);
        if (itemTemplate.FullName.Contains("PropertySubItems"))
        {
          Item parentPropertyPage = Context.Item.Parent;
          string strCity2 = CityName(parentPropertyPage.Language.Name, parentPropertyPage);
          string strPropertyTypeID = parentPropertyPage.Fields["Province"].ToString();
          if (!string.IsNullOrEmpty(strPropertyTypeID))
          {
            ID PropertyTypeID = new ID(strPropertyTypeID);
            Item PropertyTypeItem = GetItemById(PropertyTypeID);
            strPropertyType = PropertyTypeItem.Fields["Province Name"].ToString();
          }
          if (contextItem.Name == "overview")
          {
            strPageDescription = string.IsNullOrEmpty(parentPropertyPage.Fields["PageDescription"].ToString()) ? parentPropertyPage.Fields["Property Description"].ToString() : parentPropertyPage.Fields["PageDescription"].ToString();
            string strPageProperty = parentPropertyPage.Fields["Property Name"].ToString();
            strPageTitle = string.IsNullOrEmpty(parentPropertyPage.Fields["PageTitle"].ToString()) ? strPageProperty + " - " + strCity2 + ", " + strPropertyType :
                                                                                                     parentPropertyPage.Fields["PageTitle"].ToString();
          }
          else if (contextItem.Name == "careservice")
          {
            string leftNavItem2 = "care section description";
            strPageDescription = parentPropertyPage.Fields[leftNavItem2].ToString();
            strPageTitle = !contextItem.Fields["PageTitle"].ToString().Contains("Chartwell")
                                    ? contextItem.Fields["PageTitle"].ToString() + " " + parentPropertyPage.Fields["Property Name"].ToString() : contextItem.Fields["PageTitle"].ToString();
          }
          else
          {
            string leftNavItem = contextItem.Name + " section description";
            strPageDescription = (parentPropertyPage.Fields[leftNavItem] == null || !parentPropertyPage.Fields[leftNavItem].HasValue)
                                  ? parentPropertyPage.Fields["Property Description"].ToString() :
                                  string.IsNullOrEmpty(contextItem.Fields["PageDescription"].ToString())
                                  ? parentPropertyPage.Fields[leftNavItem].ToString() :
                                  contextItem.Fields["PageDescription"].ToString();

            strPageTitle = !contextItem.Fields["PageTitle"].ToString().Contains("Chartwell")
                                  ? contextItem.Fields["PageTitle"].ToString() + " " + parentPropertyPage.Fields["Property Name"].ToString() : contextItem.Fields["PageTitle"].ToString();
          }
          if (strPageDescription == string.Empty)
          {
            strPageDescription = parentPropertyPage.Fields["Property Description"].ToString();
          }
          strPageKeyword = string.IsNullOrEmpty(parentPropertyPage.Fields["PageKeyword"].ToString())
                            ? contextItem.Fields["PageKeyword"].ToString() :
                            string.IsNullOrEmpty(contextItem.Fields["PageKeyword"].ToString()) ?
                            parentPropertyPage.Fields["PageKeyword"].ToString() :
                            contextItem.Fields["PageKeyword"].ToString();

          SEOCity = strCity2;
          SEOProvince = strPropertyType;
        }
        else
        {
          strPageTitle = contextItem.Fields["PageTitle"].ToString();
          strPageDescription = contextItem.Fields["PageDescription"].ToString();
          strPageKeyword = contextItem.Fields["PageKeyword"].ToString();
        }
      }
      lstPageSEO.Add(strPageTitle);
      lstPageSEO.Add(strPageDescription);
      lstPageSEO.Add(strPageKeyword);
      lstPageSEO.Add(SEOCity);
      lstPageSEO.Add(SEOProvince);

      return lstPageSEO;
    }

    public List<Postalcode> GeoNameLocation(string currLat, string currLng)
    {
      List<Postalcode> PostCodeDetails = new List<Postalcode>();
      var url = "https://secure.geonames.net/findNearbyPostalCodesJSON?lat=" + currLat + "&lng=" + currLng + "&maxRows=10&username=chartwellrr";
      WebRequest webrequest = WebRequest.Create(url);

      using (WebResponse wrs = webrequest.GetResponse())
      using (Stream stream = wrs.GetResponseStream())
      using (StreamReader reader = new StreamReader(stream))
      {
        var json = JsonConvert.DeserializeObject<Rootobject>(reader.ReadToEnd());

        //PostCodeDetails = json.postalCodes.OrderBy(o => o.distance)
        //                      .Where(p => !string.IsNullOrEmpty(p.postalCode)
        //                               && !string.IsNullOrEmpty(p.adminName2)
        //                               && !string.IsNullOrEmpty(p.placeName)).ToList();

        PostCodeDetails = json.postalCodes.OrderBy(o => o.distance).ToList();

        return PostCodeDetails;
      }
    }
    public async Task<PostalCodeModel> GetLocDetailsFromCanadianPostalCodeDB(Postalcode LocationDetails, string constring)
    {
      PostalCodeModel PCModel = new PostalCodeModel();

      SqlConnection conn = new SqlConnection(constring);

      SqlCommand cmd = new SqlCommand("sp_SCGetPostalCode", conn)
      {
        CommandType = CommandType.StoredProcedure
      };
      cmd.Parameters.AddWithValue("@PostCode", LocationDetails.postalCode);
      conn.Open();


      var dt = new DataTable();

      using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
      {
        dt.Load(rdr);


        PCModel = (from DataRow r in dt.Rows
                   select new PostalCodeModel
                   {
                     City = r["City"].ToString(),
                     Lat = r["Latitude"].ToString(),
                     Lng = r["Longitude"].ToString(),
                   }).FirstOrDefault();
      }

      return PCModel;
    }

    public double Distance(double lat1, double lon1, double lat2, double lon2, char unit)
    {
      double theta = lon1 - lon2;
      double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
      dist = Math.Acos(dist);
      dist = rad2deg(dist);
      dist = dist * 60 * 1.1515;
      if (unit == 'K')
      {
        dist = dist * 1.609344;
      }
      else if (unit == 'N')
      {
        dist = dist * 0.8684;
      }
      return (dist);
    }

    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //::  This function converts decimal degrees to radians             :::
    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    private double deg2rad(double deg)
    {
      return (deg * Math.PI / 180.0);
    }

    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //::  This function converts radians to decimal degrees             :::
    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    private double rad2deg(double rad)
    {
      return (rad / Math.PI * 180.0);
    }

  }

  public static class CustomStringExtensions
  {
    /// <summary>
    /// Example usage: "Chartwell Residence".Like("%Residence%")
    /// </summary>
    /// <param name="toSearch"></param>
    /// <param name="toFind"></param>
    /// <returns></returns>
    public static bool Like(this string toSearch, string toFind)
    {
      return new Regex(@"\A" + new Regex(@"\.|\$|\^|\{|\[|\(|\||\)|\*|\+|\?|\\").Replace(toFind, ch => @"\" + ch).Replace('_', '.').Replace("%", ".*") + @"\z", RegexOptions.Singleline).IsMatch(toSearch);
    }
    public static string Sanitize404Url(this String sanitizedUrlFrom404)
    {
      string UrlFrom404;
      byte[] bytes = Encoding.GetEncoding(1252).GetBytes(sanitizedUrlFrom404);


      UrlFrom404 = Encoding.UTF8.GetString(bytes);
      return UrlFrom404;
    }

    public static String RemoveDiacritics(this String s)
    {
      String normalizedString = s.Normalize(NormalizationForm.FormD);
      StringBuilder stringBuilder = new StringBuilder();

      for (int i = 0; i < normalizedString.Length; i++)
      {
        Char c = normalizedString[i];
        if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
        {
          stringBuilder.Append(c);
        }
      }

      return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    public static string TrimPunctuation(this string value)
    {
      // Count start punctuation.
      for (int i = 0; i < value.Length; i++)
      {
        if (char.IsPunctuation(value[i]))
        {
          string p1 = value.Substring(0, i);
          string p2 = value.Substring(i + 1);
          value = p1 + p2;
        }
      }
      return value;
    }

    public static string AddSpacesToSentence(this string text)
    {
      if (string.IsNullOrWhiteSpace(text))
        return "";
      StringBuilder newText = new StringBuilder(text.Length * 2);
      newText.Append(text[0]);
      for (int i = 1; i < text.Length; i++)
      {
        if (char.IsUpper(text[i]) && text[i - 1] != ' ')
          newText.Append(' ');
        newText.Append(text[i]);
      }
      return newText.ToString();
    }

    public static string ToTitleCase(this string s) =>
      !string.IsNullOrEmpty(s) ? CultureInfo.InvariantCulture.TextInfo.ToTitleCase(s.ToLower()) : string.Empty;

    public static string RemoveExtraSpaces(this string s) =>
    String.Join(" ", s.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));

    public static string InsertBraces(this string value)
    {
      string[] arr1 = new string[] { "Alberta", "British Columbia", "Ontario", "Quebec" };

      var firstString = string.Empty;
      var secondString = string.Empty;
      int cnt = 0;
      bool insertBraces = false;

      string[] splitStr = value.Split(' ');
      if (splitStr.Count() == 1)
        return value;

      //if (splitStr.Count() == 3)
      //  splitStr[1] = splitStr[1] + " " + splitStr[2];

      //List<string> wordList = new List<string>();
      //wordList.Add(splitStr[0]);
      //wordList.Add(splitStr[1] + " " + splitStr[2]);

      foreach (string s in splitStr)
      {
        if (!arr1.Contains(s))
        {
          cnt++;
        }
      }

      if (cnt == splitStr.Count())
        return value;
      if (arr1.Contains(splitStr[0]))
        insertBraces = false;
      else
      {
        insertBraces = true;

      }
      if (insertBraces)
      {
        foreach (string s in arr1)
        {
          if (value.Contains(s))
          {
            var lengthStr = s.Length;
            firstString = value.Substring(0, value.Length - lengthStr);
            secondString = value.Substring(value.Trim().Length - lengthStr);
            break;
          }
        }

        if (!string.IsNullOrEmpty(splitStr[0]) && insertBraces)
          value = firstString + "(" + secondString + ")";
        else
          value = firstString + secondString;
      }
      return value;
    }
  }
  /// <summary>
  /// Save time on Solr based queries
  /// </summary>
  public class QueryHelpers
  {
    /// <summary>
    /// Returns list of SearchResultItem
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<SearchResultItem> ListSearchResultQuery(Expression<Func<SearchResultItem, bool>> predicate)
    {
      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        return context.GetQueryable<SearchResultItem>().Where(predicate).ToList();
      }
    }
    /// <summary>
    /// Returns Single Item obj
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public Item SingleSearchResultQuery(Expression<Func<SearchResultItem, bool>> predicate)
    {
      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        return context.GetQueryable<SearchResultItem>().Where(predicate).FirstOrDefault().GetItem();
      }
    }

    public SearchResultItem SingleMediaSearchResultQuery(Expression<Func<SearchResultItem, bool>> predicate)
    {
      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        var e = context.GetQueryable<SearchResultItem>().Where(predicate).FirstOrDefault();
        return context.GetQueryable<SearchResultItem>().Where(predicate).FirstOrDefault();
      }
    }
  }
}



