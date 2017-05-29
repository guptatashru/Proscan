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
    public class Agent : EntityData
    {
        [Required]
        public string LoginID { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Address { get; set; }

        [MaxLength(20)]
        public string MobileNo { get; set; }

        [MaxLength(255)]
        public string Email { get; set; }

        [MaxLength(255)]
        public string PNSID { get; set; }

        public string ProfileImage { get; set; }

        public bool isEnabled { get; set; }

        public ICollection<Retailer> Retailers { get; set; }

        public ICollection<ScanResult> ScanResults { get; set; }
    }
}