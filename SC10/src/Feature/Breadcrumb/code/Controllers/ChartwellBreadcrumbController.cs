using Chartwell.Foundation.utility;
using Chartwell.Foundation.Models;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Links;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Chartwell.Feature.Breadcrumb.Controllers
{
  public class ChartwellBreadcrumbController : Controller
  {
    ChartwellUtiles util = new ChartwellUtiles();
    // GET: Breadcrumb
    public PartialViewResult Index()
    {
      return PartialView("~/Views/Breadcrumb/Breadcrumb.cshtml", CreateModel(Context.Item));
    }

    public BreadcrumbsModel CreateModel(Item current)
    {
      List<Item> breadCrumbs = new List<Item>();

      Item PropertyItemPath1 = current;

      List<string> itemUrl = LinkManager.GetItemUrl(PropertyItemPath1)
                                        .Split('/').ToList()
                                        .Where(x => !string.IsNullOrEmpty(x) && !(x.Equals("en") || x.Equals("fr")))
                                        .ToList();

      foreach(string item in itemUrl)
      {
        breadCrumbs.Add(current);
        current = current.Parent;
      }
      breadCrumbs.Reverse();
      BreadcrumbsModel viewModel = new BreadcrumbsModel
      {
        HostSite = Request.Url.Host,
        PropertyURL = LinkManager.GetItemUrl(Context.Item.Parent) + "/" + Sitecore.Globalization.Translate.Text("overview"),
        BreadcrumbItem = breadCrumbs
      };
      return viewModel;
    }
  }
}