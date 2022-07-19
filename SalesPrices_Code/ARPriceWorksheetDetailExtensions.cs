using PX.Data;
using System;

namespace PX.Objects.AR
{
    public class ARPriceWorksheetDetailExt : PXCacheExtension<PX.Objects.AR.ARPriceWorksheetDetail>
    {


        #region UsrSalesCategory
        [PXDBString(50)]
        [PXUIField(DisplayName = "Sales Category")]

        public virtual string UsrSalesCategory { get; set; }
        public abstract class usrSalesCategory : PX.Data.BQL.BqlString.Field<usrSalesCategory> { }
        #endregion

        #region UsrLinkToImage
        [PXDBString()]
        [PXUIField(DisplayName = "Link To Image", Enabled = false)]
        public virtual string UsrLinkToImage { get; set; }
        public abstract class usrLinkToImage : PX.Data.BQL.BqlString.Field<usrLinkToImage> { }
        #endregion

        #region UsrTrendingQty
        [PXDBDecimal]
        [PXUIField(DisplayName = "Trending Qty")]

        public virtual Decimal? UsrTrendingQty { get; set; }
        public abstract class usrTrendingQty : PX.Data.BQL.BqlDecimal.Field<usrTrendingQty> { }
        #endregion
    }

}



//namespace PX.Objects.IN
//{
//    //ARPriceWorksheet
//    [PXPrimaryGraph(typeof(InventoryItemMaint))]
//    public class InventoryItemExt : PXCacheExtension<InventoryItem>
//    {


//        //    #region NoteID
//        //    public abstract class noteID : PX.Data.IBqlField { }

//        //    [PXNote(
//        //typeof(InventoryItem.inventoryCD),
//        //        typeof(InventoryItem.invtAcctID),
//        //        ForeignRelations =
//        //            new Type[] { typeof(InventoryItem.deferralAcctID)},
//        //        ShowInReferenceSelector = true,
//        //        DescriptionField = typeof(InventoryItem.inventoryCD),
//        //        Selector = typeof(InventoryItem.inventoryCD)
//        //    )]
//        //    public virtual Guid? NoteID { get; set; }
//        //    #endregion
//        #region UsrLinkToImage
//        [PXDBString(100)]
//        [PXUIField(DisplayName = "Image")]
//        public virtual string UsrLinkToImage { get; set; }
//        public abstract class usrLinkToImage : PX.Data.BQL.BqlString.Field<usrLinkToImage> { }
//        #endregion

//    }
//}