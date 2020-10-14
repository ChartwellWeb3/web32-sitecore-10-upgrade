using Chartwell.Foundation.Models;
using Chartwell.Foundation.utility;
using log4net;
using Sitecore;
using Sitecore.Links;
using Sitecore.Links.UrlBuilders;
using Sitecore.Resources.Media;
using System;
using System.Web;
using System.Web.Mvc;

namespace Chartwell.Feature.Overview.Controllers
{
  public class OverviewController : Controller
  {
    ChartwellUtiles util = new ChartwellUtiles();

    // GET: Overview
    private OverviewModel CreateModel()
    {
      ILog Logger = LogManager.GetLogger("ChartwellLog");
      var viewModel = new OverviewModel();

      var PropertyItem = Context.Item.Parent;

      try
      {
        var itemURL = LinkManager.GetItemUrl(PropertyItem, new ItemUrlBuilderOptions
        {
          UseDisplayName = true,
          LowercaseUrls = true,
          LanguageEmbedding = LanguageEmbedding.Always,
          LanguageLocation = LanguageLocation.FilePath,
          Language = PropertyItem.Language
        });

        //string strPhotoLink = "";
        //string strBrochureLink = "";

        //try
        //{
        //  strBrochureLink = util.GetBrochureUrl(PropertyItem);
        //}
        //catch { strBrochureLink = ""; }

        string strBrochureLink = "";
        string strBrochure = "";
        try
        {
          var db = Context.Database;

          if (PropertyItem.Fields["Property Brochure"].ToString() != string.Empty)
          {
            strBrochure = PropertyItem.Fields["Property Brochure"].ToString();
            strBrochureLink = HashingUtils.ProtectAssetUrl(StringUtil.EnsurePrefix('/', MediaManager.GetMediaUrl(db.GetItem(strBrochure))));
          }
        }
        catch { strBrochureLink = ""; }

        viewModel = new OverviewModel
        {
          PropertyDescription = new HtmlString(PropertyItem.Fields["Property Description"].ToString()),
          PropertySelectedLanguage = new HtmlString(PropertyItem.Language.ToString()),
          PropertyName = new HtmlString(PropertyItem.Fields["Property Name"].ToString()),
          PropertyType = util.PropertyType(Context.Language.Name, PropertyItem), // strPropertyType,
          PropertyTagLine = new HtmlString(PropertyItem.Fields["Property Tag Line"].ToString()),
          PropertyAddress = new HtmlString(PropertyItem.Fields["Street name and number"].ToString()),
          CityName = new HtmlString(util.CityName(PropertyItem.Language.Name, PropertyItem)),
          ProvinceName = new HtmlString(util.ProvinceName(Context.Language.Name, PropertyItem)),
          PostalCode = new HtmlString(PropertyItem.Fields["Postal code"].ToString()),
          GoogleReviewKeyword = new HtmlString(PropertyItem.Fields["Property Google Reviews Keyword"].ToString()),
          ReviewURL = PropertyItem.Language.Name.ToString() == "en" ? itemURL += "/reviews" : itemURL += "/critiques",
          VideoLink = PropertyItem.Fields["YouTubeLink"].HasValue ? new HtmlString(PropertyItem.Fields["YouTubeLink"].ToString()) : new HtmlString(string.Empty),
          BrochureURL = new HtmlString(strBrochureLink),
          PropertyFormattedAddress = new HtmlString(util.FormattedAddress(PropertyItem, util.ProvinceName(Context.Language.Name, PropertyItem))),
          isLandingpage = int.TryParse(PropertyItem.Fields["IsPropertyLandingPage"].Value, out int landingPage),
          InnerItem = PropertyItem
        };
      }
      catch (Exception e)
      {
        Logger.Error("Exception occured in Overview Proeprty: " + PropertyItem.Name);
        Logger.Error(e);
      }
      return viewModel;
    }

    public PartialViewResult Index()
    {
      return PartialView("~/Views/Overview/Index.cshtml", CreateModel());
    }

  }
}