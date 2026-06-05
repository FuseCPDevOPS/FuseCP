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
using System.Collections.Concurrent;
using System.Threading;

namespace FuseCP.EnterpriseServer
{
    internal static class SchedulerExecutionQueue
    {
        private const string DefaultAffinityKey = "global";
        private const string DefaultTenantKey = "tenant:global";
        private const string DefaultProviderKey = "provider:global";
        private const int DefaultMaxConcurrentExecutions = 8;
        private const int DefaultGlobalMaxConcurrentExecutions = 256;
        private const int DefaultTenantMaxConcurrentExecutions = 4;
        private const int DefaultProviderMaxConcurrentExecutions = 8;
        private const int MinConcurrentExecutions = 1;
        private const int MaxConcurrentExecutionsLimit = 1024;
        private const int MaxTaskWeight = 8;

        private enum QueueState
        {
            Queued = 1,
            Executing = 2,
            Cancelled = 3
        }

        private sealed class QueuedWorkItem
        {
            public int Key { get; }
            public string AffinityKey { get; }
            public string TenantKey { get; }
            public string ProviderKey { get; }
            public int Weight { get; }
            public Action Work { get; }

            public QueuedWorkItem(int key, string affinityKey, string tenantKey, string providerKey, int weight, Action work)
            {
                Key = key;
                AffinityKey = affinityKey;
                TenantKey = tenantKey;
                ProviderKey = providerKey;
                Weight = weight;
                Work = work;
            }
        }

        private static readonly ConcurrentQueue<QueuedWorkItem> WorkQueue = new ConcurrentQueue<QueuedWorkItem>();
        private static readonly ConcurrentDictionary<int, QueueState> States = new ConcurrentDictionary<int, QueueState>();
        private static readonly ConcurrentDictionary<string, int> ActiveExecutionUnitsByAffinity = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        private static readonly ConcurrentDictionary<string, int> ActiveExecutionUnitsByTenant = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        private static readonly ConcurrentDictionary<string, int> ActiveExecutionUnitsByProvider = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        private static int activeExecutions;
        private static int activeExecutionUnits;
        private static int dispatching;
        private static long deferralsGlobal;
        private static long deferralsAffinity;
        private static long deferralsTenant;
        private static long deferralsProvider;
        private static int maxConcurrentExecutions = DefaultMaxConcurrentExecutions;
        private static int maxGlobalConcurrentExecutions = DefaultGlobalMaxConcurrentExecutions;
        private static int maxTenantConcurrentExecutions = DefaultTenantMaxConcurrentExecutions;
        private static int maxProviderConcurrentExecutions = DefaultProviderMaxConcurrentExecutions;

        public static int MaxConcurrentExecutions => Volatile.Read(ref maxConcurrentExecutions);
        public static int MaxGlobalConcurrentExecutions => Volatile.Read(ref maxGlobalConcurrentExecutions);
        public static int MaxTenantConcurrentExecutions => Volatile.Read(ref maxTenantConcurrentExecutions);
        public static int MaxProviderConcurrentExecutions => Volatile.Read(ref maxProviderConcurrentExecutions);
        public static int ActiveExecutions => Volatile.Read(ref activeExecutions);
        public static int ActiveExecutionUnits => Volatile.Read(ref activeExecutionUnits);
        public static int QueuedExecutions => WorkQueue.Count;
        public static int ActiveTenantBuckets => ActiveExecutionUnitsByTenant.Count;
        public static int ActiveProviderBuckets => ActiveExecutionUnitsByProvider.Count;
        public static long DeferralsGlobal => Interlocked.Read(ref deferralsGlobal);
        public static long DeferralsAffinity => Interlocked.Read(ref deferralsAffinity);
        public static long DeferralsTenant => Interlocked.Read(ref deferralsTenant);
        public static long DeferralsProvider => Interlocked.Read(ref deferralsProvider);

        public static void ConfigureMaxConcurrentExecutions(int configuredValue)
        {
            int normalized = configuredValue;
            if (normalized < MinConcurrentExecutions)
                normalized = MinConcurrentExecutions;
            else if (normalized > MaxConcurrentExecutionsLimit)
                normalized = MaxConcurrentExecutionsLimit;

            Volatile.Write(ref maxConcurrentExecutions, normalized);
            Dispatch();
        }

