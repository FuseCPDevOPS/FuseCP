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

using System;
using System.Collections.Generic;
using System.Text;
using MimeKit;
using MailKit;
using MailKit.Security;
using MailKit.Net.Smtp;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Authentication;
using System.IO;

namespace FuseCP.EnterpriseServer
{
    public class MailHelper
    {
        // Total budget for a single SMTP send (connect + auth + data). An unreachable
        // server must fail fast instead of stalling callers such as MFA login.
        private const int SmtpSendTimeoutMs = 10000;

        public static async Task<int> SendMessageAsync(string from, string to, string bcc, string subject, string body,
            MimeKit.MessagePriority priority, bool isHtml, IEnumerable<MimePart> attachments)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(SmtpSendTimeoutMs));

            // MailKit's ConnectAsync can block inside the native connect and ignore the
            // cancellation token, so "await ConnectAsync(..., token)" may never complete and
            // stall callers such as MFA login. Run the send on a dedicated thread and race it
            // against a hard wall-clock budget so the request thread is guaranteed to unblock
            // within SmtpSendTimeoutMs even if the SMTP operation cannot be cancelled.
            var sendTask = Task.Run(() => SendSmtpMessage(from, to, bcc, subject, body, priority, isHtml, attachments, cts.Token));
            var completed = await Task.WhenAny(sendTask, Task.Delay(SmtpSendTimeoutMs));

            if (completed != sendTask)
            {
                cts.Cancel();
                return BusinessErrorCodes.SMTP_UNKNOWN_ERROR;
            }

