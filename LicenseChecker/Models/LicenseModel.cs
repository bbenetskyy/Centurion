using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseChecker.Models
{
    public class LicenseModel
    {
        public string Response { get; set; }
        public string Url { get; set; }
        public string Bot { get; set; }
        public string Key { get; set; }

        public LicenseModel()
        {
            Response = "json";
        }
    }
}
