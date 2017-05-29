namespace SyngentaProScanWebAPI
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class SyngentaProScanContext : DbContext
    {
        public SyngentaProScanContext()
            : base("name=SyngentaProScanContext")
        {
        }

        public virtual DbSet<Agent> Agents { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Retailer> Retailers { get; set; }
        public virtual DbSet<ScanResult> ScanResults { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Agent>()
                .HasMany(e => e.Retailers)
                .WithRequired(e => e.Agent)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Agent>()
                .HasMany(e => e.ScanResults)
                .WithRequired(e => e.Agent)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Retailer>()
                .HasMany(e => e.ScanResults)
                .WithRequired(e => e.Retailer)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ScanResult>()
                .Property(e => e.ScanLocationLat)
                .HasPrecision(18, 12);

            modelBuilder.Entity<ScanResult>()
                .Property(e => e.ScanLocationLong)
                .HasPrecision(18, 12);
        }
    }
}
