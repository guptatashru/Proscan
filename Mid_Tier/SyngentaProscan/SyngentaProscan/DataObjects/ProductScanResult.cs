using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SyngentaProscan.DataObjects
{
    public class ProductScanResult : EntityData
    {
        public string RetailerID { get; set; }

        public string ProductImage { get; set; }
        public string AgentID { get; set; }

        public long BarcodeValue { get; set; }

        public string ProductID { get; set; }

        public bool isValid { get; set; }

        public bool isAgentNotified { get; set; }

        public string ValidationResponse { get; set; }

        public DateTime ScanDate { get; set; }

        public decimal? ScanLocationLat { get; set; }

        public decimal? ScanLocationLong { get; set; }

        public int? BatchID { get; set; }

        public bool isProcessed { get; set; }

        public string CountryCode { get; set; }

        public string ProductName { get; set; }

        public string AgentEmail { get; set; }

    }

    public class BarcodeScan
    {
        public string Barcode { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string RetailerId { get; set; }
    }

    public class BatchScan
    {
        [Required]
        public string RetailerID { get; set; }

        [Required]
        public string AgentID { get; set; }

        public string BarcodeValue { get; set; }

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

    }
}