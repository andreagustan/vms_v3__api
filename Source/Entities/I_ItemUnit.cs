using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VMS.Entities
{
	public class I_ItemUnitRequest
	{
		public string ItemId { get; set; }
		public string UOM { get; set; }
		public float? Factor { get; set; }
		public float? Length { get; set; }
		public float? Width { get; set; }
		public float? Height { get; set; }
		public string? SizeUnit { get; set; }
		public float? Netto { get; set; }
		public float? Bruto { get; set; }
		public string? WeightUnit { get; set; }
		public string UpdateUser { get; set; }
	}
}
