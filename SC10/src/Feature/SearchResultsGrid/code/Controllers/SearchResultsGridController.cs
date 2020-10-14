using Chartwell.Foundation.Models;
using Chartwell.Foundation.utility;
using Sitecore;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Globalization;
using Sitecore.Links;
using Sitecore.Links.UrlBuilders;
using Sitecore.Mvc.Presentation;
using Sitecore.Resources.Media;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

public class SearchResultsGridController : Controller
{
  private readonly string constring = ConfigurationManager.ConnectionStrings["SitecoreOLP"].ToString();

  private readonly ChartwellUtiles c = new ChartwellUtiles();

  private IEnumerable<SearchResultItem> results = null;

  public string language = Context.Language.ToString();

  private bool foundSearchResults = false;
  string qryLat = string.Empty;
  string qryLng = string.Empty;

  public List<PropertySearchModel> PropertyList
  {
    get;
    set;
  }

  public ActionResult SearchResults(PropertySearchModel property)
  {
    PropertyList = new List<PropertySearchModel>();
    property.RegionList = RegionsDDL(language);
    string text = string.Empty;
    string text2 = string.Empty;

    string text3 = (HttpUtility.UrlDecode(base.Request.Url.PathAndQuery).TrimEnd('/')).ToLower();
    if (
               text3.Equals("/en/search-results")
            || text3.Equals("/search-results")
            || text3.Equals("/fr/résultat-de-la-recherche")
            || text3.Equals("/résultat-de-la-recherche")
            || text3.Equals("/en/search-results/?city=canada")
            || text3.Equals("/fr/résultat-de-la-recherche/?nom-de-la-ville=canada")
            || text3.Equals("/search-results/?city=canada")
            || text3.Equals("/résultat-de-la-recherche/?nom-de-la-ville=canada")
        )
    {
      language = c.SetLanguageFromUrl(text3).Name;
      //if (text3.Replace("/en", "").Replace("/fr", "").Replace("/", "")
      //    .Equals(Translate.Text("SearchResults")))
      //{
      text = c.GetDictionaryItem("AllProperties", language).ToLower();
      text2 = c.GetDictionaryItem("Region", language).ToLower();
      //}
    }
    else
    {
      text = Request.Params[0].ToLower();
      text2 = Request.Params.Keys[0].ToLower();
      var userLoc = c.GetUserGeoIPDetails();

      if (!Request.Url.Host.Equals("chartwell.com") &&
          !Request.Url.Host.Equals("shreeji.ca"))
      {
        property.Lat = qryLat = "43.6545";
        property.Lng = qryLng = "-79.7802";
        property.DistanceFromCurrentLocation = true;
      }
      else
      {
        property.Lat = qryLat = userLoc.Latitude.ToString();
        property.Lng = qryLng = userLoc.Longitude.ToString();
        property.DistanceFromCurrentLocation = true;
      }
    }
    if (text2.ToLower() == c.GetDictionaryItem("CitySearch", language).ToLower())
    {
      property.City = text.Replace("-", " ");
    }
    else if (text2 == "propertyname")
    {
      property.PropertyName = text.Replace("-", " ").Replace("'", "").RemoveDiacritics()
          .ToLower();
    }
    else if (text2 == "postalcode")
    {
      property.PostalCode = text.Replace("-", " ");
    }
    if (!string.IsNullOrEmpty(text) && text2.Equals(c.GetDictionaryItem("Region", language).ToLower()))
    {
      if (text.Replace("-", " ").Equals(c.GetDictionaryItem("AllProperties", language).ToLower()))
      {
        property.City = "canada";
      }
      else
      {
        property.City = text.Replace("-", " ");
      }
      property.CityLandingPageButton = "RET";
      property.Display_RegionsDD = true;
      property.IsRegion = true;
    }
    else
    {
      property.Display_RegionsDD = false;
      property.IsRegion = false;
    }
    if (!string.IsNullOrWhiteSpace(property.PropertyName) || !string.IsNullOrEmpty(property.City) || !string.IsNullOrWhiteSpace(property.PostalCode))
    {
      string empty = string.Empty;
      string searchType = text2;
      string regionalUrl = string.Empty;

      if (!string.IsNullOrEmpty(property.City))
      {
        empty = property.City;
        #region Bug #9
        if (empty.RemoveDiacritics().ToLower().Equals("ottawa") ||
            empty.RemoveDiacritics().ToLower().Equals("gatineau") ||
            empty.RemoveDiacritics().ToLower().Equals("calgary") ||
            empty.RemoveDiacritics().ToLower().Equals("quebec") ||
            empty.RemoveDiacritics().ToLower().Equals("quebec province") ||
            empty.RemoveDiacritics().ToLower().Equals("province de quebec")
            )
        {
          if (empty.RemoveDiacritics().ToLower().Equals("province de quebec"))
          {
            regionalUrl = "/fr/régionale/la-vie-en-résidence-dans-la-province-de-québec";
          }
          else
          {
            SearchResultItem regionalItem = c.GetRegionalLandingPageUrl(empty.RemoveDiacritics().ToLower(), language);
            Context.Item = ItemManager.GetItem(regionalItem.GetItem().ID, regionalItem.GetItem().Language, Sitecore.Data.Version.Latest, Context.Database);
            regionalUrl = c.GetitemUrl(regionalItem.GetItem(), regionalItem.GetItem().Language);
          }
          return Redirect(regionalUrl);
        }
        #endregion Bug #9
        else
        {
          if (empty.ToLower().Equals("canada"))
          {
            property.CityLandingPageButton = "RET";
            property.Display_RegionsDD = true;
            property.IsRegion = true;
          }
          return CitySearchResults(empty, property.IsRegion, searchType, qryLat, qryLng, property.DistanceFromCurrentLocation);
        }
      }
      if (!string.IsNullOrEmpty(property.PropertyName))
      {
        empty = property.PropertyName;
        property.SearchType = "propertyname";
        bool CheckSplitPage = false;
        results = c.CheckForSplitterPage(language, empty, ref CheckSplitPage).ToList();
        if ((results != null && results.Count() == 1) & CheckSplitPage)
        {
          List<SearchResultItem> list = results.ToList();
          string url = list[0].GetField("SplitterURL").ToString();
          return Redirect(url);
        }
        results = SearchResidence(language, empty).ToList();
        if (results != null && results.Count() == 0)
        {
          foundSearchResults = false;
          results = SearchAll(language);
          PropertyList = (from o in ProcessResults(results, language, 9)
                          orderby o.Province
                          select o).ToList();
          PropertyList[0].PageSize = 9;
          string text6 = property.PropertyType = (PropertyList[0].CityLandingPageButton = "RET");
          PropertyList[0].SearchType = property.SearchType;
          PropertyList[0].SearchText = empty.ToTitleCase();
          PropertyList[0].Display_RegionsDD = true;
          PropertyList[0].IsRegion = true;
          PropertyList[0].RegionList = RegionsDDL(language);
          PropertyList[0].FoundCitySearch = foundSearchResults;
          PropertyList[0].Lat = property.Lat;
          PropertyList[0].Lng = property.Lng;
          PropertyList[0].CityLandingPageText = c.GetDictionaryItem("SearchResultsNotFound", language);
          PropertyList[0].DistanceFromCurrentLocation = property.DistanceFromCurrentLocation;
        }
        else
        {
          if (results.Count() == 1)
          {
            List<SearchResultItem> list2 = results.ToList();
            int.TryParse(list2[0].GetField("IsSplitter").Value, out int result);
            if ((result != 0) ? true : false)
            {
              string value = list2[0].GetField("SplitterURL").Value;
              return Redirect(value);
            }
            List<SearchResultItem> list3 = (from x in c.PropertyDetails(list2[0].ItemId)
                                            where x.Language == language
                                            select x).ToList();
            Item item = list3[0].GetItem();
            //if (item.Name.ToLower().Contains(empty.ToLower()) || item.Fields["Property Name"].Value.ToLower().Contains(empty.ToLower()))
            if (item.Name.ToLower().Contains(empty.ToLower()) || item.Fields["Property Name"].Value
                         .Replace("-", " ").Replace("'", "").RemoveDiacritics().ToLower().Contains(empty.ToLower()))
            {
              string itemUrl = LinkManager.GetItemUrl(item, new ItemUrlBuilderOptions
              {
                UseDisplayName = true,
                LowercaseUrls = true,
                LanguageEmbedding = LanguageEmbedding.Always,
                LanguageLocation = LanguageLocation.FilePath,
                Language = item.Language
              });
              //if (itemUrl.Contains("sumach"))
              //{
              //  string url2 = base.Request.Url.Scheme + "://" + base.Request.Url.Host + "/en/the-sumach";
              //  return Redirect(url2);
              //}
              itemUrl = itemUrl + "/" + c.GetDictionaryItem("overview", language);
              return Redirect(itemUrl);
            }
            string itemUrl2 = LinkManager.GetItemUrl(item, new ItemUrlBuilderOptions
            {
              UseDisplayName = true,
              LowercaseUrls = true,
              LanguageEmbedding = LanguageEmbedding.Always,
              LanguageLocation = LanguageLocation.FilePath,
              Language = item.Language
            });
            //if (itemUrl2.Contains("sumach"))
            //{
            //  string url3 = base.Request.Url.Scheme + "://" + base.Request.Url.Host + "/en/the-sumach";
            //  return Redirect(url3);
            //}
            foundSearchResults = false;
            PropertyList = (from o in ProcessResults(results, language, 9)
                            orderby o.Province
                            select o).ToList();
            PropertyList[0].PageSize = 9;
            PropertyList[0].CityLandingPageText = c.GetDictionaryItem("FollowingResults", language);
            PropertyList[0].SearchText = empty.ToTitleCase();
            PropertyList[0].SearchType = "propertyname";
            PropertyList[0].CityLandingPageButton = "RET";
            PropertyList[0].Display_RegionsDD = false;
            PropertyList[0].IsRegion = false;
            PropertyList[0].Lat = property.Lat;
            PropertyList[0].Lng = property.Lng;
            PropertyList[0].DistanceFromCurrentLocation = property.DistanceFromCurrentLocation;
            return PartialView(PropertyList.ToList());
          }
          property.SearchResults = (foundSearchResults = false);
          PropertyList = new List<PropertySearchModel>();
          PropertyList = (from o in ProcessResults(results, language, 9)
                          orderby o.Province
                          select o).ToList();
          PropertyList[0].PageSize = 9;
          string text6 = property.PropertyType = (PropertyList[0].CityLandingPageButton = "RET");
          PropertyList[0].CityLandingPageText = c.GetDictionaryItem("FollowingResults", language);
          PropertyList[0].SearchText = empty.ToTitleCase();
          PropertyList[0].SearchType = "propertyname";
          PropertyList[0].SearchResults = false;

          PropertyList[0].Lat = property.Lat;
          PropertyList[0].Lng = property.Lng;
          PropertyList[0].DistanceFromCurrentLocation = property.DistanceFromCurrentLocation;
        }
        return PartialView(PropertyList.ToList());
      }
      if (!string.IsNullOrEmpty(property.PostalCode))
      {
        empty = property.PostalCode;
        return CitySearchResults(empty, property.IsRegion, searchType, qryLat, qryLng, property.DistanceFromCurrentLocation);
      }
      return PartialView();
    }
    return PartialView();
  }

