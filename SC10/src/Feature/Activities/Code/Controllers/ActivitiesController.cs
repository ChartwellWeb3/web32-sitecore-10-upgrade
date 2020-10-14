using Chartwell.Foundation.Models;
using Chartwell.Foundation.utility;
using Sitecore.Data;
using Sitecore.Data.Items;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chartwell.Feature.Activities.Controllers
{
  public class ActivitiesController : Controller
  {
    private readonly ChartwellUtiles _c = new ChartwellUtiles();

    // GET: Activities
    private ActivitiesModel CreateModel()
    {
      var lstActivitiesName = new List<Item>();

      var parentPath = Sitecore.Context.Item.Parent;
      var item = Sitecore.Context.Item;

      string strActivitiesDetailDescription = parentPath.Fields["Activities Section Description"].Value;
      if (string.IsNullOrEmpty(strActivitiesDetailDescription))
        strActivitiesDetailDescription = "";

      string strProvinceID = parentPath.Fields["Province"].Value;

      var Provinceitem = _c.GetItemByStringId(strProvinceID);

      string strProvinceName = Provinceitem.Fields["Province Name"].Value;
      string strPropertyFormattedAddress = _c.FormattedAddress(parentPath, strProvinceName);

      Sitecore.Data.Fields.MultilistField ActivityList = parentPath.Fields["Property Activities"];
      string strBrochureLink = "";

      try
      {
        strBrochureLink = _c.GetActivitiesCalendarUrl(parentPath);
      }
      catch
      {
        strBrochureLink = "";

      }

      if (ActivityList != null && ActivityList.TargetIDs != null)
      {

        foreach (ID id in ActivityList.TargetIDs)
        {
          Item targetItem = _c.GetItemById(id);
          lstActivitiesName.Add(targetItem);
        }
      }
      var viewModel = new ActivitiesModel
      {
        PropertyGuid = new HtmlString(parentPath.ID.ToString()),
        ActivitiesName = new HtmlString(parentPath.Fields["Activities Section Title"].Value),
        PropertyName = new HtmlString(parentPath.Fields["Property Name"].Value),
        PropertyTagLine = new HtmlString(parentPath.Fields["Property Tag Line"].Value),
        PropertyAddress = new HtmlString(parentPath.Fields["Street name and number"].Value),
        CityName = new HtmlString(_c.CityName(parentPath.Language.Name, parentPath)),// parentPath.Fields["Selected City"].Value),
        ProvinceName = new HtmlString(strProvinceName),
        PostalCode = new HtmlString(parentPath.Fields["Postal code"].Value),
        InnerItem = parentPath,
        ActivitiesItem = lstActivitiesName.OrderBy(o => o.Name).ToList(),
        BrochureURL = new HtmlString(strBrochureLink),
        PropertyFormattedAddress = new HtmlString(strPropertyFormattedAddress),
        ActivitiesDescription = new HtmlString(strActivitiesDetailDescription),
        TemplateItem = item
      };
      return viewModel;
    }

    public PartialViewResult Index()
    {
      return PartialView("~/Views/Activities/Activities.cshtml", CreateModel());
    }

    public PartialViewResult Events()
    {
      return PartialView("~/Views/Activities/Events.cshtml", CreateHeader());
    }

    private ActivitiesModel CreateHeader()
    {
      ChartwellUtiles util = new ChartwellUtiles();

      var item = Sitecore.Context.Item;
      var parentItem = Sitecore.Context.Item.Parent;

      string strPropertyName = parentItem.Fields["Property Name"].Value;
      string strPropertyTag = parentItem.Fields["Property Tag Line"].Value;

      string strProvinceID = parentItem.Fields["Province"].Value;
      var Provinceitem = _c.GetItemByStringId(strProvinceID);
      string strProvinceName = Provinceitem.Fields["Province Name"].Value;
      string strPropertyFormattedAddress = util.FormattedAddress(parentItem, strProvinceName);

      var viewModel = new ActivitiesModel
      {
        PropertyName = new HtmlString(strPropertyName),
        PropertyTagLine = new HtmlString(strPropertyTag),
        InnerItem = parentItem,
        PropertyFormattedAddress = new HtmlString(strPropertyFormattedAddress)
      };
      return viewModel;
    }
  }
}