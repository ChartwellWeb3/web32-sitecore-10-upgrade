using System.Collections.Generic;
using Sitecore.Data.Items;

namespace Chartwell.Foundation.Models
{
  public class BreadcrumbsModel
  {
    public string HostSite { get; set; }
    public string PropertyURL { get; set; }
    public List<Item> BreadcrumbItem { get; set; }

  }
}