  private ActionResult CitySearchResults(string searchCriteria, bool? isRegion, string searchType, string qryLat, string qryLng, bool DistanceFromCurrentLocation)
  {
    int pageSize = 9;
    if (searchType != "postalcode")
    {
      results = from x in c.SwitchSelectedCity(language, searchCriteria)
                where x.Language == language
                select x;
      if (results != null && results.Count() > 0)
      {
        if (string.IsNullOrEmpty(qryLat))
        {
          qryLat = results.ToList().FirstOrDefault().GetItem().Fields["Lat"].Value.ToString();
          qryLng = results.ToList().FirstOrDefault().GetItem().Fields["Lng"].Value.ToString();
        }
        PropertyList = CitySearchDistance(results, language, pageSize, "RET", searchCriteria, searchType, qryLat, qryLng, DistanceFromCurrentLocation);
      }
    }
    else
    {
      LatLngForPostalCode(searchCriteria.Replace(" ", "").Replace("-", ""), out string Lat, out string _);
      if (!string.IsNullOrEmpty(Lat)) //&& PropertyList.Count != 0)
      {
        foundSearchResults = true;
      }
      else
      {
        foundSearchResults = false;
      }
    }

    if (results != null && results.Count() > 0 && PropertyList.Count != 0 || foundSearchResults)
    {
      foundSearchResults = !System.Convert.ToBoolean(isRegion);
      if (PropertyList.Count == 0)
        PropertyList = CitySearchDistance(results, language, pageSize, "RET", searchCriteria, searchType, qryLat, qryLng, DistanceFromCurrentLocation);

      if (PropertyList.Count != 0)
      {
        PropertyList[0].SearchText = ((searchType == "postalcode") ? searchCriteria.ToUpper() : searchCriteria.ToTitleCase());
        PropertyList[0].Language = language;
        PropertyList[0].FoundCitySearch = true;
        PropertyList[0].CityLandingPageButton = "RET";
        PropertyList[0].CityLandingPage = ((!System.Convert.ToBoolean(isRegion)) ? true : false);
        PropertyList[0].CityLandingPageText = c.GetDictionaryItem("Retirement Homes in and around", language);
        PropertyList[0].PageSize = 9;
        PropertyList[0].SearchResults = foundSearchResults;
        PropertyList[0].SearchType = searchType;
        PropertyList[0].Lat = qryLat;
        PropertyList[0].Lng = qryLng;
        PropertyList[0].Display_RegionsDD = System.Convert.ToBoolean(isRegion);
        PropertyList[0].IsRegion = isRegion;
        PropertyList[0].RegionList = (System.Convert.ToBoolean(isRegion) ? RegionsDDL(language) : null);
        PropertyList[0].DistanceFromCurrentLocation = DistanceFromCurrentLocation; //!string.IsNullOrEmpty(qryLat);
        if (searchCriteria.ToLower() == "canada")
        {
          if (PropertyList[0].RegionList != null && PropertyList[0].RegionList.Count() > 0)
          {
            SelectListItem selectListItem = (from x in PropertyList[0].RegionList
                                             where x.Text == c.GetDictionaryItem("AllProperties", language)
                                             select x).FirstOrDefault();
            selectListItem.Selected = true;
          }
        }
        else if (PropertyList[0].RegionList != null && PropertyList[0].RegionList.Count() > 0)
        {
          SelectListItem selectListItem2 = (from x in PropertyList[0].RegionList
                                            where x.Text.ToLower() == searchCriteria
                                            select x).FirstOrDefault();
          selectListItem2.Selected = true;
        }
        if (searchType == "postalcode")
        {
          PropertyList[0].CityLandingPageText = ((PropertyList[0].PropertyType == "RET") ? c.GetDictionaryItem("RetirementHomesNear", language) : c.GetDictionaryItem("LongTermCareHomesNear", language));
        }
        else
        {
          PropertyList[0].CityLandingPageText = ((PropertyList[0].PropertyType == "RET") ? c.GetDictionaryItem("Retirement Homes in and around", language) : c.GetDictionaryItem("LongTermCareResidencesInAndAround", language));
        }
        return PartialView(PropertyList);
      }
    }
    results = SearchAll(language);
    PropertyList = ProcessResults(results, language, pageSize);
    string text3 = PropertyList[0].PropertyType = (PropertyList[0].CityLandingPageButton = "RET");
    PropertyList[0].Display_RegionsDD = true;
    PropertyList[0].IsRegion = true;
    PropertyList[0].FoundCitySearch = false;
    PropertyList[0].SearchText = ((searchType == "postalcode") ? searchCriteria.ToUpper() : searchCriteria.ToTitleCase());
    PropertyList[0].CityLandingPageText = c.GetDictionaryItem("SearchResultsNotFound", language);
    PropertyList[0].SearchResults = false;
    PropertyList[0].SearchType = searchType;
    PropertyList[0].PageSize = 9;
    PropertyList[0].RegionList = RegionsDDL(language);
    PropertyList[0].DistanceFromCurrentLocation = !string.IsNullOrEmpty(qryLat);

    SelectListItem selectListItem3 = (from x in PropertyList[0].RegionList
                                      where x.Text == searchCriteria
                                      select x).FirstOrDefault();
    if (selectListItem3 != null)
    {
      selectListItem3.Selected = true;
    }
    else
    {
      selectListItem3 = (from x in PropertyList[0].RegionList
                         where x.Text == c.GetDictionaryItem("AllProperties", language)
                         select x).FirstOrDefault();
      selectListItem3.Selected = true;
    }
    return PartialView(PropertyList.ToList());
  }

