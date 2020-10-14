using Chartwell.Foundation.Models;
using Chartwell.Foundation.utility;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Links.UrlBuilders;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Chartwell.Feature.TopNav.Controllers
{

  public class TopNavController : Controller
  {
    private ChartwellUtiles util = new ChartwellUtiles();

    // GET: TopNav
    public ActionResult Index()
    {
      List<TopNavModel> TopNavParentItems = new List<TopNavModel>();

      TopNavParentItems = (from s in util.GetTopNavParentItems("parent")
                           select new TopNavModel
                           {

                             TopNavParentItems = s.DisplayName,
                             TopNavItemUrl = (s.DisplayName == Translate.Text("Blog") || !s.HasChildren) ?
                                                               Sitecore.Links.LinkManager.GetItemUrl(s, new ItemUrlBuilderOptions { UseDisplayName = true, LowercaseUrls = true, LanguageEmbedding = Sitecore.Links.LanguageEmbedding.Always }) : "#",
                             TargetBlank = s.DisplayName == Translate.Text("Blog") ? "_blank" : "_self",
                             HasChild = s.HasChildren,
                             ULClass = string.Empty
                           }).ToList();

      var TopNavParent = (from c in util.GetTopNavParentItems("parent")
                          where !c.DisplayName.Equals(Translate.Text("Home")) &&
                                !c.DisplayName.Equals(Translate.Text("Blog")) &&
                                !c.DisplayName.Equals(Translate.Text("contact"))
                          select c.GetChildren().Where(i => i.Fields["IncludeInTopNav"].Value.Equals("1")).ToList()).ToList().Where(i => i.Count > 0);


      string listItemClasss = string.Empty;

      foreach (var parent in TopNavParent)
      {
        listItemClasss = string.Empty;
        foreach (var listItemChild in parent)
        {
          if (listItemChild.HasChildren)
          {
            listItemClasss += "<li><a href=" + "\""
                              + Sitecore.Links.LinkManager.GetItemUrl(listItemChild, new ItemUrlBuilderOptions { UseDisplayName = true, LowercaseUrls = true, LanguageEmbedding = Sitecore.Links.LanguageEmbedding.Always })
                              .Replace(" ", "-") + "\"" + "id=\"MainNav-" + Translate.Text(listItemChild.DisplayName).Replace(" ", "") + "\"" + ">" + listItemChild.DisplayName + "</a><ul class=dropDown3thLevel>";

            foreach (Item listItemInnerChild in listItemChild.Children)
            {
              if (listItemInnerChild.Fields["IncludeInTopNav"].Value.Equals("1"))
              {
                listItemClasss += "<li><a href=" + "\""
                                                + Sitecore.Links.LinkManager.GetItemUrl(listItemInnerChild, new ItemUrlBuilderOptions { UseDisplayName = true, LowercaseUrls = true, LanguageEmbedding = Sitecore.Links.LanguageEmbedding.Always })
                                                .Replace(" ", "-") + "\"" + "id=\"MainNav-" + Translate.Text(listItemInnerChild.DisplayName).Replace(" ", "") + "\"" + ">" + listItemInnerChild.DisplayName + "</a></li>";
              }
            }
            listItemClasss += "</ul></li>";
          }
          else
          {
            if (!(listItemChild.Name.Equals("careers") || listItemChild.Name.Equals("investor-relations")))
            {
              listItemClasss += "<li><a href=" + "\""
                                  + Sitecore.Links.LinkManager.GetItemUrl(listItemChild, new ItemUrlBuilderOptions { UseDisplayName = true, LowercaseUrls = true, LanguageEmbedding = Sitecore.Links.LanguageEmbedding.Always })
                                  .Replace(" ", "-") + "\"" + "id=\"MainNav-" + Translate.Text(listItemChild.DisplayName).Replace(" ", "") + "\"" + ">" + listItemChild.DisplayName + "</a></li>";
            }
            else
            {
              var getExternalUrl = util.GetExternalUrl(listItemChild.Name, Context.Language.Name );
              listItemClasss += "<li><a href="  + getExternalUrl + " id=\"MainNav-" 
                                                + Translate.Text(listItemChild.DisplayName).Replace(" ", "") 
                                                + "\"" + ">" 
                                                + listItemChild.DisplayName + "</a></li>";

            }
          }
        }
        var addCssClasstoParent = TopNavParentItems.Where(x => x.TopNavParentItems.Equals(parent[0].Parent.DisplayName)).FirstOrDefault();
        addCssClasstoParent.ULClass = listItemClasss;
      }
      return View(TopNavParentItems);
    }
  }
}