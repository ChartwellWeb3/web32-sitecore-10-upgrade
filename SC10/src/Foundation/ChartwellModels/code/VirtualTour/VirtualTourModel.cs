﻿using Sitecore.Data.Items;
using System.Web;

namespace Chartwell.Foundation.Models
{
  public class VirtualTourModel
  {
    public HtmlString CityName { get; set; }
    public Item InnerItem { get; set; }
    public HtmlString PostalCode { get; set; }
    public HtmlString PropertyAddress { get; set; }
    public HtmlString PropertyTagLine { get; set; }
    public HtmlString ProvinceName { get; set; }
    public HtmlString VideoLink { get; set; }
    public HtmlString PropertyFormattedAddress { get; set; }

  }
}