  public List<SelectListItem> RegionsDDL(string Language)
  {
    List<SelectListItem> list = new List<SelectListItem>();
    string empty = string.Empty;
    empty = base.Request.Url.ToString();
    list.Add(new SelectListItem
    {
      Text = c.GetDictionaryItem("AllProperties", Language),
      Value = (base.Request.Url.Scheme + "://" + base.Request.Url.Host + "/" + Language + "/" + c.GetDictionaryItem("SearchResults", Language) + "/?" + c.GetDictionaryItem("Region", Language) + "=" + c.GetDictionaryItem("AllProperties", Language).Replace(" ", "-")).ToLower()
    });
    list.Add(new SelectListItem
    {
      Text = c.GetDictionaryItem("Alberta", Language),
      Value = (base.Request.Url.Scheme + "://" + base.Request.Url.Host + "/" + Language + "/" + c.GetDictionaryItem("SearchResults", Language) + "/?" + c.GetDictionaryItem("Region", Language) + "=" + c.GetDictionaryItem("Alberta", Language)).ToLower()
    });
    list.Add(new SelectListItem
    {
      Text = c.GetDictionaryItem("BritishColumbia", Language),
      Value = (base.Request.Url.Scheme + "://" + base.Request.Url.Host + "/" + Language + "/" + c.GetDictionaryItem("SearchResults", Language) + "/?" + c.GetDictionaryItem("Region", Language) + "=" + c.GetDictionaryItem("BritishColumbia", Language).Replace(" ", "-")).ToLower()
    });
    list.Add(new SelectListItem
    {
      Text = c.GetDictionaryItem("Ontario", Language),
      Value = (base.Request.Url.Scheme + "://" + base.Request.Url.Host + "/" + Language + "/" + c.GetDictionaryItem("SearchResults", Language) + "/?" + c.GetDictionaryItem("Region", Language) + "=" + c.GetDictionaryItem("Ontario", Language)).ToLower()
    });
    list.Add(new SelectListItem
    {
      Text = c.GetDictionaryItem("Quebec", Language),
      Value = (base.Request.Url.Scheme + "://" + base.Request.Url.Host + "/" + Language + "/" + c.GetDictionaryItem("SearchResults", Language) + "/?" + c.GetDictionaryItem("Region", Language) + "=" + c.GetDictionaryItem("Quebec", Language)).ToLower()
    });
    return list.ToList();
  }

  private void LatLngForPostalCode(string searchCriteria, out string Lat, out string Lng)
  {
    SqlDataReader sqlDataReader = null;
    SqlConnection sqlConnection = new SqlConnection(constring);
    sqlConnection.Open();
    SqlCommand sqlCommand = new SqlCommand("sp_SCGeoNamesPostalCode", sqlConnection)
    {
      CommandType = CommandType.StoredProcedure
    };
    sqlCommand.Parameters.AddWithValue("@PostCode", searchCriteria.Substring(0, 3));
    sqlDataReader = sqlCommand.ExecuteReader();
    Lat = string.Empty;
    Lng = string.Empty;
    do
    {
      if (!sqlDataReader.Read())
      {
        return;
      }
    }
    while (string.IsNullOrWhiteSpace(sqlDataReader["City"].ToString()));
    Lat = System.Convert.ToDouble(sqlDataReader["Lat"]).ToString().Replace(",", ".");
    Lng = System.Convert.ToDouble(sqlDataReader["Lng"]).ToString().Replace(",", ".");
  }

