using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VMS.SharedKernel;

namespace VMS.Entities
{
    public class MPriceListVoucher : BaseEntity
    {
        public string supplierID { get; set; }
        public string itemID { get; set; }
        public long? beginQty { get; set; }
        public long? endQty { get; set; }
        public float? price { get; set; }
    }
    public class MPriceListVoucherExt
    {
        public long Id { get; set; }
        public string supplierID { get; set; }
        public string itemID { get; set; }
        public long? beginQty { get; set; }
        public long? endQty { get; set; }
        public float? price { get; set; }
    }

    public class MPriceListVoucherDto
    {
        public long Id { get; set; }
        public string supplierID { get; set; }
        public string itemID { get; set; }
        public long? beginQty { get; set; }
        public long? endQty { get; set; }
        public float? price { get; set; }
        public string FlagData { get; set; }
    }
    public class MPriceListVoucherGetPrice
    {        
        public string supplierID { get; set; }        
        public string itemID { get; set; }        
        public long Qty { get; set; }
    }
        
}
