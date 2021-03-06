﻿using ClosedXML.Excel;
using HK.Models;
using HK.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace HK.Controllers
{

    public enum CustomsInvoiceHeaders {
        SN = 1,
        Description,
        Quantity,
        ProductUnit,
        UnitPriceCurrency,
        UnitPrice,
        AmountCurrency,
        Amount
   };



    public class TmpExportToExcelController : BaseController
    {


         public static string NumberToText(int number, bool isUK)
    {
        if (number == 0) return "Zero";
        string and = isUK ? "and " : ""; // deals with UK or US numbering
        if (number == -2147483648) return "Minus Two Billion One Hundred " + and +
        "Forty Seven Million Four Hundred " + and + "Eighty Three Thousand " +
        "Six Hundred " + and + "Forty Eight";
        int[] num = new int[4];
        int first = 0;
        int u, h, t;
        System.Text.StringBuilder sb = new System.Text.StringBuilder(); 
        if (number < 0)
        {
            sb.Append("Minus ");
            number = -number;
        }
        string[] words0 = {"", "One ", "Two ", "Three ", "Four ", "Five ", "Six ", "Seven ", "Eight ", "Nine "};
        string[] words1 = {"Ten ", "Eleven ", "Twelve ", "Thirteen ", "Fourteen ", "Fifteen ", "Sixteen ", "Seventeen ", "Eighteen ", "Nineteen "};
        string[] words2 = {"Twenty ", "Thirty ", "Forty ", "Fifty ", "Sixty ", "Seventy ", "Eighty ", "Ninety "};
        string[] words3 = { "Thousand ", "Million ", "Billion " };
        num[0] = number % 1000;           // units
        num[1] = number / 1000;
        num[2] = number / 1000000;
        num[1] = num[1] - 1000 * num[2];  // thousands
        num[3] = number / 1000000000;     // billions
        num[2] = num[2] - 1000 * num[3];  // millions
        for (int i = 3; i > 0; i--)
        {
            if (num[i] != 0)
            {
                first = i;
                break;
            }
        }
        for (int i = first; i >= 0; i--)
        {
            if (num[i] == 0) continue;
            u = num[i] % 10;              // ones
            t = num[i] / 10;
            h = num[i] / 100;             // hundreds
            t = t - 10 * h;               // tens
            if (h > 0) sb.Append(words0[h] + "Hundred ");
            if (u > 0 || t > 0)
            {
                if (h > 0 || i < first) sb.Append(and);
                if (t == 0)
                    sb.Append(words0[u]);
                else if (t == 1)
                    sb.Append(words1[u]);
                else
                    sb.Append(words2[t - 2] + words0[u]);
            }
            if (i != 0) sb.Append(words3[i - 1]);
        }
        return sb.ToString().TrimEnd();
    }

         public int DecimalPartOfDecimal(decimal number)
         {
             return (int)(((decimal)number % 1) * 100);
         }


        // GET: ExportToExcel
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RawInvoice()
        {
            var container = db.Containers.Find(CurrentContainerID);

            var containerItems = db.TmpContainerItems
                                    .Where(a => a.ContainerID == CurrentContainerID)
                                    .GroupBy(a => new
                                    {
                                        a.ProductBuyerName,
                                        a.ProductUnit
                                    })
                                    .Select(group => new
                                    {
                                        Product = group.Key.ProductBuyerName,
                                        Quantity = group.Sum(b => b.Quantity),
                                        Unit = group.Key.ProductUnit
                                    });

            XLWorkbook wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Items");

            var table = ws.Cell("A1").InsertTable(containerItems);

            //table.ShowTotalsRow = true;
            //table.Field(4).TotalsRowFunction = XLTotalsRowFunction.Sum;
            ////// Just for fun let's add the text "Sum Of Income" to the totals row
            //table.Field(3).TotalsRowLabel = "Total Cartons";

            ws.Columns().AdjustToContents();

            ws = wb.Worksheets.Add("RAW-INVOICE");

            ws.Columns().AdjustToContents();

            var filename = ConfigurationManager.AppSettings["StorageDrive"] + container.ContainerNumber + "/rawinvoice" + "/rawinvoice-" + container.ContainerNumber + ".xlsx";

            wb.SaveAs(filename);

            return View("Success", (object)filename);
        }

        public void AddContainerInfoForPerforma(IXLWorksheet ws, Container container)
        {
            ws.Cell("A1").SetValue(container.ExporterName).Style.Font.FontSize = 20;
            ws.Range("A1:E1").Merge();

            //  ws.Cell("A2").SetValue(container.Exporter.ExporterAddress).Style.Alignment.WrapText = true;
            ws.Range("A2:B3").Merge();

            ws.Cell("A5").SetValue("Shipped Per");
            ws.Cell("B5").SetValue("");
            ws.Range("B5:C5").Merge();

            ws.Cell("A6").SetValue("On/About");
            ws.Cell("B6").SetValue("");
            ws.Range("B6:C6").Merge();

            ws.Cell("A7").SetValue("From");
            ws.Cell("B7").SetValue(container.From);
            // ws.Row(7)
            //  .Style
            //   .Alignment.SetVertical(XLAlignmentVerticalValues.Top)
            //   .Alignment.SetWrapText(true);
            ws.Range("B7:C7").Merge();
            ws.Row(7).Height = 70;

            ws.Cell("A8").SetValue("Airway Bill No. \nor B/L No.");
            ws.Cell("B8").SetValue("");
            ws.Range("B8:C8").Merge();

            ws.Row(8).Height = 30;
            // ws.Row(8).Style.Alignment.SetWrapText(true);

            ws.Cell("A9").SetValue("Letter of\nCredit No.");
            ws.Cell("B9").SetValue(container.LetterOfCreditNumber);
            ws.Range("B9:C9").Merge();
            ws.Row(9).Height = 30;
            ws.Cell("A10").SetValue("Drawn Under");
            ws.Cell("B10").SetValue(container.DrawnUnder)
                .Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            //         .Alignment.SetWrapText(true);
            ws.Range("B10:C10").Merge();
            ws.Row(10).Height = 70;
            //   ws.Row(10).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Top);

            ws.Range("A1:A10").Style.Font.Bold = true;
            ws.Range("B5:C10").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            //    ws.Range("B5:C10").Style.Alignment.WrapText = true;

            ws.Rows("5:10").Style.Alignment.SetWrapText(true)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Top);

            //Importer
            ws.Cell("E5").SetValue(container.ImporterName +
                                        "\n" + container.ImporterAddress +
                                        "\n" + "(TAX CERTIFICATE NO. " + container.ImporterTaxCertificateNumber + ")")
                                        .Style
                                            .Alignment.SetVertical(XLAlignmentVerticalValues.Top)
                                            .Alignment.SetWrapText();



            ws.Range("E5:H10").Merge().Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

            //Container Number + Container Date
            ws.Cell("F2").SetValue("PERFORMA INVOICE NO:");
            ws.Cell("G2").SetValue(container.PerformaInvoiceNumber);

            ws.Cell("F3").SetValue("DATE:");
            ws.Cell("G3").SetValue(container.Date);

            ws.Range("F2:F3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Range("G2:H2").Merge();
            ws.Range("G3:H3").Merge();
            ws.Range("G2:G3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Range("F2:H3").Style.Font.Bold = true;
        }

        public void AddContainerInfo(IXLWorksheet ws, Container container)
        {
            ws.Cell("A1").SetValue(container.ExporterName).Style.Font.FontSize = 20;
            ws.Range("A1:E1").Merge();

            ws.Cell("A2").SetValue(container.ExporterAddress).Style.Alignment.WrapText = true;
            ws.Range("A2:B3").Merge();

            ws.Cell("A5").SetValue("Shipped Per");
            ws.Cell("B5").SetValue(container.ShippedPer);
            ws.Range("B5:C5").Merge();

            ws.Cell("A6").SetValue("On/About");
            ws.Cell("B6").SetValue(container.OnAbout);
            ws.Range("B6:C6").Merge();

            ws.Cell("A7").SetValue("From");
            ws.Cell("B7").SetValue(container.From);
           // ws.Row(7)
              //  .Style
             //   .Alignment.SetVertical(XLAlignmentVerticalValues.Top)
             //   .Alignment.SetWrapText(true);
            ws.Range("B7:C7").Merge();
            ws.Row(7).Height = 70;

            ws.Cell("A8").SetValue("Airway Bill No. \nor B/L No.");
            ws.Cell("B8").SetValue(container.AirwayBillNumber);
            ws.Range("B8:C8").Merge();

            ws.Row(8).Height = 30;
           // ws.Row(8).Style.Alignment.SetWrapText(true);

            ws.Cell("A9").SetValue("Letter of\nCredit No.");
            ws.Cell("B9").SetValue(container.LetterOfCreditNumber);
            ws.Range("B9:C9").Merge();
            ws.Row(9).Height = 30;
            ws.Cell("A10").SetValue("Drawn Under");
            ws.Cell("B10").SetValue(container.DrawnUnder)
                .Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
           //         .Alignment.SetWrapText(true);
            ws.Range("B10:C10").Merge();
            ws.Row(10).Height = 70;
         //   ws.Row(10).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Top);

            ws.Range("A1:A10").Style.Font.Bold = true;
            ws.Range("B5:C10").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        //    ws.Range("B5:C10").Style.Alignment.WrapText = true;

            ws.Rows("5:10").Style.Alignment.SetWrapText(true)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Top);

            //Importer
            ws.Cell("E5").SetValue(container.ImporterName +
                                        "\n" + container.ImporterAddress +
                                        "\n" + "(TAX CERTIFICATE NO. " + container.ImporterTaxCertificateNumber + ")")
                                        .Style
                                            .Alignment.SetVertical(XLAlignmentVerticalValues.Top)
                                            .Alignment.SetWrapText();
     


            ws.Range("E5:H10").Merge().Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

            //Container Number + Container Date
            ws.Cell("F2").SetValue("INVOICE NO:");
            ws.Cell("G2").SetValue(container.CustomsInvoiceNumber);
       
            ws.Cell("F3").SetValue("DATE:");
            ws.Cell("G3").SetValue(container.Date);

            ws.Range("F2:F3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Range("G2:H2").Merge();
            ws.Range("G3:H3").Merge();
            ws.Range("G2:G3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Range("F2:H3").Style.Font.Bold = true;

        }

        public ActionResult ExportContainer()
        {
            var container = db.Containers.Find(CurrentContainerID);

            var containerItems = db.TmpContainerItems
                                    .Where(a => a.ContainerID == CurrentContainerID)
                                    .Select(a => new
                                    {
                                       a.Marka,
                                       a.PartyName,
                                       a.JobNumber,
                                       a.LotSize,
                                       a.BillOnBoardingDate,
                                       a.BillDeliveryDate,
                                       a.BillNumber,
                                       a.BillTTDAPDate,
                                       a.BillTTDAPNumber,
                                       a.CartonNumber,
                                       a.ProductBuyerName,
                                       a.Quantity,
                                       a.ProductUnit,
                                       a.Cartons,
                                       a.BuyerCurrency,
                                       a.BuyerUnitPrice,
                                       a.ProductCustomsName,
                                       a.CustomsQuantity,
                                       a.CustomsProductUnit,
                                       a.CustomsCurrency,
                                       a.CustomsUnitPrice
                                    });

            XLWorkbook wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Items");

            var table = ws.Cell("A1").InsertTable(containerItems);

            //table.ShowTotalsRow = true;
            //table.Field(4).TotalsRowFunction = XLTotalsRowFunction.Sum;
            ////// Just for fun let's add the text "Sum Of Income" to the totals row
            //table.Field(3).TotalsRowLabel = "Total Cartons";

            ws.Columns().AdjustToContents();

            ws = wb.Worksheets.Add("Container");


            var row = 1;
            
            foreach (PropertyInfo propertyInfo in container.GetType().GetProperties())
            {
                if (!propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.IsSerializable && propertyInfo.Name.ToLower() != "containerid")
                {
                    var prop = propertyInfo.GetValue(container);

                    ws.Cell(row, 1).SetValue(propertyInfo.Name);
                    ws.Cell(row, 2).SetValue(prop);
                    row++;
                }
            }

            ws.Columns().AdjustToContents();

            var filename = ConfigurationManager.AppSettings["StorageDrive"] + container.ContainerNumber + "/dataexcel" + "/dataexcel-" + container.ContainerNumber + ".xlsx";

            try
            {
                wb.SaveAs(filename);
            }
            catch (IOException e)
            {
                return View("InvalidExcel", (object) ("Please close all dataexcel sheets before continuing. " + e.Message));
            }
            

            return View("Success", (object)filename);
            //string filename = container.ContainerNumber;
            //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //Response.AddHeader("content-disposition", "attachment;filename=\"" + filename + ".xlsx\"");
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);

            //// Flush the workbook to the Response.OutputStream
            //using (MemoryStream memoryStream = new MemoryStream())
            //{
            //    wb.SaveAs(memoryStream);
            //    memoryStream.WriteTo(Response.OutputStream);
            //    memoryStream.Close();
            //}

            //Response.End();
        }


        class PackingListItem
        {
            public string Marka { get; set; }
            public string CartonNumber { get; set; }
            public string Description { get; set; }
            public decimal Quantity { get; set; }
            public string Unit { get; set; }
            public int Cartons { get; set; }
        }


        public ActionResult BuyerNamePackingList()
        {
            var container = db.Containers.Find(CurrentContainerID);

            var ci = db.TmpContainerItems
                                    .Where(a => a.ContainerID == CurrentContainerID)
                                  .ToList();
            
            var containerItems = ci.Select(a => new PackingListItem
                                    {
                                        Marka = a.Marka,
                                        CartonNumber = a.CartonNumber,
                                        Description = a.ProductBuyerName,
                                        Quantity = a.Quantity,
                                        Unit = a.ProductUnit,
                                        Cartons = a.Cartons
                                    }).OrderBy(a => a.Marka)
                                    .ThenBy(a => a.CartonNumber.Split('-')[0].Length)
                                    .ThenBy(a => a.CartonNumber.Split('-')[0])
                                    .ToList();

            string prevCartonNumber = containerItems[0].CartonNumber;

            for (int i = 1, size = containerItems.Count(); i < size; i++)
            {
                    var currCartonNumber = containerItems[i].CartonNumber;

                    if (currCartonNumber == prevCartonNumber)
                    {
                        containerItems[i].CartonNumber = "";
                    }
                    else
                    {
                        prevCartonNumber = currCartonNumber;
                    }   
            }

            XLWorkbook wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Packing List");


            AddContainerInfo(ws, container);

            var table = ws.Cell("A12").InsertTable(containerItems);

            table.ShowTotalsRow = true;
            table.Field(5).TotalsRowFunction = XLTotalsRowFunction.Sum;
            //// Just for fun let's add the text "Sum Of Income" to the totals row
            table.Field(4).TotalsRowLabel = "Total Cartons";

            ws.Columns().AdjustToContents();


            var filename = ConfigurationManager.AppSettings["StorageDrive"] + container.ContainerNumber + "/packinglist" + "/packinglist-" + container.ContainerNumber + ".xlsx";

            wb.SaveAs(filename);

            return View("Success", (object)filename);

            //string filename = container.ContainerNumber + " - Packing List";
            //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //Response.AddHeader("content-disposition", "attachment;filename=\"" + filename + ".xlsx\"");


            //// Flush the workbook to the Response.OutputStream
            //using (MemoryStream memoryStream = new MemoryStream())
            //{
            //    wb.SaveAs(memoryStream);
            //    memoryStream.WriteTo(Response.OutputStream);
            //    memoryStream.Close();
            //}

            //Response.End();
        }

        

        public ActionResult CustomsNamePackingList()
        {
            var container = db.Containers.Find(CurrentContainerID);

            var containerItems = db.TmpContainerItems
                                    .Where(a => a.ContainerID == CurrentContainerID)
                                    .GroupBy(a => new
                                    {
                                        a.ProductCustomsName,
                                        a.CustomsProductUnit
                                    })
                                    .Select(group => new
                                    {
                                        CTNS = group.Sum(b => b.Cartons),
                                        Description = group.Key.ProductCustomsName,
                                        Quantity = group.Sum(b => b.CustomsQuantity),
                                        Unit = group.Key.CustomsProductUnit,
                                        NetWeight = "",
                                        GrossWeight = ""
                                    });

            XLWorkbook wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Packing List");


            AddContainerInfo(ws, container);

            var table = ws.Cell("A12").InsertTable(containerItems);
      

            //table.ShowTotalsRow = true;
            //table.Field(0).TotalsRowFunction = XLTotalsRowFunction.Sum;
            //////// Just for fun let's add the text "Sum Of Income" to the totals row
            ////table.Field(3).TotalsRowLabel = "Total Cartons";

            var currentRow = ws.LastRowUsed();
            currentRow = currentRow.RowBelow();

            var totalContainers = db.TmpContainerItems
                                    .Where(a => a.ContainerID == CurrentContainerID)
                                    .Sum(a => a.Cartons);

            currentRow.Cell(1).SetValue(totalContainers.ToString() + " CTNS");
            currentRow.Cell(1).Style.Border.SetTopBorder(XLBorderStyleValues.Thick)
                .Border.SetBottomBorder(XLBorderStyleValues.Double);

            ws.Range(currentRow.Cell(5), currentRow.Cell(6)).Merge().SetValue(container.TotalGrossWeight)
                .Style.Border.SetTopBorder(XLBorderStyleValues.Thick)
                      .Border.SetBottomBorder(XLBorderStyleValues.Double)
                      .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);


            currentRow = currentRow.RowBelow().RowBelow().RowBelow();

            currentRow.Cell(2).SetValue("TOTAL G.W. = " + container.TotalGrossWeight);
            currentRow = currentRow.RowBelow();
            currentRow.Cell(2).SetValue("TOTAL CTN. = " + container.TotalCartons);
            currentRow = currentRow.RowBelow();

            currentRow.Cell(2).SetValue("COUNTRY OF ORIGIN : " + container.CountryOfOrigin);
            currentRow = currentRow.RowBelow();

            ws.Range(currentRow.Cell(2), currentRow.Cell(6)).Merge()
                .SetValue("GOODS TO BE PACKED IN STRONG, EXPORT STANDARD SEA-ROAD WORTHY PACKING")
                .Style.Font.SetFontSize(9.0);

            ws.Columns().AdjustToContents();


            var filename = ConfigurationManager.AppSettings["StorageDrive"] + container.ContainerNumber + "/customsinvoice" + "/customsinvoice-" + container.ContainerNumber + ".xlsx";

            wb.SaveAs(filename);

            return View("Success", (object)filename);

            //string filename = container.ContainerNumber + " - Customs Packing List";

            //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //Response.AddHeader("content-disposition", "attachment;filename=\"" + filename + ".xlsx\"");

            //// Flush the workbook to the Response.OutputStream
            //using (MemoryStream memoryStream = new MemoryStream())
            //{
            //    wb.SaveAs(memoryStream);
            //    memoryStream.WriteTo(Response.OutputStream);
            //    memoryStream.Close();
            //}

            //Response.End();
        }

        public void ExpandDescription(IXLWorksheet ws, IXLRow row)
        {
            ws.Range(row.Cell((int)CustomsInvoiceHeaders.Description), row.Cell((int)CustomsInvoiceHeaders.Amount)).Merge();
        }

        public ActionResult CustomsNameInvoice()
        {
            var container = db.Containers.Find(CurrentContainerID);

            var containerItems = db.TmpContainerItems
                                    .Where(a => a.ContainerID == CurrentContainerID)
                                    .GroupBy(a => new { a.ProductCustomsName, a.CustomsProductUnit, a.CustomsUnitPrice, a.CustomsCurrency })
                                    .OrderBy(a => a.Key.ProductCustomsName)
                                    .AsEnumerable()
                                    .Select((group, inc) => new
                                    {
                                        SN = (inc + 1),
                                        ProductCustomsName = group.Key.ProductCustomsName,
                                        Quantity = group.Sum(b => b.CustomsQuantity),
                                        Unit = group.Key.CustomsProductUnit,
                                        UnitPriceCurrency = group.Key.CustomsCurrency,
                                        Rate = group.Key.CustomsUnitPrice,
                                        AmountCurrency = group.Key.CustomsCurrency,
                                        Amount = group.Sum(b => b.CustomsQuantity) * group.Key.CustomsUnitPrice
                                    });

            XLWorkbook wb = new XLWorkbook();

            wb.PageOptions.Margins.SetTop(0.5).SetRight(0.5).SetBottom(0.5).SetLeft(0.5);
            var ws = wb.Worksheets.Add("Packing List");


            ws.Style.Font.SetFontName("Times New Roman");
            ws.Style.Font.SetFontSize(10.0);
            AddContainerInfo(ws, container);


            var currentRow = ws.LastRowUsed().RowBelow().RowBelow();

            var headerRow = currentRow;

            headerRow.Cell((int)CustomsInvoiceHeaders.SN).SetValue("SN");
            headerRow.Cell((int)CustomsInvoiceHeaders.Description).SetValue("Description");

            ws.Range(headerRow.Cell((int)CustomsInvoiceHeaders.Quantity), headerRow.Cell((int)CustomsInvoiceHeaders.ProductUnit)).Merge();
            headerRow.Cell((int)CustomsInvoiceHeaders.Quantity).SetValue("Quantity");

            ws.Range(headerRow.Cell((int)CustomsInvoiceHeaders.UnitPriceCurrency), headerRow.Cell((int)CustomsInvoiceHeaders.UnitPrice)).Merge();
            headerRow.Cell((int)CustomsInvoiceHeaders.UnitPriceCurrency).SetValue("Unit Price");
          
            ws.Range(headerRow.Cell((int)CustomsInvoiceHeaders.AmountCurrency), headerRow.Cell((int)CustomsInvoiceHeaders.Amount)).Merge();
            headerRow.Cell((int)CustomsInvoiceHeaders.AmountCurrency).SetValue("Amount");


            ws.Range(headerRow.FirstCell(), headerRow.Cell((int)CustomsInvoiceHeaders.Amount))
                .Style.Font.SetBold()
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin);

            currentRow = currentRow.RowBelow().RowBelow();



            currentRow.Cell((int)CustomsInvoiceHeaders.Description).SetValue(container.HarmonicCodes)
                                            .Style.Font.SetBold()
                                            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                            .Alignment.SetWrapText();

            ws.Range(currentRow.Cell((int)CustomsInvoiceHeaders.Description), currentRow.RowBelow().Cell((int)CustomsInvoiceHeaders.Description)).Merge();
            
            
            currentRow.Cell((int)CustomsInvoiceHeaders.AmountCurrency).SetValue(container.CostsIncluded).Style.Font.SetBold()
                                            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                            .Alignment.SetWrapText();
            ws.Range(currentRow.Cell((int)CustomsInvoiceHeaders.AmountCurrency), currentRow.RowBelow().Cell((int)CustomsInvoiceHeaders.Amount)).Merge();

            currentRow = currentRow.RowBelow().RowBelow().RowBelow();

            currentRow.FirstCell().Value = containerItems.AsEnumerable();

            var dataRange = ws.Range(headerRow.FirstCell(), ws.LastCellUsed());

            dataRange.Column((int)CustomsInvoiceHeaders.Amount).Style.NumberFormat.Format = "0.00";
            dataRange.Column((int)CustomsInvoiceHeaders.UnitPrice).Style.NumberFormat.Format = "0.00";

            dataRange.Columns(String.Join(",",
                            new String[] {
                               Convert.ToString((int)CustomsInvoiceHeaders.SN),
                               Convert.ToString((int)CustomsInvoiceHeaders.Description),
                               Convert.ToString((int)CustomsInvoiceHeaders.ProductUnit),
                               Convert.ToString((int)CustomsInvoiceHeaders.UnitPrice),
                               Convert.ToString((int)CustomsInvoiceHeaders.Amount)
                            }))
                .Style.Border.SetRightBorder(XLBorderStyleValues.Thin);

            dataRange.FirstColumn().Style.Border.SetLeftBorder(XLBorderStyleValues.Thin);

            ws.Range(ws.LastRowUsed().FirstCellUsed(), ws.LastRowUsed().LastCellUsed())
                .Style.Border.SetBottomBorder(XLBorderStyleValues.Thin);

            currentRow = ws.LastRowUsed().RowBelow();

            var totalAmount = containerItems.Sum(a => a.Amount);
            currentRow.Cell((int)CustomsInvoiceHeaders.Amount).SetValue(totalAmount);
            currentRow.Cell((int)CustomsInvoiceHeaders.Amount).Style.NumberFormat.Format = "0.00";

            currentRow.Cell((int)CustomsInvoiceHeaders.AmountCurrency).SetValue("Total").Style.Font.SetBold();

            currentRow = currentRow.RowBelow().RowBelow();

            var inWords = containerItems.First().AmountCurrency;
            inWords += " " + NumberToText((int)containerItems.Sum(a => a.Amount), false);

            if (DecimalPartOfDecimal(totalAmount) > 0)
            {
                inWords = inWords + " and " + DecimalPartOfDecimal(totalAmount) + "/100"; 
            }

            inWords += " only";

            currentRow.Cell((int)CustomsInvoiceHeaders.Description).SetValue(inWords.ToUpper());

            ExpandDescription(ws, currentRow);

            if(!String.IsNullOrWhiteSpace(container.TotalGrossWeight)) {
                currentRow = currentRow.RowBelow();
                currentRow.Cell((int)CustomsInvoiceHeaders.Description).SetValue(String.Concat("TOTAL GW: ", container.TotalGrossWeight));
                ExpandDescription(ws, currentRow);
            }

            if (!String.IsNullOrWhiteSpace(container.TotalCartons))
            {
                currentRow = currentRow.RowBelow();
                currentRow.Cell((int)CustomsInvoiceHeaders.Description).SetValue(String.Concat("TOTAL CTN: ", container.TotalCartons));
                ExpandDescription(ws, currentRow);
            }

            if (!String.IsNullOrWhiteSpace(container.CountryOfOrigin))
            {
                currentRow = currentRow.RowBelow();
                currentRow.Cell((int)CustomsInvoiceHeaders.Description).SetValue(String.Concat("COUNTRY OF ORIGIN: ", container.CountryOfOrigin));
                ExpandDescription(ws, currentRow);
            }

            if (!String.IsNullOrWhiteSpace(container.ImporterTaxCertificateNumber))
            {
                currentRow = currentRow.RowBelow();
                currentRow.Cell((int)CustomsInvoiceHeaders.Description).SetValue(String.Concat("WE HEREBY CERTIFYING THAT SHOWING EXACT QUANTITY SHIPPED APPLICANT's TAX CERTIFICATE NO.", container.ImporterTaxCertificateNumber));
                ExpandDescription(ws, currentRow);
            }

            currentRow = currentRow.RowBelow();
            currentRow.Cell((int)CustomsInvoiceHeaders.Description).SetValue("BANK INFORMATION").Style.Font.SetBold().Font.SetUnderline();
          

            if (!String.IsNullOrWhiteSpace(container.BeneficiaryBank))
            {
                currentRow = currentRow.RowBelow();
                currentRow.Cell((int)CustomsInvoiceHeaders.Description).SetValue(String.Concat("BENEFICIARY BANK: ", container.BeneficiaryBank));
                ExpandDescription(ws, currentRow);
            }

            if (!String.IsNullOrWhiteSpace(container.BeneficiaryBank))
            {
                currentRow = currentRow.RowBelow();
                currentRow.Cell((int)CustomsInvoiceHeaders.Description).SetValue(String.Concat("BENEFICIARY: ", container.ExporterName));
                ExpandDescription(ws, currentRow);
            }

            if (!String.IsNullOrWhiteSpace(container.BeneficiarySwift))
            {
                currentRow = currentRow.RowBelow();
                currentRow.Cell((int)CustomsInvoiceHeaders.Description).SetValue(String.Concat("SWIFT: ", container.BeneficiarySwift));
                ExpandDescription(ws, currentRow);
            }

            if (!String.IsNullOrWhiteSpace(container.BeneficiaryUsdAccount))
            {
                currentRow = currentRow.RowBelow();
                currentRow.Cell((int)CustomsInvoiceHeaders.Description).SetValue(String.Concat("USD ACCOUNT NO.: ", container.BeneficiaryUsdAccount));
                ExpandDescription(ws, currentRow);
            }

            ws.Columns().AdjustToContents();


            var filename = ConfigurationManager.AppSettings["StorageDrive"] + container.ContainerNumber + "/customsinvoice" + "/customsinvoice-" + container.ContainerNumber + ".xlsx";

            wb.SaveAs(filename);

            return View("Success", (object)filename);

            //string filename = container.ContainerNumber + " - Customs Invoice";

            //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //Response.AddHeader("content-disposition", "attachment;filename=\"" + filename + ".xlsx\"");

            //// Flush the workbook to the Response.OutputStream
            //using (MemoryStream memoryStream = new MemoryStream())
            //{
            //    wb.SaveAs(memoryStream);
            //    memoryStream.WriteTo(Response.OutputStream);
            //    memoryStream.Close();
            //}

            //Response.End();
        }

        public ActionResult PerformaInvoice()
        {
            var container = db.Containers.Find(CurrentContainerID);

            var containerItems = db.TmpContainerItems
                                    .Where(a => a.ContainerID == CurrentContainerID)
                                    .GroupBy(a => new { a.ProductCustomsName, a.CustomsProductUnit, a.CustomsUnitPrice, a.CustomsCurrency })
                                    .OrderBy(a => a.Key.ProductCustomsName)
                                    .AsEnumerable()
                                    .Select((group, inc) => new
                                    {
                                        SN = (inc + 1),
                                        ProductCustomsName = group.Key.ProductCustomsName,
                                        Quantity = group.Sum(b => b.CustomsQuantity),
                                        Unit = group.Key.CustomsProductUnit,
                                        UnitPriceCurrency = group.Key.CustomsCurrency,
                                        Rate = group.Key.CustomsUnitPrice,
                                        AmountCurrency = group.Key.CustomsCurrency,
                                        Amount = group.Sum(b => b.CustomsQuantity) * group.Key.CustomsUnitPrice
                                    });

            XLWorkbook wb = new XLWorkbook();

            wb.PageOptions.Margins.SetTop(0.5).SetRight(0.5).SetBottom(0.5).SetLeft(0.5);
            var ws = wb.Worksheets.Add("Packing List");


            ws.Style.Font.SetFontName("Times New Roman");
            ws.Style.Font.SetFontSize(10.0);
            AddContainerInfoForPerforma(ws, container);


            var currentRow = ws.LastRowUsed().RowBelow().RowBelow();

            var headerRow = currentRow;

            headerRow.Cell((int)CustomsInvoiceHeaders.SN).SetValue("SN");
            headerRow.Cell((int)CustomsInvoiceHeaders.Description).SetValue("Description");

            ws.Range(headerRow.Cell((int)CustomsInvoiceHeaders.Quantity), headerRow.Cell((int)CustomsInvoiceHeaders.ProductUnit)).Merge();
            headerRow.Cell((int)CustomsInvoiceHeaders.Quantity).SetValue("Quantity");

            ws.Range(headerRow.Cell((int)CustomsInvoiceHeaders.UnitPriceCurrency), headerRow.Cell((int)CustomsInvoiceHeaders.UnitPrice)).Merge();
            headerRow.Cell((int)CustomsInvoiceHeaders.UnitPriceCurrency).SetValue("Unit Price");

            ws.Range(headerRow.Cell((int)CustomsInvoiceHeaders.AmountCurrency), headerRow.Cell((int)CustomsInvoiceHeaders.Amount)).Merge();
            headerRow.Cell((int)CustomsInvoiceHeaders.AmountCurrency).SetValue("Amount");


            ws.Range(headerRow.FirstCell(), headerRow.Cell((int)CustomsInvoiceHeaders.Amount))
                .Style.Font.SetBold()
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin);

            currentRow = currentRow.RowBelow().RowBelow();



            currentRow.Cell((int)CustomsInvoiceHeaders.Description).SetValue("FOOTWEARS (HARMONIC CODE \nNO. 6400.00.00)")
                                            .Style.Font.SetBold()
                                            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                            .Alignment.SetWrapText();

            ws.Range(currentRow.Cell((int)CustomsInvoiceHeaders.Description), currentRow.RowBelow().Cell((int)CustomsInvoiceHeaders.Description)).Merge();


            currentRow.Cell((int)CustomsInvoiceHeaders.AmountCurrency).SetValue("CNF CALCUTTA SEA PORT, INDIA").Style.Font.SetBold()
                                            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                            .Alignment.SetWrapText();
            ws.Range(currentRow.Cell((int)CustomsInvoiceHeaders.AmountCurrency), currentRow.RowBelow().Cell((int)CustomsInvoiceHeaders.Amount)).Merge();

            currentRow = currentRow.RowBelow().RowBelow().RowBelow();

            currentRow.FirstCell().Value = containerItems.AsEnumerable();

            var dataRange = ws.Range(headerRow.FirstCell(), ws.LastCellUsed());

            dataRange.Column((int)CustomsInvoiceHeaders.Amount).Style.NumberFormat.Format = "0.00";
            dataRange.Column((int)CustomsInvoiceHeaders.UnitPrice).Style.NumberFormat.Format = "0.00";

            dataRange.Columns(String.Join(",",
                            new String[] {
                               Convert.ToString((int)CustomsInvoiceHeaders.SN),
                               Convert.ToString((int)CustomsInvoiceHeaders.Description),
                               Convert.ToString((int)CustomsInvoiceHeaders.ProductUnit),
                               Convert.ToString((int)CustomsInvoiceHeaders.UnitPrice),
                               Convert.ToString((int)CustomsInvoiceHeaders.Amount)
                            }))
                .Style.Border.SetRightBorder(XLBorderStyleValues.Thin);

            dataRange.FirstColumn().Style.Border.SetLeftBorder(XLBorderStyleValues.Thin);

            ws.Range(ws.LastRowUsed().FirstCellUsed(), ws.LastRowUsed().LastCellUsed())
                .Style.Border.SetBottomBorder(XLBorderStyleValues.Thin);

            currentRow = ws.LastRowUsed().RowBelow();

            var totalAmount = containerItems.Sum(a => a.Amount);
            currentRow.Cell((int)CustomsInvoiceHeaders.Amount).SetValue(totalAmount);
            currentRow.Cell((int)CustomsInvoiceHeaders.Amount).Style.NumberFormat.Format = "0.00";

            currentRow.Cell((int)CustomsInvoiceHeaders.AmountCurrency).SetValue("Total").Style.Font.SetBold();

            currentRow = currentRow.RowBelow().RowBelow();

            var inWords = containerItems.First().AmountCurrency;
            inWords += " " + NumberToText((int)containerItems.Sum(a => a.Amount), false);

            if (DecimalPartOfDecimal(totalAmount) > 0)
            {
                inWords = inWords + " and " + DecimalPartOfDecimal(totalAmount) + "/100";
            }

            inWords += " only";

            currentRow.Cell((int)CustomsInvoiceHeaders.Description).SetValue(inWords.ToUpper());

            ExpandDescription(ws, currentRow);

            if (!String.IsNullOrWhiteSpace(container.TotalGrossWeight))
            {
                currentRow = currentRow.RowBelow();
                currentRow.Cell((int)CustomsInvoiceHeaders.Description).SetValue(String.Concat("TOTAL GW: ", container.TotalGrossWeight));
                ExpandDescription(ws, currentRow);
            }

            if (!String.IsNullOrWhiteSpace(container.TotalCartons))
            {
                currentRow = currentRow.RowBelow();
                currentRow.Cell((int)CustomsInvoiceHeaders.Description).SetValue(String.Concat("TOTAL CTN: ", container.TotalCartons));
                ExpandDescription(ws, currentRow);
            }

            if (!String.IsNullOrWhiteSpace(container.CountryOfOrigin))
            {
                currentRow = currentRow.RowBelow();
                currentRow.Cell((int)CustomsInvoiceHeaders.Description).SetValue(String.Concat("COUNTRY OF ORIGIN: ", container.CountryOfOrigin));
                ExpandDescription(ws, currentRow);
            }

            if (!String.IsNullOrWhiteSpace(container.ImporterTaxCertificateNumber))
            {
                currentRow = currentRow.RowBelow();
                currentRow.Cell((int)CustomsInvoiceHeaders.Description).SetValue(String.Concat("WE HEREBY CERTIFYING THAT SHOWING EXACT QUANTITY SHIPPED APPLICANT's TAX CERTIFICATE NO.", container.ImporterTaxCertificateNumber));
                ExpandDescription(ws, currentRow);
            }

            currentRow = currentRow.RowBelow();
            currentRow.Cell((int)CustomsInvoiceHeaders.Description).SetValue("BANK INFORMATION").Style.Font.SetBold().Font.SetUnderline();


            if (!String.IsNullOrWhiteSpace(container.BeneficiaryBank))
            {
                currentRow = currentRow.RowBelow();
                currentRow.Cell((int)CustomsInvoiceHeaders.Description).SetValue(String.Concat("BENEFICIARY BANK: ", container.BeneficiaryBank));
                ExpandDescription(ws, currentRow);
            }

            if (!String.IsNullOrWhiteSpace(container.BeneficiaryBank))
            {
                currentRow = currentRow.RowBelow();
                currentRow.Cell((int)CustomsInvoiceHeaders.Description).SetValue(String.Concat("BENEFICIARY: ", container.ExporterName));
                ExpandDescription(ws, currentRow);
            }

            if (!String.IsNullOrWhiteSpace(container.BeneficiarySwift))
            {
                currentRow = currentRow.RowBelow();
                currentRow.Cell((int)CustomsInvoiceHeaders.Description).SetValue(String.Concat("SWIFT: ", container.BeneficiarySwift));
                ExpandDescription(ws, currentRow);
            }

            if (!String.IsNullOrWhiteSpace(container.BeneficiaryUsdAccount))
            {
                currentRow = currentRow.RowBelow();
                currentRow.Cell((int)CustomsInvoiceHeaders.Description).SetValue(String.Concat("USD ACCOUNT NO.: ", container.BeneficiaryUsdAccount));
                ExpandDescription(ws, currentRow);
            }

            ws.Columns().AdjustToContents();

            string saveLocation = ConfigurationManager.AppSettings["StorageDrive"] + container.ContainerNumber + "/performa" + "/performa-" + container.ContainerNumber + ".xlsx";

            wb.SaveAs(saveLocation);

            return View("Success", (object)saveLocation);
            //string filename = container.ContainerNumber + " - Customs Invoice";

            //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //Response.AddHeader("content-disposition", "attachment;filename=\"" + filename + ".xlsx\"");

            //// Flush the workbook to the Response.OutputStream
            //using (MemoryStream memoryStream = new MemoryStream())
            //{
            //    wb.SaveAs(memoryStream);
            //    memoryStream.WriteTo(Response.OutputStream);
            //    memoryStream.Close();
            //}

            //Response.End();
        }



        //public ActionResult ImportPackingList()
        //{

        //}

        //public ActionResult ExportInvoice()
        //{
        //    var container = new TmpContainersController().GetCurrentContainer();
        //    var invoices = new InvoicesController().GetInvoices(container).Select(i => new InvoiceItemExportVM(i));

        //    XLWorkbook wb = new XLWorkbook();
        //    var ws = wb.Worksheets.Add("Invoice");

        //    AddContainerInfo(ws, container);


        //    var table = ws.Cell("A12").InsertTable(invoices);

        //    table.ShowTotalsRow = true;
        //    table.Field(4).TotalsRowFunction = XLTotalsRowFunction.Sum;
        //    //// Just for fun let's add the text "Sum Of Income" to the totals row
        //    table.Field(3).TotalsRowLabel = "Total";

        //    ws.Columns().AdjustToContents();

        //    string filename = ConfigurationManager.AppSettings["StorageDrive"] + container.ContainerNumber + "/performa" + "/performa-" + container.ContainerNumber + ".xlsx";

        //    wb.SaveAs(filename);

        //    return View("Success", (object)filename);

        //    //string filename = container.ContainerNumber;

        //    //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //    //Response.AddHeader("content-disposition", "attachment;filename=\"" + filename + ".xlsx\"");

        //    //// Flush the workbook to the Response.OutputStream
        //    //using (MemoryStream memoryStream = new MemoryStream())
        //    //{
        //    //    wb.SaveAs(memoryStream);
        //    //    memoryStream.WriteTo(Response.OutputStream);
        //    //    memoryStream.Close();
        //    //}

        //    //Response.End();
        //    //return RedirectToAction("Index");
        //}


        public void BuyerBill(List<BuyerBillItem> buyerBillItems)
        {
            var container = db.Containers.Find(CurrentContainerID);

            XLWorkbook wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Bill");

            var table = ws.Cell("A2").InsertTable(buyerBillItems);

            //table.ShowTotalsRow = true;
            //table.Field(4).TotalsRowFunction = XLTotalsRowFunction.Sum;
            ////// Just for fun let's add the text "Sum Of Income" to the totals row
            //table.Field(3).TotalsRowLabel = "Total Cartons";

            ws.Columns().AdjustToContents();

            string filename = "Bill";

            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=\"" + filename + ".xlsx\"");

            // Flush the workbook to the Response.OutputStream
            using (MemoryStream memoryStream = new MemoryStream())
            {
                wb.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                memoryStream.Close();
            }

            Response.End();
        }
    }
}