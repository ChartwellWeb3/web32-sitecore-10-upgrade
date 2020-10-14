﻿using Sitecore.Data.Items;
using System.Data;
using System.Web;

namespace Chartwell.Foundation.Models
{
  public class SuiteModel
  {
    public HtmlString PropertyGuid { get; set; }
    public HtmlString PropertyName { get; set; }
    public HtmlString PropertyTagLine { get; set; }
    public HtmlString PropertyAddress { get; set; }
    public HtmlString CityName { get; set; }
    public HtmlString ProvinceName { get; set; }
    public HtmlString PostalCode { get; set; }

    public Item InnerItem { get; set; }
    public HtmlString PropertyFormattedAddress { get; set; }
    public DataTable SuitePlanTable { get; set; }
    public DataTable SuitePrestigeTable { get; set; }
    public DataTable SuiteSpecialTable { get; set; }
  }
}