using System;
using System.Net.Http;
using System.Threading.Tasks;
using LicenseChecker.Models;
using NUnit.Framework;

namespace LicenseChecker.UnitTests
{
    [TestFixture]
    public class LicenseCheckerIntegrationTests
    {
        private string _url = "http://fbtools.shop/";
        private string _key = "1003-vr9Wh1558903199";
        private string _bot = "SendPrivateMessageX";
        private LicenseModel _licenseModel;

        [SetUp]
        public void CreateHttpTest()
        {
            _licenseModel = new LicenseModel
            {
                Url = _url,
                Bot = _bot,
                Key = _key
            };
        }

        [Test]
        public async Task Check_Active_License()
        {
            // act
            var result = await LicenseChecker.CheckLicenseStatus(_licenseModel);

            // assert
            Assert.IsTrue(result.Active);
        }
    }
}
