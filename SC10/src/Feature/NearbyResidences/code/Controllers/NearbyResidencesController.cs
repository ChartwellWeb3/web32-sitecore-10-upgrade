using Chartwell.Foundation.Models;
using Chartwell.Foundation.utility;
using log4net;
using Newtonsoft.Json;
using Sitecore;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Links.UrlBuilders;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Chartwell.Feature.NearbyResidences.Controllers
{
  public class NearbyResidencesController : Controller
  {
    string constring = ConfigurationManager.ConnectionStrings["SitecoreOLP"].ToString();
    ChartwellUtiles util = new ChartwellUtiles();

    // GET: NearbyResidences

    public ActionResult Index()
    {
      var Logger = LogManager.GetLogger("ChartwellLog");

      var item = Context.Item;
      var parentItem = Context.Item.Parent;

      string strLatitude;
      string strLongitude;
      bool SkipFirstProperty;

      if (item.TemplateName != "Static Pages")
      {
        strLatitude = string.Format("{0:0.00000}", double.Parse(parentItem.Fields["Latitude"].Value));
        strLongitude = string.Format("{0:0.00000}", double.Parse(parentItem.Fields["Longitude"].Value));
        SkipFirstProperty = true;
      }
      else
      {
        var contextItem = Context.Item;
        var displayNearbyResidencesValue = util.GetItemById(contextItem.ID).Fields["DisplayNearbyResidences"].Value;
        bool displayNearbyResidences = bool.Parse(displayNearbyResidencesValue.Equals("1") ? "true" : "false");

        if (displayNearbyResidences && contextItem.TemplateName.Equals("Static Pages"))
        {
          var StrLatLng = util.GetUserGeoIPDetails();

          if (!Request.Url.Host.Equals("chartwell.com") &&
              !Request.Url.Host.Equals("shreeji.ca"))
          {
            StrLatLng.Latitude = double.Parse("43.6545");
            StrLatLng.Longitude = double.Parse("-79.7802");
          }

          strLatitude = !StrLatLng.Latitude.Equals(null) ? string.Format("{0:0.00000}", double.Parse(StrLatLng.Latitude.ToString())) : string.Empty;
          strLongitude = !StrLatLng.Longitude.Equals(null) ? string.Format("{0:0.00000}", double.Parse(StrLatLng.Longitude.ToString())) : string.Empty;
          SkipFirstProperty = false;
        }
        else
        {
          return null;
        }
      }


      List<NearbyResidencesModel> model = new List<NearbyResidencesModel>();


      if (SkipFirstProperty)
        model.AddRange(NearbyResidencesList(strLatitude, strLongitude).Skip(1).Take(4).ToList());
      else
        model.AddRange(NearbyResidencesList(strLatitude, strLongitude).Take(4).ToList());

      foreach (var m in model)
      {
        m.Parent_ContextItemID = Context.Item.ID.ToString();
      }
      return View(model);
    }

    [HttpPost]
    public JsonResult LatLngSearch(string Latitude, string Longitude, string Parent_ContextItemID, string nearbyResidencesModels)
    {
      List<NearbyResidencesModel> model = JsonConvert.DeserializeObject<List<NearbyResidencesModel>>(nearbyResidencesModels);

      Item contextItem = util.GetItemById(new ID(Parent_ContextItemID));
      if (contextItem.TemplateName == "Static Pages")
      {
        List<NearbyResidencesModel> Queryresults = NearbyResidencesList(Latitude, Longitude).ToList();

        model.Clear();
        model.AddRange(Queryresults.Take(4));
      }
      return Json(model.OrderBy(o => o.Distance), JsonRequestBehavior.AllowGet);
    }

    private List<NearbyResidencesModel> NearbyResidencesList(string Latitude, string Longitude)
    {
      var strCoordinate = Latitude + "," + Longitude;

      var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext();
      var Queryresults = context.GetQueryable<SearchResultItem>()
                                .Where(x => x.Language == Context.Language.Name && x.TemplateName.Equals("PropertyPage"))
                                .WithinRadius(s => s.Location, strCoordinate, 500)
                                .OrderByDistance(s => s.Location, strCoordinate)
                                .ToList()
                                .Where(i => util.GetItemById(new ID(i.GetItem().Fields["property type"].Value)).Name.Equals("RET"))
                                .Select(s => new NearbyResidencesModel
                                {
                                  PropertyName = s.GetItem().Fields["Property Name"].Value,
                                  PropertyFormattedAddress = util.FormattedAddress(s.GetItem(), util.GetItemByStringId(s.GetItem().Fields["Province"].Value).Fields["Province Name"].Value),
                                  PhoneNo = util.GetPhoneNumber(s.GetItem()),
                                  PropertyItemUrl = LinkManager.GetItemUrl(s.GetItem(), new ItemUrlBuilderOptions
                                  {
                                    UseDisplayName = true,
                                    LowercaseUrls = true,
                                    LanguageEmbedding = LanguageEmbedding.Always,
                                    LanguageLocation = LanguageLocation.FilePath,
                                    Language = Context.Language
                                  }) + "/" + util.GetDictionaryItem("overview", Context.Language.Name),
                                  Distance = double.Parse(string.Format("{0:0.0}", util.Distance(double.Parse(Latitude, CultureInfo.InvariantCulture), double.Parse(Longitude, CultureInfo.InvariantCulture), s.Location.Latitude, s.Location.Longitude, 'K'))),
                                  NearbyResidencesImage = util.GetImageUrl(s.GetItem()),
                                  NearbyResidence_ItemID = s.GetItem().ID.ToString()
                                }).Take(5).ToList();
      return Queryresults;
    }
  }
}
