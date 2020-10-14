using Chartwell.Foundation.Models;
using Chartwell.Foundation.utility;
using Sitecore;
using System.Web;
using System.Web.Mvc;

namespace Chartwell.Feature.VirtualTour.Controllers
{
  public class VirtualTourController : Controller
  {
    // GET: VirtualTour
    private VirtualTourModel CreateModel()
    {
      var _c = new ChartwellUtiles();
      var PhotoItemPath = Context.Item;
      var parentItem = Context.Item.Parent;

      string strCityName = _c.CityName(parentItem.Language.Name, parentItem);
      string strProvinceID = parentItem.Fields["Province"].Value;
      var Provinceitem = _c.GetItemByStringId(strProvinceID);
      string strProvinceName = Provinceitem.Fields["Province Name"].Value;
      string strPropertyFormattedAddress = _c.FormattedAddress(parentItem, strProvinceName);

      var viewModel = new VirtualTourModel
      {
        PropertyTagLine = new HtmlString(parentItem.Fields["Property Tag Line"].Value),
        PropertyAddress = new HtmlString(parentItem.Fields["Street name and number"].Value),
        PropertyFormattedAddress = new HtmlString(strPropertyFormattedAddress),
        CityName = new HtmlString(strCityName),
        ProvinceName = new HtmlString(strProvinceName),
        PostalCode = new HtmlString(parentItem.Fields["Postal code"].Value),
        VideoLink = new HtmlString(parentItem.Fields["YouTubeLink"].Value),
        InnerItem = parentItem,

      };
      return viewModel;
    }

    public PartialViewResult Index()
    {
      return PartialView("~/Views/VirtualTour/VirtualTour.cshtml", CreateModel());
    }
  }
}