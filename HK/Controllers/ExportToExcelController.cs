using ClosedXML.Excel;
using HK.Models;
using HK.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HK.Controllers
{
    public class ExcelController : Controller
    {
        // GET: ExportToExcel
        public ActionResult Index()
        {
            return View();
        }

        public void AddContainerInfo(IXLWorksheet ws, Container container)
        {
            ws.Cell("A1").SetValue(container.Exporter.ExporterName).Style.Font.FontSize = 20;
            ws.Range("A1:E1").Merge();

            ws.Cell("A2").SetValue(container.Exporter.ExporterAddress).Style.Alignment.WrapText = true;
            ws.Range("A2:B3").Merge();

            ws.Cell("A5").SetValue("Shipped Per");
            ws.Cell("B5").SetValue(container.ShippedPer);
            ws.Range("B5:C5").Merge();

            ws.Cell("A6").SetValue("On/About");
            ws.Cell("B6").SetValue(container.OnAbout);
            ws.Range("B6:C6").Merge();

            ws.Cell("A7").SetValue("From");
            ws.Cell("B7").SetValue(container.From);
            ws.Cell("B7")
                .Style
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                    .Alignment.SetWrapText(true);
            ws.Range("B7:C7").Merge();
            ws.Row(7).Height = 70;

            ws.Cell("A8").SetValue("Airway Bill No. or B/L No.");
            ws.Cell("B8").SetValue(container.AirwayBillNumber);
            ws.Range("B8:C8").Merge();

            ws.Cell("A9").SetValue("Letter of Credit No.");
            ws.Cell("B9").SetValue(container.LetterOfCreditNumber);
            ws.Range("B9:C9").Merge();

            ws.Cell("A10").SetValue("Drawn Under");
            ws.Cell("B10").SetValue(container.DrawnUnder)
                .Style
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetWrapText(true);
            ws.Range("B10:C10").Merge();
            ws.Row(10).Height = 70;

            ws.Range("A1:A10").Style.Font.Bold = true;
            ws.Range("B5:C10").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Range("B5:C10").Style.Alignment.WrapText = true;


            //Importer
            ws.Cell("D5").SetValue(container.Importer.ImporterName +
                                        "\n" + container.Importer.ImporterAddress +
                                        "\n" + container.Importer.TaxCertificateNumber)
                                        .Style
                                            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                            .Alignment.SetWrapText();
     
            ws.Range("D5:E10").Merge().Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

            //Container Number + Container Date
            ws.Cell("D2").SetValue("NO:");
            ws.Cell("E2").SetValue(container.ContainerNumber);
       
            ws.Cell("D3").SetValue("DATE:");
            ws.Cell("E3").SetValue(container.Date);

            ws.Range("D2:D3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Range("E2:E3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Range("D2:E3").Style.Font.Bold = true;

        }

        public void ExportPackingList()
        {
            var container = new ContainersController().GetCurrentContainer();
            
            var containerItems = container.ContainerItems.Select(c => new
            {
                c.CartonNumber,
                c.Buyer.BuyerCode,
                c.Product.ProductCode,
                Quantity = c.Quantity + " " +  c.QuantityType,
                c.Cartons
            });


            XLWorkbook wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Packing List");


            AddContainerInfo(ws, container);

            var table = ws.Cell("A12").InsertTable(containerItems);

            table.ShowTotalsRow = true;
            table.Field(4).TotalsRowFunction = XLTotalsRowFunction.Sum;
            //// Just for fun let's add the text "Sum Of Income" to the totals row
            table.Field(3).TotalsRowLabel = "Total Cartons";

            ws.Columns().AdjustToContents();

            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=\"Packing List.xlsx\"");

            // Flush the workbook to the Response.OutputStream
            using (MemoryStream memoryStream = new MemoryStream())
            {
                wb.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                memoryStream.Close();
            }

            Response.End();
        }

        //public ActionResult ImportPackingList()
        //{

        //}

        public ActionResult ExportInvoice()
        {
            var container = new ContainersController().GetCurrentContainer();
            var invoices = new InvoicesController().GetInvoices(container).Select(i => new InvoiceItemExportVM(i));

            XLWorkbook wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Invoice");

            AddContainerInfo(ws, container);


            var table = ws.Cell("A12").InsertTable(invoices);

            table.ShowTotalsRow = true;
            table.Field(4).TotalsRowFunction = XLTotalsRowFunction.Sum;
            //// Just for fun let's add the text "Sum Of Income" to the totals row
            table.Field(3).TotalsRowLabel = "Total";

            ws.Columns().AdjustToContents();

            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=\"Invoice.xlsx\"");

            // Flush the workbook to the Response.OutputStream
            using (MemoryStream memoryStream = new MemoryStream())
            {
                wb.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                memoryStream.Close();
            }

            Response.End();
            return RedirectToAction("Index");
        }

    }
}