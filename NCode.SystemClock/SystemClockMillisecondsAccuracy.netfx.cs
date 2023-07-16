#region Copyright Preamble

//
//    Copyright @ 2023 NCode Group
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

#endregion

using System;
using System.Diagnostics;
using System.Security;
using System.Threading;

namespace NCode.SystemClock;

/// <summary>
/// Provides access to the normal system clock with accuracy in milliseconds.
/// </summary>
public class SystemClockMillisecondsAccuracy : ISystemClockMillisecondsAccuracy
{
    /// <inheritdoc/>
    public DateTimeOffset UtcNow => IsPreciseFlag ? DateTimeOffset.UtcNow : GetUtcNow();

    private static readonly bool IsPreciseFlag = DetermineIsPreciseFlag();

    private static bool DetermineIsPreciseFlag()
    {
        var startTime = DateTime.UtcNow;

        var sw = Stopwatch.StartNew();
        SpinWait.SpinUntil(() => sw.ElapsedMilliseconds > 1, 2);

        var endTime = DateTime.UtcNow;

        var duration = endTime - startTime;
        var isPrecise = duration.TotalMilliseconds < 5.0;

        return isPrecise;
    }

    // Credits to original implementation:
    // https://github.com/dotnet/corefx/blob/master/src/System.Diagnostics.DiagnosticSource/src/System/Diagnostics/Activity.DateTime.netfx.cs

    internal static DateTimeOffset GetUtcNow()
    {
        var snapshot = _snapshot;

        var ticksDelta = (long)((Stopwatch.GetTimestamp() - snapshot.StopwatchTicks) * 10000000L /
                                (double)Stopwatch.Frequency);

        return snapshot.UtcNow.AddTicks(ticksDelta);
    }

    private class Snapshot
    {
        public readonly DateTime UtcNow = DateTime.UtcNow;
        public readonly long StopwatchTicks = Stopwatch.GetTimestamp();
    }

    private static Snapshot _snapshot = new();

    // Suppress unused field warning, as it's used to keep the timer alive
    // ReSharper disable once UnusedMember.Local
#pragma warning disable IDE0052 // Remove unread private members
    private static readonly Timer RefreshTimer = InitializeRefreshTimer();
#pragma warning restore IDE0052 // Remove unread private members

    private static void RefreshSnapshot()
    {
        // wait for DateTime.UtcNow update to the next granular value
        Thread.Sleep(1);
        _snapshot = new Snapshot();
    }

    [SecuritySafeCritical]
    private static Timer InitializeRefreshTimer()
    {
        Timer timer;
        // Don't capture the current ExecutionContext and its AsyncLocals onto the timer causing them to live forever
        var restoreFlow = false;
        try
        {
            if (!ExecutionContext.IsFlowSuppressed())
            {
                ExecutionContext.SuppressFlow();
                restoreFlow = true;
            }

            // refresh the snapshot every 2 hours
            timer = new Timer(_ => { RefreshSnapshot(); }, null, 0, 7200000);
        }
        finally
        {
            // Restore the current ExecutionContext
            if (restoreFlow)
                ExecutionContext.RestoreFlow();
        }

        return timer;
    }
}