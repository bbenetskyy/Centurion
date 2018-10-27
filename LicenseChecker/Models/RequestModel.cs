using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LicenseChecker.Models.Enums;

namespace LicenseChecker.Models
{
    public class RequestModel
    {
        public Guid secret_key { get; set; }
        public RequestActionEnum slm_action { get; set; }
        public Guid license_key { get; set; }
        public string registered_domain { get; set; }
    }
}
