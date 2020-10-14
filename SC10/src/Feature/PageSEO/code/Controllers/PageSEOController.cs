using Chartwell.Foundation.Models;
using Chartwell.Foundation.utility;
using Sitecore;
using Sitecore.Data.Items;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Chartwell.Feature.PageSEO.Controllers
{
  public class PageSEOController : Controller
  {
    // GET: PageSEO
    private readonly ChartwellUtiles utilities = new ChartwellUtiles();

    public ActionResult Index()
    {
      Item contextItem = Context.Item;

      var res = utilities.GetPageSEO(contextItem);

      PageMetaDataModel pgModel = new PageMetaDataModel()
      {
        PageTitle = res[0],
        PageDescription = res[1],
        PageKeyword = res[2],
        SEOCity = res[3],
        SEOProvince = res[4]
      };

      string pageDesc = Regex.Replace(pgModel.PageDescription, "<(.|\n)*?>", string.Empty);
      IEnumerable<string> strFirstTwoSentences = pageDesc.Split('.').Take(2);
      pgModel.PageDescription = string.Empty;
      foreach (string s in strFirstTwoSentences)
      {
        pgModel.PageDescription += s;
      }
      return View(pgModel);
    }
  }
}