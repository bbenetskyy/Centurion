using Flurl.Http;
using LicenseChecker.Models;
using LicenseChecker.Models.Enums;
using LicenseChecker.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
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
                    var response = 
                        await licenseModel.url
                            .WithClient(flurlClient)
                            .PostUrlEncodedAsync(new RequestModel
                            {
                                secret_key = licenseModel.secret_key,
                                license_key = licenseModel.license_key,
                                slm_action = Models.Enums.RequestActionEnum.check,
                                registered_domain = licenseModel.registered_domain
                            })
                            .ReceiveJson<dynamic>() as JObject;
                    if (response.GetValue("error_code") != null)
                    {
                        errorMessage = response.GetValue("message").Value<string>();
                    }
                    else
                    {
                        var status = (ActionEnum)Enum.Parse(typeof(ActionEnum), 
                            response.GetValue("status").Value<string>());

                        if (status == ActionEnum.pending)
                        {
                            response =
                                await licenseModel.url
                                    .WithClient(flurlClient)
                                    .PostUrlEncodedAsync(new RequestModel
                                    {
                                        secret_key = licenseModel.secret_key,
                                        license_key = licenseModel.license_key,
                                        slm_action = Models.Enums.RequestActionEnum.active,
                                        registered_domain = licenseModel.registered_domain
                                    })
                                    .ReceiveJson<dynamic>() as JObject;
                            if (response.GetValue("error_code") != null)
                            {
                                errorMessage = response.GetValue("message").Value<string>();
                            }
                        }
                        else if (status == ActionEnum.blocked)
                        {
                            errorMessage = "License have Blocked Status";
                        }
                        else if (status == ActionEnum.expired)
                        {
                            errorMessage = "License have Expired Status";
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
