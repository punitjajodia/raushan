using ClosedXML.Excel;
using HK.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
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
            return File("~/Content/ExcelTemplates/import-template.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
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

            var wb = new XLWorkbook(upload.InputStream);

            try
            {
                var containerWS = wb.Worksheets.Last();

                var containerRow = containerWS.FirstRowUsed();
                var container = new Container();
                while (!containerRow.IsEmpty())
                {
                    var prop = containerRow.Cell(1).GetString();
                    var value = containerRow.Cell(2).Value;

                    Type type = container.GetType();
                    PropertyInfo containerProp = type.GetProperty(prop);
                    containerProp.SetValue(container, value, null);
                    containerRow = containerRow.RowBelow();
                }
                

                if (ModelState.IsValid)
                {
                    //If container with container ID already exists, update it otherwise insert new
                    container.ContainerID = CurrentContainerID;
                    db.Entry(container).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            DeleteAllContainerItems();


            var ws = wb.Worksheets.First();

            var dataRange = ws.RangeUsed();

            // Treat the range as a table (to be able to use the column names)
            var dataTable = dataRange.AsTable();


            // Get the list of company names
            var dataObj = dataTable.DataRange.Rows()
                .Select(packingList => new {
                    CartonNumber = packingList.Field("CartonNumber").GetString(),
                    Marka = packingList.Field("Marka").GetString(),
                    PartyName = packingList.Field("PartyName").GetString(),
                    JobNumber = packingList.Field("JobNumber").GetString(),
                    PartyPhone = packingList.Field("PartyPhone").GetString(),
                    BillOnBoardingDate = packingList.Field("BillOnBoardingDate").GetString(),
                    BillDeliveryDate = packingList.Field("BillDeliveryDate").GetString(),
                    BillNumber = packingList.Field("BillNumber").GetString(),
                    BillTTDAPNumber = packingList.Field("BillTTDAPNumber").GetString(),
                    BillTTDAPDate = packingList.Field("BillTTDAPDate").GetString(),
                    LotSize = packingList.Field("LotSize").GetString(),
                    ProductCustomsName = packingList.Field("ProductCustomsName").GetString(),
                    ProductBuyerName = packingList.Field("ProductBuyerName").GetString(),
                    ProductUnit = packingList.Field("ProductUnit").GetString(),
                    Quantity = packingList.Field("Quantity").GetString(),
                    Cartons = packingList.Field("Cartons").GetString(),
                    BuyerCurrency = packingList.Field("BuyerCurrency").GetString(),
                    BuyerUnitPrice	= packingList.Field("BuyerUnitPrice").GetString(),
                    CustomsQuantity = packingList.Field("CustomsQuantity").GetString(),
                    CustomsProductUnit = packingList.Field("CustomsProductUnit").GetString(),
                    CustomsCurrency = packingList.Field("CustomsCurrency").GetString(),
                    CustomsUnitPrice = packingList.Field("CustomsUnitPrice").GetString()
                })
                .ToList();

            foreach (var item in dataObj)
            {
                var containerItem = new TmpContainerItem();
                containerItem.ContainerID = CurrentContainerID;
                containerItem.CartonNumber = item.CartonNumber;
                containerItem.Marka = item.Marka;

                var PartyData = String.IsNullOrEmpty(item.PartyName) ? 
                    db.TmpContainerItems
                    .Where(i =>
                        i.Marka == item.Marka &&
                        i.ContainerID == CurrentContainerID)
                    .Select(i => new
                        {
                            i.PartyName,
                            i.JobNumber,
                            i.BillOnBoardingDate,
                            i.BillDeliveryDate,
                            i.BillNumber,
                            i.BillTTDAPNumber,
                            i.BillTTDAPDate,
                            i.LotSize
                        }).FirstOrDefault() : new {
                            item.PartyName,
                            item.JobNumber,
                            item.BillOnBoardingDate,
                            item.BillDeliveryDate,
                            item.BillNumber,
                            item.BillTTDAPNumber,
                            item.BillTTDAPDate,
                            item.LotSize
                        };

                containerItem.PartyName = PartyData.PartyName;
                containerItem.JobNumber = PartyData.JobNumber;
                containerItem.BillOnBoardingDate = PartyData.BillOnBoardingDate;
                containerItem.BillDeliveryDate = PartyData.BillDeliveryDate;
                containerItem.BillNumber = PartyData.BillNumber;
                containerItem.BillTTDAPDate = PartyData.BillTTDAPDate;
                containerItem.BillTTDAPNumber = PartyData.BillTTDAPNumber;

               
                containerItem.ProductBuyerName = item.ProductBuyerName;
                containerItem.ProductUnit = item.ProductUnit.TrimEnd('.');

                containerItem.Quantity = Convert.ToDecimal(item.Quantity);

                var cartonNumber = containerItem.CartonNumber;

                if (String.IsNullOrEmpty(item.Cartons) || String.Equals(item.Cartons, "0"))
                {
                    if (cartonNumber.Contains("-"))
                    {
                        var parts = cartonNumber.Split('-');
                        try
                        {
                            var cartons = Convert.ToInt32(parts[1]) - Convert.ToInt32(parts[0]) + 1;
                            containerItem.Cartons = cartons;
                        }
                        catch (Exception e)
                        {
                            containerItem.Cartons = 0;
                        }

                    }
                    else
                    {
                        try
                        {
                            if (String.IsNullOrEmpty(containerItem.CartonNumber))
                            {
                                containerItem.Cartons = 0;
                            }
                            else
                            {
                                containerItem.Cartons = 1;
                            }
                        }
                        catch (Exception e)
                        {
                            containerItem.Cartons = 0;
                        }
                    }
                }
                else
                {
                    try
                    {
                        containerItem.Cartons = Convert.ToInt32(item.Cartons);
                    }
                    catch (Exception e)
                    {
                        containerItem.Cartons = 0;
                    }
                }

                
        
                
                containerItem.BuyerCurrency = item.BuyerCurrency;

                if(String.IsNullOrEmpty(item.BuyerUnitPrice)){
                    containerItem.BuyerUnitPrice = 
                                                 db.TmpContainerItems
                                                .OrderByDescending(i => i.ContainerID)
                                                .Where(i => i.PartyName == item.PartyName && i.ProductBuyerName == item.ProductBuyerName && i.ProductUnit == item.ProductUnit)
                                                .Select(i => i.BuyerUnitPrice)
                                                .FirstOrDefault();

                }
                else {
                    containerItem.BuyerUnitPrice = Convert.ToDecimal(item.BuyerUnitPrice);
                }

                if (String.IsNullOrEmpty(item.ProductCustomsName))
                {
                    containerItem.ProductCustomsName =
                                                 db.TmpContainerItems
                                                .OrderByDescending(i => i.ContainerID)
                                                .Where(i => i.ProductBuyerName == item.ProductBuyerName)
                                                .Select(i => i.ProductCustomsName)
                                                .FirstOrDefault();
                }
                else
                {
                    containerItem.ProductCustomsName = item.ProductCustomsName;
                }

                if (String.IsNullOrEmpty(item.CustomsProductUnit))
                {
                    containerItem.CustomsProductUnit =
                                                 db.TmpContainerItems
                                                .OrderByDescending(i => i.ContainerID)
                                                .Where(i => i.ProductCustomsName == item.ProductCustomsName)
                                                .Select(i => i.CustomsProductUnit)
                                                .FirstOrDefault();
                }
                else
                {
                    containerItem.CustomsProductUnit = item.CustomsProductUnit;
                }


                //Populate CustomsQuantity automatically
                if (String.IsNullOrEmpty(item.CustomsQuantity))
                {
                    if (String.IsNullOrEmpty(item.Quantity))
                    {
                        containerItem.CustomsQuantity =
                                                 db.TmpContainerItems
                                                .OrderByDescending(i => i.ContainerID)
                                                .Where(i => i.ProductBuyerName == item.ProductBuyerName)
                                                .Select(i => i.CustomsQuantity)
                                                .FirstOrDefault();
                    }
                    else
                    {
                        if (containerItem.ProductUnit == "PCS" && containerItem.CustomsProductUnit == "DOZ")
                        {
                            containerItem.CustomsQuantity = containerItem.Quantity / 12;
                        }
                        else
                        {
                            containerItem.CustomsQuantity = containerItem.Quantity;
                        }
                    }
                    
                }
                else
                {
                    containerItem.CustomsQuantity = Convert.ToDecimal(item.CustomsQuantity);
                }



                if (String.IsNullOrEmpty(item.CustomsCurrency))
                {
                    containerItem.CustomsCurrency =
                                                 db.TmpContainerItems
                                                .OrderByDescending(i => i.ContainerID)
                                                .Where(i => i.ProductBuyerName == item.ProductBuyerName)
                                                .Select(i => i.CustomsCurrency)
                                                .FirstOrDefault();

                }
                else
                {
                    containerItem.CustomsCurrency = item.CustomsCurrency;
                }

                if (String.IsNullOrEmpty(item.CustomsUnitPrice))
                {
                    containerItem.CustomsUnitPrice =
                                                 db.TmpContainerItems
                                                .OrderByDescending(i => i.ContainerID)
                                                .Where(i => i.ProductBuyerName == item.ProductBuyerName)
                                                .Select(i => i.CustomsUnitPrice)
                                                .FirstOrDefault();
                }
                else
                {
                    containerItem.CustomsUnitPrice = Convert.ToDecimal(item.CustomsUnitPrice);
                }
                

                db.TmpContainerItems.Add(containerItem);
                db.SaveChanges();
            }

            return new RedirectResult(Url.Action("Index", "TmpDashboard"));
        }
    }
}