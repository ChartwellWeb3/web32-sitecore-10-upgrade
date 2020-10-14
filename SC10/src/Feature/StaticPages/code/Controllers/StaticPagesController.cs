using System.Web.Mvc;

namespace Chartwell.Feature.StaticPages.Controllers
{
  public class StaticPagesController : Controller
  {
    // GET: StaticPages
    public ActionResult Index(string PartialItemUrlText, string StaticLanguage)
    {
      PartialItemUrlText = PartialItemUrlText.Replace(" ", "-");
      string itemUrl = Request.Url.Scheme + "://" + Request.Url.Host + "/" + StaticLanguage + "/" + PartialItemUrlText;
      return Redirect(itemUrl.ToLower());
    }
  }
}