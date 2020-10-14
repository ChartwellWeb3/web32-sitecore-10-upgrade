namespace Chartwell.Foundation.Models
{
  public class PostalCodeModel
  {
    public string PostalCode { get; set; }
    public string City { get; set; }
    public string PlaceName { get; set; }
    public string Province { get; set; }
    public string Lat { get; set; }
    public string Lng { get; set; }

  }
  public class Rootobject
  {
    public Postalcode[] postalCodes { get; set; }
  }

  public class Postalcode
  {
    public string adminCode1 { get; set; }
    public float lng { get; set; }
    public string distance { get; set; }
    public string countryCode { get; set; }
    public string postalCode { get; set; }
    public string adminName1 { get; set; }
    public string placeName { get; set; }
    public float lat { get; set; }
    public string adminName2 { get; set; }
  }
}