  private IEnumerable<SearchResultItem> SearchAll(string language)
  {
    using (IProviderSearchContext providerSearchContext = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
    {
      results = (from x in providerSearchContext.GetQueryable<SearchResultItem>()
                 where x.TemplateName == "PropertyPage"
                 where x.Language == language
                 where x.Name != "__Standard Values"
                 where x["Property ID"] != "99999"
                 select x into o
                 orderby o.Name
                 select o).ToList();
    }
    return results.ToList();
  }

  private IEnumerable<SearchResultItem> SearchPostalCode(string language, string searchCriteria)
  {
    using (IProviderSearchContext providerSearchContext = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
    {
      results = (from x in providerSearchContext.GetQueryable<SearchResultItem>()
                 where x.Path.StartsWith("/sitecore/content/Chartwell/Project/retirement-residences")
                 where x.TemplateName.Equals("PropertyPage")
                 where x.Language == language
                 where x["Property ID"] != "99999"
                 select x into o
                 orderby o.Name
                 select o).Take(1);
    }
    return results.ToList();
  }

  private IEnumerable<SearchResultItem> SearchResidence(string language, string searchCriteria)
  {
    searchCriteria = searchCriteria.RemoveDiacritics().Replace("'", "").Replace("-", " ")
        .TrimPunctuation()
        .ToLower();
    using (IProviderSearchContext providerSearchContext = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
    {
      results = (from x in providerSearchContext.GetQueryable<SearchResultItem>()
                 where x.TemplateName == "PropertyPage"
                 where x.Language == language
                 where x.Name.Contains(searchCriteria)
                 where x.Name != "__Standard Values"
                 where x["property ID"] != "99999"
                 select x into o
                 orderby o.Name
                 select o).ToList();
    }
    if (results.Count() == 0)
    {
      using (IProviderSearchContext providerSearchContext2 = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        results = (from x in providerSearchContext2.GetQueryable<SearchResultItem>()
                   where x.TemplateName == "PropertyPage"
                   where x.Language == language
                   where x.Name != "__Standard Values"
                   where x["property ID"] != "99999"
                   select x into o
                   orderby o.Name
                   select o).ToList();
      }
      //List<SearchResultItem> list = (List<SearchResultItem>)(results = (from x in results
      //                                                                  where x.GetField("Property Name").Value == searchCriteria
      //                                                                  select x).ToList());

      List<SearchResultItem> list = (List<SearchResultItem>)(results = (from x in results
                                                                        where x.GetItem().DisplayName.RemoveDiacritics().Replace("'", "").Replace("-", " ").TrimPunctuation().ToLower() == searchCriteria
                                                                        select x).ToList());

    }
    return results.ToList();
  }

  private List<PropertySearchModel> ProcessResults(IEnumerable<SearchResultItem> results, string Language, int pageSize)
  {
    PropertyList = new List<PropertySearchModel>();
    ChartwellUtiles util = new ChartwellUtiles();
    IEnumerable<PropertySearchModel> source = (from item in results
                                               select new PropertySearchModel
                                               {
                                                 ItemID = item.ItemId.ToString(),
                                                 PropertyID = item.GetField("property id").Value,
                                                 PropertyName = item.GetField("property name").Value,
                                                 PropertyType = c.PropertyType(Language, item.GetItem()),
                                                 PropertyDesc = item.GetField("property description").Value,
                                                 PropertyTagLine = item.GetField("property tag line").Value,
                                                 USP = item.GetField("USP").Value,
                                                 PhoneNo = util.GetPhoneNumber(item.GetItem()),
                                                 StreetName = item.GetField("Street name and number").Value,
                                                 City = util.CityName(Language, item.GetItem()), //item.GetField("Selected City").Value,
                                                 PostalCode = item.GetField("Postal Code").Value,
                                                 Province = ProvinceName(Language, item.GetItem()),
                                                 Country = item.GetField("Country").Value,
                                                 PropertyItemUrl = GetPropertyUrl(Language, item.GetItem()),
                                                 PropertyFormattedAddress = new HtmlString(util.FormattedAddress(item.GetItem(), ProvinceName(Language, item.GetItem()))),
                                                 InnerItem = item.GetItem(),
                                                 PropertyImage = GetImageUrl(item.GetItem()),
                                                 CityLandingPageText = ((c.PropertyType(Language, item.GetItem()) == "RET") ? "Retirement Homes" : "Long Term Care Residences"),
                                                 PageTitle = item.GetField("PageTitle").Value,
                                                 PageDescription = item.GetField("pagedescription").Value,
                                                 PageKeyword = item.GetField("pagekeyword").Value,
                                                 SearchResults = (foundSearchResults ? true : false),
                                                 PageSize = 9,
                                                 Language = Language,
                                                 PageStartIndex = 0
                                               } into o
                                               orderby o.Province
                                               select o).Skip(0).Take(pageSize);
    PropertyList = source.ToList();
    return (from o in PropertyList
            orderby o.Province
            select o).ToList();
  }

  public ActionResult CityLandingPage(string SearchText, string CityLandingPageButton, string SearchType, int PageSize, bool? IsRegion, string Language, bool? FoundCitySearch, string Lat, string Lng, bool DistanceFromCurrentLocation)
  {
    PropertySearchModel propertySearchModel = new PropertySearchModel();
    string text = base.Request.Params[0].ToLower();
    string text2 = base.Request.Params.Keys[0];
    if (System.Convert.ToBoolean(IsRegion))
    {
      if (text.Equals(Translate.Text("AllProperties")))
      {
        propertySearchModel.City = "canada";
      }
      else
      {
        propertySearchModel.City = text;
      }
      propertySearchModel.IsRegion = true;
      FoundCitySearch = ((SearchType == c.GetDictionaryItem("Region", Language).ToLower()) | FoundCitySearch);
    }
    else
    {
      propertySearchModel.Display_RegionsDD = false;
      propertySearchModel.IsRegion = false;
      propertySearchModel.Lat = Lat;
      propertySearchModel.Lng = Lng;
    }
    if (!string.IsNullOrEmpty(SearchType) && SearchType.Contains("propertyname"))
    {
      results = SearchResidence(Language, SearchText);
      if (results != null && results.Count() == 0)
      {
        results = SearchAll(Language);
      }
      PageSize += 9;
      PropertyList = (from o in ProcessResults(results, Language, PageSize)
                      orderby o.Province
                      select o).ToList();
      PropertyList[0].CityLandingPageText = ((!string.IsNullOrEmpty(CityLandingPageButton) && System.Convert.ToBoolean(!IsRegion)) ? Translate.Text("FollowingResults") : Translate.Text("SearchResultsNotFound"));
      PropertyList[0].SearchText = ((SearchType == "postalcode") ? SearchText.ToUpper() : SearchText.ToTitleCase());
      PropertyList[0].SearchType = "propertyname";
      PropertyList[0].SearchResults = false;
      PropertyList[0].Display_RegionsDD = (System.Convert.ToBoolean(IsRegion) ? true : false);
      PropertyList[0].IsRegion = System.Convert.ToBoolean(IsRegion);
      PropertyList[0].PageSize = PageSize;
      PropertyList[0].CityLandingPageButton = CityLandingPageButton;
      PropertyList[0].Lat = Lat;
      PropertyList[0].Lng = Lng;
      PropertyList[0].DistanceFromCurrentLocation = DistanceFromCurrentLocation; // !string.IsNullOrEmpty(Lat);

      if (System.Convert.ToBoolean(IsRegion))
      {
        PropertyList[0].RegionList = RegionsDDL(Language);
      }
    }
    else if (!string.IsNullOrEmpty(SearchType) && (SearchType.ToLower() == c.GetDictionaryItem("CitySearch", Language).ToLower() || SearchType.Contains("postalcode") || SearchType.Contains(c.GetDictionaryItem("Region", Language).ToLower())))
    {
      PageSize += 9;
      if (System.Convert.ToBoolean(FoundCitySearch))
      {
        if (SearchType.ToLower() == c.GetDictionaryItem("CitySearch", Language).ToLower() || SearchType.ToLower() == c.GetDictionaryItem("Region", Language).ToLower())
        {
          results = (from x in c.SwitchSelectedCity(Language, SearchText)
                     where x.Language == Language
                     select x).ToList();
        }
        else
        {
          results = SearchPostalCode(language, SearchText);
        }
        PropertyList = CitySearchDistance(results, Language, PageSize, CityLandingPageButton, SearchText, SearchType, Lat, Lng, DistanceFromCurrentLocation);
        PropertyList[0].SearchType = SearchType;
        if (SearchType == "postalcode")
        {
          PropertyList[0].CityLandingPageText = ((PropertyList[0].PropertyType == "RET") ? c.GetDictionaryItem("RetirementHomesNear", Language) : c.GetDictionaryItem("LongTermCareHomesNear", Language));
        }
        else
        {
          PropertyList[0].CityLandingPageText = ((PropertyList[0].PropertyType == "RET") ? c.GetDictionaryItem("Retirement Homes in and around", Language) : c.GetDictionaryItem("LongTermCareResidencesInAndAround", Language));
        }
        PropertyList[0].SearchText = ((SearchType == "postalcode") ? SearchText.ToUpper() : SearchText.ToTitleCase());
        PropertyList[0].Language = Language;
        PropertyList[0].CityLandingPage = true;
        PropertyList[0].CityLandingPageButton = CityLandingPageButton;
        PropertyList[0].SearchResults = ((!System.Convert.ToBoolean(IsRegion)) ? true : false);
        PropertyList[0].PageSize = PageSize;
        PropertyList[0].FoundCitySearch = ((!System.Convert.ToBoolean(IsRegion)) ? true : false);
        PropertyList[0].Display_RegionsDD = (System.Convert.ToBoolean(IsRegion) ? true : false);
        PropertyList[0].IsRegion = System.Convert.ToBoolean(IsRegion);
        PropertyList[0].RegionList = (System.Convert.ToBoolean(IsRegion) ? RegionsDDL(Language) : null);
        PropertyList[0].Lat = Lat;
        PropertyList[0].Lng = Lng;
        PropertyList[0].DistanceFromCurrentLocation = DistanceFromCurrentLocation; // !string.IsNullOrEmpty(Lat);

        if (PropertyList[0].RegionList != null && PropertyList[0].RegionList.Count() > 0)
        {
          SelectListItem selectListItem = (from x in PropertyList[0].RegionList
                                           where x.Text == SearchText
                                           select x).FirstOrDefault();
          if (selectListItem != null)
          {
            selectListItem.Selected = true;
          }
          else
          {
            selectListItem = (from x in PropertyList[0].RegionList
                              where x.Text == c.GetDictionaryItem("AllProperties", language)
                              select x).FirstOrDefault();
            selectListItem.Selected = true;
          }
        }
      }
      else
      {
        results = SearchAll(Language);
        PropertyList = ProcessResults(results, language, PageSize);
        PropertyList[0].SearchType = SearchType;
        PropertyList[0].SearchText = ((SearchType == "postalcode") ? SearchText.ToUpper() : SearchText.ToTitleCase());
        PropertyList[0].CityLandingPageButton = CityLandingPageButton;
        PropertyList[0].Language = Language;
        PropertyList[0].CityLandingPageText = c.GetDictionaryItem("SearchResultsNotFound", Language);
        PropertyList[0].SearchResults = false;
        PropertyList[0].Display_RegionsDD = (System.Convert.ToBoolean(IsRegion) ? true : false);
        PropertyList[0].IsRegion = System.Convert.ToBoolean(IsRegion);
        PropertyList[0].PageSize = PageSize;
        PropertyList[0].FoundCitySearch = false;
        PropertyList[0].Lat = Lat;
        PropertyList[0].Lng = Lng;
        PropertyList[0].DistanceFromCurrentLocation = !string.IsNullOrEmpty(Lat);

        PropertyList[0].RegionList = (System.Convert.ToBoolean(IsRegion) ? RegionsDDL(Language) : null);
        if (PropertyList[0].RegionList != null && PropertyList[0].RegionList.Count() > 0)
        {
          SelectListItem selectListItem2 = (from x in PropertyList[0].RegionList
                                            where x.Text == SearchText
                                            select x).FirstOrDefault();
          if (selectListItem2 != null)
          {
            selectListItem2.Selected = true;
          }
          else
          {
            selectListItem2 = (from x in PropertyList[0].RegionList
                               where x.Text == c.GetDictionaryItem("AllProperties", language)
                               select x).FirstOrDefault();
            selectListItem2.Selected = true;
          }
        }
      }
    }
    return PartialView(PropertyList);
  }

  public PartialViewResult Index()
  {
    return PartialView("~/Views/SearchResultsGrid/RandomIndex.cshtml", RandomModel());
  }

  private PropertyCustomModel RandomModel()
  {
    Item item = PageContext.Current.Item;
    List<Item> list = new List<Item>();
    MultilistField multilistField = item.Fields["Property"];
    string splitterPageTitle = item.Fields["Title"].ToString();
    string splitterPageDescription = item.Fields["Description"].ToString();
    if (multilistField != null && multilistField.TargetIDs != null)
    {
      ID[] targetIDs = multilistField.TargetIDs;
      foreach (ID itemID in targetIDs)
      {
        Item item2 = c.GetItemById(itemID);
        list.Add(item2);
      }
    }
    List<PropertySearchModel> list2 = new List<PropertySearchModel>();
    foreach (Item item6 in list)
    {
      var item3 = c.GetItemById(item6.ID);
      string itemUrl = LinkManager.GetItemUrl(item3, new ItemUrlBuilderOptions
      {
        UseDisplayName = true,
        LowercaseUrls = true,
        LanguageEmbedding = LanguageEmbedding.Always,
        LanguageLocation = LanguageLocation.FilePath,
        Language = Context.Language
      });
      string id = item3.Fields["Province"].ToString();
      Item item4 = c.GetItemByStringId(id);
      string text = item4.Fields["Province Name"].ToString();
      ChartwellUtiles _c = new ChartwellUtiles();
      string value = _c.FormattedAddress(item3, text);
      PropertySearchModel item5 = new PropertySearchModel
      {
        ItemID = item6.ID.ToString(),
        PropertyID = item6.Fields["property id"].ToString(),
        PropertyName = item6.Fields["Property Name"].ToString(),
        PropertyDesc = item6.Fields["Property Description"].ToString(),
        PropertyTagLine = item6.Fields["Property Tag Line"].ToString(),
        USP = item6.Fields["USP"].ToString(),
        PhoneNo = _c.GetPhoneNumber(item6),
        StreetName = item6.Fields["Street name and number"].ToString(),
        City = _c.CityName(item6.Language.Name, item6), // item6.Fields["Selected City"].Value,
        PostalCode = item6.Fields["Postal code"].ToString(),
        Province = text,
        PropertyItemUrl = itemUrl + "/" + Translate.Text("overview"),
        PropertyFormattedAddress = new HtmlString(value),
        InnerItem = item3,
        Longitude = item6.Fields["Longitude"].ToString(),
        Latitude = item6.Fields["Latitude"].ToString()
      };
      list2.Add(item5);
    }
    return new PropertyCustomModel
    {
      lstProperty = list2,
      SplitterPageTitle = splitterPageTitle,
      SplitterPageDescription = splitterPageDescription
    };
  }

  public PartialViewResult SplitterPageIndex()
  {
    return PartialView("~/Views/SearchResultsGrid/SplitterPage.cshtml", CreateSplitterModel());
  }

  private PropertyCustomModel CreateSplitterModel()
  {
    Database database = Context.Database;
    Item item = Context.Item;
    List<Item> list = new List<Item>();
    MultilistField multilistField = item.Fields["SelectedProperty"];
    string splitterPageTitle = item.Fields["Title"].ToString();
    string splitterPageDescription = item.Fields["Description"].ToString();
    if (multilistField != null && multilistField.TargetIDs != null)
    {
      ID[] targetIDs = multilistField.TargetIDs;
      foreach (ID itemID in targetIDs)
      {
        Item item2 = c.GetItemById(itemID);
        list.Add(item2);
      }
    }
    List<PropertySearchModel> list2 = new List<PropertySearchModel>();
    int num = 0;
    foreach (Item item6 in list)
    {
      num++;
      Item item3 = c.GetItemById(item6.ID);
      string itemUrl = LinkManager.GetItemUrl(item3, new ItemUrlBuilderOptions
      {
        UseDisplayName = true,
        LowercaseUrls = true,
        LanguageEmbedding = LanguageEmbedding.Always,
        LanguageLocation = LanguageLocation.FilePath,
        Language = Context.Language
      });
      ChartwellUtiles _c = new ChartwellUtiles();
      string id = item3.Fields["Province"].ToString();
      ID itemId = new ID(id);
      Item item4 = c.GetItemById(itemId);
      string text = item4.Fields["Province Name"].ToString();
      string fieldName = "PropertyDescription" + num;
      string value = _c.FormattedAddress(item3, text);
      PropertySearchModel item5 = new PropertySearchModel
      {
        ItemID = item6.ID.ToString(),
        PropertyID = item6.Fields["property id"].ToString(),
        PropertyName = item6.Fields["Property Name"].ToString(),
        PropertyDesc = item6.Fields["Property Description"].ToString(),
        PropertyTagLine = item6.Fields["Property Tag Line"].ToString(),
        PhoneNo = _c.GetPhoneNumber(item6),
        USP = item.Fields[fieldName].ToString(),
        StreetName = item6.Fields["Street name and number"].ToString(),
        City = _c.CityName(item6.Language.Name, item6), // item6.Fields["Selected City"].Value,
        PostalCode = item6.Fields["Postal code"].ToString(),
        Province = text,
        PropertyItemUrl = itemUrl + "/" + Translate.Text("overview"),
        PropertyFormattedAddress = new HtmlString(value),
        InnerItem = item3,
        Longitude = item6.Fields["Longitude"].ToString(),
        Latitude = item6.Fields["Latitude"].ToString(),
        SplitterPageTitle = splitterPageTitle,
        SplitterPageDescription = splitterPageDescription
      };
      list2.Add(item5);
    }
    return new PropertyCustomModel
    {
      lstProperty = list2,
      SplitterPageTitle = splitterPageTitle,
      SplitterPageDescription = splitterPageDescription
    };
  }

  public PartialViewResult NewPropertyIndex()
  {
    return PartialView("~/Views/SearchResultsGrid/NewResidencePage.cshtml", CreateNewPropertyModel());
  }

  private PropertyCustomModel CreateNewPropertyModel()
  {
    List<Item> list = new List<Item>();
    HashSet<string> lstProvinces = new HashSet<string>();
    List<PropertySearchModel> lstNewResidences = new List<PropertySearchModel>();
    list = c.NewestResidencesList();
    foreach (Item item in list)
    {
      string itemUrl = LinkManager.GetItemUrl(item, new ItemUrlBuilderOptions
      {
        UseDisplayName = true,
        LowercaseUrls = true,
        LanguageEmbedding = LanguageEmbedding.Always,
        LanguageLocation = LanguageLocation.FilePath,
        Language = Context.Language
      });
      lstProvinces.Add(ProvinceName(Context.Language.Name, item));
      PropertySearchModel NewResidencePageModel = new PropertySearchModel
      {
        ItemID = item.ID.ToString(),
        PropertyID = item.Fields["property id"].ToString(),
        PropertyName = item.Fields["Property Name"].ToString(),
        PropertyDesc = item.Fields["Property Description"].ToString(),
        PropertyTagLine = item.Fields["Property Tag Line"].ToString(),
        USP = item.Fields["USP"].ToString(),
        PhoneNo = c.GetPhoneNumber(item),
        StreetName = item.Fields["Street name and number"].ToString(),
        City = c.CityName(item.Language.Name, item), // item.Fields["Selected City"].Value,
        PostalCode = item.Fields["Postal code"].ToString(),
        Province = ProvinceName(Context.Language.Name, item),
        PropertyItemUrl = itemUrl + "/" + Translate.Text("overview"),
        PropertyFormattedAddress = new HtmlString(c.FormattedAddress(item, ProvinceName(Context.Language.Name, item))), // new HtmlString(value),
        InnerItem = item,
        Longitude = item.Fields["Longitude"].ToString(),
        Latitude = item.Fields["Latitude"].ToString()
      };
      lstNewResidences.Add(NewResidencePageModel);
    }
    return new PropertyCustomModel
    {
      lstProperty = lstNewResidences,
      lstPropertyProvince = lstProvinces
    };
  }

  public List<PropertySearchModel> CitySearchDistance(IEnumerable<SearchResultItem> queryResult, string Language, int PageSize, string paramPropertyType, string searchCriteria, string searchType, string qryLat, string qryLng, bool DistanceFromCurrentLocation)
  {
    IEnumerable<PropertySearchModel> source = null;
    string strLatitude = string.Empty;
    string strLongitude = string.Empty;
    if (searchType == "postalcode")
    {
      LatLngForPostalCode(searchCriteria.Replace(" ", ""), out string Lat, out string Lng);
      strLatitude = Lat;
      strLongitude = Lng;

      if (string.IsNullOrEmpty(qryLat) || qryLat.Equals("lat"))
      {
        qryLat = strLatitude;
        qryLng = strLongitude;
      }
    }
    else
    {
      SearchResultItem searchResultItem = (from x in c.PropertyDetails(queryResult.ToList()[0].ItemId)
                                           where x.Language == Language
                                           select x).ToList().FirstOrDefault();
      strLatitude = searchResultItem.Fields["lat"].ToString();
      strLongitude = searchResultItem.Fields["lng"].ToString();
      if (string.IsNullOrEmpty(qryLat) || qryLat.Equals("lat"))
      {
        qryLat = strLatitude;
        qryLng = strLongitude;
      }
    }
    string o = strLatitude + "," + strLongitude;
    ChartwellUtiles util = new ChartwellUtiles();
    List<PropertySearchModel> list = new List<PropertySearchModel>();
    using (IProviderSearchContext providerSearchContext = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
    {
      Expression<Func<SearchResultItem, bool>> first = PredicateBuilder.True<SearchResultItem>();
      first = first.And((SearchResultItem d) => d.Language == Language);
      first = first.And((SearchResultItem d) => d.TemplateName == "PropertyPage");
      first = first.And((SearchResultItem d) => d["Property ID"] != "99999");
      first = first.And((SearchResultItem d) => d.Name != "__Standard Values");
      List<SearchResultItem> source2 = providerSearchContext.GetQueryable<SearchResultItem>().Where(first).WithinRadius((SearchResultItem s) => s.Location, o, 1000.0)
          .OrderByDistance((SearchResultItem d) => d.Location, o)
          .ToList();
      source = (from item in source2
                select new PropertySearchModel
                {
                  SearchText = searchCriteria,
                  ItemID = item.ItemId.ToString(),
                  PropertyID = item.GetField("property id").Value,
                  PropertyName = item.GetField("property name").Value,
                  PropertyType = c.PropertyType(Language, item.GetItem()),
                  PropertyDesc = item.GetField("property description").Value,
                  PropertyTagLine = item.GetField("property tag line").Value,
                  USP = item.GetField("USP").Value,
                  PhoneNo = util.GetPhoneNumber(item.GetItem()),
                  StreetName = item.GetField("Street name and number").Value,
                  City = util.CityName(Language, item.GetItem()), // item.GetField("Selected City").Value,
                  PostalCode = item.GetField("Postal Code").Value,
                  Province = ProvinceName(Language, item.GetItem()),
                  Country = item.GetField("Country").Value,
                  PropertyItemUrl = GetPropertyUrl(Language, item.GetItem()),
                  PropertyFormattedAddress = new HtmlString(util.FormattedAddress(item.GetItem(), ProvinceName(Language, item.GetItem()))),
                  InnerItem = item.GetItem(),
                  Distance = string.Format("{0:0.0}", c.Distance(double.Parse(qryLat, CultureInfo.InvariantCulture), double.Parse(qryLng, CultureInfo.InvariantCulture), item.Location.Latitude, item.Location.Longitude, 'K')) + " km",
                  CityCentreDistance = string.Format("{0:0.0}", c.Distance(double.Parse(strLatitude, CultureInfo.InvariantCulture), double.Parse(strLongitude, CultureInfo.InvariantCulture), item.Location.Latitude, item.Location.Longitude, 'K')) + " km",
                  PropertyImage = GetImageUrl(item.GetItem()),
                  CityLandingPageText = ((c.PropertyType(Language, item.GetItem()) == "RET") ? "Retirement Homes" : "Long Term Care Residences"),
                  PageTitle = item.GetField("PageTitle").Value,
                  PageDescription = item.GetField("pagedescription").Value,
                  PageKeyword = item.GetField("pagekeyword").Value,
                  SearchResults = (foundSearchResults ? true : false),
                  PageSize = 9,
                  Language = language,
                  PageStartIndex = 0,
                  DistanceFromCurrentLocation = DistanceFromCurrentLocation
                  //Lat = qryLat,
                  //Lng = qryLng
                } into x
                where x.PropertyType == paramPropertyType
                select x).Skip(0).Take(PageSize).ToList();
    }
    return source.ToList();
  }

  private string GetPropertyUrl(string Language, Item PropertyItem)
  {
    string itemUrl = LinkManager.GetItemUrl(PropertyItem, new ItemUrlBuilderOptions
    {
      UseDisplayName = true,
      LowercaseUrls = true,
      AlwaysIncludeServerUrl = false,
      LanguageEmbedding = LanguageEmbedding.Always,
      LanguageLocation = LanguageLocation.FilePath,
      Language = PropertyItem.Language
    });
    string empty = string.Empty;
    // empty = ((!itemUrl.Contains("sumach")) ? (itemUrl + "/" + Translate.TextByLanguage("overview", PropertyItem.Language)) : (base.Request.Url.Scheme + "://" + base.Request.Url.Host + "/en/the-sumach"));
    empty = itemUrl + "/" + Translate.TextByLanguage("overview", PropertyItem.Language);
    return empty.Replace("/sitecore/shell/chartwell", "");
  }

  private string GetImageUrl(Item SearchPropertyItem)
  {
    ImageField imageField = SearchPropertyItem.Fields["Thumbnail Photo"];
    string text = (imageField != null) ? MediaManager.GetMediaUrl(imageField.MediaItem) : string.Empty;
    return text.Replace("/sitecore/shell", "");
  }

  private string ProvinceName(string Language, Item SearchPropertyItem)
  {
    string text = SearchPropertyItem.Fields["Province"].ToString();
    string result = string.Empty;
    if (!string.IsNullOrEmpty(text))
    {
      ID itemID = new ID(text);
      List<SearchResultItem> list = (from x in c.PropertyDetails(itemID)
                                     where x.Language == Language
                                     select x).ToList();
      Item item = list[0].GetItem();
      result = item.Fields["Province Name"].ToString();
    }
    return result;
  }

  public List<PropertySearchModel> PostalCodeDistance(IEnumerable<SearchResultItem> queryResult, string lat, string lng, string Language)
  {
    List<PropertySearchModel> list = new List<PropertySearchModel>();
    foreach (SearchResultItem item6 in queryResult)
    {
      string o = lat + "," + lng;
      IProviderSearchContext providerSearchContext = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext();
      IOrderedQueryable<SearchResultItem> source = (from item in providerSearchContext.GetQueryable<SearchResultItem>()
                                                    where item.Language == Language
                                                    select item).WithinRadius((SearchResultItem s) => s.Location, o, 500.0).OrderByDistance((SearchResultItem s) => s.Location, o);
      List<SearchResultItem> list2 = source.ToList();
      foreach (SearchResultItem item7 in list2)
      {
        List<SearchResultItem> list3 = (from x in c.PropertyDetails(item7.ItemId)
                                        where x.Language == Language
                                        select x).ToList();
        Item item2 = list3[0].GetItem();
        ChartwellUtiles chartwellUtiles = new ChartwellUtiles();
        string itemUrl = LinkManager.GetItemUrl(item2, new ItemUrlBuilderOptions
        {
          UseDisplayName = true,
          LowercaseUrls = true,
          LanguageEmbedding = LanguageEmbedding.Always,
          LanguageLocation = LanguageLocation.FilePath,
          Language = item2.Language
        });
        string text = itemUrl + "/" + c.GetDictionaryItem("overview", Language);
        string text2 = item2.Fields["Province"].ToString();
        string text3 = string.Empty;
        if (!string.IsNullOrEmpty(text2))
        {
          ID itemID = new ID(text2);
          List<SearchResultItem> list4 = (from x in c.PropertyDetails(itemID)
                                          where x.Language == Language
                                          select x).ToList();
          Item item3 = list4[0].GetItem();
          text3 = item3.Fields["Province Name"].ToString();
        }
        string text4 = item2.Fields["property type"].ToString();
        string text5 = string.Empty;
        if (!string.IsNullOrEmpty(text4))
        {
          ID itemID2 = new ID(text4);
          List<SearchResultItem> list5 = (from x in c.PropertyDetails(itemID2)
                                          where x.Language == Language
                                          select x).ToList();
          Item item4 = list5[0].GetItem();
          text5 = item4.Fields["property type"].ToString();
        }
        string value = item7.GetField("property name").Value;
        ImageField imageField = item2.Fields["Thumbnail Photo"];
        string mediaUrl = MediaManager.GetMediaUrl(imageField.MediaItem);
        string distance = string.Format("{0:0.0}", c.Distance(double.Parse(lat, CultureInfo.InvariantCulture), double.Parse(lng, CultureInfo.InvariantCulture), item7.Location.Latitude, item7.Location.Longitude, 'K')) + " km";
        string value2 = chartwellUtiles.FormattedAddress(item2, text3);
        PropertySearchModel item5 = new PropertySearchModel
        {
          ItemID = item7.ItemId.ToString(),
          PropertyID = item7.GetField("property id").Value,
          PropertyName = value,
          PropertyType = text5,
          PropertyDesc = item7.GetField("property description").Value,
          PropertyTagLine = item7.GetField("property tag line").Value,
          USP = item7.GetField("USP").Value,
          PhoneNo = chartwellUtiles.GetPhoneNumber(item2),
          StreetName = item7.GetField("Street name and number").Value,
          City = chartwellUtiles.CityName(item7.Language, item7.GetItem()), // item7.GetField("Selected City").Value,
          PostalCode = item7.GetField("Postal Code").Value,
          Province = text3,
          Country = item7.GetField("Country").Value,
          PropertyItemUrl = text.Replace("/sitecore/shell/chartwell", ""),
          PropertyFormattedAddress = new HtmlString(value2),
          Longitude = item7.GetField("Longitude").Value,
          Latitude = item7.GetField("Latitude").Value,
          InnerItem = item2,
          Distance = distance,
          PropertyImage = mediaUrl.Replace("/sitecore/shell", ""),
          Language = Language,
          CityLandingPageText = ((text5 == "RET") ? "Retirement Homes" : "Long Term Care Residences"),
          SearchResults = (foundSearchResults ? true : false)
        };
        list.Add(item5);
        base.TempData["ListWithDistance"] = list;
      }
    }
    return list;
  }

  [HttpPost]
  public async Task<JsonResult> LatLngSearch(string Latitude, string Longitude, string SearchText, string PropertyType, int PageSize, string Language)
  {
    List<PropertySearchModel> model = new List<PropertySearchModel>();

    var LocationDetails = c.GeoNameLocation(Latitude, Longitude).FirstOrDefault();

    PostalCodeModel PCModel = await c.GetLocDetailsFromCanadianPostalCodeDB(LocationDetails, constring);

    var nearbyList = from x in c.SwitchSelectedCity(Language, SearchText)
                     where x.Language == language
                     select x;

    SearchResultItem searchResultItem = (from x in c.PropertyDetails(nearbyList.ToList()[0].ItemId)
                                         where x.Language == Language
                                         select x).ToList().FirstOrDefault();

    var strLatitude = searchResultItem.Fields["lat"].ToString();
    var strLongitude = searchResultItem.Fields["lng"].ToString();

    string o = strLatitude + "," + strLongitude;
    ChartwellUtiles util = new ChartwellUtiles();
    List<PropertySearchModel> list = new List<PropertySearchModel>();
    using (IProviderSearchContext providerSearchContext = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
    {
      Expression<Func<SearchResultItem, bool>> first = PredicateBuilder.True<SearchResultItem>();
      first = first.And((SearchResultItem d) => d.Language == Language);
      first = first.And((SearchResultItem d) => d.TemplateName == "PropertyPage");
      first = first.And((SearchResultItem d) => d["Property ID"] != "99999");
      first = first.And((SearchResultItem d) => d.Name != "__Standard Values");
      var source2 = providerSearchContext.GetQueryable<SearchResultItem>().Where(first).WithinRadius((SearchResultItem s) => s.Location, o, 1000.0)
                                                            .OrderByDistance((SearchResultItem d) => d.Location, o)
                                                            .ToList()
                                                            .Where(i => c.GetItemById(new ID(i.GetItem().Fields["property type"].Value)).Name.Equals(PropertyType))
                                                            .Select(s => new PropertySearchModel
                                                            {
                                                              City = PCModel.City.ToTitleCase(),
                                                              PropertyType = c.PropertyType(Language, s.GetItem()),
                                                              CityCentreDistance = string.Format("{0:0.0}", c.Distance(double.Parse(strLatitude, CultureInfo.InvariantCulture), double.Parse(strLongitude, CultureInfo.InvariantCulture), s.Location.Latitude, s.Location.Longitude, 'K')) + " km",
                                                              Distance = string.Format("{0:0.0}", c.Distance(double.Parse(LocationDetails.lat.ToString(), CultureInfo.InvariantCulture), double.Parse(LocationDetails.lng.ToString(), CultureInfo.InvariantCulture), s.Location.Latitude, s.Location.Longitude, 'K')) + " km",
                                                            }).Take(PageSize).ToList();

      //var source = (from item in source2
      //              select new PropertySearchModel
      //              {
      //                City = PCModel.City.ToTitleCase(),
      //                PropertyType = c.PropertyType(Language, item.GetItem()),
      //                CityCentreDistance = string.Format("{0:0.0}", c.Distance(double.Parse(strLatitude, CultureInfo.InvariantCulture), double.Parse(strLongitude, CultureInfo.InvariantCulture), item.Location.Latitude, item.Location.Longitude, 'K')) + " km",
      //                Distance = string.Format("{0:0.0}", c.Distance(double.Parse(LocationDetails.lat.ToString(), CultureInfo.InvariantCulture), double.Parse(LocationDetails.lng.ToString(), CultureInfo.InvariantCulture), item.Location.Latitude, item.Location.Longitude, 'K')) + " km",
      //              } into x
      //              where x.PropertyType == PropertyType
      //              select x).Skip(0).Take(PageSize).ToList();

      model = source2;
      return Json(model, JsonRequestBehavior.AllowGet);
    }
  }
}
