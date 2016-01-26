using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HK.Controllers
{
    public class TmpDeliveryController : BaseController
    {
        // GET: TmpDelivery
        public void Index()
        {
            var importer = db.Containers.Where(c => c.ContainerID == CurrentContainerID).First().ImporterName;

            var containerItems = db.TmpContainerItems
                                    .Where(a => a.ContainerID == CurrentContainerID)
                                    .GroupBy(a => new
                                    {
                                           a.PartyName,
                                           a.Marka
                                    })
                                    .Select(group => new
                                    {
                                        Party = group.Key.PartyName,
                                        Marka = group.Key.Marka,
                                        Cartons = group.Sum(i => i.Cartons)
                                    })
                                    .OrderBy(i => new
                                    {
                                        i.Party,
                                        i.Marka
                                    })                                   
                                    ;

            XLWorkbook wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Packing List");

            ws.Cell("A1").SetValue(importer).Style.Font.SetFontSize(20.0);

            var table = ws.Cell("A3").InsertTable(containerItems);

            table.ShowTotalsRow = true;
            table.Field(2).TotalsRowFunction = XLTotalsRowFunction.Sum;
            ////// Just for fun let's add the text "Sum Of Income" to the totals row
            table.Field(1).TotalsRowLabel = "Total";

            ws.Columns().AdjustToContents();

            string filename = "Delivery";
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