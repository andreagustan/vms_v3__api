using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VMS.Entities
{
    public class T_SO_Bulk
    {
        public T_SO_Bulk() {
            DataDetail = new List<T_SODetailRequest>();
        }

        public string? soNo { get; set; }
        public DateTime? soDate { get; set; }
        public DateTime? expDate { get; set; }
        public DateTime? deliveryDate { get; set; }
        public string? customerID { get; set; }
        public string? customerName { get; set; }
        public string? branchID { get; set; }
        public string? branchName { get; set; }
        public string? delAddress { get; set; }
        public string? salesmanID { get; set; }
        public string? salesmanName { get; set; }
        public int? stsFreeGoods { get; set; }
        public int? stsPartial { get; set; }
        public string? remark { get; set; }
        public float? totalTransaction { get; set; }
        public float? discAmount { get; set; }
        public float? taxAmount { get; set; }
        public float? finalTotal { get; set; }
        public int? stsActive { get; set; }
        public int? stsPosted { get; set; }
        public string? entryUser { get; set; }
        //public string? Source { get; set; }
        public bool JSONProcess { get; set; }
        [Required]
        public string mode { get; set; }
        public List<T_SODetailRequest> DataDetail { get; set; }


    }
}