        public static void ConfigureGlobalMaxConcurrentExecutions(int configuredValue)
        {
            int normalized = configuredValue;
            if (normalized < MinConcurrentExecutions)
                normalized = MinConcurrentExecutions;
            else if (normalized > MaxConcurrentExecutionsLimit)
                normalized = MaxConcurrentExecutionsLimit;

            Volatile.Write(ref maxGlobalConcurrentExecutions, normalized);
            Dispatch();
        }

        public static void ConfigureTenantMaxConcurrentExecutions(int configuredValue)
        {
            int normalized = configuredValue;
            if (normalized < MinConcurrentExecutions)
                normalized = MinConcurrentExecutions;
            else if (normalized > MaxConcurrentExecutionsLimit)
                normalized = MaxConcurrentExecutionsLimit;

            Volatile.Write(ref maxTenantConcurrentExecutions, normalized);
            Dispatch();
        }

        public static void ConfigureProviderMaxConcurrentExecutions(int configuredValue)
        {
            int normalized = configuredValue;
            if (normalized < MinConcurrentExecutions)
                normalized = MinConcurrentExecutions;
            else if (normalized > MaxConcurrentExecutionsLimit)
                normalized = MaxConcurrentExecutionsLimit;

            Volatile.Write(ref maxProviderConcurrentExecutions, normalized);
            Dispatch();
        }

        public static bool IsQueued(int key)
        {
            return States.TryGetValue(key, out QueueState state) && state == QueueState.Queued;
        }

        public static bool TryEnqueue(int key, Action work)
        {
            return TryEnqueue(key, DefaultAffinityKey, DefaultTenantKey, DefaultProviderKey, work, 1);
        }

        public static bool TryEnqueue(int key, Action work, int weight)
        {
            return TryEnqueue(key, DefaultAffinityKey, DefaultTenantKey, DefaultProviderKey, work, weight);
        }

        public static bool TryEnqueue(int key, string affinityKey, Action work, int weight)
        {
            return TryEnqueue(key, affinityKey, DefaultTenantKey, DefaultProviderKey, work, weight);
        }

        public static bool TryEnqueue(int key, string affinityKey, string tenantKey, string providerKey, Action work, int weight)
        {
            if (work == null)
                return false;

            if (!States.TryAdd(key, QueueState.Queued))
                return false;

            WorkQueue.Enqueue(new QueuedWorkItem(
                key,
                NormalizeAffinityKey(affinityKey),
                NormalizeTenantKey(tenantKey),
                NormalizeProviderKey(providerKey),
                NormalizeWeight(weight),
                work));
            Dispatch();
            return true;
        }

        public static bool TryCancel(int key)
        {
            return States.TryUpdate(key, QueueState.Cancelled, QueueState.Queued);
        }

        private static void Dispatch()
        {
            if (Interlocked.CompareExchange(ref dispatching, 1, 0) != 0)
                return;

            try
            {
                int maxConcurrentPerAffinity = Volatile.Read(ref maxConcurrentExecutions);
                int maxGlobalConcurrent = Volatile.Read(ref maxGlobalConcurrentExecutions);
                int maxTenantConcurrent = Volatile.Read(ref maxTenantConcurrentExecutions);
                int maxProviderConcurrent = Volatile.Read(ref maxProviderConcurrentExecutions);
                int scanBudget = WorkQueue.Count;

                while (scanBudget-- > 0 && Volatile.Read(ref activeExecutionUnits) < maxGlobalConcurrent && WorkQueue.TryDequeue(out QueuedWorkItem item))
                {
                    if (!States.TryGetValue(item.Key, out QueueState state) || state == QueueState.Cancelled)
                    {
                        States.TryRemove(item.Key, out _);
                        continue;
                    }

                    int affinityActiveUnits = GetAffinityActiveUnits(item.AffinityKey);
                    int tenantActiveUnits = GetTenantActiveUnits(item.TenantKey);
                    int providerActiveUnits = GetProviderActiveUnits(item.ProviderKey);
                    bool blockedByGlobal = Volatile.Read(ref activeExecutionUnits) + item.Weight > maxGlobalConcurrent;
                    bool blockedByAffinity = affinityActiveUnits + item.Weight > maxConcurrentPerAffinity;
                    bool blockedByTenant = tenantActiveUnits + item.Weight > maxTenantConcurrent;
                    bool blockedByProvider = providerActiveUnits + item.Weight > maxProviderConcurrent;

                    if (blockedByGlobal || blockedByAffinity || blockedByTenant || blockedByProvider)
                    {
                        if (blockedByGlobal)
                            Interlocked.Increment(ref deferralsGlobal);
                        if (blockedByAffinity)
                            Interlocked.Increment(ref deferralsAffinity);
                        if (blockedByTenant)
                            Interlocked.Increment(ref deferralsTenant);
                        if (blockedByProvider)
                            Interlocked.Increment(ref deferralsProvider);

                        WorkQueue.Enqueue(item);
                        continue;
                    }

                    if (!States.TryUpdate(item.Key, QueueState.Executing, QueueState.Queued))
                        continue;

                    Interlocked.Increment(ref activeExecutions);
                    Interlocked.Add(ref activeExecutionUnits, item.Weight);
                    AddAffinityActiveUnits(item.AffinityKey, item.Weight);
                    AddTenantActiveUnits(item.TenantKey, item.Weight);
                    AddProviderActiveUnits(item.ProviderKey, item.Weight);

                    Thread worker = new Thread(() => Execute(item))
                    {
                        IsBackground = true,
                        Priority = ThreadPriority.BelowNormal
                    };
                    worker.Start();
                }
            }
            finally
            {
                Volatile.Write(ref dispatching, 0);

                if (!WorkQueue.IsEmpty && Volatile.Read(ref activeExecutionUnits) < Volatile.Read(ref maxGlobalConcurrentExecutions))
                {
                    Dispatch();
                }
            }
        }

