namespace Chartwell.Foundation.Models
{
  public class TopNavModel
  {
    public string TopNavParentItems { get; set; }
    public string TopNavItemUrl { get; set; }

    public bool HasChild { get; set; }

    public string TargetBlank { get; set; }

    public string ULClass { get; set; }

  }
}