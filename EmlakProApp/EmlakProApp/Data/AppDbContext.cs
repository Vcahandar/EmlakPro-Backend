using EmlakProApp.Models;
using EmlakProApp.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EmlakProApp.Data
{
	public class AppDbContext: IdentityDbContext<AppUser>
	{
		public AppDbContext(DbContextOptions<AppDbContext> options)
		: base(options)
		{
		}

		public DbSet<OtpCode> OtpCodes { get; set; }


		//public DbSet<BuildingType> building_type { get; set; }
		//public DbSet<Document> document { get; set; }
		//public DbSet<Metro> metro { get; set; }
		//public DbSet<OwnerType> owner_type { get; set; }
		//public DbSet<PropertyType> property_type { get; set; }
		//public DbSet<Region> region { get; set; }
		//public DbSet<RepairRate> repair_rate { get; set; }
		//public DbSet<RoomCount> room_count { get; set; }

		//protected override void OnModelCreating(ModelBuilder modelBuilder)
		//{

		//	modelBuilder.Entity<Document>()
		//		.HasKey(d => d.IdDocument);

		//	modelBuilder.Entity<Metro>()
		//		.HasKey(m => m.IdMetro);
		//	modelBuilder.Entity<Metro>()
		//		.HasIndex(m => m.FkIdRegion)
		//		.HasDatabaseName("IX_fk_id_region");

		//	modelBuilder.Entity<OwnerType>()
		//		.HasKey(o => o.IdOwnerType);

		//	modelBuilder.Entity<PropertyType>()
		//		.HasKey(p => p.IdPropertyType);

		//	modelBuilder.Entity<Region>()
		//		.HasKey(r => r.IdRegion);
		//	modelBuilder.Entity<Region>()
		//		.HasIndex(r => r.RegionCode)
		//		.HasDatabaseName("IX_region_code");

		//	modelBuilder.Entity<RepairRate>()
		//		.HasKey(r => r.IdRepairRate);
		//	// Default dəyəri təyin etmək üçün:
		//	modelBuilder.Entity<RepairRate>()
		//		.Property(r => r.IsActive)
		//		.HasDefaultValue(0);

		//	modelBuilder.Entity<RoomCount>()
		//		.HasKey(r => r.IdRoomCount);

		//	modelBuilder.Entity<BuildingType>()
		//		.HasKey(b => b.IdBuildingType);

		//	base.OnModelCreating(modelBuilder);
		//}
	}
}
