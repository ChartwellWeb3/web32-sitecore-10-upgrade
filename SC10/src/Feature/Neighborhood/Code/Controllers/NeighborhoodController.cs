using Chartwell.Foundation.Models;
using Chartwell.Foundation.utility;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Resources.Media;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chartwell.Feature.Neighborhood.Controllers
{
  public class NeighborhoodController : Controller
  {
    // GET: Neighborhood
    private NeighbourhoodModel CreateModel()
    {
      var c = new ChartwellUtiles();

      var item = Context.Item;
      var parentItem = Context.Item.Parent;

      string sitecoreid = parentItem.ID.ToString();

      var Provinceitem = c.GetItemByStringId(parentItem.Fields["Province"].Value);
      string strProvinceName = Provinceitem.Fields["Province Name"].Value;

      string strPropertyFormattedAddress = c.FormattedAddress(parentItem, strProvinceName);

      string strNeighbourName = parentItem.Fields["Neighborhood Section Description"].Value;

      List<Item> lstNeighbourName = new List<Item>();
      Sitecore.Data.Fields.MultilistField NeighbourList = parentItem.Fields["Neighborhood Amenity"];

      if (NeighbourList != null && NeighbourList.TargetIDs != null)
      {
        foreach (ID id in NeighbourList.TargetIDs)
        {
          Item targetItem = c.GetItemById(id);
          lstNeighbourName.Add(targetItem);
        }
      }
      var viewModel = new NeighbourhoodModel
      {
        PropertyGuid = new HtmlString(sitecoreid),
        NeighbourhoodName = new HtmlString(strNeighbourName),
        PropertyName = new HtmlString(parentItem.Fields["Property Name"].Value),
        PropertyTagLine = new HtmlString(parentItem.Fields["Property Tag Line"].Value),
        PropertyAddress = new HtmlString(parentItem.Fields["Street name and number"].Value),
        CityName = new HtmlString(c.CityName(parentItem.Language.Name, parentItem)),
        ProvinceName = new HtmlString(strProvinceName),
        PostalCode = new HtmlString(parentItem.Fields["Postal code"].Value),
        InnerItem = parentItem,
        PropertyFormattedAddress = new HtmlString(strPropertyFormattedAddress),
        TemplateItem = item,
        NeighbourhoodItem = lstNeighbourName.OrderBy(o => o.Name).ToList()
      };
      return viewModel;
    }

    public PartialViewResult Index()
    {
      return PartialView("~/Views/Neighborhood/Neighbourhood.cshtml", CreateModel());     
    }
  }
}