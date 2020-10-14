using Chartwell.Foundation.Models;
using Chartwell.Foundation.utility;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;
using Sitecore.Resources.Media;
using System;
using System.Data;
using System.Web;
using System.Web.Mvc;

namespace Chartwell.Feature.Suites.Controllers
{
  public class SuiteController : Controller
  {
    // GET: Suite
    public PartialViewResult SuitePlan()
    {
      return PartialView("~/Views/Suite/Suite.cshtml", CreateSuitePlan());
    }
    private SuiteModel CreateSuitePlan()
    {
      ChartwellUtiles c = new ChartwellUtiles();

      var CurrentItem = GetDataSourceItem();
      var parentItem = Context.Item.Parent;

      string strProvinceID = parentItem.Fields["Province"].ToString();
      var Provinceitem = c.GetItemByStringId(strProvinceID);
      string strProvinceName = Provinceitem.Fields["Province Name"].ToString();
      string strPropertyFormattedAddress = c.FormattedAddress(parentItem, strProvinceName);

      string Table1header = CurrentItem.Fields["Suite Table1 Title"].HasValue ?
        CurrentItem.Fields["Suite Table1 Title"].ToString() : String.Empty;

      var dtSuites = new DataTable();
      var dtPrestigeSuites = new DataTable();
      var dtSpecialSuites = new DataTable();

      if (Table1header != String.Empty)
      {
        dtSuites.TableName = Table1header;
        dtSuites = GenerateSuitePlanTable(CurrentItem, dtSuites);
      }
      string Table2header = CurrentItem.Fields["Suite Table2 Title"].HasValue ? CurrentItem.Fields["Suite Table2 Title"].ToString() : String.Empty;
      if (Table2header != String.Empty)
      {
        dtPrestigeSuites.TableName = Table2header;
        dtPrestigeSuites = GeneratePrestigePlanTable(CurrentItem, dtPrestigeSuites);
      }
      string Table3header = CurrentItem.Fields["Suite Table3 Title"].HasValue ? CurrentItem.Fields["Suite Table3 Title"].ToString() : String.Empty;
      if (Table3header != String.Empty)
      {
        dtSpecialSuites.TableName = Table3header;
        dtSpecialSuites = GenerateSpecialTable(CurrentItem, dtSpecialSuites);
      }
      var viewModel = new SuiteModel
      {
        PropertyName = new HtmlString(parentItem.Fields["Property Name"].ToString()),
        PropertyTagLine = new HtmlString(parentItem.Fields["Property Tag Line"].ToString()),
        InnerItem = parentItem,
        PropertyFormattedAddress = new HtmlString(strPropertyFormattedAddress),
        SuitePlanTable = dtSuites,
        SuitePrestigeTable = dtPrestigeSuites,
        SuiteSpecialTable = dtSpecialSuites
      };
      return viewModel;
    }

