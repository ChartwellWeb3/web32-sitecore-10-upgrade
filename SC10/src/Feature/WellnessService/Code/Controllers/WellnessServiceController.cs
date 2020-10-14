using Chartwell.Foundation.Models;
using Chartwell.Foundation.utility;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sitecore.Resources.Media;

namespace Chartwell.Feature.WellnessService.Controllers
{
  public class WellnessServiceController : Controller
  {
    // GET: WellnessService
    private WellnessServiceModel CreateModel()
    {
      var _c = new ChartwellUtiles();

      var WellnessTemplateItem = Context.Item;
      var WellnessItem = Context.Item.Parent;

      string strProvinceID = WellnessItem.Fields["Province"].Value;
      var Provinceitem = _c.GetItemByStringId(strProvinceID);
      string strProvinceName = Provinceitem.Fields["Province Name"].Value;
      string strPropertyFormattedAddress = _c.FormattedAddress(WellnessItem, strProvinceName);

      Sitecore.Data.Fields.ImageField imageField = WellnessItem.Fields["Background Image"];
      if (imageField == null || imageField.MediaItem == null)
      {
        imageField = WellnessItem.Fields["Thumbnail Photo"];
      }
      HtmlString imgTag;
      if (imageField != null && imageField.MediaItem != null)
      {
        MediaItem image = new MediaItem(imageField.MediaItem);
        string src = StringUtil.EnsurePrefix('/', MediaManager.GetMediaUrl(image));
        imgTag = new HtmlString(src);
      }
      else
      {
        imgTag = new HtmlString(string.Empty);
      }

      List<Item> lstWellnessName = new List<Item>();
      Sitecore.Data.Fields.MultilistField WellnessList = WellnessItem.Fields["Property Wellness"];

      if (WellnessList != null && WellnessList.TargetIDs != null)
      {
        foreach (ID id in WellnessList.TargetIDs)
        {
          Item targetItem = _c.GetItemById(id);
          lstWellnessName.Add(targetItem);
        }
      }
      var viewModel = new WellnessServiceModel
      {
        WellnessTitle = new HtmlString(WellnessItem.Fields["Wellness Section Title"].Value),
        WellnessName = new HtmlString(WellnessItem.Fields["Wellness Section Description"].Value),
        PropertyName = new HtmlString(WellnessItem.Fields["Property Name"].Value),
        PropertyTagLine = new HtmlString(WellnessItem.Fields["Property Tag Line"].Value),
        PropertyAddress = new HtmlString(WellnessItem.Fields["Street name and number"].Value),
        CityName = new HtmlString(_c.CityName(WellnessItem.Language.Name, WellnessItem)),
        ProvinceName = new HtmlString(strProvinceName),
        PostalCode = new HtmlString(WellnessItem.Fields["Postal code"].Value),
        InnerItem = WellnessItem,
        PropertyGuid = new HtmlString(WellnessItem.ID.ToString()),
        WellnessItem = lstWellnessName.OrderBy(o => o.Name).ToList(),
        PropertyFormattedAddress = new HtmlString(strPropertyFormattedAddress),
        TemplateItem = WellnessTemplateItem,
        BackgroundImage = imgTag,

      };
      return viewModel;
    }

    public PartialViewResult Index()
    {
      return PartialView("~/Views/WellnessService/Wellness.cshtml", CreateModel());
    }
  }
}