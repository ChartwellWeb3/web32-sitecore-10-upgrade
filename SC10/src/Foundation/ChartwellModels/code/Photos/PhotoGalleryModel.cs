using Sitecore.Data.Items;
using System.Web;

namespace Chartwell.Foundation.Models
{
  public class PhotoGalleryModel
  {
    public HtmlString PropertyFormattedAddress { get; set; }
    public Item InnerItem { get; set; }
    public string HashedUrl { get; set; }
    public string ImageMetaDataAlt { get; set; }
    public string ImageMetaDataDesc { get; set; }
    public string ImageMetaDataTitle { get; set; }
    public HtmlString BackgroundImage { get; set; }
  }
}