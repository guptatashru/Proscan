namespace SyngentaProScanWebAPI
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    [JsonObject(IsReference = true)]
    [Table("ScanResult")]
    public partial class ScanResult
    {
        public int ScanResultID { get; set; }

        public int RetailerID { get; set; }

        public int AgentID { get; set; }

        public long? BarcodeValue { get; set; }

        public int? ProductID { get; set; }

        public bool isValid { get; set; }

        public bool isAgentNotified { get; set; }

        public string ValidationResponse { get; set; }

        public DateTime? ScanDate { get; set; }

        public decimal? ScanLocationLat { get; set; }

        public decimal? ScanLocationLong { get; set; }

        public int? BatchID { get; set; }

        public bool isProcessed { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public string CountryCode { get; set; }

        public virtual Agent Agent { get; set; }

        public virtual Product Product { get; set; }

        public virtual Retailer Retailer { get; set; }


    }
}
