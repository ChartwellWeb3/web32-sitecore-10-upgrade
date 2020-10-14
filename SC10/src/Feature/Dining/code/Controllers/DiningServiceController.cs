using Chartwell.Foundation.Models;
using Chartwell.Foundation.utility;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Resources.Media;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chartwell.Feature.DiningService.Controllers
{
  public class DiningServiceController : Controller
  {
    // GET: DiningService
    private DiningModel CreateModel()
    {
      var _c = new ChartwellUtiles();

      var item = Context.Item;
      var parent = Context.Item.Parent;

      string sitecoreid = parent.ID.ToString();

      string strProvinceID = parent.Fields["Province"].Value;

      var Provinceitem = _c.GetItemByStringId(strProvinceID);
      var provinceName = Provinceitem.Fields["Province Name"].Value;
      string strPropertyFormattedAddress = _c.FormattedAddress(parent, provinceName);

      var lstDiningService = new List<Item>();
      MultilistField DiningList = parent.Fields["Dining Service"];

      string strBrochureLink = "";
      try
      {
        strBrochureLink = _c.GetDiningUrl(parent);
      }
      catch { strBrochureLink = ""; }
      if (DiningList != null && DiningList.TargetIDs != null)
      {
        foreach (ID id in DiningList.TargetIDs)
        {
          Item targetItem = _c.GetItemById(id);
          lstDiningService.Add(targetItem);
        }
      }
      var viewModel = new DiningModel
      {
        PropertyGuid = new HtmlString(sitecoreid),
        DiningTitle = new HtmlString(parent.Fields["Dining Section Title"].Value),
        DiningDescription = new HtmlString(parent.Fields["Dining Section Description"].Value),
        PropertyName = new HtmlString(parent.Fields["Property Name"].Value),
        PropertyTagLine = new HtmlString(parent.Fields["Property Tag Line"].Value),
        PropertyAddress = new HtmlString(parent.Fields["Street name and number"].Value),
        CityName = new HtmlString(_c.CityName(parent.Language.Name, parent)),
        ProvinceName = new HtmlString(provinceName),
        PostalCode = new HtmlString(parent.Fields["Postal code"].Value),
        InnerItem = parent,
        BrochureURL = new HtmlString(strBrochureLink),
        DiningServiceItem = lstDiningService.OrderBy(o => o.Name).ToList(),
        PropertyFormattedAddress = new HtmlString(strPropertyFormattedAddress),
        TemplateItem = parent
      };
      return viewModel;
    }

    public PartialViewResult Index()
    {
      return PartialView("~/Views/DiningService/Dining.cshtml", CreateModel());
    }
  }
}