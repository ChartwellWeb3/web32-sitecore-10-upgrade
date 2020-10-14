using Chartwell.Foundation.Models;
using Chartwell.Foundation.utility;
using log4net;
using Sitecore;
using Sitecore.Links;
using System;
using System.Web;
using System.Web.Mvc;
using Sitecore.Resources.Media;
using Sitecore.Data.Items;
using Sitecore.Data.Fields;
using Sitecore.Links.UrlBuilders;

namespace Chartwell.Feature.InfectionProtocol.Controllers
{
  public class InfectionProtocolController : Controller
  {

    ChartwellUtiles _c = new ChartwellUtiles();

    private InfectionProtocolModel CreateModel()
    {
      ILog Logger = LogManager.GetLogger("ChartwellLog");
      var viewModel = new InfectionProtocolModel();

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
        string strBrochureLink = "";

        try
        {
          strBrochureLink = _c.GetBrochureUrl(PropertyItem);
        }
        catch { strBrochureLink = ""; }

        viewModel = new InfectionProtocolModel
        {
          PropertyDescription = new HtmlString(PropertyItem.Fields["Property Description"].ToString()),
          PropertySelectedLanguage = new HtmlString(PropertyItem.Language.ToString()),
          PropertyName = new HtmlString(PropertyItem.Fields["Property Name"].ToString()),
          PropertyTagLine = new HtmlString(PropertyItem.Fields["Property Tag Line"].ToString()),
          PropertyAddress = new HtmlString(PropertyItem.Fields["Street name and number"].ToString()),
          CityName = new HtmlString(_c.CityName(PropertyItem.Language.Name, PropertyItem)),
          ProvinceName = new HtmlString(_c.ProvinceName(Context.Language.Name, PropertyItem)),
          PostalCode = new HtmlString(PropertyItem.Fields["Postal code"].ToString()),
          InnerItem = PropertyItem,
          PropertyFormattedAddress = new HtmlString(_c.FormattedAddress(PropertyItem, _c.ProvinceName(Context.Language.Name, PropertyItem))),
          SafetyAndOutbreakPreventionTitle = new HtmlString(PropertyItem.Fields["Safety and Outbreak Prevention Title"].ToString()),
          SafetyAndOutbreakPreventionDescription = new HtmlString(PropertyItem.Fields["Safety and Outbreak Prevention description"].ToString()),
          SafetyAndOutbreakPreventionYTVideoLink = new HtmlString(PropertyItem.Fields["Safety and Outbreak Prevention YT Video Link"].ToString())
        };
      }
      catch (Exception e)
      {
        Logger.Error("Exception occured in Infection Protocol Item: " + PropertyItem.Name);
        Logger.Error(e);
      }
      return viewModel;
    }

    // GET: InfectionProtocol
    public ActionResult Index()
    {
      return PartialView("~/Views/InfectionProtocol/Index.cshtml", CreateModel());
    }
  }
}