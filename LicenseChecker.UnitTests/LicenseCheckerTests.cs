using System;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl.Http.Testing;
using LicenseChecker.Models;
using NUnit.Framework;

namespace LicenseChecker.UnitTests
{
    [TestFixture]
    public class LicenseCheckerTests
    {
        private HttpTest _httpTest;

        [SetUp]
        public void CreateHttpTest()
        {
            _httpTest = new HttpTest();
        }

        [TearDown]
        public void DisposeHttpTest()
        {
            _httpTest.Dispose();
        }

        [Test]
        public async Task Check_Active_License()
        {
            // arrange
            var url = "http://api.mysite.com/*";
            var licenseModel = new LicenseModel
            {
                url = url
            };

            _httpTest.RespondWith(@"{
	            	'result': 'success',
                    'message': 'License key details retrieved.',
	            	'license_key': '5bbce86b1fa62',
	            	'status': 'active'
	            }", 200);

            // act
            var result = await LicenseChecker.CheckLicenseStatus(licenseModel);

            // assert
            _httpTest.ShouldHaveCalled(url)
                .WithVerb(HttpMethod.Post);
            Assert.IsTrue(result.Active);
        }

        [Test]
        public async Task Check_Pending_And_Active_License()
        {
            // arrange
            var url = "http://api.mysite.com/*";
            var licenseModel = new LicenseModel
            {
                url = url
            };

            _httpTest.RespondWith(@"{
	            	'result': 'success',
                    'message': 'License key details retrieved.',
	            	'license_key': '5bbce86b1fa62',
	            	'status': 'pending'
	            }", 200)
                .RespondWith(@"{
	            	'result': 'success',
                    'message': 'License key details retrieved.',
	            	'license_key': '5bbce86b1fa62',
	            	'status': 'active'
	            }", 200);

            // act
            var result = await LicenseChecker.CheckLicenseStatus(licenseModel);

            // assert
            _httpTest.ShouldHaveCalled(url)
                .WithVerb(HttpMethod.Post);
            Assert.IsTrue(result.Active);
        }

        [Test]
        public async Task Check_Pending_without_Active()
        {
            // arrange
            var url = "http://api.mysite.com/*";
            var licenseModel = new LicenseModel
            {
                url = url
            };

            _httpTest.RespondWith(@"{
	            	'result': 'success',
                    'message': 'License key details retrieved.',
	            	'license_key': '5bbce86b1fa62',
	            	'status': 'pending'
	            }", 200)
                .RespondWith(@"{
                    'result': 'error',
                    'message': 'Reached maximum activation. License key already in use on pc-mac-addr',
                    'error_code': 110
                }", 200);
            // act
            var result = await LicenseChecker.CheckLicenseStatus(licenseModel);

            // assert
            _httpTest.ShouldHaveCalled(url)
                .WithVerb(HttpMethod.Post);
            Assert.IsFalse(result.Active);
            Assert.AreEqual(result.ErrorMessage, 
                "Reached maximum activation. License key already in use on pc-mac-addr");
        }

        [Test]
        public async Task Check_Blocked_License()
        {
            // arrange
            var url = "http://api.mysite.com/*";
            var licenseModel = new LicenseModel
            {
                url = url
            };

            _httpTest.RespondWith(@"{
	            	'result': 'success',
                    'message': 'License key details retrieved.',
	            	'license_key': '5bbce86b1fa62',
	            	'status': 'blocked'
	            }", 200);

            // act
            var result = await LicenseChecker.CheckLicenseStatus(licenseModel);

            // assert
            _httpTest.ShouldHaveCalled(url)
                .WithVerb(HttpMethod.Post);
            Assert.IsFalse(result.Active);
            Assert.AreEqual(result.ErrorMessage,
                "License have Blocked Status");
        }

        [Test]
        public async Task Check_Expired_License()
        {
            // arrange
            var url = "http://api.mysite.com/*";
            var licenseModel = new LicenseModel
            {
                url = url
            };

            _httpTest.RespondWith(@"{
	            	'result': 'success',
                    'message': 'License key details retrieved.',
	            	'license_key': '5bbce86b1fa62',
	            	'status': 'expired'
	            }", 200);

            // act
            var result = await LicenseChecker.CheckLicenseStatus(licenseModel);

            // assert
            _httpTest.ShouldHaveCalled(url)
                .WithVerb(HttpMethod.Post);
            Assert.IsFalse(result.Active);
            Assert.AreEqual(result.ErrorMessage,
                "License have Expired Status");
        }
    }
}
