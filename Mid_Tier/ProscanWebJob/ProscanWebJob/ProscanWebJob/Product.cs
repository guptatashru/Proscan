namespace ProscanWebJob
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Product
    {
        public string Id { get; set; }

        public int MaterialID { get; set; }

        [StringLength(255)]
        public string AUn { get; set; }

        public int? Numerat { get; set; }

        public string description { get; set; }

        public int? Denom { get; set; }

        public long? EANNumber { get; set; }

        public long? EAN_UPC { get; set; }

        public string ProfileImage { get; set; }

        [StringLength(5)]
        public string CountryCode { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] Version { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }

        public bool Deleted { get; set; }
    }
}
