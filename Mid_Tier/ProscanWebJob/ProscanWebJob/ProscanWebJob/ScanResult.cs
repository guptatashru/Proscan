namespace ProscanWebJob
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ScanResult
    {
        public string Id { get; set; }

        [Required]
        [StringLength(128)]
        public string RetailerID { get; set; }

        [Required]
        [StringLength(128)]
        public string AgentID { get; set; }

        public long BarcodeValue { get; set; }

        public string ProductID { get; set; }

        public bool isValid { get; set; }

        public bool isAgentNotified { get; set; }

        [StringLength(255)]
        public string ValidationResponse { get; set; }

        public DateTime ScanDate { get; set; }

        public decimal? ScanLocationLat { get; set; }

        public decimal? ScanLocationLong { get; set; }

        public int? BatchID { get; set; }

        public bool isProcessed { get; set; }

        [StringLength(5)]
        public string CountryCode { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] Version { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }

        public bool Deleted { get; set; }

        public virtual Agent Agent { get; set; }

        public virtual Retailer Retailer { get; set; }
    }
}
