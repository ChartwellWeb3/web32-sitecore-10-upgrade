using Chartwell.Foundation.Models;
using Chartwell.Foundation.utility;
using Sitecore;
using Sitecore.Collections;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Globalization;
using Sitecore.Links;
using Sitecore.Links.UrlBuilders;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace Chartwell.Feature.Language.Controllers
{
  public class ChartwellLanguageController : Controller
  {
    private readonly ChartwellUtiles util = new ChartwellUtiles();

    // GET: ChartwellLanguage
    public ActionResult Index(PropertySearchModel searchModel)
    {
      searchModel.RegionLang = Context.Language.Name;
      searchModel.SortedDictionary = new SortedDictionary<string, string>();
      searchModel.Language = Context.Language.Name;
      searchModel.ServerRole = ConfigurationManager.AppSettings["role:define"].ToString();

      LanguageCollection languages = LanguageManager.GetLanguages(Context.Database);
      var queryStringToAppend = Request.QueryString.ToString().Split('&').Where(c => !c.ToLower().Contains(Translate.Text("CitySearch").ToLower().Replace(" ", "-"))
                                                                                     && !c.ToLower().Contains("postalcode") && !c.ToLower().Contains("propertyname")).ToList();

      foreach (Sitecore.Globalization.Language lang in languages)
      {
        if (lang.ToString() == "fr")
        {
          string text = GetitemUrl(Context.Item, lang);
          searchModel.SortedDictionary.Add(lang.CultureInfo.DisplayName, text.ToLower());
        }
        else
        {
          string text8 = GetitemUrl(Context.Item, lang);
          searchModel.SortedDictionary.Add(lang.CultureInfo.DisplayName, text8.ToLower());
        }
      }
      return PartialView("~/Views/ChartwellLanguage/Index.cshtml", searchModel);
    }

    private string GetitemUrl(Item item, Sitecore.Globalization.Language language)
    {
      string itemUrl = LinkManager.GetItemUrl(Context.Database.GetItem(item.ID, LanguageManager.GetLanguage(language.ToString())), new ItemUrlBuilderOptions
      {
        UseDisplayName = true,
        LowercaseUrls = true,
        LanguageEmbedding = LanguageEmbedding.Always,
        LanguageLocation = LanguageLocation.FilePath,
        Language = language
      });

      string searchParamQueryString = util.GetRedirectUrl(language);

      return itemUrl.Replace(" ", "-") + searchParamQueryString.TrimEnd(new char[] { '/', ' ' });
    }

  }
}