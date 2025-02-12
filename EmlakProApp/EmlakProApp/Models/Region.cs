using System.ComponentModel.DataAnnotations.Schema;

namespace EmlakProApp.Models
{
	public class Region
	{
		[Column("id_region")]
		public int IdRegion { get; set; }

		[Column("region_code")]
		public string? RegionCode { get; set; }
		[Column("region_name")]
		public string? RegionName { get; set; }

		[Column("keyword_01")]
		public string? Keyword01 { get; set; }

		[Column("keyword_02")]
		public string? Keyword02 { get; set; }

		[Column("keyword_03")]
		public string? Keyword03 { get; set; }
    }
}
