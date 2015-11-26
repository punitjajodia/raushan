using HK.Models;
using HK.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HK.Controllers
{
    public class BuyerInvoicesController : BaseController
    {
        // GET: BuyerInvoices
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task Edit([Bind(Include = "ProductContainerBuyerPriceID,ProductID,ContainerID,BuyerID,Unit,Price")] ProductContainerBuyerPrice productContainerBuyerPrice)
        {
            productContainerBuyerPrice.ContainerID = CurrentContainerID;
            if (ModelState.IsValid)
            {
                if (productContainerBuyerPrice.ProductContainerBuyerPriceID == 0)
                {
                    db.ProductContainerBuyerPrices.Add(productContainerBuyerPrice);
                }
                else
                {
                    db.Entry(productContainerBuyerPrice).State = EntityState.Modified;
                }
                await db.SaveChangesAsync();
            }
        }

        public PartialViewResult List()
        {
            var currentContainer = new ContainersController().GetCurrentContainer();
            var containerItems = currentContainer.ContainerItems;

        

            var buyerInvoiceItems = containerItems.GroupBy(i => new { i.Buyer, i.Product })
                          .Select(group =>
                                new BuyerInvoiceItem
                                {
                                    ProductContainerBuyerPrice = new Models.ProductContainerBuyerPrice
                                        {   
                                            BuyerID = group.Key.Buyer.BuyerID,
                                            Buyer = group.Key.Buyer,
                                            ProductID = group.Key.Product.ProductID,
                                            Product = group.Key.Product
                                        },
                                    Quantity = group.Sum(w => w.Quantity),
                                    Cartons = group.Sum(w => w.Cartons)
                                })
                          .OrderBy(i => i.ProductContainerBuyerPrice.BuyerID).ThenBy(i => i.ProductContainerBuyerPrice.ProductID)
                          .ToList();

            foreach(var buyerInvoiceItem in buyerInvoiceItems){
                

                var productContainerBuyerPrice = db.ProductContainerBuyerPrices.Where(
                    pcbp => pcbp.ContainerID == CurrentContainerID  && 
                        pcbp.ProductID == buyerInvoiceItem.ProductContainerBuyerPrice.ProductID && 
                        pcbp.BuyerID == buyerInvoiceItem.ProductContainerBuyerPrice.BuyerID).FirstOrDefault();

                if (productContainerBuyerPrice != null)
                {
                    buyerInvoiceItem.ProductContainerBuyerPrice = productContainerBuyerPrice;
                }
                else {
                    var previousProductBuyerPrice = db.ProductContainerBuyerPrices.Where(
                    pcbp => pcbp.ProductID == buyerInvoiceItem.ProductContainerBuyerPrice.ProductID &&
                        pcbp.BuyerID == buyerInvoiceItem.ProductContainerBuyerPrice.BuyerID)
                        .OrderByDescending(pcbp => pcbp.ContainerID)
                        .FirstOrDefault();
                    if (previousProductBuyerPrice != null)
                    {
                        buyerInvoiceItem.ProductContainerBuyerPrice = previousProductBuyerPrice;
                    }
                }

                switch (buyerInvoiceItem.ProductContainerBuyerPrice.Unit)
                {
                    //If the pricing is in dozens, revert back the quantity to dozen because everything was turned to pieces before
                    case ProductUnit.DOZ:
                        buyerInvoiceItem.Quantity /= 12;
                        break;
                }
            }

            return PartialView(buyerInvoiceItems);
        }
    }
}