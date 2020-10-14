using Chartwell.Foundation.Models;
using Chartwell.Foundation.utility;
using Sitecore;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data;
using Sitecore.Globalization;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace Chartwell.Feature.MainSearch.Controllers
{
  public class SearchController : Controller
  {
    private ChartwellUtiles util = new ChartwellUtiles();
    string constring = ConfigurationManager.ConnectionStrings["SitecoreOLP"].ToString();

    // GET: Search
    public ActionResult Index()
    {
      PropertySearchModel property = new PropertySearchModel();
      property.Language = Context.Language.Name;
      property.ServerRole = ConfigurationManager.AppSettings["role:define"].ToString();
      property.ContextItemID = Context.Item.ID;

      var userGeoIPDetails = util.GetUserGeoIPDetails();
      if (userGeoIPDetails.City.Equals("N/A"))
        property.City = string.Empty;
      else
        property.City = userGeoIPDetails.City;

      return View(property);
      //return View();


    }

    [HttpPost]
    public JsonResult SearchProperty(string Prefix)
    {
      string contextLanguage;
      contextLanguage = Request.Form["lang"];
      List<SearchResultItem> results = null;

      var predicate = PredicateBuilder.True<SearchResultItem>();
      predicate = predicate.And(p => p.Path.StartsWith("/sitecore/content/Chartwell/Project/PropertySearch"));
      predicate = predicate.And(p => p.TemplateName.Equals("PropertySearch"));
      predicate = predicate.And(p => p.Language == Context.Language.Name);

      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {
        results = context.GetQueryable<SearchResultItem>().Where(predicate).ToList();
      }

      IEnumerable<PropertySearchModel> propertyNameList = null;

      propertyNameList = (from c in results
                          select new PropertySearchModel
                          {
                            PropertyName = c.GetItem().Fields["Property Name"].Value
                          })
                      .Where(x => x.PropertyName.Replace("'", "").Replace("-", " ")
                                                .RemoveDiacritics().ToLower()
                                                .Contains(Prefix.Replace("'", "").Replace("-", " ")
                                                .RemoveDiacritics().ToLower()))
                      .OrderBy(o => o.PropertyName).Take(25)
                      .ToList();

      return Json(propertyNameList, JsonRequestBehavior.AllowGet);
    }

    [HttpPost]
    public JsonResult CitySearch(string term)
    {

      string contextLanguage;
      contextLanguage = Request.Form["lang"];

      var origTerm = term.ToTitleCase().RemoveDiacritics().Replace("-", "");
      term = term.ToTitleCase().RemoveDiacritics().Replace("-", "").Replace(" ", "");

      List<SearchResultItem> matches;

      using (var context = ContentSearchManager.GetIndex("sitecore_web_index").CreateSearchContext())
      {

        var predicate = PredicateBuilder.True<SearchResultItem>();
        predicate = predicate.And(p => p.Path.StartsWith("/sitecore/content/Chartwell/Project/Content Shared Folder/City"));
        predicate = predicate.And(p => p.Name.Contains(term) || p.Name.Contains(origTerm) || p.Content.Contains(origTerm));
        predicate = predicate.And(p => p.Language == Context.Language.Name);
        matches = context.GetQueryable<SearchResultItem>().Where(predicate).Take(50).ToList();
      }

      IEnumerable<PropertySearchModel> CityNameList = null;

      CityNameList = (from p in matches
                      group p by p.GetItem().Fields["City Name"].Value into g
                      select new PropertySearchModel
                      {
                        City = g.Key.ToString()
                      })
                      .OrderBy(o => o.City).ToList();

      return Json(CityNameList, JsonRequestBehavior.AllowGet);
    }

    [HttpPost]
    public ActionResult Index(PropertySearchModel property)
    {

      var lang = Request.Form["Language"];
      property.ContextItemID = new ID(Request.Form["ContextItemID"]);

      if (string.IsNullOrEmpty(property.City) && string.IsNullOrEmpty(property.PostalCode) && string.IsNullOrEmpty(property.PropertyName))
      {
        string newUrl = util.GetitemUrl(util.GetItemById(property.ContextItemID), Context.Language);
        return Redirect(newUrl.ToLower());
      }

      string SearchTermValue = string.Empty;
      string queryStrKey = string.Empty;

      if (Request.Form.Count > 0)
      {
        var qsList = Request.Form.AllKeys
                            .Select(key => new { Name = key.ToString(), Value = Request.Form[key.ToString()] })
                            .Where(k => !string.IsNullOrWhiteSpace(k.Value) && !string.IsNullOrWhiteSpace(k.Name) &&
                                (k.Name.ToLower() == "city" || k.Name.ToLower() == "propertyname" || k.Name.ToLower() == "postalcode"))
                            .ToList();

        var newUrl = string.Empty;
        var cityQueryString = string.Empty;

        queryStrKey = string.Empty;
        if (qsList[0].Name.ToLower() == "city" || qsList[0].Name.ToLower() == "nom de la ville")
        {
          queryStrKey = Translate.TextByLanguage("CitySearch", Context.Language);
        }
        else if (qsList[0].Name.ToLower() == "propertyname")
        {
          queryStrKey = "propertyname";
        }
        else if (qsList[0].Name.ToLower() == "postalcode")
        {
          queryStrKey = "postalcode";
        }
        newUrl = Request.Url.Scheme + "://" + Request.Url.Host + "/" + Context.Language.Name + "/" + Translate.TextByLanguage("SearchResults", Context.Language) + "/?" +
                                              queryStrKey.ToLower() + "=" + qsList[0].Value.ToLower().Replace(" ", "-");

        return Redirect(newUrl.ToLower());
      }
      else
      {
        string newUrl = util.GetitemUrl(util.GetItemById(property.ContextItemID), Context.Language);
        return Redirect(newUrl.ToLower());
      }
    }

    [HttpPost]
    public async Task<JsonResult> LatLngSearch(string Latitude, string Longitude)
    {

      var LocationDetails = util.GeoNameLocation(Latitude, Longitude).FirstOrDefault();

      PostalCodeModel PCModel = new PostalCodeModel();

      SqlConnection conn = new SqlConnection(constring);

      SqlCommand cmd = new SqlCommand("sp_SCGetPostalCode", conn)
      {
        CommandType = CommandType.StoredProcedure
      };
      cmd.Parameters.AddWithValue("@PostCode", LocationDetails.postalCode);
      conn.Open();


      var dt = new DataTable();

      using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
      {
        dt.Load(rdr);


        PCModel = (from DataRow r in dt.Rows
                                          select new PostalCodeModel
                                          {
                                            City = r["City"].ToString(),
                                            Lat = r["Latitude"].ToString(),
                                            Lng = r["Longitude"].ToString(),
                                          }).FirstOrDefault();
      }


      PropertySearchModel property = new PropertySearchModel
      {
        City = PCModel.City.ToTitleCase()
      };
      return Json(property, JsonRequestBehavior.AllowGet);
    }

  }
}
