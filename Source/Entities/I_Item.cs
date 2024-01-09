using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VMS.Entities
{
    public class I_ItemBulk_Request
    {
		public I_ItemBulk_Request() {
			DataDetail = new List<I_ItemUnitRequest>();
		}

		public string ItemId { get; set; }
		public string? ItemName { get; set; }
		public string? ItemStructName { get; set; }
		public string? CategoryId { get; set; }
		public string? ItemType { get; set; }
		public string? Barcode1 { get; set; }
		public string? Barcode2 { get; set; }
		public string? Barcode3 { get; set; }
		public int? Active { get; set; }
		public int? BOM { get; set; }
		public string? BaseUOM { get; set; }
		public float? QtyMultiplyOrder { get; set; }
		public int? OpenPrice { get; set; }
		public int? Konversi { get; set; }
		public int? PPN { get; set; }
		public string? StatusItem { get; set; }
		public float? Point { get; set; }
		public string? EntryUser { get; set; }
		public string? OldItemId { get; set; }
		public string? UOMOrder { get; set; }
		public string? MainSupplierId { get; set; }
		public int? StsSO { get; set; }
		public int? StsPO { get; set; }
		public int? FlagRetur { get; set; }
		public int? FlagTukarGuling { get; set; }
		public int? FlagBKL { get; set; }
		public string? KodeTag { get; set; }
		public int? idDivisi { get; set; }
		public int? idDepartement { get; set; }
		public int? idSubDepartement { get; set; }
		public int? idKategori { get; set; }
		public int? ClassId { get; set; }
		public int? idPajak { get; set; }
		public int? idJenisProduk { get; set; }
		public int? idKemasan { get; set; }
		public float? RetDay { get; set; }
		public bool JSONProcess { get; set; }
		[Required]
		public string mode { get; set; }
		public List<I_ItemUnitRequest> DataDetail { get; set; }
    }

}
