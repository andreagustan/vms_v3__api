using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VMS.Entities
{
	public class T_SODetailRequest
	{
		public int ID { get; set; }
		public string? SONo { get; set; }
		public string? ItemID { get; set; }
		public string? ItemName { get; set; }
		public string? UOM { get; set; }
		public float? Qty { get; set; }
		public float? Price { get; set; }
		public float? DiscRate1 { get; set; }
		public float? DiscRate2 { get; set; }
		public float? DiscAmount { get; set; }
		public float? SubTotal { get; set; }
		//public string ExportedFileNo { get; set; }

	}

}
