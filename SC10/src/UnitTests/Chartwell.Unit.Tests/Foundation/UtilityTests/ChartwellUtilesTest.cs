using Chartwell.Foundation.utility;
using Chartwell.Unit.Tests.DbFakeBL.ChartwellUtiles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data;
using System;
using System.Linq;

namespace Chartwell.Unit.Tests.Foundation.UtilityTests
{
  [TestClass]
  public class ChartwellUtilesTest
  {
    private string ExpectedPath { get; set; }
    private string ActualPath { get; set; }

    [TestMethod]
    public void TestGetPhoneNumber()
    {
      using(var cs = new ChartwellUtilesBL())
      {
       
      }
    }

    [TestMethod]
    public void TestGetDictionaryItem()
    {
      using(var cs = new ChartwellUtilesBL())
      {
        var predicate = PredicateBuilder.True<SearchResultItem>();
        predicate = predicate.And(p => p.Path.StartsWith("/sitecore/content/Dictionary"));
        predicate = predicate.And(p => p.Name == "item1");

        cs.InitializeDictionaryItemsList();

        var matches = cs.index.CreateSearchContext().GetQueryable<SearchResultItem>().Where(predicate).ToList();

        Assert.IsTrue(matches.Count == 2);
      }
    }

    [TestMethod]
    public void TestPropertyDetails()
    {
      using(var cs = new ChartwellUtilesBL())
      {
        var id = ID.NewID;
        cs.InitializePropertyDetailsList(id);

        var result = cs.index.CreateSearchContext().GetQueryable<SearchResultItem>().Where(x => x.ItemId == id).ToList();

        Assert.IsTrue(result.Count == 2);
      }
    }

    [TestMethod]
    public void TestGetYardiForCommunity()
    {
      using(var cs = new ChartwellUtilesBL())
      {
        cs.InitializeYardiCommunitiesList("chartwell");

        var result = (from x in cs.index.CreateSearchContext().GetQueryable<SearchResultItem>()
        where x.TemplateName == "PropertyPage"
        where x.Name != "__Standard Values"
        where x["Property ID"] != "99999"
        where x["SplitterURL"].Equals("chartwell")
        select x).ToList();

        Assert.IsTrue(result.Count == 1);
      }
    }

    [TestMethod]
    public void TestSearchSelectedCity()
    {
      using(var cs = new ChartwellUtilesBL())
      {
        cs.InitializeCityList();

        var results = cs.index.CreateSearchContext().GetQueryable<SearchResultItem>()
         .Where(x => x.TemplateName == "City")
         .Where(x => x.Language == "en")
         .Where(x => x.Name == "Toronto".RemoveDiacritics().Replace(" ", ""))
         .OrderBy(o => o.Name).ToList();

        Assert.IsTrue(results.Count == 2);
      }
    }

    [TestMethod]
    public void TestGetRegionalLPUrl()
    {
      using(var cs = new ChartwellUtilesBL())
      {
        cs.InitializeRegionalLPList();

        var results = cs.index.CreateSearchContext().GetQueryable<SearchResultItem>()
         .Where(x => x.TemplateName == "RegionalPropertiesPage")
         .Where(x => x.Language == "en")

         .Where(x => x.Name.StartsWith("retirement-living") && x.Name.Contains("toronto")).ToList();

        Assert.IsTrue(results.Count == 2);
      }
    }

    [TestMethod]
    public void TestGetItem()
    {
      using(var cs = new ChartwellUtilesBL())
      {
        cs.InitializeItemList();

        var predicate = PredicateBuilder.True<SearchResultItem>();
        predicate = predicate.And(x => x.TemplateName == "Custom Property");

        var results = cs.index.CreateSearchContext().GetQueryable<SearchResultItem>().Where(predicate).ToList();

        Assert.IsTrue(results.Count == 2);
      }
    }

    [TestMethod]
    public void TestGetMediaitemRedirectuRL()
    {
      var ItemName = "item";
      using (var cs = new ChartwellUtilesBL())
      {
        cs.InitializeMediaItemList(ItemName);

        var predicate = PredicateBuilder.True<SearchResultItem>();
        predicate = predicate.And((SearchResultItem p) => p.TemplateName == "URL-Mapping");
        predicate = predicate.And((SearchResultItem p) => p["dnn url"].Equals(ItemName, StringComparison.InvariantCultureIgnoreCase));

        var results = cs.index.CreateSearchContext().GetQueryable<SearchResultItem>().Where(predicate).ToList();

        Assert.IsTrue(results.Count == 2);
      }
    }

