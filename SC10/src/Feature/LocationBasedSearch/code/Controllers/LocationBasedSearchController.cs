using Chartwell.Foundation.Models;
using Chartwell.Foundation.utility;
using Sitecore;
using Sitecore.CES.GeoIp.Core.Model;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Chartwell.Feature.LocationBasedSearch.Controllers
{
    public class LocationBasedSearchController : Controller
  {
    readonly string constring = ConfigurationManager.ConnectionStrings["SitecoreOLP"].ToString();

    private readonly ChartwellUtiles util = new ChartwellUtiles();

    // GET: LocationBasedSearch
    public ActionResult Index(PropertySearchModel property)
    {

      var contextItem = Context.Item;
      var displayLoactionValue = util.GetItemById(contextItem.ID).Fields["DisplayLocation"].Value;

      bool displayLocation = bool.Parse(displayLoactionValue.Equals("1") ? "true" : "false");

      if (displayLocation)
      {
        WhoIsInformation userLocation = util.GetUserGeoIPDetails();

        if (!userLocation.IsUnknown)
        {
          property.City = userLocation.City;
          property.Latitude = userLocation.Latitude.ToString();
          property.Longitude = userLocation.Longitude.ToString();

          var closestResidence = util.ClosestResidenceDetails(userLocation.Latitude.ToString(), userLocation.Longitude.ToString(), Context.Language.Name);
          property.PropertyItemUrl = closestResidence.Split(',').Where(x => x.Contains("PropertyItemUrl")).FirstOrDefault().Split('=').Where(y => !y.Contains("PropertyItemUrl")).FirstOrDefault(); //closestResidence.Replace("/sitecore/shell/chartwell", "");
          property.PropertyName = closestResidence.Split(',').Where(x => x.Contains("PropertyName")).FirstOrDefault().Split('=').Where(y => !y.Contains("PropertyName")).FirstOrDefault();
        }
        else
        {
          property.City = string.Empty;
        }

        return View(property);
      }
      return null;
    }

    [HttpPost]
    public async Task<JsonResult> LatLngSearch(string Latitude, string Longitude)
    {
      var propertyItemUrl = string.Empty;
      var propertyName = string.Empty;
      var city = string.Empty;

      var LocationDetails = util.GeoNameLocation(Latitude, Longitude).FirstOrDefault();

      PostalCodeModel PCModel = await util.GetLocDetailsFromCanadianPostalCodeDB(LocationDetails, constring);

      var closestResidence = util.ClosestResidenceDetails(LocationDetails.lat.ToString(), LocationDetails.lng.ToString(), Context.Language.Name);


      city = PCModel.City.ToTitleCase(); 
      propertyItemUrl = closestResidence.Split(',').Where(x => x.Contains("PropertyItemUrl")).FirstOrDefault().Split('=').Where(y => !y.Contains("PropertyItemUrl")).FirstOrDefault();
      propertyName = closestResidence.Split(',').Where(x => x.Contains("PropertyName")).FirstOrDefault().Split('=').Where(y => !y.Contains("PropertyName")).FirstOrDefault();


      PropertySearchModel property = new PropertySearchModel
      {
        City = city,
        PropertyItemUrl = propertyItemUrl,
        PropertyName = propertyName,
        Latitude = string.Format("{0:0.0000000000}", double.Parse(Latitude)),
        Longitude = string.Format("{0:0.0000000000}", double.Parse(Longitude))
      };
      return Json(property, JsonRequestBehavior.AllowGet);
    }
  }
}