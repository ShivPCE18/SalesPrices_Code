using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.IN;
using PX.Objects.SO;
using static PX.Objects.AR.CustomerMaint;
using PX.Web.UI;
using System;
using System.Collections;
using PX.Objects.CR;

namespace PX.Objects.AR
{
    public class ARPriceWorksheetMaint_Extension : PXGraphExtension<ARPriceWorksheetMaint>
    {

        public PXSelect<SOLine, Where<SOLine.inventoryID, Equal<Current<ARPriceWorksheetDetail.inventoryID>>>> lines;
        public PXSelect<BAccount> childAccounts;

        public PXSelect<InventoryItem> inventory;
        private decimal? trendingQty = 0;
        private string description;

        #region Event Handlers

        //protected void ARPriceWorksheetDetail_RowSelected(PXCache cache, PXRowSelectedEventArgs e, PXRowSelected InvokeBaseHandler)

        [PXOverride]
        public virtual void Persist(Action del)
        {
            del?.Invoke();
        }

        protected void ARPriceWorksheetDetail_RowSelected(PXCache cache, PXRowSelectedEventArgs e, PXRowSelected InvokeBaseHandler)
        {
            if (InvokeBaseHandler != null)
                InvokeBaseHandler(cache, e);
            var row = (ARPriceWorksheetDetail)e.Row;
            if (row != null)
            {
                var ext = row.GetExtension<ARPriceWorksheetDetailExt>();

                InventoryItemMaint inventoryItemMaint = PXGraph.CreateInstance<InventoryItemMaint>();

                inventoryItemMaint.Item.Current = inventoryItemMaint.Item.Search<InventoryItem.inventoryID>(row.InventoryID);

                InventoryItem inv = PXSelect<InventoryItem, Where<InventoryItem.inventoryCD, Equal<Current<ARPriceWorksheetDetail.priceCode>>>>.Select(this.Base);

                inv = inventoryItemMaint.Item.Insert(inv);

                if (inventoryItemMaint != null)
                {

                    var fileAttachments = PXNoteAttribute.GetFileNotes(inventoryItemMaint.Item.Cache, inventoryItemMaint.Item.Current);

                    foreach (Guid fileId in fileAttachments)
                    {
                        //ext.UsrLinkToImage = ControlHelper.GetAttachedFileUrl(null, fileId.ToString());
                        cache.SetValue<ARPriceWorksheetDetailExt.usrLinkToImage>(row, ControlHelper.GetAttachedFileUrl(null, fileId.ToString()));
                        break;
                    }

                    CustomerMaint customerMaint = PXGraph.CreateInstance<CustomerMaint>();

                    customerMaint.BAccount.Current = customerMaint.BAccount.Search<Customer.acctCD>(row.PriceCode);


                    if (customerMaint != null && !string.IsNullOrEmpty(row.PriceCode))
                    {
                        decimal? TotalQty = 0;
                        DateTime backMonth = DateTime.Now.AddMonths(-3);


                        //Customer inventory qty on sales order
                        foreach (PXResult<SOLine> item in lines.Select())
                        {
                            SOLine line = (SOLine)item;

                            if (line.CustomerID == row.CustomerID && line.OrderDate >= backMonth)
                            {
                                TotalQty += line.Qty;
                            }
                        }


                        //Child account inventory qty on sales order
                        foreach (BAccount item in childAccounts.Select())
                        {
                            foreach (PXResult<SOLine> item2 in lines.Select())
                            {
                                SOLine line = (SOLine)item2;
                                //if (item.BAccountID != row.CustomerID && line.OrderDate >= backMonth && item.BAccountID == line.CustomerID )
                                if (item.BAccountID != row.CustomerID && line.OrderDate >= backMonth && item.ParentBAccountID == line.CustomerID)
                                    TotalQty += line.Qty;
                            }
                        }

                        //Trending qty of previous 3 month
                        if (TotalQty != 0)
                            trendingQty = TotalQty / 3;

                        cache.SetValue<ARPriceWorksheetDetailExt.usrTrendingQty>(row, trendingQty);
                        //ext.UsrTrendingQty = TotalQty / 3;


                    }

                    //throw new PXRedirectRequiredException(inventoryItemMaint, "Doc");

                    INItemCategory iNItemCategory = PXSelect<INItemCategory, Where<INItemCategory.inventoryID, Equal<Required<INItemCategory.inventoryID>>>>.Select(this.Base, row.InventoryID);

                    if (iNItemCategory != null)
                    {
                        INCategory category = PXSelect<INCategory, Where<INCategory.categoryID, Equal<Required<INCategory.categoryID>>>>.Select(this.Base, iNItemCategory.CategoryID);
                        if (category != null)
                        {
                            if (category.ParentID != 0)
                            {
                                INCategory parent = PXSelect<INCategory, Where<INCategory.categoryID, Equal<Required<INCategory.parentID>>>>.Select(this.Base, category.ParentID);
                                description = parent.Description + " > " + category.Description;
                            }
                            else
                                description = category.Description;

                            cache.SetValue<ARPriceWorksheetDetailExt.usrSalesCategory>(row, description);
                        }
                        //ext.UsrSalesCategory = category.Description;

                    }
                }

            }

        }
        #endregion
    }



}