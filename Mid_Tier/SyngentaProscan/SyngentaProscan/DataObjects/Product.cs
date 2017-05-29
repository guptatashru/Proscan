using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SyngentaProscan.DataObjects
{
    public class Product : EntityData
    {
        public int MaterialID { get; set; }

        [MaxLength(255)]
        public string AUn { get; set; }

        public int? Numerat { get; set; }

        public string description { get; set; }

        public int? Denom { get; set; }

        public long? EANNumber { get; set; }

        public long? EAN_UPC { get; set; }

        public string ProfileImage { get; set; }

        [MaxLength(5)]
        public string CountryCode { get; set; }

    }
}