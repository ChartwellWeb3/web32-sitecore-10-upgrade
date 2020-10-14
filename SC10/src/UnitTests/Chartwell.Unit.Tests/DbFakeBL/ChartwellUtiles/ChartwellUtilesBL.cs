using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.FakeDb;

namespace Chartwell.Unit.Tests.DbFakeBL.ChartwellUtiles
{
  public class ChartwellUtilesBL : BaseDbFakeBL
  {
    public void InitializeDictionaryItemsList()
    {
      var sRList = new List<SearchResultItem>
        {
          new SearchResultItem{Name = "item1", Path = "/sitecore/content/Dictionary"},
          new SearchResultItem{Name = "item1", Path = "/sitecore/content/Dictionary"}
        };

      BaseInitializeListItems("/sitecore/content/Dictionary", sRList);
    }

    public void InitializePropertyDetailsList(ID id)
    {
      var propDetList = new List<SearchResultItem>
      {
        new SearchResultItem{ItemId = id},
        new SearchResultItem{ItemId = id}
      };

      BaseInitializeListItems("/sitecore/content/details", propDetList);
    }

    public void InitializeYardiCommunitiesList(string comName)
    {
      var yardiList = new List<SearchResultItem>
      {
        new SearchResultItem
        {
          TemplateName = "PropertyPage",
          Name = "__Standard Values",
          ["Property ID"] = "99999",
          ["SplitterURL"] = comName
        },
        new SearchResultItem
        {
          TemplateName = "PropertyPage",
          Name = "Regular Values",
          ["Property ID"] = "15642",
          ["SplitterURL"] = comName
        }
      };

      BaseInitializeListItems("/sitecore/content/yardi", yardiList);
    }

    public void InitializeCityList()
    {
      var cityList = new List<SearchResultItem>
      {
        new SearchResultItem
        {
          Path = "/sitecore/content/Chartwell/Project/Content Shared Folder/City",
          TemplateName = "City",
          Language = "en",
          Name = "Toronto",
          ItemId = ID.NewID,
          ["City Name"] = "toronto "
        },
        new SearchResultItem
        {
          Path = "/sitecore/content/Chartwell/Project/Content Shared Folder/City",
          TemplateName = "City",
          Language = "en",
          Name = "Toronto",
          ItemId = ID.NewID,
          ["City Name"] = "toronto "
        },
        new SearchResultItem
        {
          Path = "Not the one",
          TemplateName = "City",
          Language = "fr",
          Name = "Boston",
          ItemId = ID.NewID,
          ["City Name"] = "boston-"
        }
      };

      BaseInitializeListItems("/sitecore/content/yardi", cityList);
    }

    public void InitializeRegionalLPList()
    {
      var lpList = new List<SearchResultItem>
      {
        new SearchResultItem
        {
          TemplateName = "RegionalPropertiesPage",
          Language = "en",
          Name = "retirement-living/toronto"
        },
        new SearchResultItem
        {
          TemplateName = "RegionalPropertiesPage",
          Language = "en",
          Name = "retirement-living/toronto"
        },new SearchResultItem
        {
          TemplateName = "RegionalPropertiesPage",
          Language = "en",
          Name = "retirement-living/boston"
        }
      };

      BaseInitializeListItems("/sitecore/content/lp", lpList);
    }

    public void InitializeItemList()
    {
      var itemList = new List<SearchResultItem>
      {
        new SearchResultItem {TemplateName = "Custom Property"},
        new SearchResultItem {TemplateName = "Custom Property"},
        new SearchResultItem {TemplateName = "Not a Custom Property"}
      };

      BaseInitializeListItems("/sitecore/content/items", itemList);
    }

    public void InitializeMediaItemList(string itemName)
    {
      var mediaList = new List<SearchResultItem>
      {
        new SearchResultItem {TemplateName = "URL-Mapping", ["dnn url"] = itemName},
        new SearchResultItem {TemplateName = "Wrong URL-Mapping", ["dnn url"] = itemName},
        new SearchResultItem {TemplateName = "URL-Mapping", ["dnn url"] = itemName}
      };

      BaseInitializeListItems("/sitecore/content/media", mediaList);
    }

    internal void InitializeAddresses()
    {
      var addList = new List<SearchResultItem>
      {
        new SearchResultItem
        {
          Language = "en",
          ["Street name and number"] = "100 milverton rd",
          ["Selected City"] = "mississauga",
          ["Postal Code"] = "m1c0a8"
        },
        new SearchResultItem
        {
          Language = "en",
          ["Street name and number"] = "100 milverton rd",
          ["Selected City"] = "mississauga",
          ["Postal Code"] = "m1c0a8"
        },
        new SearchResultItem
        {
          Language = "fr",
          ["Street name and number"] = "200 milverton rd",
          ["Selected City"] = "toronto",
          ["Postal Code"] = "m1c0a8"
        }
      };

      BaseInitializeListItems("/sitecore/content/addresses", addList);
    }

    public void InitializeWebsiteList()
    {
      var urlList = new List<SearchResultItem>
      {
        new SearchResultItem { Url = "https://chartwell.com/welcome", TemplateName = "SplitterPage" }
      };
    }

    public void InitializeBrochureUrl()
    {
      var itemBrochureUrl = new List<SearchResultItem>
      { 
        new SearchResultItem { TemplateName = "Brochure-Url", ["Property Brochure"] = "/images/brochure/image1"} ,
        new SearchResultItem { TemplateName = "Brochure-Url-2", ["Property Brochure"] = "/images/brochure/image2"} ,
      };

      BaseInitializeListItems("/sitecore/content/media/", itemBrochureUrl);

    }
  }
}
