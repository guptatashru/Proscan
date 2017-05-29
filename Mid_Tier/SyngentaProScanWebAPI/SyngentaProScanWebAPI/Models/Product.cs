namespace SyngentaProScanWebAPI
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Runtime.Serialization;

    [JsonObject(IsReference = true)]
    [Table("Product")]
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            ScanResults = new HashSet<ScanResult>();
        }

        public int ProductID { get; set; }

        public int MaterialID { get; set; }

        public string AUn { get; set; }

        public int? Numerat { get; set; }

        public string description { get; set; }

        public int? Denom { get; set; }

        public long? EANNumber { get; set; }

        public long? EAN_UPC { get; set; }

        public string ProfileImage { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public string CountryCode { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ScanResult> ScanResults { get; set; }
    }
}
