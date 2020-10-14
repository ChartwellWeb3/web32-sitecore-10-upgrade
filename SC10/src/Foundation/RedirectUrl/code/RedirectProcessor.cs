using Chartwell.Foundation.utility;
using log4net;
using Sitecore;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Globalization;
using Sitecore.Pipelines.HttpRequest;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chartwell.Foundation.Redirect
{
  /// <summary>
  /// Pipeline modification to turn Aliases into 301 redirects
  /// </summary>
  public class RedirectProcessor : HttpRequestProcessor
  {
    public ILog Logger = LogManager.GetLogger("ChartwellLog");
    readonly ChartwellUtiles util = new ChartwellUtiles();
    public Language StrLang { get; set; }

    public override void Process(HttpRequestArgs args)
    {
      if (HttpContext.Current.Request.RawUrl.Contains("/sitecore") || HttpContext.Current.Request.RawUrl.Equals("/")
                                                                   || HttpContext.Current.Request.RawUrl.ToLower().Equals("/en")
                                                                   || HttpContext.Current.Request.RawUrl.ToLower().Equals("/fr"))
        return;

      string path = string.Empty;
      string UrlFrom404 = string.Empty;
      string UrlToSanitize = string.Empty;

      string RawUrlOnLoad = !HttpContext.Current.Request.RawUrl.ToString().Equals("/") ? HttpContext.Current.Request.RawUrl.ToString().TrimEnd('/').ToLower() :
                                                                                         HttpContext.Current.Request.RawUrl.ToString().ToLower();

      RawUrlOnLoad = HandleAccentedCharacters(RawUrlOnLoad);

      var QueryString = string.Empty;


      if (ValidRedirects(RawUrlOnLoad) && !HttpContext.Current.Request.QueryString.HasKeys())
      {
        if (HttpContext.Current.Request.QueryString.HasKeys())
        {
          QueryString = RawUrlOnLoad.Substring(RawUrlOnLoad.LastIndexOf("?"));
        }
        if (string.IsNullOrEmpty(QueryString))
        {
          var url = util.GetUrlFromUrlMapping(RawUrlOnLoad);
          if (url != null)
          {
            var UrlMap = url["dnn url"].TrimEnd('/');
            if (url["dnn url"].Contains("/en") && !url["dnn url"].Equals(RawUrlOnLoad))
            {
              RawUrlOnLoad = UrlMap;
            }
            else
            {
              if (RawUrlOnLoad.Equals("covid-19") || RawUrlOnLoad.Equals("/virus-covid-19"))
                RawUrlOnLoad = UrlMap;
            }

            if (UrlMap.Replace("/en/", "/").Replace("/fr/", "/").Equals(RawUrlOnLoad.Replace(" ", "-").Replace("/en/", "/").Replace("/fr/", "/")) && !(UrlMap.Contains("blog") || UrlMap.Contains("blogue")))
            {
              if (RawUrlOnLoad.Equals("/home"))
              {
                HttpContext.Current.Response.Redirect("/en");
              }
              else
              {
                string urlRedirect = string.Empty;
                if (url["new url"].ToString().ToLower().Equals("/en") || url["new url"].ToString().ToLower().Equals("/fr"))
                {
                  urlRedirect = url["new url"].ToString().ToLower();
                }
                else
                {
                  if (!HttpContext.Current.Request.Url.Host.Equals("chartwell.com") && !url["new url"].ToString().ToLower().StartsWith("http://"))
                    urlRedirect = !RawUrlOnLoad.Contains("/fr/") && !url["new url"].Contains("/en") ? "/" + url.Language.Name + "/" + url["new url"].ToString().ToLower() : url["new url"].ToString().ToLower();
                  else if (!HttpContext.Current.Request.Url.Host.Equals("chartwell.com") && url["new url"].ToString().ToLower().StartsWith("http://"))
                    urlRedirect = url["new url"].ToString().ToLower().Replace("careersatchartwell.com", "dnnstage");
                  else
                    urlRedirect = url["new url"].ToString().ToLower();
                }
                HttpContext.Current.Response.Redirect(urlRedirect);
              }
            }
          }

          if (!RawUrlOnLoad.Equals("/en") && !RawUrlOnLoad.Equals("/fr") && !(RawUrlOnLoad.ToLower().Equals("/careers") ||
                                                                              RawUrlOnLoad.ToLower().Equals("/carrières") ||
                                                                              RawUrlOnLoad.ToLower().Replace(" ", "-").Equals("/investor-relations") ||
                                                                              RawUrlOnLoad.ToLower().Equals("/investorrelations") ||
                                                                              RawUrlOnLoad.ToLower().Equals("/investisseurs")))
          {

            var splitRawUrl = RawUrlOnLoad.Split('/').Where(s =>
                              !string.IsNullOrEmpty(s)).ToList(); //&& !(s.Equals("en") || s.Equals("fr"))

            var ItemFromUrl = RawUrlOnLoad.Substring(RawUrlOnLoad.LastIndexOf('/') + 1);

            var CheckForStaticItem = ItemFromUrl.Split('/').Where(s =>
                                    !string.IsNullOrEmpty(s) && !(s.Equals("en") || s.Equals("fr"))).ToList().Count == 1 &&
                                                                                    !splitRawUrl.Contains("retirement-residences") &&
                                                                                    !splitRawUrl.Contains("résidences-pour-retraités");
            var itemType = util.GetItemTypeForItemFromUrl(RawUrlOnLoad);
            List<SearchResultItem> PropertySubItemFromUrl = null;
            if (itemType != null && itemType.TemplateName.Equals("PropertyPage"))
            {
              PropertySubItemFromUrl = util.LeftNavChildItemTemplateName(Translate.Text("overview")).ToList();
              ItemFromUrl = Translate.Text("overview");
            }
            else
            {
              PropertySubItemFromUrl = util.LeftNavChildItemTemplateName(ItemFromUrl).ToList();
            }

            bool IsPropertyPage = false;
            if (CheckForStaticItem)
              IsPropertyPage = util.GetStaticPageItem(ItemFromUrl).Count == 0 ? true : false;
            else
              IsPropertyPage = true;

            Language.TryParse(splitRawUrl.Contains("en") ? "en" : splitRawUrl.Contains("fr") ? "fr" : string.Empty, out Language LangFromUrl);

            if (PropertySubItemFromUrl.Count != 0 && IsPropertyPage)
            {
              bool ForceRedirection = false;
              if (string.IsNullOrEmpty(LangFromUrl.Name))
              {
                LangFromUrl = PropertySubItemFromUrl.Where(p => p.GetItem().DisplayName.Replace("-", "").Replace("'", "").Replace(" ", "").RemoveDiacritics().ToLower()
                                                                 .Contains(ItemFromUrl.Replace("-", "").Replace("'", "").Replace(" ", "").RemoveDiacritics().ToLower()) ||
                                                                 ItemFromUrl.RemoveDiacritics().Replace("-", "").Replace(" ", "").Contains(p.GetItem()
                                                                 .DisplayName.ToLower().RemoveDiacritics().Replace("-", "").Replace(" ", "")))
                                                                 .FirstOrDefault().GetItem().Language;
                ForceRedirection = true;
              }

              if (!string.IsNullOrEmpty(LangFromUrl.Name))
              {
                var PropertySubItem = PropertySubItemFromUrl.Where(i => i.GetItem().DisplayName.Replace("-", "").Replace(" ", "").RemoveDiacritics().Equals(ItemFromUrl.Replace("-", "").Replace(" ", "").RemoveDiacritics()) ||
                                                                   i.GetItem().DisplayName.Replace("-", "").Replace("'", "").Replace(" ", "").RemoveDiacritics().ToLower()
                                                                    .Contains(ItemFromUrl.Replace("-", "").Replace("'", "").Replace(" ", "").RemoveDiacritics().ToLower()) ||
                                                                     ItemFromUrl.RemoveDiacritics().Replace("-", "").Replace(" ", "").Contains(i.GetItem().DisplayName.ToLower().Replace(" ", "").RemoveDiacritics().Replace("-", "")))
                                                                    .FirstOrDefault().GetItem();

                var RedirectionPropertySubItem = PropertySubItemFromUrl.Where(l => l.Language.Equals(LangFromUrl.Name)).FirstOrDefault().GetItem();

                if (Context.Item == null || ForceRedirection || (!LangFromUrl.Name.Equals(PropertySubItem.Language.Name)
                                          && RedirectionPropertySubItem.DisplayName != "photos"))
                {
                  if (ForceRedirection) ForceRedirection = false;

                  if (PropertySubItemFromUrl != null && PropertySubItemFromUrl.Count != 0)
                  {
                    var ItemNameToSearchFromUrl = splitRawUrl.Where(t => !(t.Equals("en") || t.Equals("fr")) &&
                                                                         !t.Equals(PropertySubItem.DisplayName) &&
                                                                         !(t.Equals("retirement-residences") || t.Equals("résidences-pour-retraités"))).FirstOrDefault();

                    var PropertyItemList = util.DetermineRedirection(ItemNameToSearchFromUrl).ToList();

                    Item PropertyItem = null;
                    if (PropertyItemList != null && PropertyItemList.Count > 0)
                    {
                      var item = PropertyItemList.Where(i => i.Language.Equals(LangFromUrl.Name)
                                                  && i.TemplateName != "Static Pages"
                                                  && (i.GetItem().DisplayName.Replace("-", " ").Replace("'", "").Replace(" ", "").ToLower().RemoveDiacritics()
                                           .Contains(ItemNameToSearchFromUrl.Replace("-", " ").Replace("'", "").Replace(" ", "").ToLower().RemoveDiacritics()) ||
                                           i.Name.Equals(ItemNameToSearchFromUrl.Replace("-", " ").Replace("'", "").Replace(" ", "").RemoveDiacritics()))
                                           ).FirstOrDefault().GetItem();

                      PropertyItem = item;
                    }
                    if (PropertyItem != null)
                    {
                      var PropertyItemUrl = PropertyItem.Children.Where(c => c.Language.Equals(LangFromUrl) &&
                                                                             c.DisplayName.Equals(RedirectionPropertySubItem.DisplayName)).FirstOrDefault();

                      // TODO - Handle Community Pages with Left Nav item name 
                      if (PropertyItemUrl != null)
                      {
                        Context.Item = ItemManager.GetItem(PropertyItemUrl.ID, PropertyItemUrl.Language, Sitecore.Data.Version.Latest, Context.Database);
                        var RedirectUrl = util.GetitemUrl(PropertyItemUrl, PropertyItemUrl.Language);
                        RedirectUrl = RedirectUrl.Substring(RedirectUrl.LastIndexOf("/") + 1).Equals("nous-joindre") ? RedirectUrl.Replace("nous-joindre", "nous joindre") : RedirectUrl;
                        HttpContext.Current.Response.Redirect(RedirectUrl);
                      }
                      else
                      {
                        Context.Item = ItemManager.GetItem(PropertyItem.ID, PropertyItem.Language, Sitecore.Data.Version.Latest, Context.Database);
                        var RedirectUrl = util.GetitemUrl(PropertyItem, PropertyItem.Language);
                        RedirectUrl = RedirectUrl.Substring(RedirectUrl.LastIndexOf("/") + 1).Equals("nous-joindre") ? RedirectUrl.Replace("nous-joindre", "nous joindre") : RedirectUrl;
                        HttpContext.Current.Response.Redirect(RedirectUrl);
                      }
                    }
                  }
                }
                if (RawUrlOnLoad.Contains("nous-joindre"))
                {
                  var RedirectUrl = RawUrlOnLoad.Substring(RawUrlOnLoad.LastIndexOf("/") + 1).Equals("nous-joindre") ? RawUrlOnLoad.Replace("nous-joindre", "nous joindre") : RawUrlOnLoad;
                  HttpContext.Current.Response.Redirect(RedirectUrl);
                }
              }
            }
            else
            {
              //var StaticPageItemList = util.DetermineRedirection(ItemFromUrl).ToList()
              //                             .Where(d => d.GetItem().DisplayName.Equals(ItemFromUrl) || d.GetItem().DisplayName.Replace("-", " ").Replace("'", "").ToLower()
              //                             .Contains(ItemFromUrl.Replace("-", " ").Replace("'", "").ToLower())).OrderBy(o => ItemFromUrl).ToList();
              var StaticPageItemList = util.DetermineRedirection(ItemFromUrl).ToList()
                                              .Where(d => d.GetItem().DisplayName.Replace(" ", "-").ToLower().Equals(ItemFromUrl) || d.GetItem().DisplayName.Replace("-", " ").Replace("'", "").Replace("’", "'").ToLower()
                                              .Contains(ItemFromUrl.Replace("-", " ").Replace("'", "").Replace("’", "'").ToLower())).ToList();

              if (StaticPageItemList.Count() == 2)
                StaticPageItemList = StaticPageItemList.Where(d => d.GetItem().DisplayName.Replace(" ", "-").ToLower().Equals(ItemFromUrl)).ToList();

              if (StaticPageItemList.Count != 0)
              {
                List<SearchResultItem> FilterStaticPagesItemList = new List<SearchResultItem>();
                if (StaticPageItemList.Count > 2)
                {
                  FilterStaticPagesItemList = StaticPageItemList.Where(n => n.GetItem().DisplayName.ToLower().Replace("-", " ").Replace("'", "").Equals(ItemFromUrl.ToLower().Replace("-", " ").Replace("'", ""))).ToList();
                }

                var StaticPageItemID = FilterStaticPagesItemList.Count == 0 ? util.PropertyDetails(StaticPageItemList.Where(s => s.TemplateName.Equals("Static Pages") ||
                                                                                                                                 s.TemplateName.Equals("PropertyPage") ||
                                                                                                                                 s.TemplateName.Equals("customPage") ||
                                                                                                                                 s.TemplateName.Equals("RegionalPropertiesPage") ||
                                                                                                                                 s.TemplateName.Equals("SplitterPage") ||
                                                                                                                                 s.TemplateName.Equals("NewResidencePage")
                                                                                                                                 ).FirstOrDefault().GetItem().ID) :
                                                                              util.PropertyDetails(FilterStaticPagesItemList.Where(s => s.TemplateName.Equals("Static Pages") || s.TemplateName.Equals("PropertyPage") || s.TemplateName.Equals("customPage")).FirstOrDefault().GetItem().ID);

                var StaticItemLangFromUrl = StaticPageItemID.Where(i => i.GetItem().DisplayName.ToLower().Replace("-", " ").Replace("'", "")
                                                                         .Contains(ItemFromUrl.ToLower().Replace("-", " ").Replace("'", "")))
                                                                         .OrderBy(o => o.Language)
                                                                         .FirstOrDefault().GetItem();

                if (!string.IsNullOrEmpty(LangFromUrl.Name))
                {
                  if (!LangFromUrl.Name.Equals(StaticItemLangFromUrl.Language.Name))
                  {
                    var RedirectUrl = string.Empty;
                    var StaticPageRedirect = StaticPageItemID.Where(d => d.Language.Equals(LangFromUrl.Name)).FirstOrDefault().GetItem();
                    Context.Item = ItemManager.GetItem(StaticPageRedirect.ID, StaticPageRedirect.Language, Sitecore.Data.Version.Latest, Context.Database);

                    if (Context.Item.TemplateName != "PropertyPage")
                    {
                      RedirectUrl = util.GetitemUrl(StaticPageRedirect, StaticPageRedirect.Language);
                    }
                    else
                    {
                      RedirectUrl = util.GetitemUrl(StaticPageRedirect, StaticPageRedirect.Language) + "/" + Translate.Text("overview");
                    }
                    HttpContext.Current.Response.Redirect(RedirectUrl);
                  }
                  else
                  {
                    var StaticPageRedirect = StaticPageItemID.Where(d => d.Language.Equals(LangFromUrl.Name)).FirstOrDefault().GetItem();
                    if ((StaticPageRedirect.Parent.Name.Equals("Project") || StaticPageRedirect.Parent.Name.Equals("retirement-residences")) && StaticPageRedirect.HasChildren)
                    {
                      var RedirectUrl = util.GetitemUrl(StaticPageRedirect.Parent.Name.Equals("Project") ?
                                          StaticPageRedirect.GetChildren().FirstOrDefault() : StaticPageRedirect.GetChildren().Where(c => c.Name.Equals("overview")).FirstOrDefault(),
                                          StaticPageRedirect.Language);
                      HttpContext.Current.Response.Redirect(RedirectUrl);
                    }
                    else
                    {
                      Context.Item = ItemManager.GetItem(StaticPageRedirect.ID, StaticPageRedirect.Language, Sitecore.Data.Version.Latest, Context.Database);
                    }
                    if (UrlContainsAccentedCharacters())
                    {
                      var RedirectUrl = util.GetitemUrl(StaticPageRedirect, StaticPageRedirect.Language);
                      HttpContext.Current.Response.Redirect(RedirectUrl);
                    }
                  }
                }
                else
                {
                  LangFromUrl = StaticItemLangFromUrl.Language;
                  string RedirectUrl = string.Empty;
                  var StaticPageRedirect = StaticPageItemID.Where(d => d.Language.Equals(LangFromUrl.Name)).FirstOrDefault().GetItem();

                  if ((StaticPageRedirect.Parent.Name.Equals("Project") || StaticPageRedirect.Parent.Name.Equals("retirement-residences")) && StaticPageRedirect.HasChildren)
                  {
                    RedirectUrl = util.GetitemUrl(StaticPageRedirect.Parent.Name.Equals("Project") ?
                                        StaticPageRedirect.GetChildren().FirstOrDefault() : StaticPageRedirect.GetChildren().Where(c => c.Name.Equals("overview")).FirstOrDefault(),
                                        StaticPageRedirect.Language);
                  }
                  else
                  {
                    RedirectUrl = util.GetitemUrl(StaticPageRedirect, StaticPageRedirect.Language);
                  }
                  Context.Item = ItemManager.GetItem(StaticPageRedirect.ID, StaticPageRedirect.Language, Sitecore.Data.Version.Latest, Context.Database);
                  HttpContext.Current.Response.Redirect(RedirectUrl);
                }
              }
            }
          }
          else if (RawUrlOnLoad.Equals("/careers") || RawUrlOnLoad.Equals("/carrières"))
          {
            var RedirectForCareers = string.Empty;
            var HostName = HttpContext.Current.Request.Url.Host;
            var ItemFromUrl = RawUrlOnLoad.Substring(RawUrlOnLoad.LastIndexOf('/') + 1);

            if (HostName.Equals("chartwell.com"))
            {
              RedirectForCareers = ItemFromUrl.Equals("careers") ? HttpContext.Current.Request.Url.Scheme + "://" + "careersatchartwell.com" : HttpContext.Current.Request.Url.Scheme + "://" + "careersatchartwell.com/fr";
            }
            else
            {
              RedirectForCareers = ItemFromUrl.Equals("careers") ? "http://dnnstage" : "http://dnnstage/fr";
            }
            HttpContext.Current.Response.Redirect(RedirectForCareers, true);
          }
          else if (RawUrlOnLoad.ToLower().Replace(" ", "-").Equals("/investor-relations") ||
                   RawUrlOnLoad.ToLower().Equals("/investisseurs") ||
                   RawUrlOnLoad.ToLower().Equals("/investorrelations"))
          {
            var RedirectForInvestorRelations = string.Empty;
            var ItemFromUrl = RawUrlOnLoad.Substring(RawUrlOnLoad.LastIndexOf('/') + 1);

            RedirectForInvestorRelations = ItemFromUrl.Replace(" ", "-").ToLower().Equals("investor-relations") || ItemFromUrl.ToLower().Equals("investorrelations") ? "https://investors.chartwell.com/company-profile/" : "https://investors.chartwell.com/French/Profil-de-la-socit/";

            HttpContext.Current.Response.Redirect(RedirectForInvestorRelations, true);
          }

        }
        else if (ValidRedirectsBlog(RawUrlOnLoad) && !HttpContext.Current.Request.QueryString.HasKeys())
        {
          var OrigLang = string.Empty;
          var ContextlangFromUrl = util.GetBlogPost(RawUrlOnLoad, ref OrigLang);

          if (OrigLang != ContextlangFromUrl.Language.Name)
          {
            Context.SetLanguage(ContextlangFromUrl.Language, true);
            string ItemUrlOnLoad = string.Empty;
            Context.Item = ItemManager.GetItem(ContextlangFromUrl.ID, ContextlangFromUrl.Language, Sitecore.Data.Version.Latest, Context.Database);
            ItemUrlOnLoad = util.GetitemUrl(ContextlangFromUrl, ContextlangFromUrl.Language);
            HttpContext.Current.Response.Redirect(ItemUrlOnLoad);
          }
        }
        else if (RawUrlOnLoad.Contains("search-results") || RawUrlOnLoad.Contains("résultat-de-la-recherche"))
        {
          var queryStrKey = string.Empty;
          Language.TryParse(RawUrlOnLoad.Contains("/en") ? "en" : RawUrlOnLoad.Contains("/fr") ? "fr" : string.Empty, out Language LangFromUrl);

          var SearchKeyWord = RawUrlOnLoad.Split('/').Where(s => !string.IsNullOrEmpty(s)).ToList();
          if (SearchKeyWord[2].ToLower().Contains("city") || SearchKeyWord[2].ToLower().Contains("nom-de-la-ville"))
          {
            queryStrKey = (util.GetDictionaryItem("CitySearch", SearchKeyWord[0])).ToLower();
          }
          else if (SearchKeyWord[2].Contains("propertyname"))
          {
            queryStrKey = "propertyname";
          }
          else if (SearchKeyWord[2].Contains("postalcode"))
          {
            queryStrKey = "postalcode";
          }

          var SearchKeyWordLang = util.GetDictionaryItem("SearchResults").Where(x => x.GetItem().Fields["phrase"].Value.Equals(SearchKeyWord[1])).FirstOrDefault().Language;

          if (!LangFromUrl.Name.Equals(SearchKeyWordLang))
          {
            var SearchRedirectUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host + "/" + LangFromUrl.Name + "/" +
                                                                              util.GetDictionaryItem("SearchResults", LangFromUrl.Name) + "/?" +
                                                                              queryStrKey + "=" + RawUrlOnLoad.Substring(RawUrlOnLoad.LastIndexOf("=") + 1);
            HttpContext.Current.Response.Redirect(SearchRedirectUrl.ToLower());
          }
        }
      }
      else if (ValidRedirectsBlog(RawUrlOnLoad) && !HttpContext.Current.Request.QueryString.HasKeys())
      {

        var blogSplitRawUrl = RawUrlOnLoad.Split('/').Where(s =>
                    !string.IsNullOrEmpty(s)).ToList();

        Language.TryParse(blogSplitRawUrl.Contains("en") ? "en" : blogSplitRawUrl.Contains("fr") ? "fr" : string.Empty, out Language BlogLangFromUrl);

        var OrigLang = BlogLangFromUrl.Name;

        var blogPostItem = RawUrlOnLoad.Substring(RawUrlOnLoad.LastIndexOf('/') + 1).RemoveDiacritics();

        var blogItem = util.GetBlogPost(RawUrlOnLoad, ref OrigLang);

        if (blogItem != null)
        {
          Context.Item = ItemManager.GetItem(blogItem.ID, blogItem.Language, Sitecore.Data.Version.Latest, Context.Database);
          var blogRedirectUrl = util.GetitemUrl(blogItem, blogItem.Language);

          if (RawUrlOnLoad.Equals("/en/blogue") || RawUrlOnLoad.Equals("/fr/blog") || Context.Item.Language.Name != BlogLangFromUrl.Name)
          {
            //var blogRedirectUrl = util.GetitemUrl(blogItem, blogItem.Language);
            //OrigLang = Context.Item.Language.Name;
            HttpContext.Current.Response.Redirect(blogRedirectUrl);
          }
          else if (UrlContainsAccentedCharacters() || (!RawUrlOnLoad.Equals(blogRedirectUrl) && !RawUrlOnLoad.Contains("/fr")))
          {
            //var blogRedirectUrl = util.GetitemUrl(blogItem, blogItem.Language);
            HttpContext.Current.Response.Redirect(blogRedirectUrl);
          }
          else if (RawUrlOnLoad.Contains("/fr") && RawUrlOnLoad.Contains("/blog/"))
          {
            HttpContext.Current.Response.Redirect(blogRedirectUrl);
          }
        }
      }
      else if (HttpContext.Current.Request.QueryString.HasKeys() && (RawUrlOnLoad.Contains("search-results") || RawUrlOnLoad.Contains("résultat-de-la-recherche")) && !RawUrlOnLoad.Contains("/ErrorHandling"))
      {
        var queryStrKey = string.Empty;
        var queryStrKeyValue = string.Empty;
        bool ForceRedirection = false;
        if (!RawUrlOnLoad.Contains("/en") && !RawUrlOnLoad.Contains("/fr"))
        {
          RawUrlOnLoad = RawUrlOnLoad.Contains("search-results") ? "/en" + RawUrlOnLoad : "/fr" + RawUrlOnLoad;
          ForceRedirection = true;
        }
        Language.TryParse(RawUrlOnLoad.Contains("/en") ? "en" : RawUrlOnLoad.Contains("/fr") ? "fr" : string.Empty, out Language LangFromUrl);

        var UrlWithQueryString = RawUrlOnLoad.Split('&');

        var SearchKeyWord = UrlWithQueryString[0].Split('/').Where(s => !string.IsNullOrEmpty(s)).ToList();

        if (RawUrlOnLoad.Contains("&"))
        {
          queryStrKeyValue = RawUrlOnLoad.Substring(RawUrlOnLoad.IndexOf("?") + 1, (RawUrlOnLoad.Substring(RawUrlOnLoad.IndexOf("?")).Length -
                                                                                    RawUrlOnLoad.Substring(RawUrlOnLoad.IndexOf("&")).Length) - 1)
                                                                                    .Split('=').FirstOrDefault().ToLower();
        }
        else
        {
          queryStrKeyValue = RawUrlOnLoad.Substring(RawUrlOnLoad.IndexOf("?") + 1).Split('=').FirstOrDefault().ToLower();
        }

        if (queryStrKeyValue.Contains("city") || queryStrKeyValue.ToLower().Contains("nom-de-la-ville"))
        {
          queryStrKey = util.GetDictionaryItem("CitySearch", LangFromUrl.Name).ToLower();
        }
        else if (queryStrKeyValue.Contains("propertyname"))
        {
          queryStrKey = "propertyname";
        }
        else if (queryStrKeyValue.Contains("postalcode"))
        {
          queryStrKey = "postalcode";
        }

        var SearchKeyWordLang = util.GetDictionaryItem("SearchResults").Where(x => x.GetItem().Fields["phrase"].Value.Equals(string.IsNullOrEmpty(LangFromUrl.Name) ? SearchKeyWord[0] : SearchKeyWord[1])).FirstOrDefault().Language;

        if (!LangFromUrl.Name.Equals(SearchKeyWordLang) || ForceRedirection)
        {
          var SearchRedirectUrl = string.Join("&", UrlWithQueryString)
                                        .Replace("search-results", util.GetDictionaryItem("SearchResults", LangFromUrl.Name)).ToLower()
                                        .Replace("résultat-de-la-recherche", util.GetDictionaryItem("SearchResults", LangFromUrl.Name)).ToLower()
                                        .Replace(queryStrKeyValue, queryStrKey);

          HttpContext.Current.Response.Redirect(SearchRedirectUrl.ToLower());
        }
      }
    }
    private static bool UrlContainsAccentedCharacters()
    {
      return HttpContext.Current.Request.RawUrl.Contains("ÃƒÆ’Ã‚Â©") ||
             HttpContext.Current.Request.RawUrl.Contains("Ã©") ||
             HttpContext.Current.Request.RawUrl.Contains("ã©") ||
             HttpContext.Current.Request.RawUrl.Contains("Ã¨") ||
             HttpContext.Current.Request.RawUrl.Contains("ÃƒÆ’Ã‚Â¨") ||
             HttpContext.Current.Request.RawUrl.Contains("Ã¹") ||
             HttpContext.Current.Request.RawUrl.Contains("Ãª") ||
             HttpContext.Current.Request.RawUrl.Contains("Ã®") ||
             HttpContext.Current.Request.RawUrl.Contains("ÃƒÆ’Ã‚Â´") ||
             HttpContext.Current.Request.RawUrl.Contains("Ã§") ||
             HttpContext.Current.Request.RawUrl.Contains("Ã ") ||
             HttpContext.Current.Request.RawUrl.Contains("%C3%83%C2%A9") ||
             HttpContext.Current.Request.RawUrl.Contains("â€™") ||
             HttpContext.Current.Request.RawUrl.Contains("Ã%C2%A0") ||
             HttpContext.Current.Request.RawUrl.Contains("ãƒâ©") ||
             HttpContext.Current.Request.RawUrl.Contains("ãƒâ§") ||
             HttpContext.Current.Request.RawUrl.Contains("ãƒâ") ||
             HttpContext.Current.Request.RawUrl.Contains("ãƒâª") ||
             HttpContext.Current.Request.RawUrl.Contains("ÃƒÆ’Ã‚Æ’Ãƒâ€ Ã‚â€™ÃƒÆ’Ã‚â€šÃƒâ€šÃ‚Â©") ||
             HttpContext.Current.Request.RawUrl.Contains("Ã¢") ||
             HttpContext.Current.Request.RawUrl.Contains("ÃƒÂ¢Ã‚â‚¬Ã‚â„¢") ||
             HttpContext.Current.Request.RawUrl.Contains("Ã£Æ'Ã¢Â©") ||
             HttpContext.Current.Request.RawUrl.Contains("£Æ’Ã¢Â©") ||
             HttpContext.Current.Request.RawUrl.Contains("ã£æ’ã¢â©") ||
             HttpContext.Current.Request.RawUrl.Contains("Ã£Æ’Ã¢Â§") ||
             HttpContext.Current.Request.RawUrl.Contains("ã£æ’ã¢â§");
    }

    private static string HandleAccentedCharacters(string RawUrlOnLoad)
    {
      if (RawUrlOnLoad.Contains("ÃƒÆ’Ã‚Â©") || RawUrlOnLoad.Contains("Ã©") || RawUrlOnLoad.Contains("ã©")
                                            || RawUrlOnLoad.Contains("%C3%83%C2%A9") || RawUrlOnLoad.Contains("ãƒâ©")
                                            || RawUrlOnLoad.Contains("ÃƒÆ’Ã‚Æ’Ãƒâ€ Ã‚â€™ÃƒÆ’Ã‚â€šÃƒâ€šÃ‚Â©") || RawUrlOnLoad.Contains("Ã£Æ'Ã¢Â©")
                                            || RawUrlOnLoad.Contains("Ã£Æ’Ã¢Â©") || RawUrlOnLoad.Contains("ã£æ’ã¢â©"))
      {
        RawUrlOnLoad = RawUrlOnLoad.Replace("Ã©", "é").Replace("ã©", "é")
                                                      .Replace("ÃƒÆ’Ã‚Â©", "é").Replace("%C3%83%C2%A9", "é")
                                                      .Replace("ãƒâ©", "é").Replace("ÃƒÆ’Ã‚Æ’Ãƒâ€ Ã‚â€™ÃƒÆ’Ã‚â€šÃƒâ€šÃ‚Â©", "é")
                                                      .Replace("Ã£Æ'Ã¢Â©", "é").Replace("Ã£Æ’Ã¢Â©", "é");
      }

      if (RawUrlOnLoad.Contains("Ã¨") || RawUrlOnLoad.Contains("ÃƒÆ’Ã‚Â¨"))
      {
        RawUrlOnLoad = RawUrlOnLoad.Replace("ÃƒÆ’Ã‚Â¨", "è").Replace("Ã¨", "è");
      }

      if (RawUrlOnLoad.Contains("Ã¹"))
      {
        RawUrlOnLoad = RawUrlOnLoad.Replace("Ã¹", "ù");
      }

      if (RawUrlOnLoad.Contains("Ãª"))
      {
        RawUrlOnLoad = RawUrlOnLoad.Replace("Ãª", "ê");
      }

      if (RawUrlOnLoad.Contains("Ã®"))
      {
        RawUrlOnLoad = RawUrlOnLoad.Replace("Ã®", "î");
      }

      if (RawUrlOnLoad.Contains("ÃƒÆ’Ã‚Â´"))
      {
        RawUrlOnLoad = RawUrlOnLoad.Replace("ÃƒÆ’Ã‚Â´", "ô");
      }

      if (RawUrlOnLoad.Contains("Ã§") || RawUrlOnLoad.Contains("ãƒâ§") || RawUrlOnLoad.Contains("Ã£Æ’Ã¢Â§") || RawUrlOnLoad.Contains("ã£æ’ã¢â§"))
      {
        RawUrlOnLoad = RawUrlOnLoad.Replace("Ã§", "ç").Replace("ãƒâ§", "ç").Replace("Ã£Æ’Ã¢Â§", "ç").Replace("ã£æ’ã¢â§", "ç");
      }

      if (RawUrlOnLoad.Contains("Ã ") || RawUrlOnLoad.Contains("Ã%C2%A0") || RawUrlOnLoad.Contains("ãƒâ"))
      {
        RawUrlOnLoad = RawUrlOnLoad.Replace("Ã ", "à").Replace("Ã%C2%A0", "à").Replace("ãƒâ", "à");
      }

      if (RawUrlOnLoad.Contains("â€™") || RawUrlOnLoad.Contains("ÃƒÂ¢Ã‚â‚¬Ã‚â„¢"))
      {
        RawUrlOnLoad = RawUrlOnLoad.Replace("â€™", "'").Replace("ÃƒÂ¢Ã‚â‚¬Ã‚â„¢", "'");
      }

      if (RawUrlOnLoad.Contains("ãƒâª"))
      {
        RawUrlOnLoad = RawUrlOnLoad.Replace("ãƒâª", "ê");
      }

      if (RawUrlOnLoad.Contains("Ã¢"))
      {
        RawUrlOnLoad = RawUrlOnLoad.Replace("Ã¢", "â");
      }

      return RawUrlOnLoad;
    }

    private static bool ValidRedirects(string RawUrlOnLoad)
    {
      return !RawUrlOnLoad.Equals("/") && !(RawUrlOnLoad.Equals("/en") || RawUrlOnLoad.Equals("/fr")) &&
                                          !RawUrlOnLoad.Contains("sitecore") && !RawUrlOnLoad.Contains("shell") &&
                                          !RawUrlOnLoad.Contains("api") && !RawUrlOnLoad.Contains("system") &&
                                          !RawUrlOnLoad.Contains("LatLngSearch") &&
                                          !RawUrlOnLoad.Contains("/ErrorHandling") &&
                                          !(RawUrlOnLoad.Equals("/en/blog") || RawUrlOnLoad.Equals("/fr/blogue")) &&
                                          !RawUrlOnLoad.Contains("formbuilder") &&
                                          !RawUrlOnLoad.Contains("/sitecore/api/ssc/EXM") &&
                                          !(RawUrlOnLoad.Contains("blog") || RawUrlOnLoad.Contains("blogue"));

    }
    private static bool ValidRedirectsBlog(string RawUrlOnLoad)
    {
      return !RawUrlOnLoad.Equals("/") && !(RawUrlOnLoad.Equals("/en") || RawUrlOnLoad.Equals("/fr")) &&
                                          !RawUrlOnLoad.Contains("sitecore") && !RawUrlOnLoad.Contains("shell") &&
                                          !RawUrlOnLoad.Contains("api") && !RawUrlOnLoad.Contains("system") &&
                                          !RawUrlOnLoad.Contains("LatLngSearch") &&
                                          !RawUrlOnLoad.Contains("/ErrorHandling") &&
                                          !RawUrlOnLoad.Contains("/sitecore/api/ssc/EXM") &&
                                          !(RawUrlOnLoad.Contains("search-results") || RawUrlOnLoad.Contains("résultat-de-la-recherche")) &&
                                          !(RawUrlOnLoad.Equals("/en/blog") || RawUrlOnLoad.Equals("/fr/blogue")) &&
                                          !RawUrlOnLoad.Contains("formbuilder");
    }
  }
}
