using ClosedXML.Excel;
using HK.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
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
                var containerWS = wb.Worksheet("Container");
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


            var ws = wb.Worksheet("Items");

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
         
                containerItem.ProductBuyerName = item.ProductBuyerName;
                containerItem.ProductUnit = item.ProductUnit.TrimEnd('.');

                containerItem.Quantity = Convert.ToDecimal(item.Quantity);

                containerItem.PartyName = item.PartyName;
                containerItem.JobNumber = item.JobNumber;
                containerItem.LotSize = item.LotSize;
                containerItem.BillOnBoardingDate = item.BillOnBoardingDate;
                containerItem.BillDeliveryDate = item.BillDeliveryDate;
                containerItem.BillNumber = item.BillNumber;
                containerItem.BillTTDAPDate = item.BillTTDAPDate;
                containerItem.BillTTDAPNumber = item.BillTTDAPNumber;

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
                                                .LastOrDefault();

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
                                                .LastOrDefault();
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
                                                .LastOrDefault();
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
                                                .LastOrDefault();
                    }
                    else
                    {
                        if ((containerItem.ProductUnit == "PCS" || containerItem.ProductUnit == "PRS") && containerItem.CustomsProductUnit == "DOZ")
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
                                                .LastOrDefault();

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
                                                .Where(i => i.ProductCustomsName == item.ProductCustomsName)
                                                .Select(i => i.CustomsUnitPrice)
                                                .LastOrDefault();
                }
                else
                {
                    containerItem.CustomsUnitPrice = Convert.ToDecimal(item.CustomsUnitPrice);
                }
                
                db.TmpContainerItems.Add(containerItem);
                db.SaveChanges();
            }

            var MarkaPartyInfo = db.TmpContainerItems
                                    .Where(c => c.ContainerID == CurrentContainerID)
                                    .GroupBy(c => new
                                    {
                                        c.Marka
                                    })
                                    .Select(group =>
                                       new
                                       {
                                           Marka = group.Key.Marka,
                                           PartyName = group.FirstOrDefault(a => a.PartyName != "").PartyName,
                                           JobNumber = group.FirstOrDefault(a => a.JobNumber != "").JobNumber,
                                           LotSize = group.FirstOrDefault(a => a.LotSize != "").LotSize,
                                           BillOnBoardingDate = group.FirstOrDefault(a => a.BillOnBoardingDate != "").BillOnBoardingDate,
                                           BillDeliveryDate = group.FirstOrDefault(a => a.BillDeliveryDate != "").BillDeliveryDate,
                                           BillTTDAPDate = group.FirstOrDefault(a => a.BillTTDAPDate != "").BillTTDAPDate,
                                           BillTTDAPNumber = group.FirstOrDefault(a => a.BillTTDAPNumber != "").BillTTDAPNumber
                                       }
                                    ).ToDictionary(a => a.Marka);


            db.TmpContainerItems
                .Where(c => c.ContainerID == CurrentContainerID)
                .ToList()
                .ForEach(a => {
                    a.PartyName = MarkaPartyInfo[a.Marka].PartyName;
                    a.JobNumber = MarkaPartyInfo[a.Marka].JobNumber;
                    a.LotSize = MarkaPartyInfo[a.Marka].LotSize;
                    a.BillOnBoardingDate = MarkaPartyInfo[a.Marka].BillOnBoardingDate;
                    a.BillDeliveryDate = MarkaPartyInfo[a.Marka].BillDeliveryDate;
                    a.BillTTDAPDate = MarkaPartyInfo[a.Marka].BillTTDAPDate;
                    a.BillTTDAPNumber = MarkaPartyInfo[a.Marka].BillTTDAPNumber;
                });

            db.SaveChanges();


            var CustomsInfo = db.TmpContainerItems
                                .Where(c => c.ContainerID == CurrentContainerID)
                                .GroupBy(c => new
                                {
                                    c.ProductBuyerName
                                })
                                .Select(group =>
                                    new {
                                        group.Key.ProductBuyerName,
                                        group.FirstOrDefault(a => a.ProductCustomsName != "").ProductCustomsName,
                                        group.FirstOrDefault(a => a.CustomsProductUnit != "").CustomsProductUnit,
                                        group.FirstOrDefault(a => a.CustomsCurrency != "").CustomsCurrency,
                                        group.FirstOrDefault(a => a.CustomsUnitPrice != 0).CustomsUnitPrice
                                }).ToDictionary(a => a.ProductBuyerName);


            db.TmpContainerItems
               .Where(c => c.ContainerID == CurrentContainerID)
               .ToList()
               .ForEach(a =>
               {
                   a.ProductCustomsName = CustomsInfo[a.ProductBuyerName].ProductCustomsName;
                   a.CustomsProductUnit = CustomsInfo[a.ProductBuyerName].CustomsProductUnit;
                   a.CustomsCurrency = CustomsInfo[a.ProductBuyerName].CustomsCurrency;
                   a.CustomsUnitPrice = CustomsInfo[a.ProductBuyerName].CustomsUnitPrice;
               });

            db.SaveChanges();

            return new RedirectResult(Url.Action("Index", "TmpDashboard"));
        }
    }
}