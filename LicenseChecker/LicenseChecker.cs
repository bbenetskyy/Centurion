using Flurl.Http;
using LicenseChecker.Models;
using LicenseChecker.Models.Enums;
using LicenseChecker.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace LicenseChecker
{
    public static class LicenseChecker
    {
        public static async Task<Response> CheckLicenseStatus(LicenseModel licenseModel)
        {
            string errorMessage = null;
            using (var flurlClient = new FlurlClient())
            {
                try
                {
                    var response = await
                         licenseModel.Url
                            .WithClient(flurlClient)
                            .SetQueryParams(new
                            {
                                tmpl = licenseModel.Response,
                                key = licenseModel.Key,
                                bot = licenseModel.Bot,
                                upc = WindowsIdentity.GetCurrent().Name
                            })
                            .GetJsonAsync<dynamic>() as JObject;
                    if (response.GetValue("error") != null)
                    {
                        errorMessage = response.GetValue("error").Value<string>();
                    }
                    else
                    {
                        var active = response.GetValue("active").Value<bool>();
                        var expiredDate = response.GetValue("expire").Value<DateTime>();
                        if (expiredDate <= DateTime.Now)
                        {
                            errorMessage = "License have Expired Status";
                        }
                        else if (!active)
                        {
                            errorMessage = "License does not have Actie Status";
                        }
                    }
                }
                catch (FlurlHttpException ex)
                {
                    errorMessage =
                        $"Failed with the http request to get the tokwn using flurl.\n" +
                        $"Exception: {ex.Message}\n" +
                        $"HttpStatusCode: {ex.Call.HttpStatus}\n" +
                        $"Message: {await ex.GetResponseStringAsync()}";
                    //TODO: move it separate recursive function
                    if (ex.InnerException != null)
                    {
                        errorMessage += $"Inner Exception: {ex.InnerException.Message} ({ex.InnerException.GetType().Name})";
                        if (ex.InnerException.InnerException != null)
                            errorMessage += $"Inner Inner Exception: {ex.InnerException.InnerException.Message} ({ex.InnerException.InnerException.GetType().Name})";
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = $"Exception: {ex.Message} ({ex.GetType().Name})";
                    //TODO: move it separate recursive function
                    if (ex.InnerException != null)
                    {
                        errorMessage += $"Inner Exception: {ex.InnerException.Message} ({ex.InnerException.GetType().Name})";
                        if (ex.InnerException.InnerException != null)
                            errorMessage += $"Inner Inner Exception: {ex.InnerException.InnerException.Message} ({ex.InnerException.InnerException.GetType().Name})";
                    }
                }
            }
            return new Response
            {
                Active = errorMessage == null,
                ErrorMessage = errorMessage
            };
        }
    }
}
