namespace ProscanWebJob
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ProScanWebJobModel : DbContext
    {
        public ProScanWebJobModel()
            : base("name=ProScanWebJobModel")
        {
        }

        public virtual DbSet<Agent> Agents { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Retailer> Retailers { get; set; }
        public virtual DbSet<ScanResult> ScanResults { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Agent>()
                .Property(e => e.Version)
                .IsFixedLength();

            modelBuilder.Entity<Agent>()
                .HasMany(e => e.Retailers)
                .WithRequired(e => e.Agent)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Agent>()
                .HasMany(e => e.ScanResults)
                .WithRequired(e => e.Agent)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .Property(e => e.Version)
                .IsFixedLength();

            modelBuilder.Entity<Retailer>()
                .Property(e => e.Version)
                .IsFixedLength();

            modelBuilder.Entity<Retailer>()
                .HasMany(e => e.ScanResults)
                .WithRequired(e => e.Retailer)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ScanResult>()
                .Property(e => e.Version)
                .IsFixedLength();
        }
    }
}
