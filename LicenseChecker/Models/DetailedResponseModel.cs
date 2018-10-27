using System;
using LicenseChecker.Models.Enums;

namespace LicenseChecker.Models
{
    public class DetailedResponseModel
    {
        public string result { get; set; }
        public string message { get; set; }
        public string license_key { get; set; }
        public ActionEnum status { get; set; }
        public int max_allowed_domains { get; set; }
        public string email { get; set; }
        public RegisteredDomainModel[] registered_domains { get; set; }
        public DateTime date_created { get; set; }
        public DateTime date_renewed { get; set; }
        public DateTime date_expiry { get; set; }
        public string product_ref { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string company_name { get; set; }
        public long txn_id { get; set; }
    }
}
