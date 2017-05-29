namespace ProscanWebJob
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Retailer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Retailer()
        {
            ScanResults = new HashSet<ScanResult>();
        }

        public string Id { get; set; }

        [Required]
        public string LoginID { get; set; }

        [Required]
        [StringLength(128)]
        public string AgentID { get; set; }

        [StringLength(255)]
        public string RetailerCode { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Address { get; set; }

        [StringLength(20)]
        public string MobileNo { get; set; }

        [StringLength(255)]
        public string Email { get; set; }

        [StringLength(255)]
        public string PNSID { get; set; }

        public string ProfileImage { get; set; }

        public bool isEnabled { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] Version { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }

        public bool Deleted { get; set; }

        public virtual Agent Agent { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ScanResult> ScanResults { get; set; }
    }
}
