using System;
using UnityEngine;

public class TimeProvider : MonoBehaviour
{
    private const long TicksPerMillisecond = 10000;               
    private const long TicksAtEpoch = 621355968000000000;   
    private const double TicksToSeconds = 1E-07;          

    private static TimeProvider instance;         

    private long _monotonicClockAdjustment;

    private long _unscaledTicksSinceStartup;       

    private long _scaledTicksSinceStartup;         

    private long _unscaledDeltaTicks;              

    private long _scaledDeltaTicks;                

    private long _lastFrameTicks;                  

    private bool _isPaused;                        

    public static TimeProvider Instance
    {
        get { return UnityTools.EnsureSingleton<TimeProvider>(ref TimeProvider.instance); }
    }

    public static long MonotonicClockMillis
    {
        get;
        private set;
    }

    public bool UseAndroidJniTimeQuery
    {
        set
        {
        }
    }

    private void Awake()
    {
        if ((UnityEngine.Object)TimeProvider.instance != (UnityEngine.Object)null)
        {
            UnityEngine.Object.Destroy((UnityEngine.Object)this);
        }
        else
        {
            TimeProvider.instance = this;
            _lastFrameTicks = DateTime.UtcNow.Ticks;
        }
    }

    private void Update()
    {
        if (this._isPaused)
            return;
        long ticks = DateTime.UtcNow.Ticks;
        this._unscaledDeltaTicks = ticks - this._lastFrameTicks;
        this._lastFrameTicks = ticks;
        this._unscaledTicksSinceStartup += this._unscaledDeltaTicks;
        this._scaledDeltaTicks = (long)((double)this._unscaledDeltaTicks * (double)Time.timeScale);
        this._scaledTicksSinceStartup += this._scaledDeltaTicks;
    }

   

    private void OnApplicationPause(bool paused)
    {
        this._isPaused = paused;
        if (paused)
            return;
        this._lastFrameTicks = DateTime.UtcNow.Ticks;
    }


    public void Resume()
    {
        this.OnApplicationPause(false);
    }

    public double UnscaledTimeSinceStartup
    {
        get
        {
            return (double)this._unscaledTicksSinceStartup * 1E-07;
        }
    }

    public double ScaledTimeSinceStartup
    {
        get
        {
            return (double)this._scaledTicksSinceStartup * 1E-07;
        }
    }

    public long TimeMillis
    {
        get
        {
            return (this._lastFrameTicks - 621355968000000000L) / 10000L;
        }
    }

    public TimeProvider()
    {
    }
}
