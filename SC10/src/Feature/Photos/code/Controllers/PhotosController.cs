using Chartwell.Foundation.Models;
using Chartwell.Foundation.utility;
using Sitecore;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Links.UrlBuilders;
using Sitecore.Resources.Media;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chartwell.Feature.Photos.Controllers
{
  public class PhotosController : Controller
  {
    // GET: Photos

    public PartialViewResult Index()
    {
      return PartialView("~/Views/Photos/Index.cshtml", CreateModel());
    }

    private List<PhotoGalleryModel> CreateModel()
    {
      var ImageDetails = new List<PhotoGalleryModel>();
      var c = new ChartwellUtiles();

      var item = Context.Item;
      var parentItem = Context.Item.Parent;
      MultilistField PhotoList;

      var Provinceitem = c.GetItemByStringId(parentItem.Fields["Province"].Value);
      string strProvinceName = Provinceitem.Fields["Province Name"].Value;

      ImageField imageField = parentItem.Fields["Background Image"];
      if (imageField == null || imageField.MediaItem == null)
      {
        imageField = parentItem.Fields["Thumbnail Photo"];
      }

      HtmlString BackgroundImage;
      if (imageField != null && imageField.MediaItem != null)
      {
        MediaItem image = new MediaItem(imageField.MediaItem);
        string src = StringUtil.EnsurePrefix('/', MediaManager.GetMediaUrl(image));
        BackgroundImage = new HtmlString(src);
      }
      else
      {
        BackgroundImage = new HtmlString(string.Empty);
      }

      string strPropertyFormattedAddress = c.FormattedAddress(parentItem, strProvinceName);

      PhotoList = parentItem.Fields["Photos"];
      if (PhotoList.Count == 0)
      {
        PhotoList = item.Fields["PropertyImage"];
      }

      if (PhotoList != null && PhotoList.TargetIDs != null)
      {

        ImageDetails = (from photo in PhotoList.GetItems()
                        select new PhotoGalleryModel
                        {
                          HashedUrl = StringUtil.EnsurePrefix('/', MediaManager.GetMediaUrl(photo, new MediaUrlBuilderOptions { MaxWidth = 1280 })),
                          ImageMetaDataAlt = MediaManager.GetMedia(photo).MediaData.MediaItem.Alt,
                          ImageMetaDataTitle = (MediaManager.GetMedia(photo).MediaData.MediaItem.Title.Length != 0) ? MediaManager.GetMedia(photo).MediaData.MediaItem.Title : MediaManager.GetMedia(photo).MediaData.MediaItem.Alt,
                          ImageMetaDataDesc = MediaManager.GetMedia(photo).MediaData.MediaItem.Description
                        }).ToList();
        ImageDetails[0].PropertyFormattedAddress = new HtmlString(strPropertyFormattedAddress);
        ImageDetails[0].InnerItem = parentItem;
        ImageDetails[0].BackgroundImage = BackgroundImage;
      }
      return ImageDetails;
    }

    public PartialViewResult PropertyCarousel()
    {
      return PartialView("~/Views/Photos/PropertyCarousel.cshtml", CarouselModel());
    }
    private List<PhotoGalleryModel> CarouselModel()
    {
      List<PhotoGalleryModel> CarouselImageDetails = new List<PhotoGalleryModel>();

      var item = Context.Item;
      var parent = Context.Item.Parent;
      MultilistField PhotoList;

      PhotoList = parent.Fields["PropertyCarousel"];
      ImageField backgroundImage = parent.Fields["Background Image"];

      if (PhotoList != null && PhotoList.TargetIDs != null && PhotoList.Count > 0)
      {
        CarouselImageDetails = (from carouselImage in PhotoList.GetItems()
                                select new PhotoGalleryModel
                                {
                                  HashedUrl = StringUtil.EnsurePrefix('/', MediaManager.GetMediaUrl(carouselImage, new MediaUrlBuilderOptions { MaxWidth = 1280 }))
                                }).ToList();
      }
      else
      {
        if (backgroundImage != null && backgroundImage.MediaItem != null)
        {
          MediaItem image = new MediaItem(backgroundImage.MediaItem);
          string src = StringUtil.EnsurePrefix('/', MediaManager.GetMediaUrl(image));
          PhotoGalleryModel tmp = new PhotoGalleryModel();
          tmp.BackgroundImage = new HtmlString(src);

          CarouselImageDetails.Add(tmp);
        }
      }
      return CarouselImageDetails;
    }
  }
}