    private DataTable GenerateSpecialTable(Item CurrentItem, DataTable dtSpecialSuites)
    {
      string Col1Title = CurrentItem.Fields["Suite Table3 Header1"].HasValue ? CurrentItem.Fields["Suite Table3 Header1"].ToString() : String.Empty;
      string Col2Title = CurrentItem.Fields["Suite Table3 Header2"].HasValue ? CurrentItem.Fields["Suite Table3 Header2"].ToString() : String.Empty;
      string Col3Title = CurrentItem.Fields["Suite Table3 Header3"].HasValue ? CurrentItem.Fields["Suite Table3 Header3"].ToString() : String.Empty;
      string Col4Title = CurrentItem.Fields["Suite Table3 Header4"].HasValue ? CurrentItem.Fields["Suite Table3 Header4"].ToString() : String.Empty;
      //photos 
      string Col5Title = CurrentItem.Fields["Suite Table3 Header5"].HasValue ? CurrentItem.Fields["Suite Table3 Header5"].ToString() : String.Empty;

      if (Col1Title != string.Empty) { dtSpecialSuites.Columns.Add(new DataColumn(Col1Title)); }
      if (Col2Title != string.Empty) { dtSpecialSuites.Columns.Add(new DataColumn(Col2Title)); }
      if (Col3Title != string.Empty) { dtSpecialSuites.Columns.Add(new DataColumn(Col3Title)); }
      if (Col4Title != string.Empty) { dtSpecialSuites.Columns.Add(new DataColumn(Col4Title)); }
      if (Col5Title != string.Empty) { dtSpecialSuites.Columns.Add(new DataColumn(Col5Title)); }
      int totalColumnCount = dtSpecialSuites.Columns.Count;
      int totalRowsCount = CalculateTable3Rows(CurrentItem);
      for (int i = 0; i < totalRowsCount; i++)
      {
        int rowIndex = i + 1;
        DataRow row = dtSpecialSuites.NewRow();
        string desiredRowColumn = "";
        for (int j = 0; j < totalColumnCount; j++)
        {
          int columnIndex = j + 1;
          desiredRowColumn = String.Concat("desiredRowColumn", j);
          var rowItem = CurrentItem.Fields["SuiteT3-R" + rowIndex + "C" + columnIndex].ToString();
          if (columnIndex == 1 && rowItem != "")
          {
            Item targetItem = Sitecore.Context.Database.Items[rowItem];
            desiredRowColumn = targetItem.Fields["Suit Type Name"].ToString();
          }
          else if (columnIndex == 2 && rowItem != "")
          {
            desiredRowColumn = HashingUtils.ProtectAssetUrl(Sitecore.StringUtil.EnsurePrefix('/', MediaManager.GetMediaUrl(Context.Database.GetItem(rowItem))));

          }
          //photos
          else if (columnIndex == 5 && rowItem != "")
          {
            desiredRowColumn = HashingUtils.ProtectAssetUrl(Sitecore.StringUtil.EnsurePrefix('/', MediaManager.GetMediaUrl(Context.Database.GetItem(rowItem))));

          }
          else
          {
            desiredRowColumn = rowItem;
          }

          row[j] = desiredRowColumn;

        }
        dtSpecialSuites.Rows.Add(row);

      }
      return dtSpecialSuites;
    }