            return await sendTask;
        }

        private static int SendSmtpMessage(string from, string to, string bcc, string subject, string body,
            MimeKit.MessagePriority priority, bool isHtml, IEnumerable<MimePart> attachments, CancellationToken cancellationToken)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    SystemSettings settings;

					using (var systemController = new SystemController()) {
                        settings = systemController.GetSystemSettingsInternal(
                           SystemSettings.SMTP_SETTINGS,
                           true
                       );
                    }
                    string smtpServer = settings["SmtpServer"];
                    int smtpPort = settings.GetInt("SmtpPort");
                    string smtpUsername = settings["SmtpUsername"];
                    string smtpPassword = settings["SmtpPassword"];
                    bool enableSsl = Utils.ParseBool(settings["SmtpEnableSsl"], false);
                    bool enableLegacySSL = Utils.ParseBool(settings["SmtpEnableLegacySSL"], false);

                    // Determine SecureSocketOptions based on EnableSsl and Port
                    // Common logic: Port 465 usually implies SslOnConnect, others often use StartTls. Adjust as needed.
                    SecureSocketOptions secureSocketOptions = SecureSocketOptions.Auto;
                    if (enableSsl)
                    {
                        secureSocketOptions = smtpPort == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;
                    }

                    client.SslProtocols = enableLegacySSL ? SslProtocols.None : SslProtocols.Tls12 | SslProtocols.Tls13;
                    client.Timeout = SmtpSendTimeoutMs;








                    // Connect
                    client.Connect(smtpServer, smtpPort, secureSocketOptions, cancellationToken);

                    // Authenticate if username is provided
                    if (!String.IsNullOrEmpty(smtpUsername))
                    {
                        client.Authenticate(smtpUsername, smtpPassword, cancellationToken);
                    }

                    using var message = new MimeMessage();
                    message.From.Add(MailboxAddress.Parse(from));
                    message.To.Add(MailboxAddress.Parse(to));
                    if (!String.IsNullOrEmpty(bcc))
                    {
                        message.Bcc.Add(MailboxAddress.Parse(bcc));
                    }
                    message.Subject = subject;
                    message.Priority = priority;

                    // Create body
                    var bodyBuilder = new BodyBuilder();
                    if (isHtml)
                    {
                        bodyBuilder.HtmlBody = body;
                    }
                    else
                    {
                        bodyBuilder.TextBody = body;
                    }

                    // Add attachments if any
                    if (attachments != null) //
                    {
                        foreach (var attachment in attachments) // Now iterating MimePart objects
                        {
                            if (attachment.IsAttachment) // Check if it's meant as an attachment or inline
                            {
                                bodyBuilder.Attachments.Add(attachment); // Add to regular attachments
                            }
                            else
                            {
                                // If it's inline (ContentDisposition.Inline), add to LinkedResources
                                // This assumes the HTML body references it via cid:ContentId
                                bodyBuilder.LinkedResources.Add(attachment); //
                            }
                        }
                    }

                    message.Body = bodyBuilder.ToMessageBody();


                    // Send messages
                    client.Send(message, cancellationToken);

                    return 0;
                }
                catch (OperationCanceledException)
                {
                    return BusinessErrorCodes.SMTP_UNKNOWN_ERROR;
                }
                catch (SmtpCommandException)
                {
                    return BusinessErrorCodes.SMTP_GENERAL_FAILURE;
                }
                catch (MailKit.Security.AuthenticationException)
                {
                    return BusinessErrorCodes.SMTP_CLIENT_NOT_PERMITTED;
                }
                catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                {
                    return BusinessErrorCodes.SMTP_UNKNOWN_ERROR;
                }
                finally
                {
                    // Disconnect (best effort: a failed teardown must not mask the send result)
                    if (client.IsConnected)
                    {
                        try
                        {
                            client.Disconnect(true, cancellationToken);
                        }
                        catch (System.Exception ex) when (ex is OperationCanceledException || ex is SmtpCommandException || ex is System.IO.IOException)
                        {
                            System.Diagnostics.Trace.TraceInformation("SMTP teardown failed (best effort, ignored): " + ex.GetType().Name);
                        }
                    }
                }
            }
        }

        public static int SendMessage(string from, string to, string bcc, string subject, string body,
           System.Net.Mail.MailPriority priority, bool isHtml, System.Net.Mail.Attachment[] attachments)
        {
            // Convert priority
            MimeKit.MessagePriority mailkitPriority;
            switch (priority)
            {
                case System.Net.Mail.MailPriority.High:
                    mailkitPriority = MessagePriority.Urgent;
                    break;
                case System.Net.Mail.MailPriority.Low:
                    mailkitPriority = MessagePriority.NonUrgent;
                    break;
                default:
                    mailkitPriority = MessagePriority.Normal;
                    break;
            }

            var mailkitAttachments = new List<MimePart>();
            if (attachments != null) //
            {
                foreach (var attachment in attachments) //
                {
                    using (attachment)
                    {
                        try
                        {
                            // Determine ContentType - MimeKit has better auto-detection but respecting original if possible
                            var contentType = MimeKit.ContentType.Parse(attachment.ContentType?.ToString() ?? "application/octet-stream"); //

                            // Create MimePart - primarily handles streams.
                            // System.Net.Mail.Attachment exposes its content via ContentStream.
                            var mimePart = new MimePart(contentType)
                            {
                                Content = new MimeContent(attachment.ContentStream), // Get stream
                                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment), // Mark as attachment
                                ContentTransferEncoding = attachment.TransferEncoding == System.Net.Mime.TransferEncoding.Base64 ? ContentEncoding.Base64 : ContentEncoding.Default, // Map encoding
                                FileName = attachment.Name // Set filename
                            };

                            // Handle ContentId if present (for inline attachments)
                            if (!string.IsNullOrEmpty(attachment.ContentId)) //
                            {
                                mimePart.ContentId = attachment.ContentId; //
                                                                           // Override ContentDisposition for inline
                                mimePart.ContentDisposition = new ContentDisposition(ContentDisposition.Inline); //
                            }

                            mailkitAttachments.Add(mimePart); //
                        }
                        catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                        {
                            // Log the error if a specific attachment fails conversion
                            Console.WriteLine($"Error converting attachment '{attachment.Name}': {ex.Message}"); //
                                                                                                                 // Decide whether to continue without this attachment or fail the whole send
                        }
                    }
                }
            }


            try
            {
                using var cts = new CancellationTokenSource(SmtpSendTimeoutMs);

                // Do NOT use sync-over-async here (SendMessageAsync().GetAwaiter().GetResult()).
                // The caller is a WebForms request thread; the async method's continuation is
                // scheduled through the request SynchronizationContext/thread pool, which can
                // never run while this thread is blocked on GetResult() - a permanent deadlock
                // that stalls MFA login indefinitely. Task.Wait(timeout) instead blocks directly
                // on the worker task's completion handle, so the send is hard-bounded by
                // SmtpSendTimeoutMs regardless of the surrounding synchronization context.
                var sendTask = Task.Run(() => SendSmtpMessage(from, to, bcc, subject, body, mailkitPriority, isHtml, mailkitAttachments, cts.Token));
                if (!sendTask.Wait(SmtpSendTimeoutMs))
                {
                    cts.Cancel();
                    return BusinessErrorCodes.SMTP_UNKNOWN_ERROR;
                }
                return sendTask.Result;
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                return BusinessErrorCodes.SMTP_UNKNOWN_ERROR;
            }
        }

        public static int SendMessage(string from, string to, string subject, string body, bool isHtml)
        {
            return SendMessage(from, to, null, subject, body, System.Net.Mail.MailPriority.Normal, isHtml, null);
        }

        public static int SendMessage(string from, string to, string bcc, string subject, string body, bool isHtml)
        {
            return SendMessage(from, to, bcc, subject, body, System.Net.Mail.MailPriority.Normal, isHtml, null);
        }

        public static int SendMessage(string from, string to, string bcc, string subject, string body,
            System.Net.Mail.MailPriority priority, bool isHtml)
        {
            return SendMessage(from, to, bcc, subject, body, priority, isHtml, null);
        }

    }
}



