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
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using FuseCP.Server.Utils;

namespace FuseCP.Providers.HostedSolution
{
    /// <summary>
    /// Exchange Log Helper Methods
    /// </summary>
    public static class ExchangeLog
    {
        public static string LogPrefix = "Exchange";

        public static void LogStart(string message, params object[] args)
        {
            string text = String.Format(message, args);
            Log.WriteStart("{0} {1}", LogPrefix, text);
        }

        public static void LogEnd(string message, params object[] args)
        {
            string text = String.Format(message, args);
            Log.WriteEnd("{0} {1}", LogPrefix, text);
        }

        public static void LogInfo(string message, params object[] args)
        {
            string text = String.Format(message, args);
            Log.WriteInfo("{0} {1}", LogPrefix, text);
        }

        public static void LogWarning(string message, params object[] args)
        {
            string text = String.Format(message, args);
            Log.WriteWarning("{0} {1}", LogPrefix, text);
        }

        public static void LogError(Exception ex)
        {
            Log.WriteError(LogPrefix, ex);
        }

        public static void LogError(string message, Exception ex)
        {
            string text = String.Format("{0} {1}", LogPrefix, message);
            Log.WriteError(text, ex);
        }

        public static void DebugInfo(string message, params object[] args)
        {
            string text = String.Format(message, args);
            Log.WriteInfo("{0} {1}", LogPrefix, text);
        }

        public static void DebugCommand(Command cmd)
        {
            StringBuilder sb = new StringBuilder(cmd.CommandText);
            foreach (CommandParameter parameter in cmd.Parameters)
            {
                string formatString = " -{0} {1}";
                if (parameter.Value is string) formatString = " -{0} '{1}'";
                else if (parameter.Value is SwitchParameter) formatString = " -{0}:${1}";
                else if (parameter.Value is bool) formatString = " -{0} ${1}";
                object valueToLog = parameter.Value;
                if (IsSensitiveParameterName(parameter.Name) || ContainsEmailAddress(parameter.Value))
                    valueToLog = "********";
                sb.AppendFormat(formatString, parameter.Name, valueToLog);
            }
            Log.WriteInfo("{0} {1}", LogPrefix, sb.ToString());
        }

        private static bool IsSensitiveParameterName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            string lowered = name.ToLowerInvariant();
            return lowered.Contains("password") || lowered.Contains("passwd") || lowered.Contains("secret") || lowered.Contains("token")
                || lowered.Contains("email") || lowered.Contains("smtp");
        }

        private static bool ContainsEmailAddress(object value)
        {
            if (value is string text)
                return LooksLikeEmailAddress(text);

            if (value is System.Collections.IEnumerable enumerable)
            {
                foreach (object item in enumerable)
                {
                    if (item is string itemText && LooksLikeEmailAddress(itemText))
                        return true;
                }
            }

            return false;
        }

        private static bool LooksLikeEmailAddress(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            if (value.Contains(' ') || value.IndexOf('@') <= 0)
                return false;

            return value.LastIndexOf('.') > value.IndexOf('@');
        }
    }
}
