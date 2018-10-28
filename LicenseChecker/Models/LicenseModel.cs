using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseChecker.Models
{
    public class LicenseModel
    {
        public string url { get; set; }
        public Guid? secret_key { get; set; }
        public Guid? license_key { get; set; }
        public string registered_domain { get; set; }
    }
}