    private DataTable GenerateSuitePlanTable(Item CurrentItem, DataTable dtSuites)
    {

      string Col1Title = CurrentItem.Fields["Suite Table1 Header1"].HasValue ? CurrentItem.Fields["Suite Table1 Header1"].ToString() : String.Empty;
      string Col2Title = CurrentItem.Fields["Suite Table1 Header2"].HasValue ? CurrentItem.Fields["Suite Table1 Header2"].ToString() : String.Empty;
      string Col3Title = CurrentItem.Fields["Suite Table1 Header3"].HasValue ? CurrentItem.Fields["Suite Table1 Header3"].ToString() : String.Empty;
      string Col4Title = CurrentItem.Fields["Suite Table1 Header4"].HasValue ? CurrentItem.Fields["Suite Table1 Header4"].ToString() : String.Empty;
      //photos 
      string Col5Title = CurrentItem.Fields["Suite Table1 Header5"].HasValue ? CurrentItem.Fields["Suite Table1 Header5"].ToString() : String.Empty;

      if (Col1Title != string.Empty) { dtSuites.Columns.Add(new DataColumn(Col1Title)); }
      if (Col2Title != string.Empty) { dtSuites.Columns.Add(new DataColumn(Col2Title)); }
      if (Col3Title != string.Empty) { dtSuites.Columns.Add(new DataColumn(Col3Title)); }
      if (Col4Title != string.Empty) { dtSuites.Columns.Add(new DataColumn(Col4Title)); }
      if (Col5Title != string.Empty) { dtSuites.Columns.Add(new DataColumn(Col5Title)); }
      int totalColumnCount = dtSuites.Columns.Count;
      int totalRowsCount = CalculateTable1Rows(CurrentItem);
      for (int i = 0; i < totalRowsCount; i++)
      {
        int rowIndex = i + 1;
        DataRow row = dtSuites.NewRow();
        string desiredRowColumn = "";
        for (int j = 0; j < totalColumnCount; j++)
        {
          int columnIndex = j + 1;
          desiredRowColumn = String.Concat("desiredRowColumn", j);
          var rowItem = CurrentItem.Fields["SuiteT1-R" + rowIndex + "C" + columnIndex].ToString();
          if (columnIndex == 1 && rowItem != "")
          {
            Item targetItem = Sitecore.Context.Database.Items[rowItem];
            desiredRowColumn = targetItem.Fields["Suit Type Name"].ToString();
          }
          else if (columnIndex == 2 && rowItem != "")
          {
            desiredRowColumn = HashingUtils.ProtectAssetUrl(Sitecore.StringUtil.EnsurePrefix('/', MediaManager.GetMediaUrl(Context.Database.GetItem(rowItem))));

          }
          //photos
          else if (columnIndex == 5 && rowItem != "")
          {
            desiredRowColumn = HashingUtils.ProtectAssetUrl(Sitecore.StringUtil.EnsurePrefix('/', MediaManager.GetMediaUrl(Context.Database.GetItem(rowItem))));

          }
          else
          {
            desiredRowColumn = rowItem;
          }

          row[j] = desiredRowColumn;

        }
        dtSuites.Rows.Add(row);

      }
      return dtSuites;
    }
    private DataTable GeneratePrestigePlanTable(Item CurrentItem, DataTable dtSuites)
    {

      string Col1Title = CurrentItem.Fields["Suite Table2 Header1"].HasValue ? CurrentItem.Fields["Suite Table2 Header1"].ToString() : String.Empty;
      string Col2Title = CurrentItem.Fields["Suite Table2 Header2"].HasValue ? CurrentItem.Fields["Suite Table2 Header2"].ToString() : String.Empty;
      string Col3Title = CurrentItem.Fields["Suite Table2 Header3"].HasValue ? CurrentItem.Fields["Suite Table2 Header3"].ToString() : String.Empty;
      string Col4Title = CurrentItem.Fields["Suite Table2 Header4"].HasValue ? CurrentItem.Fields["Suite Table2 Header4"].ToString() : String.Empty;
      string Col5Title = CurrentItem.Fields["Suite Table2 Header5"].HasValue ? CurrentItem.Fields["Suite Table2 Header5"].ToString() : String.Empty;
      if (Col1Title != string.Empty) { dtSuites.Columns.Add(new DataColumn(Col1Title)); }
      if (Col2Title != string.Empty) { dtSuites.Columns.Add(new DataColumn(Col2Title)); }
      if (Col3Title != string.Empty) { dtSuites.Columns.Add(new DataColumn(Col3Title)); }
      if (Col4Title != string.Empty) { dtSuites.Columns.Add(new DataColumn(Col4Title)); }
      if (Col5Title != string.Empty) { dtSuites.Columns.Add(new DataColumn(Col5Title)); }
      int totalColumnCount = dtSuites.Columns.Count;
      int totalRowsCount = CalculateTable2Rows(CurrentItem);
      for (int i = 0; i < totalRowsCount; i++)
      {
        int rowIndex = i + 1;
        DataRow row = dtSuites.NewRow();
        string desiredRowColumn = "";
        for (int j = 0; j < totalColumnCount; j++)
        {
          int columnIndex = j + 1;
          desiredRowColumn = String.Concat("desiredRowColumn", j);
          var rowItem = CurrentItem.Fields["SuiteT2-R" + rowIndex + "C" + columnIndex].ToString();
          if (columnIndex == 1 && rowItem != "")
          {
            Item targetItem = Sitecore.Context.Database.Items[rowItem];
            desiredRowColumn = targetItem.Fields["Suit Type Name"].ToString();
          }
          else if (columnIndex == 2 && rowItem != "")
          {
            desiredRowColumn = HashingUtils.ProtectAssetUrl(Sitecore.StringUtil.EnsurePrefix('/', MediaManager.GetMediaUrl(Context.Database.GetItem(rowItem))));

          }
          else if (columnIndex == 5 && rowItem != "")
          {
            desiredRowColumn = HashingUtils.ProtectAssetUrl(Sitecore.StringUtil.EnsurePrefix('/', MediaManager.GetMediaUrl(Context.Database.GetItem(rowItem))));

          }
          else
          {
            desiredRowColumn = rowItem;
          }

          row[j] = desiredRowColumn;

        }
        dtSuites.Rows.Add(row);

      }
      return dtSuites;
    }

