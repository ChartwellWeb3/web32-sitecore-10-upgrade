using Chartwell.Foundation.Models;
using Chartwell.Foundation.utility;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Chartwell.Feature.LeftNavigation.Controllers
{
  public class LeftNavigationController : Controller
  {
    public string selPropertyName = "";
    // GET: Navigation
    public PartialViewResult Index()
    {

      Item CurrentItem = RenderingContext.Current.ContextItem;

      Item Section = CurrentItem.Parent;

      return PartialView("~/Views/LeftNavigation/LeftNavigation.cshtml", CreateNavigationItem(Section));
    }


    public bool iSFieldDescriptionEmpty(string strFieldValue)
    {
      bool isEmpty = false;

      if (string.IsNullOrEmpty(strFieldValue) || strFieldValue == "NULL")
      { isEmpty = true; }

      return isEmpty;
    }

    private LeftNavigationModel CreateNavigationItem(Item rootsite)
    {
      ChartwellUtiles util = new ChartwellUtiles();

      List<Item> LeftNavItems = new List<Item>();
      var itemIds = Context.Item.Parent.Fields["LeftNavOrder"].Value.Split('|').ToList().Select(c => new ID(Guid.Parse(c))).ToList();
      LeftNavItems = util.LeftNavItems(itemIds, rootsite);

      var PropertyItem = Context.Item.Parent;
      bool isEventDisplay = false;

      isEventDisplay = chkPropertyEvents(PropertyItem);

      var menu = new LeftNavigationModel()
      {
        PhoneNo = util.GetPhoneNumber(PropertyItem),
        InnerItem = PropertyItem,
        Children = LeftNavItems,
        isEventDisplay = isEventDisplay,
        PropertyLocationUrl = "https://maps.google.com?q=" + 
                               PropertyItem.Fields["Latitude"].ToString() + 
                               "," + PropertyItem.Fields["Longitude"].ToString()
      };

      return menu;
    }

    private bool chkPropertyEvents(Item PropertyItem)
    {
      bool isDisplayEvent = false;
      if (PropertyItem.Fields["Event Start Date"].HasValue && PropertyItem.Fields["Event End Date"].HasValue)

      {
        DateField startEventDate = PropertyItem.Fields["Event Start Date"];

        DateField endEventDate = PropertyItem.Fields["Event End Date"];

        int result1 = DateTime.Compare(startEventDate.DateTime.Date, DateTime.Today);
        int result2 = DateTime.Compare(DateTime.Today, endEventDate.DateTime.Date);
        if (result1 <= 0 && result2 <= 0)
        { isDisplayEvent = true; }
      }
      return isDisplayEvent;

    }
  }
}