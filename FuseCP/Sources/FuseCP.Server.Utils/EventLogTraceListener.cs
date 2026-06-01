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
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
#if !NETSTANDARD2_0
using System.Runtime.Versioning;
#endif

namespace FuseCP.Server.Utils
{
#if !NETSTANDARD2_0
    [SupportedOSPlatform("windows")]
#endif
    public class EventLogTraceListener : TraceListener
    {
        private const int MaxEventLogMessageLength = 30000;
        private EventLog eventLog;
        private bool nameSet;

        public EventLog EventLog
        {
            get
            {
                return this.eventLog;
            }
            set
            {
                this.eventLog = value;
            }
        }

        public override string Name
        {
            get
            {
                if (!this.nameSet && (this.eventLog != null))
                {
                    this.nameSet = true;
                    base.Name = this.eventLog.Source;
                }
                return base.Name;
            }
            set
            {
                this.nameSet = true;
                base.Name = value;
            }
        }

        public EventLogTraceListener(EventLog eventLog)
            : base((eventLog != null) ? eventLog.Source : string.Empty)
        {
            this.eventLog = eventLog;
        }

        public EventLogTraceListener(string source)
        {
            try
            {
                if (!EventLog.SourceExists(source))
                {
                    EventLog.CreateEventSource(source, source);
                }

                this.eventLog = new EventLog();
                this.eventLog.Source = source;
                this.eventLog.ModifyOverflowPolicy(OverflowAction.OverwriteAsNeeded, 0);
            }
            catch (Exception ex) when (!(ex is OutOfMemoryException) && !(ex is StackOverflowException) && !(ex is AccessViolationException))
            {
                SafeDebugWrite("EventLogTraceListener init failed: " + ex.Message);
                this.eventLog = null;
            }
        }

        public EventLogTraceListener() : this("FuseCP") { }

        private EventInstance CreateEventInstance(TraceEventType severity, int id)
        {
            if (id > 0xffff)
            {
                id = 0xffff;
            }
            if (id < 0)
            {
                id = 0;
            }
            EventInstance instance = new EventInstance((long)id, 0);
            if ((severity == TraceEventType.Error) || (severity == TraceEventType.Critical))
            {
                instance.EntryType = EventLogEntryType.Error;
                return instance;
            }
            if (severity == TraceEventType.Warning)
            {
                instance.EntryType = EventLogEntryType.Warning;
                return instance;
            }
            instance.EntryType = EventLogEntryType.Information;
            return instance;
        }

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType severity, int id, params object[] data)
        {
            if ((base.Filter == null) || base.Filter.ShouldTrace(eventCache, source, severity, id, null, null, null, data))
            {
                EventInstance instance = this.CreateEventInstance(severity, id);
                StringBuilder builder = new StringBuilder();
                if (data != null)
                {
                    for (int num = 0; num < data.Length; num++)
                    {
                        if (num != 0)
                        {
                            builder.Append(", ");
                        }
                        if (data[num] != null)
                        {
                            builder.Append(data[num]);
                        }
                    }
                }
                WriteEventSafe(instance, builder.ToString());
            }
        }

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType severity, int id, object data)
        {
            if ((base.Filter == null) || base.Filter.ShouldTrace(eventCache, source, severity, id, null, null, data, null))
            {
                EventInstance instance = this.CreateEventInstance(severity, id);
                WriteEventSafe(instance, data);
            }
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType severity, int id, string message)
        {
            if ((base.Filter == null) || base.Filter.ShouldTrace(eventCache, source, severity, id, message, null, null, null))
            {
                EventInstance instance = this.CreateEventInstance(severity, id);
                WriteEventSafe(instance, message);
            }
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType severity, int id, string format, params object[] args)
        {
            if ((base.Filter == null) || base.Filter.ShouldTrace(eventCache, source, severity, id, format, args, null, null))
            {
                EventInstance instance1 = this.CreateEventInstance(severity, id);
                if (args == null)
                {
                    WriteEventSafe(instance1, format);
                }
                else if (string.IsNullOrEmpty(format))
                {
                    string[] textArray1 = new string[args.Length];
                    for (int num1 = 0; num1 < args.Length; num1++)
                    {
                        textArray1[num1] = args[num1].ToString();
                    }
                    WriteEventSafe(instance1, textArray1);
                }
                else
                {
                    WriteEventSafe(instance1, string.Format(CultureInfo.InvariantCulture, format, args));
                }
            }
        }

        public override void Write(string message)
        {
            WriteEntrySafe(message);
        }

        public override void WriteLine(string message)
        {
            this.Write(message);
        }

        public override void Close()
        {
            if (this.eventLog != null)
            {
                this.eventLog.Close();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Close();
            }
        }

        private void WriteEntrySafe(string message)
        {
            if (this.eventLog == null)
            {
                return;
            }

            try
            {
                this.eventLog.WriteEntry(TruncateMessage(message));
            }
            catch (Exception ex) when (!(ex is OutOfMemoryException) && !(ex is StackOverflowException) && !(ex is AccessViolationException))
            {
                SafeDebugWrite("EventLog write failed: " + ex.Message);
            }
        }

        private void WriteEventSafe(EventInstance instance, params object[] values)
        {
            if (this.eventLog == null)
            {
                return;
            }

            try
            {
                object[] sanitized = (values ?? Array.Empty<object>())
                    .Select(v => (object)TruncateMessage(v?.ToString()))
                    .ToArray();

                this.eventLog.WriteEvent(instance, sanitized);
            }
            catch (Exception ex) when (!(ex is OutOfMemoryException) && !(ex is StackOverflowException) && !(ex is AccessViolationException))
            {
                SafeDebugWrite("EventLog write failed: " + ex.Message);
            }
        }

        private static string TruncateMessage(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            return input.Length <= MaxEventLogMessageLength
                ? input
                : input.Substring(0, MaxEventLogMessageLength) + "... [truncated]";
        }

        private static void SafeDebugWrite(string message)
        {
            try
            {
                Debug.WriteLine(message);
                Console.Error.WriteLine(message);
            }
            catch
            {
                // Avoid surfacing logging backend failures.
            }
        }
    }
}
