using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sitecore;
using Sitecore.Data;
using Sitecore.FakeDb;

namespace Chartwell.Unit.Tests
{
  [TestClass]
  public class UnitTest1
  {
    [TestMethod]
    public void Test()
    {
      using(var db = GetFullBreadcrumbItems())
      {
        var e = db.GetItem("/sitecore/content/Parent/Child");
      }
    }

    private static Db GetFullBreadcrumbItems()
    {
      return new Db
        {
            new DbItem("Parent", ID.NewID)
            {
                new DbItem("Child", ID.NewID)
                {
                    new DbItem("Grandchild", ID.NewID)
                },
                new DbField("Some Field Of Parent Item")
                {
                     Value = "something"
                },
            }
        };
    }
  }
}
