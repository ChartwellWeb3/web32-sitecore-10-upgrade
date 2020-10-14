namespace Chartwell.Foundation.Models
{
  public class NearbyResidencesModel
  {
    public string PropertyName { get; set; }
    public string PhoneNo { get; set; }
    public double Distance { get; set; }
    public string PropertyFormattedAddress { get; set; }
    public string PropertyItemUrl { get; set; }

    public string NearbyResidencesImage { get; set; }
    public string Parent_ContextItemID { get; set; }
    public string NearbyResidence_ItemID { get; set; }
    public string NearbyLat { get; set; }
    public string NearbyLng { get; set; }
  }
}