        private static void Execute(QueuedWorkItem item)
        {
            try
            {
                item.Work();
            }
            finally
            {
                States.TryRemove(item.Key, out _);
                Interlocked.Add(ref activeExecutionUnits, -item.Weight);
                AddAffinityActiveUnits(item.AffinityKey, -item.Weight);
                AddTenantActiveUnits(item.TenantKey, -item.Weight);
                AddProviderActiveUnits(item.ProviderKey, -item.Weight);
                Interlocked.Decrement(ref activeExecutions);
                Dispatch();
            }
        }

        private static int GetAffinityActiveUnits(string affinityKey)
        {
            return ActiveExecutionUnitsByAffinity.TryGetValue(affinityKey, out int units) ? units : 0;
        }

        private static void AddAffinityActiveUnits(string affinityKey, int delta)
        {
            int next = ActiveExecutionUnitsByAffinity.AddOrUpdate(affinityKey, Math.Max(0, delta), (_, current) => Math.Max(0, current + delta));
            if (next == 0)
            {
                ActiveExecutionUnitsByAffinity.TryRemove(affinityKey, out _);
            }
        }

        private static int GetTenantActiveUnits(string tenantKey)
        {
            return ActiveExecutionUnitsByTenant.TryGetValue(tenantKey, out int units) ? units : 0;
        }

        private static int GetProviderActiveUnits(string providerKey)
        {
            return ActiveExecutionUnitsByProvider.TryGetValue(providerKey, out int units) ? units : 0;
        }

        private static void AddTenantActiveUnits(string tenantKey, int delta)
        {
            int next = ActiveExecutionUnitsByTenant.AddOrUpdate(tenantKey, Math.Max(0, delta), (_, current) => Math.Max(0, current + delta));
            if (next == 0)
            {
                ActiveExecutionUnitsByTenant.TryRemove(tenantKey, out _);
            }
        }

        private static void AddProviderActiveUnits(string providerKey, int delta)
        {
            int next = ActiveExecutionUnitsByProvider.AddOrUpdate(providerKey, Math.Max(0, delta), (_, current) => Math.Max(0, current + delta));
            if (next == 0)
            {
                ActiveExecutionUnitsByProvider.TryRemove(providerKey, out _);
            }
        }

        private static int NormalizeWeight(int weight)
        {
            int normalized = weight;
            if (normalized < 1)
                normalized = 1;
            else if (normalized > MaxTaskWeight)
                normalized = MaxTaskWeight;

            return normalized;
        }

        private static string NormalizeAffinityKey(string affinityKey)
        {
            return string.IsNullOrWhiteSpace(affinityKey) ? DefaultAffinityKey : affinityKey.Trim();
        }

        private static string NormalizeTenantKey(string tenantKey)
        {
            return string.IsNullOrWhiteSpace(tenantKey) ? DefaultTenantKey : tenantKey.Trim();
        }

        private static string NormalizeProviderKey(string providerKey)
        {
            return string.IsNullOrWhiteSpace(providerKey) ? DefaultProviderKey : providerKey.Trim();
        }
    }
}