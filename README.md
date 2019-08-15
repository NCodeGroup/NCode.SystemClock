# Overview

[![Build status](https://ci.appveyor.com/api/projects/status/1eoh6ahm0hukqw8d/branch/master?svg=true)](https://ci.appveyor.com/project/polewskm/ncode-systemclock/branch/master)

Provides an abstraction of ISystemClock to assist application testing and corresponding implementations with 1s and 1ms accuracy.

# Problem Statement

Depending on the underlying Operating System, the accuracy of `DateTime.UtcNow`
on .NET Framework is around ~16ms due to the usage of the RTC. Some newer .NET
Frameworks use higher resolution timers such as QPC to obtain the highest level
of accuracy. This library provides two implementations of `ISystemClock` with 1s
and 1ms accuracy.

# Contracts

[![NuGet Downloads](https://img.shields.io/nuget/dt/NCode.SystemClock.svg?style=flat)](https://www.nuget.org/packages/NCode.SystemClock/)
[![NuGet Version](https://img.shields.io/nuget/v/NCode.SystemClock.svg?style=flat)](https://www.nuget.org/packages/NCode.SystemClock/)

```csharp
/// <summary>
/// Abstracts the system clock to facilitate testing.
/// </summary>
public interface ISystemClock
{
	/// <summary>
	/// Retrieves the current system time in UTC.
	/// </summary>
	DateTimeOffset UtcNow { get; }
}

/// <summary>
/// Provides access to the normal system clock with accuracy in milliseconds.
/// </summary>
public interface ISystemClockMillisecondsAccuracy : ISystemClock
{
    // nothing
}

public class SystemClockMillisecondsAccuracy : ISystemClockMillisecondsAccuracy { /* ... */ }

/// <summary>
/// Provides access to the normal system clock with accuracy in seconds.
/// </summary>
/// <remarks>
/// This implementation is particularly useful in JWT authentication because
/// <code>expires_in</code> only supports whole seconds and milliseconds do
/// not round-trip when serializing.
/// </remarks>
public interface ISystemClockSecondsAccuracy : ISystemClock
{
    // nothing
}

public class SystemClockSecondsAccuracy : ISystemClockSecondsAccuracy { /* ... */ }
```

# References

* [Acquiring high-resolution time stamps](https://docs.microsoft.com/en-us/windows/win32/sysinfo/acquiring-high-resolution-time-stamps)
* [Accuracy and accuracy of DateTime](https://blogs.msdn.microsoft.com/ericlippert/2010/04/08/precision-and-accuracy-of-datetime/)

# Release Notes

* v1.0.0 - Initial release
