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