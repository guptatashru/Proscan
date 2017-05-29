using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SyngentaProscan.DataObjects
{
    public class SyngentaUser
    {
        public string Role { get; set; }

        public string RetailerId { get; set; }

        public string AgentId { get; set; }

        public string Email { get; set; }
    }
}