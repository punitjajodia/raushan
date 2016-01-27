using ClosedXML.Excel;
using HK.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
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
                .Select(t => t.Marka)
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

        public ActionResult Create()
        {
            var container = db.Containers.Find(CurrentContainerID);
            var items = String.IsNullOrEmpty(Request.QueryString["party"]) ? new List<String>() : Request.QueryString["party"].Split(',').ToList();

            var containerItems = db.TmpContainerItems.Where(t => t.ContainerID == CurrentContainerID).ToList();

            var billInfo = containerItems
                 .Where(c => items.Contains(c.PartyName))
                 .Select(c => new {
                     c.PartyName,
                     c.JobNumber,
                     c.BillOnBoardingDate,
                     c.BillDeliveryDate,
                     c.BillNumber
                 }).FirstOrDefault();

            var exportContainerItems = containerItems
                .Where(c => items.Contains(c.PartyName))
                .GroupBy(c => new {
                    c.Marka,
                    c.ProductBuyerName,
                    c.BuyerUnitPrice,
                    c.ProductUnit
                })
                .Select(group => new 
                {
                    Cartons = group.Sum(i => i.Cartons),
                    Marka = group.Key.Marka,

                    Product = group.Key.ProductBuyerName,
             
                    Quantity = group.Sum(i => i.Quantity),
                    Unit = group.Key.ProductUnit,
                    Rate = group.Key.BuyerUnitPrice,
                    Total = group.Key.BuyerUnitPrice * group.Sum(i => i.Quantity)
                })
                .ToList();


            XLWorkbook wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Bill");

        
            ws.Cell("A1").SetValue("Job No.");
            ws.Cell("B1").SetValue(billInfo.JobNumber);
            ws.Cell("A2").SetValue("Party Name");
            ws.Cell("B2").SetValue(billInfo.PartyName);


            ws.Cell("A3").SetValue("OnBoarding Date");
            ws.Cell("B3").SetValue(billInfo.BillOnBoardingDate);

            ws.Cell("A4").SetValue("Delivery Date");
            ws.Cell("B4").SetValue(billInfo.BillDeliveryDate);

            ws.Cell("A5").SetValue("BillNumber");
            ws.Cell("B5").SetValue(billInfo.BillNumber);

            var table = ws.Cell("A7").InsertTable(exportContainerItems);

            table.ShowTotalsRow = true;
            table.Field(0).TotalsRowFunction = XLTotalsRowFunction.Sum;
            table.Field(6).TotalsRowFunction = XLTotalsRowFunction.Sum;
            ////// Just for fun let's add the text "Sum Of Income" to the totals row
            table.Field(5).TotalsRowLabel = "Total";

            ws.Columns().AdjustToContents();


            var filename = ConfigurationManager.AppSettings["StorageDrive"] + container.ContainerNumber + "/bill/" + "/bill-" + Request.QueryString["party"].Replace(',', '_') + "-" + container.ContainerNumber + ".xlsx";

            wb.SaveAs(filename);

            return View("Success", (object)filename);

            //string filename = "Bill";

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

        public void Export()
        {

            var items = String.IsNullOrEmpty(Request.QueryString["buyer"]) ? new List<String>() : Request.QueryString["buyer"].Split(',').ToList();

            var containerItems = db.TmpContainerItems.Where(t => t.ContainerID == CurrentContainerID).ToList();

            var exportContainerItems = containerItems
                .Where(c => items.Contains(c.Marka))
                .Select(c => new BuyerBillItem
                {
                    Marka = c.Marka,
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