    private int CalculateTable1Rows(Item currentItem)
    {
      int totalrows = 0;
      var s1 = currentItem.Fields["SuiteT1-R1C1"].HasValue ? totalrows++ : totalrows;
      var s2 = currentItem.Fields["SuiteT1-R2C1"].HasValue ? totalrows++ : totalrows;
      var s3 = currentItem.Fields["SuiteT1-R3C1"].HasValue ? totalrows++ : totalrows;
      var s4 = currentItem.Fields["SuiteT1-R4C1"].HasValue ? totalrows++ : totalrows;
      var s5 = currentItem.Fields["SuiteT1-R5C1"].HasValue ? totalrows++ : totalrows;
      var s6 = currentItem.Fields["SuiteT1-R6C1"].HasValue ? totalrows++ : totalrows;
      var s7 = currentItem.Fields["SuiteT1-R7C1"].HasValue ? totalrows++ : totalrows;
      var s8 = currentItem.Fields["SuiteT1-R8C1"].HasValue ? totalrows++ : totalrows;
      var s9 = currentItem.Fields["SuiteT1-R9C1"].HasValue ? totalrows++ : totalrows;
      var s10 = currentItem.Fields["SuiteT1-R10C1"].HasValue ? totalrows++ : totalrows;
      var s11 = currentItem.Fields["SuiteT1-R11C1"].HasValue ? totalrows++ : totalrows;
      var s12 = currentItem.Fields["SuiteT1-R12C1"].HasValue ? totalrows++ : totalrows;
      return totalrows;
    }
    private int CalculateTable2Rows(Item currentItem)
    {
      int totalrows = 0;
      var s1 = currentItem.Fields["SuiteT2-R1C1"].HasValue ? totalrows++ : totalrows;
      var s2 = currentItem.Fields["SuiteT2-R2C1"].HasValue ? totalrows++ : totalrows;
      var s3 = currentItem.Fields["SuiteT2-R3C1"].HasValue ? totalrows++ : totalrows;
      var s4 = currentItem.Fields["SuiteT2-R4C1"].HasValue ? totalrows++ : totalrows;
      var s5 = currentItem.Fields["SuiteT2-R5C1"].HasValue ? totalrows++ : totalrows;
      var s6 = currentItem.Fields["SuiteT2-R6C1"].HasValue ? totalrows++ : totalrows;
      var s7 = currentItem.Fields["SuiteT2-R7C1"].HasValue ? totalrows++ : totalrows;
      var s8 = currentItem.Fields["SuiteT2-R8C1"].HasValue ? totalrows++ : totalrows;
      var s9 = currentItem.Fields["SuiteT2-R9C1"].HasValue ? totalrows++ : totalrows;
      var s10 = currentItem.Fields["SuiteT2-R10C1"].HasValue ? totalrows++ : totalrows;
      var s11 = currentItem.Fields["SuiteT2-R11C1"].HasValue ? totalrows++ : totalrows;
      var s12 = currentItem.Fields["SuiteT2-R12C1"].HasValue ? totalrows++ : totalrows;
      return totalrows;
    }
    private int CalculateTable3Rows(Item currentItem)
    {
      int totalrows = 0;
      var s1 = currentItem.Fields["SuiteT3-R1C1"].HasValue ? totalrows++ : totalrows;
      var s2 = currentItem.Fields["SuiteT3-R2C1"].HasValue ? totalrows++ : totalrows;
      var s3 = currentItem.Fields["SuiteT3-R3C1"].HasValue ? totalrows++ : totalrows;
      var s4 = currentItem.Fields["SuiteT3-R4C1"].HasValue ? totalrows++ : totalrows;
      var s5 = currentItem.Fields["SuiteT3-R5C1"].HasValue ? totalrows++ : totalrows;
      var s6 = currentItem.Fields["SuiteT3-R6C1"].HasValue ? totalrows++ : totalrows;
      var s7 = currentItem.Fields["SuiteT3-R7C1"].HasValue ? totalrows++ : totalrows;
      var s8 = currentItem.Fields["SuiteT3-R8C1"].HasValue ? totalrows++ : totalrows;
      var s9 = currentItem.Fields["SuiteT3-R9C1"].HasValue ? totalrows++ : totalrows;
      var s10 = currentItem.Fields["SuiteT3-R10C1"].HasValue ? totalrows++ : totalrows;
      var s11 = currentItem.Fields["SuiteT3-R11C1"].HasValue ? totalrows++ : totalrows;
      var s12 = currentItem.Fields["SuiteT3-R12C1"].HasValue ? totalrows++ : totalrows;
      return totalrows;
    }
    private Item GetDataSourceItem()
    {


      return RenderingContext.Current.Rendering.Item;


    }
  }
}