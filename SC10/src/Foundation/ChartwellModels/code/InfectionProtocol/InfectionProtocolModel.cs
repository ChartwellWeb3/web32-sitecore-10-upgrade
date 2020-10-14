using Sitecore.Data.Items;
using System.Web;

namespace Chartwell.Foundation.Models
{
  public class InfectionProtocolModel
  {
    public HtmlString PropertyName { get; set; }
    public HtmlString PropertySelectedLanguage { get; set; }
    public HtmlString PropertyTagLine { get; set; }
    public HtmlString PropertyAddress { get; set; }
    public HtmlString CityName { get; set; }
    public HtmlString ProvinceName { get; set; }
    public HtmlString PostalCode { get; set; }
    public HtmlString PropertyDescription { get; set; }
    public Item InnerItem { get; set; }
    public HtmlString PropertyFormattedAddress { get; set; }
    public HtmlString SafetyAndOutbreakPreventionDescription { get; set; }
    public HtmlString SafetyAndOutbreakPreventionTitle { get; set; }
    public HtmlString SafetyAndOutbreakPreventionYTVideoLink { get; set; }
  }
}