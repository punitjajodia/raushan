﻿using ClosedXML.Excel;
using HK.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HK.Controllers
{
    public class TmpImportFromExcelController : BaseController
    {
        public PartialViewResult Upload()
        {
            return PartialView();
        }

        public FilePathResult PackingListImportTemplateExcel()
        {
            return File("~/Content/ExcelTemplates/ImportPackingList.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public void DeleteAllContainerItems()
        {
            db.TmpContainerItems.RemoveRange(db.TmpContainerItems.Where(c => c.ContainerID == CurrentContainerID));
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

            //string path = System.IO.Path.Combine(
            //              Server.MapPath("~/Content/Uploads"), file.FileName);
            //// file is uploaded
            //upload.SaveAs(path);

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
                    BuyerName = packingList.Field("BuyerName").GetString(), 
                    ProductCustomsName = packingList.Field("ProductCustomsName").GetString(),
                    ProductBuyerName = packingList.Field("ProductBuyerName").GetString(),
                    ProductUnit = packingList.Field("ProductUnit").GetString(),
                    Quantity = packingList.Field("Quantity").GetString(),
                    Cartons = packingList.Field("Cartons").GetString(),
                    BuyerCurrency = packingList.Field("BuyerCurrency").GetString(),
                    BuyerUnitPrice	= packingList.Field("BuyerUnitPrice").GetString(),
                    CustomsCurrency = packingList.Field("CustomsCurrency").GetString(),
                    CustomsUnitPrice = packingList.Field("CustomsUnitPrice").GetString()
                })
                .ToList();

            foreach (var item in dataObj)
            {
                    
                var containerItem = new TmpContainerItem();
                containerItem.ContainerID = CurrentContainerID;
                containerItem.CartonNumber = item.CartonNumber;
                containerItem.BuyerName = item.BuyerName; 
                containerItem.ProductCustomsName = item.ProductCustomsName;
                containerItem.ProductBuyerName = item.ProductBuyerName;
                containerItem.ProductUnit = (ProductUnit)Enum.Parse(typeof(ProductUnit), item.ProductUnit.TrimEnd('.'));

                containerItem.Quantity = Convert.ToDecimal(item.Quantity);

                try
                {
                    containerItem.Cartons = Convert.ToInt32(item.Cartons);
                }
                catch (Exception e)
                {
                    containerItem.Cartons = 0;
                }
                
                containerItem.BuyerCurrency = item.BuyerCurrency;

                try
                {
                    containerItem.BuyerUnitPrice = Convert.ToDecimal(item.BuyerUnitPrice);
                }
                catch
                {
                    containerItem.BuyerUnitPrice = 0;
                }
                
                containerItem.CustomsCurrency = item.CustomsCurrency;

                try
                {
                    containerItem.CustomsUnitPrice = Convert.ToDecimal(item.CustomsUnitPrice);
                }
                catch
                {
                    containerItem.CustomsUnitPrice = 0;
                }
                

                db.TmpContainerItems.Add(containerItem);
                db.SaveChanges();
            }

            return new RedirectResult(Url.Action("Index", "TmpDashboard"));
        }
    }
}