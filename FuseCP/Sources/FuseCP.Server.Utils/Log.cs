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
using System.Text.RegularExpressions;
using System.Text;

namespace FuseCP.Server.Utils
{
    /// <summary>
    /// Application log.
    /// </summary>
    public sealed class Log
    {
        private static readonly TraceSwitch logSeverity = new TraceSwitch("Log", "General trace switch");
        private static readonly Regex SensitivePairRegex = new Regex("(?i)(password|pwd|token|apikey|secret|connectionstring)\\s*[=:]\\s*([^;\\s]+)", RegexOptions.Compiled);
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
                    txt.Append($"[{DateTime.Now:G}] ERROR: ");
                    txt.AppendLine(SanitizeLogText(message));
                    while (ex != null) {
                        txt.AppendLine(SanitizeLogText(ex.ToString()));
                        ex = ex.InnerException;
                        if (ex != null)
                        {
                            txt.AppendLine("Inner Exception:");
                        }
                    }
                    Trace.TraceError(txt.ToString());
                }
            }
            catch (Exception swallowedEx) when (!(swallowedEx is OutOfMemoryException) && !(swallowedEx is StackOverflowException) && !(swallowedEx is AccessViolationException)) { System.Diagnostics.Trace.TraceWarning("Exception swallowed: " + swallowedEx.Message); }
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
                    WriteError(ex.Message, ex);
                }
            }
            catch (Exception swallowedEx) when (!(swallowedEx is OutOfMemoryException) && !(swallowedEx is StackOverflowException) && !(swallowedEx is AccessViolationException)) { System.Diagnostics.Trace.TraceWarning("Exception swallowed: " + swallowedEx.Message); }
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
            catch (Exception swallowedEx) when (!(swallowedEx is OutOfMemoryException) && !(swallowedEx is StackOverflowException) && !(swallowedEx is AccessViolationException)) { System.Diagnostics.Trace.TraceWarning("Exception swallowed: " + swallowedEx.Message); }
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
            catch (Exception swallowedEx) when (!(swallowedEx is OutOfMemoryException) && !(swallowedEx is StackOverflowException) && !(swallowedEx is AccessViolationException)) { System.Diagnostics.Trace.TraceWarning("Exception swallowed: " + swallowedEx.Message); }
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
            catch (Exception swallowedEx) when (!(swallowedEx is OutOfMemoryException) && !(swallowedEx is StackOverflowException) && !(swallowedEx is AccessViolationException)) { System.Diagnostics.Trace.TraceWarning("Exception swallowed: " + swallowedEx.Message); }
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
            catch (Exception swallowedEx) when (!(swallowedEx is OutOfMemoryException) && !(swallowedEx is StackOverflowException) && !(swallowedEx is AccessViolationException)) { System.Diagnostics.Trace.TraceWarning("Exception swallowed: " + swallowedEx.Message); }
        }

        private static string FormatIncomingMessage(string message, string tag, params object[] args)
        {
            if (message == null)
            {
                message = String.Empty;
            }

            if (args != null && args.Length > 0)
            {
                try
                {
                    message = String.Format(message, args);
                }
                catch (FormatException)
                {
                    message = message + " | args=" + String.Join(", ", args);
                }
            }

            return String.Concat(String.Format("[{0:G}] {1}: ", DateTime.Now, tag), SanitizeLogText(message));
        }

        private static string SanitizeLogText(string input)
        {
            if (String.IsNullOrEmpty(input))
            {
                return input;
            }

            string sanitized = input.Replace("\r", String.Empty).Replace("\n", " ");
            return SensitivePairRegex.Replace(sanitized, "$1=[REDACTED]");
        }


    }
}


