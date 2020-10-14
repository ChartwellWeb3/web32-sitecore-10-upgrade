using Chartwell.Unit.Tests.DbFakeBL.FeaturesBL.NewestPropertiesBL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sitecore.ContentSearch.SearchTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chartwell.Unit.Tests.Feature.NewestPropertiesTests
{
  [TestClass]
  public class NewestPropertyTest
  {
    [TestMethod]
    public void GetAllFilteredPropeties()
    {
      using(var nbl = new NewestPropertiesBL())
      {
        nbl.InitializeProperties();

        var results = nbl.index.CreateSearchContext().GetQueryable<SearchResultItem>()
          .Where(x => x.Fields["isnewprop"].ToString().Equals("true")).ToList();
        
        var propList = nbl.ConvertModel(results);

        var orderedPropList = nbl.SpecifyOrder(propList, PropertyOrder.ASC);
        Assert.IsTrue(orderedPropList.Count == 4);

      }
    }
  }
}
