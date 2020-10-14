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

namespace Chartwell.Feature.Amenities.Controllers
{
  public class AmenitiesController : Controller
  {
    // GET: Amenities
    private AmenitiesModel CreateModel()
    {
      var _c = new ChartwellUtiles();

      var item = Context.Item;
      var parentItem = Context.Item.Parent;

      var Provinceitem = _c.GetItemByStringId(parentItem.Fields["Province"].Value);
      string strProvinceName = Provinceitem.Fields["Province Name"].Value;

      string strPropertyFormattedAddress = _c.FormattedAddress(parentItem, strProvinceName);

      List<Item> lstAmenities = new List<Item>();
      MultilistField AmenitiesList = parentItem.Fields["Property Amenities"];

      if (AmenitiesList != null && AmenitiesList.TargetIDs != null)
      {
        foreach (ID id in AmenitiesList.TargetIDs)
        {
          Item targetItem = _c.GetItemById(id);
          lstAmenities.Add(targetItem);
        }
      }
      var viewModel = new AmenitiesModel
      {
        PropertyGuid = new HtmlString(parentItem.ID.ToString()),
        PropertyName = new HtmlString(parentItem.Fields["Property Name"].Value),
        PropertyTagLine = new HtmlString(parentItem.Fields["Property Tag Line"].Value),
        PropertyAddress = new HtmlString(parentItem.Fields["Street name and number"].Value),
        CityName = new HtmlString(_c.CityName(parentItem.Language.Name, parentItem)),
        ProvinceName = new HtmlString(strProvinceName),
        PostalCode = new HtmlString(parentItem.Fields["Postal code"].Value),
        AmenitiesTitle = new HtmlString(parentItem.Fields["Amenities Section Title"].Value),
        AmenitiesDescription = new HtmlString(parentItem.Fields["Amenities Section Description"].Value),
        AmenitiesItem = lstAmenities.OrderBy(o => o.Name).ToList(),
        InnerItem = parentItem,
        PropertyFormattedAddress = new HtmlString(strPropertyFormattedAddress),
        TemplateItem = item
      };
      return viewModel;
    }

    public PartialViewResult Index()
    {
      return PartialView("~/Views/Amenities/Amenities.cshtml", CreateModel());
    }
  }
}