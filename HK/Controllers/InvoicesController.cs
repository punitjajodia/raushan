using HK.DAL;
using HK.Models;
using HK.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using System.IO;

namespace HK.Controllers
{
    public class InvoicesController : BaseController
    {

        // GET: Invoices

        public List<InvoiceItem> GetInvoices()
        {
            var container = new ContainersController().GetCurrentContainer();
            var containerItems = container.ContainerItems;
            return GetInvoices(containerItems);
        }
        
        public List<InvoiceItem> GetInvoices(Container container)
        {
            var containerItems = container.ContainerItems;
            return GetInvoices(containerItems);
        }

        public List<InvoiceItem> GetInvoices(List<ContainerItem> containerItems)
        {
           // var containerItems = new ContainerItemsController().GetContainerItems();

            List<InvoiceItem> invoiceItems = new List<InvoiceItem>();

            foreach (var c in containerItems)
            {
                var invoiceItem = new InvoiceItem();

                invoiceItem.ProductID = c.Product.ProductID;
                invoiceItem.ProductCode = c.Product.ProductCode;
                invoiceItem.Unit = c.Product.ProductUnit;

                switch (c.QuantityType)
                {
                    case ProductUnit.DOZ:
                        invoiceItem.Quantity = invoiceItem.Unit == ProductUnit.DOZ ? c.Quantity : c.Quantity * 12;
                        break;
                    default:
                        invoiceItem.Quantity = c.Quantity;
                        break;
                }

                invoiceItem.UnitPrice = c.Product.ProductUnitPrice;
                invoiceItem.Amount = invoiceItem.Quantity * invoiceItem.UnitPrice;
                invoiceItems.Add(invoiceItem);
            }


            var invoices = invoiceItems.GroupBy(i => new {i.ProductID, i.ProductCode, i.UnitPrice, i.Unit })
                          .Select(group =>
                                new InvoiceItem
                                {
                                    ProductID = group.Key.ProductID,
                                    ProductCode = group.Key.ProductCode,
                                    Quantity = group.Sum(w => w.Quantity),
                                    Unit = group.Key.Unit
                                })
                                .ToList();

            foreach (var invoice in invoices)
            {
                var productContainerPrice = db.ProductContainerPrices.Where(pcp => pcp.ContainerID == CurrentContainerID && pcp.ProductID == invoice.ProductID).FirstOrDefault();
               
                if (productContainerPrice == null)
                {
                    var previousProductPrice = db.ProductContainerPrices.Where(pcp => pcp.ProductID == invoice.ProductID)
                        .OrderByDescending(pcp => pcp.ContainerID)
                        .FirstOrDefault();
                    if (previousProductPrice != null)
                    {
                        invoice.ProductContainerPrice = previousProductPrice;
                    }
                    else
                    {
                        invoice.ProductContainerPrice = new ProductContainerPrice();
                        invoice.ProductContainerPrice.QuantityType = ProductUnit.PCS;
                    }
                }
                else
                {
                    invoice.ProductContainerPrice = productContainerPrice;
                    switch (invoice.ProductContainerPrice.QuantityType)
                    {
                        //If the pricing is in dozens, revert back the quantity to dozen because everything was turned to pieces before
                        case ProductUnit.DOZ:
                            invoice.Quantity /= 12;
                            break;
                    }
                }
                invoice.Amount = invoice.Quantity * invoice.ProductContainerPrice.Price;
            }

            return invoices;
        }

        public ActionResult Index()
        {
            return View(GetInvoices());
        }

        public PartialViewResult List()
        {
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductCode");
            return PartialView(GetInvoices());
        }

        public PartialViewResult List(List<InvoiceItem> invoiceItems)
        {
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductCode");
            return PartialView(GetInvoices());
        }


        public ActionResult Export()
        {
            var invoices = GetInvoices().Select(i => new InvoiceItemExportVM(i));

            XLWorkbook wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Invoice");

            ws.Cell("A1").InsertTable(invoices);

            //table.ShowTotalsRow = true;
            //table.Field(0).TotalsRowFunction = XLTotalsRowFunction.Sum;
            //// Just for fun let's add the text "Sum Of Income" to the totals row
            //table.Field(1).TotalsRowLabel = "Sum Of Income";

            ws.Columns().AdjustToContents();

            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=\"Daybook.xlsx\"");

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