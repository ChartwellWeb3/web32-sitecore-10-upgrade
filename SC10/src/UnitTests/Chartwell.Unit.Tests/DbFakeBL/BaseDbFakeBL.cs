using NSubstitute;
using Sitecore.ContentSearch;
using Sitecore.FakeDb;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data;
using Sitecore;
using Sitecore.Collections;
using Sitecore.ContentSearch.SearchTypes;

namespace Chartwell.Unit.Tests.DbFakeBL
{
  public class BaseDbFakeBL : IDisposable
  {
    public ISearchIndex index { get; set; }

    public BaseDbFakeBL()
    {
      index = Substitute.For<ISearchIndex>();
      ContentSearchManager.SearchConfiguration.Indexes.Add("index", index);
    }

    public string TestGetItem()
    {
      using (var db = new Db{new DbItem("home")})
      {
        InitializeItem();

        Item result =
        index
        .CreateSearchContext()
        .GetQueryable<SearchResultItem>()
        .Single()
        .GetItem();

        return result.Paths.FullPath;
      }
    }

    public void InitializeItem()
    {
      using (var db = new Db { new DbItem("item") })
      {
        var searchResultItem =
       Substitute.For<SearchResultItem>();

        searchResultItem
          .GetItem()
          .Returns(db.GetItem("/sitecore/content/home"));

        index
          .CreateSearchContext()
          .GetQueryable<SearchResultItem>()
          .Returns((new[] { searchResultItem }).AsQueryable());
      }
    }

    public void BaseInitializeListItems<T>(string testItemPath, List<T> listObj)
    {
      using (var db = new Db { new DbItem("item") })
      {
        var sRI = Substitute.For<SearchResultItem>();

        sRI
          .GetItem()
          .Returns(db.GetItem(testItemPath));

        index
          .CreateSearchContext()
          .GetQueryable<T>()
          .Returns((listObj)
          .AsQueryable());

        var k = index
           .CreateSearchContext()
           .GetQueryable<T>().ToList();
      }
    }

    public Item CreateSimpleTestItem()
    {
      using (var db = new Db { new DbItem("Home") { { "Title", "Welcome!" } } })
      {
        return db.GetItem("/sitecore/content/home");
      }
    }

    public void Dispose()
    {
      ContentSearchManager.SearchConfiguration.Indexes.Remove("index");
    }
  }
}
