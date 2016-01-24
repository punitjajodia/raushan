using ClosedXML.Excel;
using HK.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HK.Controllers
{
    public class TmpBuyerBillController : BaseController
    {
        // GET: TmpBuyerBill
        public ActionResult Index()
        {
            var buyers = db.TmpContainerItems
                .Where(t => t.ContainerID == CurrentContainerID)
                .Select(t => t.BuyerName)
                .Distinct()
                .ToList();

            return View(buyers);
        }

        public ActionResult SelectParty()
        {
            var parties = db.TmpContainerItems
                .Where(t => t.ContainerID == CurrentContainerID)
                .Select(t => t.PartyName)
                .Distinct()
                .ToList();

            return View(parties);

        }

        public void Create()
        {

            var items = String.IsNullOrEmpty(Request.QueryString["party"]) ? new List<String>() : Request.QueryString["party"].Split(',').ToList();

            var containerItems = db.TmpContainerItems.Where(t => t.ContainerID == CurrentContainerID).ToList();

            var billInfo = containerItems
                 .Where(c => items.Contains(c.PartyName))
                 .Select(c => new {
                     c.PartyName,
                     c.PartyPhone,
                     c.JobNumber,
                     c.BillOnBoardingDate,
                     c.BillDeliveryDate,
                     c.BillNumber
                 }).FirstOrDefault();

            var exportContainerItems = containerItems
                .Where(c => items.Contains(c.PartyName))
                .GroupBy(c => new {
                    c.BuyerName,
                    c.ProductBuyerName,
                    c.BuyerUnitPrice,
                    c.ProductUnit
                })
                .Select(group => new 
                {
                    Cartons = group.Sum(i => i.Cartons),
                    Marka = group.Key.BuyerName,

                    Product = group.Key.ProductBuyerName,
                    
                    Rate = group.Key.BuyerUnitPrice,
                    Quantity = group.Sum(i => i.Quantity),
                    Unit = group.Key.ProductUnit,
                    
                    Total = group.Key.BuyerUnitPrice * group.Sum(i => i.Quantity)
                })
                .ToList();


            XLWorkbook wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Bill");

        
            ws.Cell("A1").SetValue("Job No.");
            ws.Cell("B1").SetValue(billInfo.JobNumber);
            ws.Cell("A2").SetValue("Party Name");
            ws.Cell("B2").SetValue(billInfo.PartyName);

            ws.Cell("A3").SetValue("Party Phone");
            ws.Cell("B3").SetValue(billInfo.PartyPhone);

            ws.Cell("A4").SetValue("OnBoarding Date");
            ws.Cell("B4").SetValue(billInfo.BillOnBoardingDate);

            ws.Cell("A5").SetValue("Delivery Date");
            ws.Cell("B5").SetValue(billInfo.BillDeliveryDate);

            ws.Cell("A6").SetValue("BillNumber");
            ws.Cell("B6").SetValue(billInfo.BillNumber);

            var table = ws.Cell("A8").InsertTable(exportContainerItems);

            table.ShowTotalsRow = true;
            table.Field(5).TotalsRowFunction = XLTotalsRowFunction.Sum;
            ////// Just for fun let's add the text "Sum Of Income" to the totals row
            table.Field(4).TotalsRowLabel = "Total";

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

        public void Export()
        {

            var items = String.IsNullOrEmpty(Request.QueryString["buyer"]) ? new List<String>() : Request.QueryString["buyer"].Split(',').ToList();

            var containerItems = db.TmpContainerItems.Where(t => t.ContainerID == CurrentContainerID).ToList();

            var exportContainerItems = containerItems
                .Where(c => items.Contains(c.BuyerName))
                .Select(c => new BuyerBillItem
                {
                    Marka = c.BuyerName,
                    Product = c.ProductBuyerName,
                    Rate = c.BuyerUnitPrice,
                    Quantity = c.Quantity,
                    Unit = c.ProductUnit,
                    Total = c.BuyerUnitPrice * c.Quantity
                })
                .ToList();


            XLWorkbook wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Bill");

            var table = ws.Cell("A2").InsertTable(exportContainerItems);

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