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

    public class InvalidCartonNumberException : FormatException
    {

    }


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
            db.SaveChanges();
        }


        public int CartonsFromCartonNumber(string cartonNumber)
        {
            if (cartonNumber.EndsWith("CTN"))
            {
                cartonNumber = cartonNumber.Substring(0, cartonNumber.LastIndexOf("CTN")).Trim();
                return Convert.ToInt32(cartonNumber);
            }

            if (cartonNumber.EndsWith("CTNS"))
            {
                cartonNumber = cartonNumber.Substring(0, cartonNumber.LastIndexOf("CTNS")).Trim();
                return Convert.ToInt32(cartonNumber);
            }

            int total = 0;
            var parts = cartonNumber.Split(',');

            try
            {
                foreach (string part in parts)
                {
                    var combo = part.Split('-');
                    if (combo.Count() == 1)
                    {
                        total += 1;
                    }
                    else
                    {
                        total += Convert.ToInt32(combo[1]) - Convert.ToInt32(combo[0]) + 1;
                    }
                }
            }
            catch (FormatException e)
            {
                throw new FormatException("Carton number " + cartonNumber + " format is incorrect");
            }
            
           
            return total;
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
                return View("InvalidExcel", (object)("Something wrong with container information<br/>" + e.Message));
            }

            DeleteAllContainerItems();


            var ws = wb.Worksheet("Items");

            var dataRange = ws.RangeUsed();

            // Treat the range as a table (to be able to use the column names)

            var dataTable = dataRange.AsTable();

                var dataObj = dataTable.DataRange.Rows()
              .Select(packingList => new
              {
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
                  BuyerUnitPrice = packingList.Field("BuyerUnitPrice").GetString(),
                  CustomsQuantity = packingList.Field("CustomsQuantity").GetString(),
                  CustomsProductUnit = packingList.Field("CustomsProductUnit").GetString(),
                  CustomsCurrency = packingList.Field("CustomsCurrency").GetString(),
                  CustomsUnitPrice = packingList.Field("CustomsUnitPrice").GetString()
              })
              .ToList();
            
               
           
            // Get the list of company names
           

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

                try
                {
                    containerItem.Cartons = CartonsFromCartonNumber(cartonNumber); 
                }
                catch (FormatException e)
                {
                    return View("InvalidExcel", (object)e.Message);
                }


                if (!containerItem.CartonNumber.Contains("CTN") && db.TmpContainerItems
                        .Any(c => c.ContainerID == CurrentContainerID && c.CartonNumber == item.CartonNumber && c.Marka == item.Marka))
                {
                    containerItem.Cartons = 0;
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
                                                .DefaultIfEmpty("")
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
                        if ((containerItem.ProductUnit.Trim().Equals("PCS") || containerItem.ProductUnit.Trim().Equals("PRS")) && containerItem.CustomsProductUnit.Trim().Equals("DOZ"))
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
                                                .Where(i => i.ProductCustomsName == item.ProductCustomsName)
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


            //db.TmpContainerItems
            //    .Where(c => c.ContainerID == CurrentContainerID)
            //    .ToList()
            //    .ForEach(a => {
            //        a.PartyName = MarkaPartyInfo[a.Marka].PartyName;
            //        a.JobNumber = MarkaPartyInfo[a.Marka].JobNumber;
            //        a.LotSize = MarkaPartyInfo[a.Marka].LotSize;
            //        a.BillOnBoardingDate = MarkaPartyInfo[a.Marka].BillOnBoardingDate;
            //        a.BillDeliveryDate = MarkaPartyInfo[a.Marka].BillDeliveryDate;
            //        a.BillTTDAPDate = MarkaPartyInfo[a.Marka].BillTTDAPDate;
            //        a.BillTTDAPNumber = MarkaPartyInfo[a.Marka].BillTTDAPNumber;
            //    });

            //db.SaveChanges();

            //var CustomsInfo = db.TmpContainerItems
            //                    .Where(c => c.ContainerID == CurrentContainerID)
            //                    .GroupBy(c => new
            //                    {
            //                        c.ProductBuyerName
            //                    })
            //                    .Select(group =>
            //                        new {
            //                            ProductBuyerName = group.Key.ProductBuyerName,
            //                            ProductCustomsName = group.FirstOrDefault(a => a.ProductCustomsName != "").ProductCustomsName,
            //                            CustomsProductUnit = group.FirstOrDefault(a => a.CustomsProductUnit != "").CustomsProductUnit,
            //                            CustomsCurrency = group.FirstOrDefault(a => a.CustomsCurrency != "").CustomsCurrency,
            //                            CustomsUnitPrice = (decimal?)group.FirstOrDefault(a => a.CustomsUnitPrice != 0).CustomsUnitPrice ?? 0
            //                    }).ToDictionary(a => a.ProductBuyerName);

            //db.TmpContainerItems
            //   .Where(c => c.ContainerID == CurrentContainerID)
            //   .ToList()
            //   .ForEach(a =>
            //   {
            //       a.ProductCustomsName = CustomsInfo[a.ProductBuyerName].ProductCustomsName;
            //       a.CustomsProductUnit = CustomsInfo[a.ProductBuyerName].CustomsProductUnit;
            //       a.CustomsCurrency = CustomsInfo[a.ProductBuyerName].CustomsCurrency;
            //       a.CustomsUnitPrice = CustomsInfo[a.ProductBuyerName].CustomsUnitPrice;
            //   });

            db.SaveChanges();

            return new RedirectResult(Url.Action("Index", "TmpDashboard"));
        }
    }
}