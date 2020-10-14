using Chartwell.Foundation.Models;
using Chartwell.Foundation.utility;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Sitecore.Resources.Media;

namespace Chartwell.Feature.ServiceLevel.Controllers
{
  public class CareServiceController : Controller
  {
    // GET: CareService
    private CareServiceModel CreateModel()
    {
      var _c = new ChartwellUtiles();
      var item = Context.Item;
      var parent = Context.Item.Parent;

      var Provinceitem = _c.GetItemByStringId(parent.Fields["Province"].Value);
      string strProvinceName = Provinceitem.Fields["Province Name"].Value;
      string strPropertyFormattedAddress = _c.FormattedAddress(parent, strProvinceName);

      var lstCareService = new Dictionary<Item, HtmlString>();
      MultilistField CareServiceList = parent.Fields["Property Care services"];

      if (CareServiceList != null && CareServiceList.TargetIDs != null)
      {
        foreach (ID id in CareServiceList.TargetIDs)
        {
          string _CareServiceDatabase = "/sitecore/content/Chartwell/Project/Content Shared Folder/Care Service";
          Item targetItem = _c.GetItemById(id);
          string strNeighbourID = targetItem.DisplayName;
          _CareServiceDatabase += "/" + strNeighbourID;
          Item CareServiceitem = _c.GetItemByTemplate(_CareServiceDatabase);

          HtmlString strServiceDescrb = new HtmlString(CareServiceitem.Fields["Care Service Description"].Value);

          lstCareService.Add(targetItem, strServiceDescrb);
          _CareServiceDatabase = string.Empty;
        }
      }

      var viewModel = new CareServiceModel
      {
        CareServiceTitle = new HtmlString(parent.Fields["Care Section Title"].Value),
        CareServiceName = new HtmlString(parent.Fields["Care Section Description"].Value),
        PropertyName = new HtmlString(parent.Fields["Property Name"].Value),
        PropertyTagLine = new HtmlString(parent.Fields["Property Tag Line"].Value),
        PropertyAddress = new HtmlString(parent.Fields["Street name and number"].Value),
        CityName = new HtmlString(_c.CityName(parent.Language.Name, parent)),
        ProvinceName = new HtmlString(strProvinceName),
        PostalCode = new HtmlString(parent.Fields["Postal code"].Value),
        PropertyGuid = new HtmlString(item.ID.ToString()),
        InnerItem = parent,
        PropertyFormattedAddress = new HtmlString(strPropertyFormattedAddress),
        CareServiceItem = lstCareService,
        TemplateItem = item
      };

      return viewModel;
    }

    public PartialViewResult Index()
    {
      return PartialView("~/Views/CareService/CareService.cshtml", CreateModel());
    }
  }
}