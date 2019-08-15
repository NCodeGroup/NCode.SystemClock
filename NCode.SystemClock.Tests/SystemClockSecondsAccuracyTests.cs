#region Copyright Preamble
// 
//    Copyright @ 2019 NCode Group
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
using System.Threading;
using Xunit;

namespace NCode.SystemClock.Tests
{
    /// <summary/>
    public class SystemClockSecondsAccuracyTests
    {
        /// <summary/>
        [Fact]
        public void UtcNowHasZeroMilliseconds()
        {
            var systemClock = new SystemClockSecondsAccuracy();

            var sw = Stopwatch.StartNew();

            var startTime = systemClock.UtcNow;
            Assert.Equal(0, startTime.Millisecond);

            SpinWait.SpinUntil(() => sw.ElapsedMilliseconds > 1, 2);

            var endTime = systemClock.UtcNow;
            Assert.Equal(0, endTime.Millisecond);

            Assert.True(sw.ElapsedMilliseconds > 0);

            var duration = endTime - startTime;
            Assert.Equal(0.0, duration.TotalMilliseconds);
        }

        /// <summary/>
        [Fact]
        public void GetUtcNowAccuracyIsSeconds()
        {
            var systemClock = new SystemClockSecondsAccuracy();

            var nowActual = systemClock.UtcNow;
            var nowExpected = DateTimeOffset.UtcNow;

            var diff = nowActual - nowExpected;
            var seconds = diff.TotalSeconds;
            Assert.True(seconds < 1.0);
        }

    }
}