#if UNITY_STANDALONE_WIN
#define DO_MESURE_WIN
#else
#if UNITY_EDITOR_WIN
#define DO_MESURE_WIN
#endif
#endif

#if UNITY_STANDALONE_OSX
#define DO_MESURE_OSX
#else
#if UNITY_EDITOR_OSX
#define DO_MESURE_OSX
#endif
#endif

using System;
using System.Runtime.InteropServices;
using System.Threading;

[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Imported External code.")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Imported External code.")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "Imported External code")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Imported External code")]

public struct HighPerformanceCounter
{
    private static double performanceFrequency = 0;
    private long startTime;
    private long total;
    private int count;

    private double durationLastClear;
    private int countLastClear;

    public int Count
    {
        get { return this.count; }
    }

    // Returns the duration of the timer (in seconds)
    public double Duration
    {
        get
        {
            return (double)this.total / (double)HighPerformanceCounter.performanceFrequency;
        }
    }

    public double DurationPerCall
    {
        get
        {
            return (double)this.total / ((double)this.Count * (double)HighPerformanceCounter.performanceFrequency);
        }
    }

    public double DurationPerCallLastClear
    {
        get
        {
            return this.durationLastClear / (double)this.countLastClear;
        }
    }

    public double DurationLastClear
    {
        get
        {
            return this.durationLastClear;
        }
    }

    public int CountLastClear
    {
        get
        {
            return this.countLastClear;
        }
    }

    public bool Started
    {
        get
        {
            return this.startTime > 0;
        }
    }
    
#if DO_MESURE_WIN
    [DllImport("Kernel32.dll")]
    private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

    [DllImport("Kernel32.dll")]
    private static extern bool QueryPerformanceFrequency(out long lpFrequency);
#endif

#if DO_MESURE_OSX
    private struct MachTimebaseInfoDataT 
    {
        public uint Number;
        public uint Denom;
    }

    [DllImport("/System/Library/Frameworks/CoreServices.framework/CoreServices")]
    private static extern int64 mach_absolute_time();

    [DllImport("/System/Library/Frameworks/CoreServices.framework/CoreServices")]
    private static extern void mach_timebase_info(out MachTimebaseInfoDataT info);
#endif
    
    public static void InitFreq()
    {
        if (HighPerformanceCounter.performanceFrequency == 0)
        {
            HighPerformanceCounter.performanceFrequency = 0;
#if DO_MESURE_WIN
            long performanceFrequency;
            if (QueryPerformanceFrequency(out performanceFrequency) == false)
            {
                // high-performance counter not supported
                throw new System.ComponentModel.Win32Exception();
            }

            HighPerformanceCounter.performanceFrequency = (double)performanceFrequency;
#endif

#if DO_MESURE_OSX
            MachTimebaseInfoDataT timebaseInfo;
            mach_timebase_info(out timebaseInfo);
            HiPerformanceCounter.PerformanceFrequency = 1000 * 1000 * 1000 * (double)timebaseInfo.Denom / (double)timebaseInfo.Number;
#endif
        }
    }

    public void Clear()
    {
        this.durationLastClear = (double)this.total / (double)HighPerformanceCounter.performanceFrequency;
        this.countLastClear = this.count;
        this.count = 0;
        this.total = 0;
    }

    // Start the timer
    public void Start()
    {
#if DO_MESURE_WIN
        QueryPerformanceCounter(out this.startTime);
#endif
        #if DO_MESURE_OSX
        this.startTime = mach_absolute_time();
        #endif
    }

    // Stop the timer
    public void Stop()
    {
        long stopTime = 0;
#if DO_MESURE_WIN
        QueryPerformanceCounter(out stopTime);
#endif
#if DO_MESURE_OSX
        stopTime = mach_absolute_time();
#endif

        this.total += stopTime - this.startTime;
        this.startTime = 0;
        ++this.count;
    }

    public void Stop(int autoClearIfCountSuperior)
    {
        long stopTime = 0;
#if DO_MESURE_WIN
        QueryPerformanceCounter(out stopTime);
#endif
#if DO_MESURE_OSX
        stopTime = mach_absolute_time();
#endif

        this.total += stopTime - this.startTime;
        this.startTime = 0;
        ++this.count;

        if (this.count > autoClearIfCountSuperior)
        {
            this.Clear();
        }
    }
}