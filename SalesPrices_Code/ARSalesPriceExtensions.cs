using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.TX;
using PX.Objects;
using System.Collections.Generic;
using System;

namespace PX.Objects.AR
{
  public class ARSalesPriceExt : PXCacheExtension<PX.Objects.AR.ARSalesPrice>
  {
    #region UsrImageToLink
    [PXDBString()]
    [PXUIField(DisplayName="Image",Enabled =false)]

    public virtual string UsrImageToLink { get; set; }
    public abstract class usrImageToLink : PX.Data.BQL.BqlString.Field<usrImageToLink> { }
    #endregion

    #region UsrTrendingQty
    [PXDBDecimal]
    [PXUIField(DisplayName="Trending Qty")]

    public virtual Decimal? UsrTrendingQty { get; set; }
    public abstract class usrTrendingQty : PX.Data.BQL.BqlDecimal.Field<usrTrendingQty> { }
    #endregion

    #region UsrCategory
    [PXDBString(100)]
    [PXUIField(DisplayName="Sales Category")]

    public virtual string UsrCategory { get; set; }
    public abstract class usrCategory : PX.Data.BQL.BqlString.Field<usrCategory> { }
    #endregion
  }
}