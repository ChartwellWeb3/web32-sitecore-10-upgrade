using Chartwell.Foundation.Models;
using Chartwell.Foundation.utility;
using Sitecore;
using Sitecore.Analytics;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Links.UrlBuilders;
using Sitecore.Resources.Media;
using Sitecore.XConnect;
using Sitecore.XConnect.Client;
using Sitecore.XConnect.Client.Configuration;
using Sitecore.XConnect.Collection.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

public class RecentlyViewedPagesController : Controller
{
    private ChartwellUtiles util = new ChartwellUtiles();

    public ActionResult Index()
    {
        List<RecentlyViewedPagesModel> list = new List<RecentlyViewedPagesModel>();
        if (Tracker.Enabled)
        {
            list = GetRecentInteractions();
        }
        return PartialView(list);
    }

    public List<RecentlyViewedPagesModel> GetRecentInteractions()
    {
        string identifier = Tracker.Current.Contact.ContactId.ToString("N");
        List<RecentlyViewedPagesModel> result = new List<RecentlyViewedPagesModel>();
        using (XConnectClient context = SitecoreXConnectClientConfiguration.GetClient())
        {
            IdentifiedContactReference reference = new IdentifiedContactReference("xDB.Tracker", identifier);
            Contact contact = context.Get(reference, new ContactExecutionOptions(new ContactExpandOptions { }));
            if (contact != null)
            {
                Guid id = Guid.Parse(contact.Id.ToString());
                //ContactExpandOptions expandOptions = new ContactExpandOptions
                //{
                //    Interactions = new RelatedInteractionsExpandOptions
                //    {
                //        Limit = int.MaxValue
                //    }
                //    .Expand<IpInfo>()
                //};
                Contact contact2 = context.Get(new ContactReference(id), new ContactExecutionOptions(new ContactExpandOptions
                {
                    Interactions = new RelatedInteractionsExpandOptions
                    {
                        Limit = int.MaxValue
                    }
                    .Expand<IpInfo>()
                }));
                List<PageViewEvent> list = new List<PageViewEvent>();
                foreach (Interaction interaction in contact2.Interactions)
                {
                    list.AddRange(from x in interaction.Events.OfType<PageViewEvent>()
                                  where x.Url.Contains(util.GetDictionaryItem("overview", Context.Language.Name))
                                  && !x.Url.Contains("/VoiceDictation") && !x.Url.Contains("404") && !x.Url.Contains(util.GetDictionaryItem("SearchResults", Context.Language.Name))
                                  select x);
                }
                list.Reverse();
                result = (from interaction in list
                          select new RecentlyViewedPagesModel
                          {
                              RecentlyViewedPageUrl = interaction.Url,
                              InteractionItemID = interaction.ItemId,
                              InteractionPropertyName = util.PropertyDetails(new ID(interaction.ItemId.ToString()))
                              .Where(l=>l.Language == interaction.ItemLanguage).FirstOrDefault().GetItem()
                              .Parent.Fields["Property Name"].Value,
                              InteractionImage = GetImageUrl(util.PropertyDetails(new ID(interaction.ItemId.ToString())).FirstOrDefault().GetItem()
                             .Parent)
                          }).GroupBy((RecentlyViewedPagesModel x) => x.InteractionItemID, (Guid key, IEnumerable<RecentlyViewedPagesModel> group) => group.First()).Take(4).ToList();
            }
        }
        return result;
    }

    private string GetImageUrl(Item SearchPropertyItem)
    {
        ImageField imageField = SearchPropertyItem.Fields["Thumbnail Photo"];
        string text = (imageField != null) ?
            HashingUtils.ProtectAssetUrl(MediaManager.GetMediaUrl(imageField.MediaItem, new MediaUrlBuilderOptions { MaxWidth = 1280 })) : string.Empty;
        return text.Replace("/sitecore/shell", "");
    }
}