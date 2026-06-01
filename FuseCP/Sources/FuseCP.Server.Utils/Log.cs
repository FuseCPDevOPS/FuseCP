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
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace FuseCP.Server.Utils
{
    /// <summary>
    /// Application log.
    /// </summary>
    public sealed class Log
    {
        private static readonly TraceSwitch logSeverity = new TraceSwitch("Log", "General trace switch");
        private const string GenericErrorMessage = "An error occurred. See server logs for details.";

        private static void TraceSwallowedException(Exception ex)
        {
            try
            {
                string text = ex == null
                    ? "Exception swallowed."
                    : "Exception swallowed: " + SanitizeLogText(ex.GetType().FullName);

                Debug.WriteLine(text);
                Console.Error.WriteLine(text);
            }
            catch
            {
                // Never throw from logging fallback paths.
            }
        }

        private Log()
        {
        }
        public static TraceLevel LogLevel
        {
            get => logSeverity.Level;
            set => logSeverity.Level = value;
        }

        /// <summary>
        /// Write error to the log.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="ex">Exception.</param>
        public static void WriteError(string message, Exception ex)
        {
            try
            {
                if (logSeverity.TraceError)
                {
                    StringBuilder txt = new StringBuilder();
                    txt.Append("[");
                    txt.Append(DateTime.Now.ToString("G", CultureInfo.InvariantCulture));
                    txt.Append("] ERROR: ");
                    txt.AppendLine(FormatIncomingMessage(message, "ERROR"));

                    Exception current = ex;
                    while (current != null)
                    {
                        txt.AppendLine("[" + current.GetType().FullName + "] " + SanitizeLogText(current.Message));
                        if (!String.IsNullOrWhiteSpace(current.StackTrace))
                        {
                            txt.AppendLine(SanitizeLogText(current.StackTrace));
                        }

                        current = current.InnerException;
                        if (current != null)
                        {
                            txt.AppendLine("Inner Exception:");
                        }
                    }

                    Trace.TraceError(txt.ToString());
                }
            }
            catch (Exception swallowedEx) when (!(swallowedEx is OutOfMemoryException) && !(swallowedEx is StackOverflowException) && !(swallowedEx is AccessViolationException)) { TraceSwallowedException(swallowedEx); }
        }

        /// <summary>
        /// Write error to the log.
        /// </summary>
        /// <param name="ex">Exception.</param>
        public static void WriteError(Exception ex)
        {

            try
            {
                if (ex != null)
                {
                    WriteError(GenericErrorMessage, ex);
                }
            }
            catch (Exception swallowedEx) when (!(swallowedEx is OutOfMemoryException) && !(swallowedEx is StackOverflowException) && !(swallowedEx is AccessViolationException)) { TraceSwallowedException(swallowedEx); }
        }

        /// <summary>
        /// Write info message to log
        /// </summary>
        /// <param name="message"></param>
        public static void WriteInfo(string message, params object[] args)
        {
            try
            {
                if (logSeverity.TraceInfo)
                {
                    Trace.TraceInformation(FormatIncomingMessage(message, "INFO", args));
                }
            }
            catch (Exception swallowedEx) when (!(swallowedEx is OutOfMemoryException) && !(swallowedEx is StackOverflowException) && !(swallowedEx is AccessViolationException)) { TraceSwallowedException(swallowedEx); }
        }

        /// <summary>
        /// Write info message to log
        /// </summary>
        /// <param name="message"></param>
        public static void WriteWarning(string message, params object[] args)
        {
            try
            {
                if (logSeverity.TraceWarning)
                {
                    System.Diagnostics.Trace.TraceWarning(FormatIncomingMessage(message, "WARNING", args));
                }
            }
            catch (Exception swallowedEx) when (!(swallowedEx is OutOfMemoryException) && !(swallowedEx is StackOverflowException) && !(swallowedEx is AccessViolationException)) { TraceSwallowedException(swallowedEx); }
        }

        /// <summary>
        /// Write start message to log
        /// </summary>
        /// <param name="message"></param>
        public static void WriteStart(string message, params object[] args)
        {
            try
            {
                if (logSeverity.TraceInfo)
                {
                    Trace.TraceInformation(FormatIncomingMessage(message, "START", args));
                }
            }
            catch (Exception swallowedEx) when (!(swallowedEx is OutOfMemoryException) && !(swallowedEx is StackOverflowException) && !(swallowedEx is AccessViolationException)) { TraceSwallowedException(swallowedEx); }
        }

        /// <summary>
        /// Write end message to log
        /// </summary>
        /// <param name="message"></param>
        public static void WriteEnd(string message, params object[] args)
        {
            try
            {
                if (logSeverity.TraceInfo)
                {
                    Trace.TraceInformation(FormatIncomingMessage(message, "END", args));
                }
            }
            catch (Exception swallowedEx) when (!(swallowedEx is OutOfMemoryException) && !(swallowedEx is StackOverflowException) && !(swallowedEx is AccessViolationException)) { TraceSwallowedException(swallowedEx); }
        }

        private static string FormatIncomingMessage(string message, string tag, params object[] args)
        {
            string messageTemplate = String.IsNullOrEmpty(message) ? String.Empty : message;
            object[] sanitizedArgs = (args != null && args.Length > 0) ? SanitizeLogArguments(args) : Array.Empty<object>();

            string formattedMessage;
            if (sanitizedArgs.Length == 0)
            {
                formattedMessage = messageTemplate;
            }
            else
            {
                try
                {
                    formattedMessage = string.Format(CultureInfo.InvariantCulture, messageTemplate, sanitizedArgs);
                }
                catch (FormatException)
                {
                    formattedMessage = messageTemplate + " | args=" + string.Join(", ", sanitizedArgs.Select(a => a?.ToString() ?? "null"));
                }
            }

            return "[" + DateTime.Now.ToString("G", CultureInfo.InvariantCulture) + "] " + tag + ": " + SanitizeLogText(formattedMessage);
        }

        private static string SanitizeLogText(string input)
        {
            if (String.IsNullOrEmpty(input))
            {
                return input;
            }

            return input.Replace("\r", String.Empty).Replace("\n", " ");
        }

        private static object[] SanitizeLogArguments(object[] args)
        {
            var sanitized = new object[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                sanitized[i] = SanitizeLogArgument(args[i]);
            }

            return sanitized;
        }

        private static object SanitizeLogArgument(object value)
        {
            if (value == null)
            {
                return null;
            }

            if (value is string s)
            {
                return SanitizeLogText(s);
            }

            if (value is bool || value is byte || value is sbyte || value is short || value is ushort ||
                value is int || value is uint || value is long || value is ulong || value is float ||
                value is double || value is decimal || value is char || value is Enum || value is DateTime ||
                value is DateTimeOffset || value is TimeSpan || value is Guid)
            {
                return value;
            }

            string text = value.ToString();
            return SanitizeLogText(text);
        }


    }
}


