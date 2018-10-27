using LicenseChecker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseChecker
{
    public static class LicenseChecker
    {
        public static Response CheckLicenseStatus()
        {
            return new Response
            {
                Active = true
            };
        }
    }
}
