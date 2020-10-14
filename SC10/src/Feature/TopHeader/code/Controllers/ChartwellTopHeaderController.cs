using Chartwell.Foundation.utility;
using Sitecore;
using Sitecore.Data;
using Sitecore.Mvc.Presentation;
using System.Web.Mvc;

namespace Chartwell.Feature.ChartwellTopHeader.Controllers
{
  public class ChartwellTopHeaderController : Controller
  {
    // GET: ChartwellLanguage
    public ActionResult Index()
    {
      var c = new ChartwellUtiles();
      var ItemPath = Context.Item;
      var PropertyItem = Context.Item.Parent;

      string selectedlang = Sitecore.Context.Language.ToString();
      if (PropertyItem.Template.Name == "PropertyPage")
      {
        string strPropertyTypeID = PropertyItem.Fields["property type"].Value;
        string strPropertyType = string.Empty;
        if (!string.IsNullOrEmpty(strPropertyTypeID))
        {
          var PropertyTypeItem = c.GetItemByStringId(strPropertyTypeID);
          strPropertyType = PropertyTypeItem.Fields["property type"].Value;
        }
        string strProvinceID = PropertyItem.Fields["Province"].Value;        
        var Provinceitem = c.GetItemByStringId(strProvinceID);
        string strProvinceName = Provinceitem.Fields["Province Name"].Value.ToLower();

        if (strProvinceName == "quebec" || strProvinceName == "québec")
        {
          if (selectedlang == "en")
            return View("~/Views/ChartwellTopHeader/Index.cshtml");
          else { return View("~/Views/ChartwellTopHeader/Indexfr.cshtml"); }
        }
        else
          return new EmptyResult();
      }
      else
      {
        if (selectedlang == "en")
          return View("~/Views/ChartwellTopHeader/Index.cshtml");
        else { return View("~/Views/ChartwellTopHeader/Indexfr.cshtml"); }
      }
    }


  }
}