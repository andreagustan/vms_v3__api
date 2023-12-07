using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VMS.Entities
{
    public class T_PermintaanBarangRequest
    {
        public T_PermintaanBarangRequest() 
        {
            DataDetail = new List<T_PermintaanBarangDetailRequest>();
        }
        public string PBNo { get; set; }
        public DateTime? PBDate { get; set; }
        public string FromBranchID { get; set; }
        public string FromBranchName { get; set; }
        public string ToBranchID { get; set; }
        public string ToBranchName { get; set; }
        public float? Total { get; set; }
        public string Remark { get; set; }
        public string TransNo { get; set; }
        public string EntryUser { get; set; }
        public int? StsPosted { get; set; }
        public DateTime? ExpireDate { get; set; }
        public int? StsTransfer { get; set; }
        //public string Source { get; set; }
        public List<T_PermintaanBarangDetailRequest> DataDetail { get; set; }
    }
}
