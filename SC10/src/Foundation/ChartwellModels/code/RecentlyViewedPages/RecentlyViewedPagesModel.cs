using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chartwell.Foundation.Models
{
  public class RecentlyViewedPagesModel
  {
    public string RecentlyViewedPageUrl { get; set; }
    public Guid InteractionItemID { get; set; }
    public string InteractionImage { get; set; }
    public string InteractionPropertyName { get; set; }

  }
}