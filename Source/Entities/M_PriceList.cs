using System.Collections.Generic;
using System;

namespace VMS.Entities
{
    public class M_PriceListVoucher
    {
        public string? Id { get; set; }
        public string supplierID { get; set; }
        public string? supplierName { get; set; }
        public string itemID { get; set; }
        public string? itemName { get; set; }
        public int? beginQty { get; set; }
        public int? endQty { get; set; }
        public float? price { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? CreateUser { get; set; }
        public int IsDeleted { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? UpdateUser { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string? DeletedUser { get; set; }
        public string? mode { get; set; }
        public string? userlogin { get; set; }
    }
}