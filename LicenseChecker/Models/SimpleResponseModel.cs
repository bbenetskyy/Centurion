namespace LicenseChecker.Models
{
    public class SimpleResponseModel
    {
        public string result { get; set; }
        public string message { get; set; }
        public int? error_code { get; set; }
    }
}
