using ClosedXML.Excel;
using HK.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HK.Controllers
{
    public class ImportFromExcelController : BaseController
    {
        public PartialViewResult Upload()
        {
            return PartialView();
        }

        public FilePathResult PackingListImportTemplateExcel()
        {
            return File("~/Content/ExcelTemplates/ImportPackingList.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public int GetBuyerID(string BuyerCode)
        {
            var match = db.Buyers.FirstOrDefault(b => b.BuyerCode == BuyerCode);
            if (match != null)
            {
                return match.BuyerID;
            }
            else
            {
                var newBuyerToInsert = new Buyer
                {
                    BuyerCode = BuyerCode
                };
                db.Buyers.Add(newBuyerToInsert);
                db.SaveChanges();
                return newBuyerToInsert.BuyerID;
            }
        }

        public int GetProductID(string ProductCode)
        {
            var match = db.Products.FirstOrDefault(b => b.ProductCode == ProductCode);
            if (match != null)
            {
                return match.ProductID;
            }
            else
            {
                var newProductToInsert = new Product
                {
                    ProductCode = ProductCode
                };
                db.Products.Add(newProductToInsert);
                db.SaveChanges();
                return newProductToInsert.ProductID;
            }
        }

        public void DeleteAllContainerItems()
        {
            db.ContainerItems.RemoveRange(db.ContainerItems.Where(c => c.ContainerID == CurrentContainerID));
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase upload)
        {
            if (!ModelState.IsValid)
            {
                return View("Error");
            }

            if (upload == null && upload.ContentLength == 0)
            {
                return View("Error");
            }

            //var file = new FilePath
            //{
            //    FileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(upload.FileName),
            //    FileType = FileType.Excel
            //};

           

            //Remove all items of current container
            DeleteAllContainerItems();

            var wb = new XLWorkbook(upload.InputStream);
            var ws = wb.Worksheets.First();

            var dataRange = ws.RangeUsed();

            // Treat the range as a table (to be able to use the column names)
            var dataTable = dataRange.AsTable();

            // Get the list of company names
            var dataObj = dataTable.DataRange.Rows()
                .Select(packingList => new {
                    CartonNumber = packingList.Field("CartonNumber").GetString(),
                    BuyerCode	= packingList.Field("BuyerCode").GetString(),
                    ProductCode= packingList.Field("ProductCode").GetString(),
                    Quantity = packingList.Field("Quantity").GetString(),
                    QuantityType = packingList.Field("QuantityType").GetString(),
                    Cartons = packingList.Field("Cartons").GetString()
                })
                .ToList();

            foreach (var item in dataObj)
            {
                var containerItem = new ContainerItem();
                containerItem.CartonNumber = item.CartonNumber;
                containerItem.BuyerID = GetBuyerID(item.BuyerCode);
                containerItem.ProductID = GetProductID(item.ProductCode);
                containerItem.Quantity = Convert.ToInt32(item.Quantity);
                containerItem.QuantityType = (ProductUnit)Enum.Parse(typeof(ProductUnit), item.QuantityType);
                containerItem.ContainerID = CurrentContainerID;
                containerItem.Cartons = Convert.ToInt32(item.Cartons);
                db.ContainerItems.Add(containerItem);
                db.SaveChanges();
            }

            return new RedirectResult(Url.Action("Index", "Dashboard") + "#PackingList");
        }
    }
}