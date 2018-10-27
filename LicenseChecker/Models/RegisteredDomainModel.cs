namespace LicenseChecker.Models
{
    public class RegisteredDomainModel
    {
        public long id { get; set; }
        public long lic_key_id { get; set; }
        public string lic_key { get; set; }
        public string registered_domain { get; set; }
        public string item_reference { get; set; }
    }
}
