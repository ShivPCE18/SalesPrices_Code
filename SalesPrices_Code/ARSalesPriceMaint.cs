using System;
using System.Collections.Generic;
using PX.Data;
using System.Collections;
using PX.Objects.AR.Repositories;
using PX.Objects.IN;
using PX.SM;
using PX.TM;
using PX.Objects.SO;
using PX.Objects.GL;
using PX.Objects.CM;
using System.Linq;
using PX.Common;
using PX.Objects.CS;
using OwnedFilter = PX.TM.OwnedFilter;
using PX.Objects.Common;
using PX.Objects;
using PX.Objects.AR;
using PX.Web.UI;
using static PX.Objects.AR.CustomerMaint;
using PX.Objects.CR;
using PX.Data.BQL.Fluent;

namespace PX.Objects.AR
{
    public class ARSalesPriceMaint_Extension : PXGraphExtension<ARSalesPriceMaint>
    {
        #region view
        public PXSelect<SOLine, Where<SOLine.inventoryID, Equal<Current<ARSalesPrice.inventoryID>>>> lines;
        public SelectFrom<InventoryItem>.View inventories;
        public PXSelect<BAccount> accounts;
        private decimal? trendingQty=0;
        #endregion
        #region Event Handlers
        protected void ARSalesPrice_RowSelected(PXCache cache, PXRowSelectedEventArgs e, PXRowSelected InvokeBaseHandler)
        {
            if (InvokeBaseHandler != null)
                InvokeBaseHandler(cache, e);

            var row = (ARSalesPrice)e.Row;
            if (row != null)
            {
                var ext = row.GetExtension<ARSalesPriceExt>();

                InventoryItemMaint inventoryItemMaint = PXGraph.CreateInstance<InventoryItemMaint>();

                inventoryItemMaint.Item.Current = inventoryItemMaint.Item.Search<InventoryItem.inventoryID>(row.InventoryID);


                if (inventoryItemMaint != null)
                {

                    var fileAttachments = PXNoteAttribute.GetFileNotes(inventoryItemMaint.Item.Cache, inventoryItemMaint.Item.Current);

                    foreach (Guid fileId in fileAttachments)
                    {
                        cache.SetValue<ARSalesPriceExt.usrImageToLink>(row, ControlHelper.GetAttachedFileUrl(null, fileId.ToString()));
                        break;
                    }

                    CustomerMaint customerMaint = PXGraph.CreateInstance<CustomerMaint>();

                    customerMaint.BAccount.Current = customerMaint.BAccount.Search<Customer.acctCD>(row.PriceCode);


                    if (row.PriceType == "C" && !string.IsNullOrEmpty(row.PriceCode))
                    {
                        decimal? TotalQty = 0;
                        DateTime backMonth = DateTime.Now.AddMonths(-3);


                        //Customer inventory qty on sales order
                        foreach (PXResult<SOLine> item in lines.Select())
                        {
                            SOLine line = (SOLine)item;
                            if (line.CustomerID == row.CustomerID && line.OrderDate >= backMonth && row.InventoryID == line.InventoryID)
                            {
                                TotalQty += line.Qty;
                            }
                        }




                        //Child account inventory qty on sales order
                        BAccount mainAccount = accounts.Search<BAccount.acctCD>(row.PriceCode.Trim());

                        if (mainAccount != null)
                        {

                            foreach (BAccount childAccount in accounts.Select())
                            {

                                if (mainAccount.BAccountID != null && childAccount.ParentBAccountID == mainAccount.BAccountID)
                                    foreach (PXResult<SOLine> item2 in lines.Select())
                                    {
                                        SOLine line = (SOLine)item2;
                                        if (line.CustomerID == childAccount.BAccountID && line.OrderDate >= backMonth && row.InventoryID == line.InventoryID)
                                            TotalQty += line.Qty;
                                    }
                            }

                            //Trending qty of previous 3 month
                            if (TotalQty != 0)
                                trendingQty = TotalQty / 3;

                            cache.SetValue<ARSalesPriceExt.usrTrendingQty>(row, trendingQty);
                        }
                    }

                    //throw new PXRedirectRequiredException(inventoryItemMaint, "Doc");

                    INItemCategory iNItemCategory = PXSelect<INItemCategory, Where<INItemCategory.inventoryID, Equal<Required<INItemCategory.inventoryID>>>>.Select(this.Base, row.InventoryID);

                    if (iNItemCategory != null)
                    {
                        INCategory category = PXSelect<INCategory, Where<INCategory.categoryID, Equal<Required<INCategory.categoryID>>>>.Select(this.Base, iNItemCategory.CategoryID);
                        if (category != null)
                            cache.SetValue<ARSalesPriceExt.usrCategory>(row, category.Description);

                    }

                    //PXCache cache2 = this.Base.Caches[typeof(ARSalesPrice)];
                    //cache.Persist(row, PXDBOperation.Update);
                    //this.Base.Save.Press();
                    //this.Base.Save.SetPressed(true);
                    //cache.Graph.Actions.PressSave();
                }


            }
        }



        #endregion
    }
}