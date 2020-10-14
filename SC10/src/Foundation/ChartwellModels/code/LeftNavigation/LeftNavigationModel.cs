using Sitecore.Data.Items;
using System.Collections.Generic;

namespace Chartwell.Foundation.Models
{
  public class LeftNavigationModel
  {
    public List<Item> Children { get; set; }
    public string PhoneNo { get; set; }
    public string PropertyLocationUrl { get; set; }
    public Item InnerItem { get; set; }
    public bool isEventDisplay { get; set; }

  }
}