    [TestMethod]
    public void TestSearchSelectedCityWithSpaces()
    {
      using(var cs = new ChartwellUtilesBL())
      {
        cs.InitializeCityList();

        var results = cs.index.CreateSearchContext().GetQueryable<SearchResultItem>()
         .Where(x => x.TemplateName == "City")
         .Where(x => x.Language == "en")
         .Where(x => x.Name.ToLower() == "Toronto".RemoveDiacritics().TrimPunctuation().ToLower())
         .OrderBy(o => o.Name).ToList();

        Assert.IsTrue(results.Count == 2);
      }
    }

    [TestMethod]
    public void TestSwitchSelectedCity()
    {
      var searchCriteria = "toronto-";
      using (var cs = new ChartwellUtilesBL())
      {
        cs.InitializeCityList();

        var predicate = PredicateBuilder.True<SearchResultItem>();
        //var crit = searchCriteria.Replace("-", " ");
        predicate = predicate.And(p => p.Path.StartsWith("/sitecore/content/Chartwell/Project/Content Shared Folder/City"));
        predicate = predicate.And(p => p["City Name"].Equals(searchCriteria.Replace("-", " ")));
        predicate = predicate.Or(p => p["City Name"] == searchCriteria.Replace(" ", "-"));

        var matches = cs.index.CreateSearchContext().GetQueryable<SearchResultItem>().Where(predicate).ToList();
        
        string selectedItem = "";
        var selectedItemID = new ID();
        foreach (SearchResultItem item in matches)
        {
          if (item.Fields["city name"].ToString().ToLower() == searchCriteria.Replace("-", " ").ToLower() ||
            item.Fields["city name"].ToString().ToLower() == searchCriteria.Replace(" ", "-").ToLower())
          {
            selectedItem = item.Name;
            selectedItemID = item.ItemId;
            break;
          }
        }
        var results = cs.index.CreateSearchContext().GetQueryable<SearchResultItem>()
          .Where(x => x.TemplateName == "City")
          .Where(x => x.ItemId == selectedItemID)
          .OrderBy(o => o.Name).ToList();
        

        Assert.IsTrue(results.Count == 1);
      }
    }

    [TestMethod]
    public void TestFormattedAddress()
    {
      using (var cs = new ChartwellUtilesBL())
      {
        cs.InitializeAddresses();

        var ActualAddressValue = "";
        var ProvinceName = "Ontario";
        var ExpectedValue = "100 milverton rd, mississauga, Ontario m1c0a8";

        var results = cs.index.CreateSearchContext().GetQueryable<SearchResultItem>().ToList();

        foreach (var item in results)
        {
          if (item.Language.ToString() == "en")
          {
            ActualAddressValue = item.Fields["street name and number"].ToString() + ", "
                        + item.Fields["selected city"].ToString() + ", " + ProvinceName + " "
                        + item.Fields["postal code"].ToString();
            break;
          }
          else
          {
            string str = item.Fields["postal code"].ToString();
            ActualAddressValue = item.Fields["street name and number"].ToString() + ", " 
              + item.Fields["selected city"].ToString() 
              + " (" + ProvinceName + ")" + "m1c0a8";
          }
        }

        Assert.AreEqual(ExpectedValue, ActualAddressValue);
      }
    }

    [TestMethod]
    public void TestGetItemForUrl()
    {
      using(var cs = new ChartwellUtilesBL())
      {
        cs.InitializeWebsiteList();
      }
    }

    [TestMethod]
    public void TestContentSearch()
    {
      using (var cs = new ChartwellUtilesBL())
      {
        ExpectedPath = "/sitecore/content/home";
        ActualPath = cs.TestGetItem();

        Assert.AreEqual(ExpectedPath, ActualPath);
      }
    }

    [TestMethod]
    public void TestGetItemById()
    {
      using(var cs = new ChartwellUtilesBL())
      {
        cs.InitializeItem();
      }
    }

    [TestMethod]
    public void TestGetBrochureUrl()
    {
      using(var cs = new ChartwellUtilesBL())
      {
        cs.InitializeBrochureUrl();

        var result = cs.index.CreateSearchContext().GetQueryable<SearchResultItem>()
         .Where(x => x.TemplateName == "Brochure-Url")
         .Where(x => x["Property Brochure"] == "images/brochure/image1").Select(x => x.GetItem()).FirstOrDefault();

        var expectedResult = "/sitecore/content/media/images/brochure/image1";

        Assert.AreEqual(expectedResult, result["Property Brochure"].ToString());
      }
    }
    [TestMethod]
    public void TestGetDiningUrl()
    {

    }
    [TestMethod]
    public void TestGetActivitiesCalendar()
    {

    }
  }
}
