using Chartwell.Foundation.utility;
using Sitecore.Data.Items;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.Resources.Media;
using System.Linq;
using System.Web;

namespace Chartwell.Foundation.MediaRedirect
{
  public class MediaRedirectProcessor : HttpRequestProcessor
  {
    private readonly ChartwellUtiles GetMediaUrl = new ChartwellUtiles();

    public override void Process(HttpRequestArgs args)
    {
      if (!(HttpContext.Current.Request.Url.PathAndQuery.Contains("blog") || HttpContext.Current.Request.Url.PathAndQuery.Contains("blogue"))
                                                                            && !HttpContext.Current.Request.Url.PathAndQuery.Equals("/")
                                                                            && !(HttpContext.Current.Request.Url.PathAndQuery.Equals("/home")
                                                                            || HttpContext.Current.Request.Url.PathAndQuery.Equals("/accueil"))
                                                                            )
      {
        //char[] arr1 = new char[] { '/', '?', '.' };
        //MediaItem mediaItem = null;
        string newUrl = string.Empty;

        string LocalPath = string.Empty, MediaUrl = string.Empty;

        if (HttpContext.Current.Request.Url.PathAndQuery.Contains("/-/media")
            && !HttpContext.Current.Request.Url.PathAndQuery.Contains(".jpg")
            && !HttpContext.Current.Request.Url.PathAndQuery.Contains(".mp4")
            && !HttpContext.Current.Request.Url.PathAndQuery.Contains(".ashx")
            && !HttpContext.Current.Request.Url.PathAndQuery.Contains(".png")
            && !HttpContext.Current.Request.Url.PathAndQuery.Contains("Images")
            )
        {
          //MediaUrl = HttpContext.Current.Request.Url.PathAndQuery.Split(arr1).Where(x => x.Contains("chartwell")).FirstOrDefault();
          MediaUrl = HttpContext.Current.Request.Url.PathAndQuery;

          if (!string.IsNullOrEmpty(MediaUrl))
          {
            var newUrlItem = GetMediaUrl.GetMediaItemRedirectUrl(MediaUrl);
            newUrl = string.IsNullOrEmpty(newUrlItem) ? string.Empty : newUrlItem;
            //LocalPath = HttpContext.Current.Request.Url.LocalPath.Split('.').Where(x => !x.Contains("pdf")).FirstOrDefault().Replace(@"/-/media", "");
          }
        }

        if (!string.IsNullOrEmpty(MediaUrl) && !string.IsNullOrEmpty(newUrl)) // && mediaItem != null)
        {
          if (!string.IsNullOrEmpty(MediaUrl)) // && (mediaItem != null && !LocalPath.ToLower().Equals(mediaItem.MediaPath.ToLower())) && mediaItem.MimeType.Equals("application/pdf"))
          {
            var RedirectMediaUrl = HttpContext.Current.Request.Url.Scheme
                                    + "://"
                                    + HttpContext.Current.Request.Url.Host
                                    + newUrl; // MediaManager.GetMediaUrl(GetMediaUrl.GetMediaItemRedirectUrl(MediaUrl));

            HttpContext.Current.Response.Redirect(RedirectMediaUrl);
          }
        }
      }
    }
  }
}