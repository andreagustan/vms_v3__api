using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VMS.SharedKernel;

namespace VMS.Entities
{
    public class VoucherDetail : BaseEntity
    {
        public string refId { get; set; }
        public string itemID { get; set; }
        public string startNo { get; set; }
        public string endNo { get; set; }
        public DateTime? expDateVoucher { get; set; }
        public string expDateVoucherDateUs { get; set; }
        public long? qty { get; set; }
        public string sources { get; set; }
    }

    public class VoucherDetailExt
    {
        public long Id { get; set; }
        public string refId { get; set; }
        public string itemID { get; set; }
        public string startNo { get; set; }
        public string endNo { get; set; }
        public DateTime? expDateVoucher { get; set; }
        public long? qty { get; set; }
        public string sources { get; set; }
    }

    public class VoucherDetailDto
    {
        public long Id { get; set; }
        public string refId { get; set; }
        public string itemID { get; set; }
        public string startNo { get; set; }
        public string endNo { get; set; }
        public DateTime? expDateVoucher { get; set; }
        public long? qty { get; set; }
        public string sources { get; set; }
        public string FlagData { get; set; }
    }


}
