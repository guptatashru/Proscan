namespace SyngentaProScanWebAPI
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Runtime.Serialization;

    [Table("Agent")]
    [JsonObject(IsReference = true)]
    public partial class Agent
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Agent()
        {
            Retailers = new HashSet<Retailer>();
            ScanResults = new HashSet<ScanResult>();
        }

        public int AgentID { get; set; }

        [Required]
        public string LoginID { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string MobileNo { get; set; }

        public string Email { get; set; }

        public string PNSID { get; set; }

        public string ProfileImage { get; set; }

        public bool isEnabled { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime CreatedAt { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Retailer> Retailers { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ScanResult> ScanResults { get; set; }
    }
}
