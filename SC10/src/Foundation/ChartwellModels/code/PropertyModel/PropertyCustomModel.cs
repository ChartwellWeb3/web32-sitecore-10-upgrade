using System.Collections.Generic;

namespace Chartwell.Foundation.Models
{
  public class PropertyCustomModel
  {
    public string SplitterPageTitle { get; set; }
    public string SplitterPageDescription { get; set; }
    public List<PropertySearchModel> lstProperty { get; set; }

    public HashSet<string> lstPropertyProvince { get; set; }

  }
}