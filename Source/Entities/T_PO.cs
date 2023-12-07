using System.Collections.Generic;
using System;

namespace VMS.Entities
{
    public class T_PO
    {
        public string? PONo { get; set; }
        public DateTime? PODate { get; set; }
        public DateTime? ExpDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string? SupplierID { get; set; }
        public string? SupplierName { get; set; }
        public string BranchID { get; set; }
        public string? BranchName { get; set; }
        public string? DelAddress { get; set; }
        public Int64? StsFreeGoods { get; set; }
        public Int64? StsPartial { get; set; }
        public string? Remark { get; set; }
        public float? TotalTransaction { get; set; }
        public float? DiscRate { get; set; }
        public float? DiscAmount { get; set; }
        public float? DiscAmount1 { get; set; }
        public float? TaxRate { get; set; }
        public float? TaxAmount { get; set; }
        public float? FinalTotal { get; set; }
        public Int64? StsActive { get; set; }
        public Int64? StsPosted { get; set; }
        public string? EntryUser { get; set; }
        public float? PPHRate { get; set; }
        public float? PPHAmount { get; set; }
        public string? PoType { get; set; }
        public string? SoNo { get; set; }
        public string? InputVoucherType { get; set; }
        public string? Source { get; set; }
        public string? mode { get; set; }
        public string? userlogin { get; set; }
        //public string DataDetail { get; set; }
        public List<T_PODetail>? DataDetail { get; set; }
    }

    public class T_PODetail
    {
        public int id { get; set; }
        public string? PONo { get; set; }
        public string? ItemID { get; set; }
        public string? ItemName { get; set; }
        public string? UOM { get; set; }
        public float? QTY { get; set; }
        public float? Price { get; set; }
        public float? DiscRate1 { get; set; }
        public float? DiscRate2 { get; set; }
        public float? DiscAmount { get; set; }
        public float? SubTotal { get; set; }
        public int? idpajak { get; set; }
        public float? ppn { get; set; }
        public string? itemIdHadiah { get; set; }
        public int? qtyIn { get; set; }
        public int? qtyHadiah { get; set; }

    }
}
