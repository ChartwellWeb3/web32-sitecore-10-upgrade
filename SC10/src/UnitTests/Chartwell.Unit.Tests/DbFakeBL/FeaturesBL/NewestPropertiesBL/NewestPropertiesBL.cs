using Chartwell.Unit.Tests.Models.NewestProperties;
using Sitecore.ContentSearch.SearchTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chartwell.Unit.Tests.DbFakeBL.FeaturesBL.NewestPropertiesBL
{
  public enum PropertyOrder
  {
    DESC = 1, ASC, MOST_PROPERTIES
  }
  public class NewestPropertiesBL : BaseDbFakeBL
  {
    public void InitializeProperties()
    {
      var propList = new List<SearchResultItem>
      {
        new SearchResultItem {  Name = "Ont residence", Path = "/sitecore/content/Home", ["isnewprop"] = "false" },
        new SearchResultItem {  Name = "Ont residence 2", Path = "/sitecore/content/Home", ["isnewprop"] = "true" },
        new SearchResultItem {  Name = "Quebec residence 5", Path = "/sitecore/content/Home", ["isnewprop"] = "true" },
        new SearchResultItem {  Name = "Alberta residence", Path = "/sitecore/content/Home", ["isnewprop"] = "true" },
        new SearchResultItem {  Name = "Quebec residence",  Path = "/sitecore/content/Home", ["isnewprop"] = "true" }
      };

      BaseInitializeListItems("/sitecore/content/Home", propList);
    }

    public List<NPModel> ConvertModel(List<SearchResultItem> items)
    {
      var propList = new List<NPModel>();
      foreach (var item in items)
      {
        propList.Add(new NPModel
        {
          IsNewProperty = bool.Parse(item["isnewprop"]),
          ItemPath = item.Path,
          PropertyName = item.Name
        });
      }

      return propList;
    }

    public List<NPModel> SpecifyOrder(List<NPModel> items, PropertyOrder ord)
    {
      switch (ord)
      {
        case PropertyOrder.DESC:
          return items.OrderByDescending(x => x.PropertyName).ToList();
        case PropertyOrder.ASC:
          return items.OrderBy(x => x.PropertyName).ToList();
        default:
          return items;
      }
    }
  }
}
