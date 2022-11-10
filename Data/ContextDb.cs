using Microsoft.EntityFrameworkCore;
using UpxApi.Models;

namespace UpxApi.Data
{
    public class ContextDb : DbContext
    {
        public ContextDb(DbContextOptions<ContextDb> options) : base(options) { }
        public DbSet<Spot> Spots { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Student> Students { get; set; }
        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {    
            modelbuilder.Entity<Region>()
                .HasKey(p => p.RegionId);

            modelbuilder.Entity<Region>()
                .Property(p => p.Name)
                .IsRequired()
                .HasColumnType("varchar(max)");

            modelbuilder.Entity<Region>()
                .ToTable("Regions");

            modelbuilder.Entity<Spot>()
                .Property(p => p.Name)
                .IsRequired()
                .HasColumnType("varchar(10)");

            modelbuilder.Entity<Spot>()
                .Property(p => p.Type)
                .HasDefaultValue("car")
                .HasColumnType("varchar(50)");

            modelbuilder.Entity<Spot>()
                .Property(p => p.Latitude)
                .IsRequired()
                .HasColumnType("float(53)");

            modelbuilder.Entity<Spot>()
                .Property(p => p.Longitude)
                .IsRequired()
                .HasColumnType("float(53)");

            modelbuilder.Entity<Spot>()
                .Property(p => p.Address)
                .IsRequired()
                .HasColumnType("varchar(max)");

            modelbuilder.Entity<Spot>()
                .Property(p => p.Region)
                .IsRequired()
                .HasColumnType("varchar(10)");

            modelbuilder.Entity<Spot>()
                .Property(p => p.Occupied)
                .HasDefaultValue(0)
                .HasColumnType("bit");

            modelbuilder.Entity<Spot>()
                .ToTable("Spots");
            
            modelbuilder.Entity<Student>()
                .HasKey(p => p.Id);

            modelbuilder.Entity<Student>()
                .Property(p => p.Name)
                .IsRequired()
                .HasColumnType("varchar(max)");

            modelbuilder.Entity<Student>()
                .Property(p => p.Ra)
                .IsRequired()
                .HasColumnType("varchar(10)");

            modelbuilder.Entity<Student>()
                .Property(p => p.SpotId)
                .HasColumnType("int");

            modelbuilder.Entity<Student>()
                .ToTable("Students");

            base.OnModelCreating(modelbuilder);
        }

    }
}
