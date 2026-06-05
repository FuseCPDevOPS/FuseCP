// Copyright (C) 2025 FuseCP
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using FuseCP.Providers.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace FuseCP.EnterpriseServer
{
    public class CheckWebsitesSslTask : SchedulerTask
    {
        private const int ProgressLogInterval = 100;
        private int httpTimeoutSeconds = 15;
        private int requestAttempts = 2;
        private int requestRetryDelayMs = 250;

        private readonly string domainVariableKey = "[domain]";
        private readonly string urlVariableKey = "[url]";
        private readonly string issuerVariableKey = "[issuer]";
        private readonly string expiresInDaysVariableKey = "[expires_in_days]";
        private readonly string expiresOnDateVariableKey = "[expires_on_date]";
        private readonly string errorVariableKey = "[error]";

        private bool mailToCustomer;
        private bool sendBcc;
        private string bccMail;
        private string expirationMailSubject;
        private string expirationMailBody;
        private bool send30DaysBeforeExpiration;
        private bool send14DaysBeforeExpiration;
        private bool sendTodayExpired;
        private bool sendSslError;
        private string errorMailSubject;
        private string errorMailBody;
        private string mailFrom;

        public override void DoWork()
        {
            BackgroundTask topTask = TaskManager.TopTask;

            mailToCustomer = Convert.ToBoolean(topTask.GetParamValue("SEND_MAIL_TO_CUSTOMER"));
            sendBcc = Convert.ToBoolean(topTask.GetParamValue("SEND_BCC"));
            bccMail = (string)topTask.GetParamValue("BCC_MAIL");
            expirationMailSubject = (string)topTask.GetParamValue("EXPIRATION_MAIL_SUBJECT");
            expirationMailBody = (string)topTask.GetParamValue("EXPIRATION_MAIL_BODY");
            send30DaysBeforeExpiration = Convert.ToBoolean(topTask.GetParamValue("SEND_30_DAYS_BEFORE_EXPIRATION"));
            send14DaysBeforeExpiration = Convert.ToBoolean(topTask.GetParamValue("SEND_14_DAYS_BEFORE_EXPIRATION"));
            sendTodayExpired = Convert.ToBoolean(topTask.GetParamValue("SEND_TODAY_EXPIRED"));
            sendSslError = Convert.ToBoolean(topTask.GetParamValue("SEND_SSL_ERROR"));
            errorMailSubject = (string)topTask.GetParamValue("ERROR_MAIL_SUBJECT");
            errorMailBody = (string)topTask.GetParamValue("ERROR_MAIL_BODY");
            httpTimeoutSeconds = NormalizeInt(topTask.GetParamValue("SSL_REQUEST_TIMEOUT_SECONDS"), 15, 5, 300);
            requestAttempts = NormalizeInt(topTask.GetParamValue("SSL_REQUEST_ATTEMPTS"), 2, 1, 8);
            requestRetryDelayMs = NormalizeInt(topTask.GetParamValue("SSL_REQUEST_RETRY_DELAY_MS"), 250, 0, 5000);

            if (sendBcc && String.IsNullOrEmpty(bccMail))
            {
                TaskManager.WriteWarning("Specify 'BCC Mail To' task parameter");
                sendBcc = false;
            }
            if (!mailToCustomer && !sendBcc)
            {
                TaskManager.WriteWarning("Set 'Send Mail To Customer' or 'BCC Mail To' task parameter");
                return;
            }
            if (send30DaysBeforeExpiration || send14DaysBeforeExpiration || sendTodayExpired)
            {
                if (String.IsNullOrEmpty(expirationMailSubject))
                {
                    TaskManager.WriteWarning("Set 'Expiration Mail Subject' task parameter");
                    return;
                }
                if (String.IsNullOrEmpty(expirationMailBody))
                {
                    TaskManager.WriteWarning("Set 'Expiration Mail Body' task parameter");
                    return;
                }
            }
            if (sendSslError)
            {
                if (String.IsNullOrEmpty(errorMailSubject))
                {
                    TaskManager.WriteWarning("Set 'Error Mail Subject' task parameter");
                    return;
                }
                if (String.IsNullOrEmpty(errorMailBody))
                {
                    TaskManager.WriteWarning("Set 'Error Mail Body' task parameter");
                    return;
                }
            }

            SystemSettings settings = SystemController.GetSystemSettingsInternal(SystemSettings.SMTP_SETTINGS, false);
            if (settings != null)
            {
                mailFrom = settings["SmtpUsername"];
            }
            if (String.IsNullOrEmpty(mailFrom))
            {
                TaskManager.WriteWarning("You need to configure SMTP settings first");
                return;
            }

            int totalProcessed = 0;
            int totalErrors = 0;
            if (topTask.EffectiveUserId == 1)
            {
                DataSet serviceItems = PackageController.GetRawPackageItemsPaged(1, ResourceGroups.Web, typeof(WebSite), true, "ItemName", "%%", "", 0, Int32.MaxValue);
                CheckWebsites(serviceItems, ref totalProcessed, ref totalErrors);
            }
            else
            {
                foreach (var packageId in PackageController.GetMyPackages(topTask.EffectiveUserId).Select(p => p.PackageId))
                {
                    try
                    {
                        DataSet serviceItems = PackageController.GetRawPackageItemsPaged(packageId, ResourceGroups.Web, typeof(WebSite), true, "ItemName", "%%", "", 0, Int32.MaxValue);
                        CheckWebsites(serviceItems, ref totalProcessed, ref totalErrors);
                    }
                    catch (Exception ex) when (!(ex is OutOfMemoryException) && !(ex is StackOverflowException) && !(ex is AccessViolationException))
                    {
                        totalErrors++;
                        TaskManager.WriteError("SSL check failed while loading websites for package '{0}'. Error: {1}", packageId.ToString(), ex.ToString());
                    }
                }
            }

            TaskManager.Write("SSL check finished. Processed websites: {0}, certificate errors: {1}", totalProcessed.ToString(), totalErrors.ToString());
        }

        private void CheckWebsites(DataSet serviceItems, ref int totalProcessed, ref int totalErrors)
        {
            if (serviceItems == null) return;
            int recordsCount = (int)serviceItems.Tables[0].Rows[0][0];
            if (recordsCount == 0) return;
            DataView dvItems = serviceItems.Tables[1].DefaultView;
            foreach (DataRowView row in dvItems.Cast<DataRowView>().Where(r => typeof(WebSite).Equals(Type.GetType((string)r["TypeName"]))))
            {
                try
                {
                    string domain = (string)row["ItemName"];
                    if (String.IsNullOrEmpty(domain)) continue;
                    string url = "https://" + domain;
                    string email = (string)row["Email"];

                    var varList = new List<KeyValuePair<string, string>>();
                    varList.Add(new KeyValuePair<string, string>(domainVariableKey, domain));
                    varList.Add(new KeyValuePair<string, string>(urlVariableKey, url));

                    CheckCertificateResult certResult = GetServerCertificate(url, HttpMethod.Head);
                    X509Certificate2 cert = certResult.Certificate;
                    if (cert == null)
                    {
                        certResult = GetServerCertificate(url, HttpMethod.Get);
                        cert = certResult.Certificate;
                        if (cert == null)
                        {
                            if (!sendSslError) continue;
                            string errorMessage = String.IsNullOrWhiteSpace(certResult.ErrorMessage) ? "Unknown SSL connection error" : certResult.ErrorMessage;

                            varList.Add(new KeyValuePair<string, string>(errorVariableKey, errorMessage));
                            SendEmail(errorMailSubject, errorMailBody, email, varList);
                            totalErrors++;
                            totalProcessed++;

                            if (totalProcessed % ProgressLogInterval == 0)
                            {
                                TaskManager.Write("SSL check progress: processed {0} websites", totalProcessed.ToString());
                            }
                            continue;
                        }
                    }

                    string expirationDateString = cert.GetExpirationDateString();
                    string issuer = cert.Issuer;
                    DateTime expirationDate = DateTime.Parse(expirationDateString);
                    string expiresOnDate = expirationDate.ToString("yyyy-MM-dd");
                    DateTime current = DateTime.UtcNow.Date;
                    int expiresInDays = (expirationDate - current).Days;

                    varList.Add(new KeyValuePair<string, string>(issuerVariableKey, issuer));
                    varList.Add(new KeyValuePair<string, string>(expiresInDaysVariableKey, expiresInDays.ToString()));
                    varList.Add(new KeyValuePair<string, string>(expiresOnDateVariableKey, expiresOnDate));

                    if (send30DaysBeforeExpiration && expiresInDays == 30)
                    {
                        SendEmail(expirationMailSubject, expirationMailBody, email, varList);
                    }
                    if (send14DaysBeforeExpiration && expiresInDays == 14)
                    {
                        SendEmail(expirationMailSubject, expirationMailBody, email, varList);
                    }
                    if (sendTodayExpired && expiresInDays == 0)
                    {
                        SendEmail(expirationMailSubject, expirationMailBody, email, varList);
                    }

                    totalProcessed++;
                    if (totalProcessed % ProgressLogInterval == 0)
                    {
                        TaskManager.Write("SSL check progress: processed {0} websites", totalProcessed.ToString());
                    }
                }
                catch (Exception ex) when (!(ex is OutOfMemoryException) && !(ex is StackOverflowException) && !(ex is AccessViolationException))
                {
                    totalErrors++;
                    TaskManager.WriteError("SSL check failed for website '{0}'. Error: {1}", row["ItemName"].ToString(), ex.ToString());
                }
            }
        }

        private void SendEmail(string subject, string body, string customerEmail, List<KeyValuePair<string, string>> varList)
        {
            foreach (KeyValuePair<string, string> keyValuePair in varList)
            {
                body = body.Replace(keyValuePair.Key, keyValuePair.Value);
                subject = subject.Replace(keyValuePair.Key, keyValuePair.Value);
            }

            string mailTo = null;
            string bcc = null;

            if (mailToCustomer) mailTo = customerEmail;
            if (sendBcc && String.IsNullOrEmpty(mailTo)) mailTo = bccMail;
            if (sendBcc && !String.IsNullOrEmpty(mailTo)) bcc = bccMail;

            int res = MailHelper.SendMessage(mailFrom, mailTo, bcc, subject, body, true);
            if (res != 0) TaskManager.WriteError("SMTP Error. Code: " + res);
        }

        private async Task<CheckCertificateResult> GetServerCertificateAsync(string url, HttpMethod httpMethod)
        {
            X509Certificate2 certificate = null;
            HttpResponseMessage httpResponse = null;
            try
            {
                using var httpClientHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, error) =>
                    {
                        certificate = cert;
                        return true;
                    }
                };

                using var httpClient = new HttpClient(httpClientHandler)
                {
                    Timeout = TimeSpan.FromSeconds(httpTimeoutSeconds)
                };
                using var request = new HttpRequestMessage(httpMethod, url);
                httpResponse = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            }
            catch (Exception e) when (!(e is OutOfMemoryException) && !(e is StackOverflowException) && !(e is AccessViolationException))
            {
                string errorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                if (httpResponse != null) errorMessage += ", HTTP Response Code: " + httpResponse.StatusCode;
                return new CheckCertificateResult(certificate, errorMessage);
            }
            finally
            {
                httpResponse?.Dispose();
            }

            return new CheckCertificateResult(certificate, null);
        }

        private CheckCertificateResult GetServerCertificate(string url, HttpMethod httpMethod)
        {
            var retry = SchedulerTaskReliability.ExecuteWithRetry(
                () =>
                {
                    var response = GetServerCertificateAsync(url, httpMethod).GetAwaiter().GetResult();
                    if (response.Certificate == null && !string.IsNullOrWhiteSpace(response.ErrorMessage))
                    {
                        throw new Exception(response.ErrorMessage);
                    }

                    return response;
                },
                requestAttempts,
                requestRetryDelayMs,
                (attempt, ex, isTimeout) =>
                {
                    TaskManager.WriteError("SSL probe failed for '{0}' attempt {1} ({2}). Error: {3}",
                        url,
                        attempt.ToString(),
                        isTimeout ? "timeout" : "error",
                        ex.ToString());
                });

            if (retry.Success)
                return retry.Value;

            return new CheckCertificateResult(null, retry.LastException?.Message ?? "SSL probe failed");
        }

        private static int NormalizeInt(object rawValue, int defaultValue, int min, int max)
        {
            int parsed;
            if (!int.TryParse(Convert.ToString(rawValue), out parsed))
                parsed = defaultValue;

            if (parsed < min)
                parsed = min;
            if (parsed > max)
                parsed = max;

            return parsed;
        }

        private class CheckCertificateResult
        {
            private readonly X509Certificate2 certificate;
            private readonly string errorMessage;

            public X509Certificate2 Certificate {
                get
                {
                    return certificate;
                }
            }

            public string ErrorMessage
            {
                get
                {
                    return errorMessage;
                }
            }

            public CheckCertificateResult(X509Certificate2 certificate, string errorMessage)
            {
                this.certificate = certificate;
                this.errorMessage = errorMessage;
            }
        }
    }
}
