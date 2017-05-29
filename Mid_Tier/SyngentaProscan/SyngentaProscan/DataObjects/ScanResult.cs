using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SyngentaProscan.DataObjects
{
    public class ScanResult : EntityData 
    {
        [Required]
        public string RetailerID { get; set; }

        [Required]
        public string AgentID { get; set; }

        public long BarcodeValue { get; set; }

        public string ProductID { get; set; }
                
        public bool isValid { get; set; }

        public bool isAgentNotified { get; set; }

        [MaxLength(255)]
        public string ValidationResponse { get; set; }

        public DateTime ScanDate { get; set; }

        public decimal? ScanLocationLat { get; set; }

        public decimal? ScanLocationLong { get; set; }

        public int? BatchID { get; set; }
                
        public bool isProcessed { get; set; }

        [MaxLength(5)]
        public string CountryCode { get; set; }

        [ForeignKey("RetailerID")]
        public Retailer Retailer { get; set; }

        [ForeignKey("AgentID")]     
        public Agent Agent { get; set; }
    }
}