// Copyright (C) 2026 FuseCP
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

namespace FuseCP.EnterpriseServer
{
    internal static class SchedulerTaskReliability
    {
        internal sealed class RetryResult<T>
        {
            public bool Success { get; set; }
            public T Value { get; set; }
            public Exception LastException { get; set; }
            public bool LastWasTimeout { get; set; }
            public int TimeoutFailures { get; set; }
            public int ErrorFailures { get; set; }
        }

        public static RetryResult<T> ExecuteWithRetry<T>(
            Func<T> action,
            int attempts,
            int baseDelayMs,
            Action<int, Exception, bool> onAttemptFailure)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (attempts < 1)
                attempts = 1;

            if (baseDelayMs < 0)
                baseDelayMs = 0;

            var result = new RetryResult<T>();

            for (int attempt = 1; attempt <= attempts; attempt++)
            {
                try
                {
                    result.Value = action();
                    result.Success = true;
                    return result;
                }
                catch (Exception ex) when (!(ex is OutOfMemoryException) && !(ex is StackOverflowException) && !(ex is AccessViolationException))
                {
                    bool isTimeout = IsTimeout(ex);
                    result.LastException = ex;
                    result.LastWasTimeout = isTimeout;
                    if (isTimeout)
                        result.TimeoutFailures++;
                    else
                        result.ErrorFailures++;

                    onAttemptFailure?.Invoke(attempt, ex, isTimeout);

                    if (attempt < attempts)
                    {
                        int delay = baseDelayMs * (1 << (attempt - 1));
                        if (delay > 0)
                            Thread.Sleep(Math.Min(delay, 10000));
                    }
                }
            }

            return result;
        }

        public static bool IsTimeout(Exception ex)
        {
            if (ex == null)
                return false;

            if (ex is TimeoutException || ex is TaskCanceledException || ex is OperationCanceledException)
                return true;

            if (ex is WebException webEx && webEx.Status == WebExceptionStatus.Timeout)
                return true;

            string message = ex.Message ?? string.Empty;
            if (message.IndexOf("timeout", StringComparison.OrdinalIgnoreCase) >= 0 ||
                message.IndexOf("timed out", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }

            return IsTimeout(ex.InnerException);
        }
    }
}
