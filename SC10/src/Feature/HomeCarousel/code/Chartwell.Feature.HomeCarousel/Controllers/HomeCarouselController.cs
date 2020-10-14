using Chartwell.Foundation.Models;
using Chartwell.Foundation.utility;
using Sitecore;
using Sitecore.Links;
using Sitecore.Links.UrlBuilders;
using System.Collections.Generic;
using System.Web.Mvc;

public class HomeCarouselController : Controller
{
  private readonly ChartwellUtiles _c = new ChartwellUtiles();
  // GET: Carousel
  public PartialViewResult Index()
  {
    return PartialView("~/Views/HomeCarousel/Index.cshtml", GetCarouselImages());
  }

  public PartialViewResult SmallImages()
  {
    return PartialView("~/Views/HomeCarousel/SmallImages.cshtml", GetSmallCarouselImages());
  }

  public List<CarouselModel> GetCarouselImages()
  {
    var carouselList = _c.GetCarouselItems();
    var list = new List<CarouselModel>();

    foreach (var item in carouselList)
    {
      string itemUrl = LinkManager.GetItemUrl(item, new ItemUrlBuilderOptions
      {
        UseDisplayName = true,
        LowercaseUrls = true,
        LanguageEmbedding = LanguageEmbedding.Always,
        LanguageLocation = LanguageLocation.FilePath,
        Language = Context.Language
      });
      list.Add(new CarouselModel
      {
        Link = itemUrl + "/" + Sitecore.Globalization.Translate.Text("overview"),
        Heading = item.Fields["Property Name"].Value,
        ImageLink = _c.GetImageUrl(item),
        Address = _c.FormattedAddress(item, _c.ProvinceName(Context.Language.Name, item)),
        Telephone = _c.GetPhoneNumber(item),
      });
    }
    return list;
  }
  
  public List<string> GetSmallCarouselImages()
  {
    var imageList = _c.GetCarouselItems();
    var list = new List<string>();

    foreach (var img in imageList)
    {
      list.Add(_c.GetImageUrl(img));
    }

    return list;
  }
}
