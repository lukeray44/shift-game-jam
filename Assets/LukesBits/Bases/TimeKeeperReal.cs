using System.Collections;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

[ExecuteAlways]
public class TimeKeeperReal : AdditiveSingleton<TimeKeeperReal>
{
    public static bool Pause;
    protected override void onInitialise()
    {
        DontDestroyOnLoad(gameObject);
    }
    public List<RealTimer> timers = new List<RealTimer>();
    public List<RealFuncTimer> funcTimers = new List<RealFuncTimer>(15);
    public static int fixedFrameCount => main.t_fixedFrameCount;
    public static float realTime => Time.unscaledTime;
    public int t_fixedFrameCount;
    private void FixedUpdate()
    {
        if (!Application.isPlaying) Destroy(gameObject);
        t_fixedFrameCount++;
    }

    [ContextMenu("Test")]
    public void TestTimer()
    {
        AddTimer(5).OnTimerEnd += () => print("Finished");
        AddFuncTimer(5, f => print(f)).OnTimerEnd += () => print("Function Finished");
    }
    private void Update()
    {


        if (funcTimers.Count != 0)
        {
            foreach (RealFuncTimer f in funcTimers)
            {
                if (f.CanAct)
                {
                    if (f.curve != null)
                    {
                        f.function(f.getT(f.curve.Evaluate(f.PercentageComplete)));
                    }
                    else
                    {
                        f.function(f.getT(f.PercentageComplete));
                    }
                }


            }

        }

        while (timers.Count > 0)
        {

            RealTimer t = timers[0];
            bool canAct = t.CanAct;
            if (t.PercentageComplete == 1 || !canAct)
            {
                t.Completed = true;
                if (canAct) t.OnTimerEnd.Invoke();
                timers.RemoveAt(0);
                if (t.repeat) InsertTimer(t);
            }
            else
            {
                break;
            }
        }
    }
    public RealTimer InsertTimer(RealTimer time)
    {

        if (timers.Count != 0)
        {
            for (int i = 0; i < timers.Count; i++)
            {
                if (timers[i].TimeEnd > time.TimeEnd)
                {
                    timers.Insert(i, time);
                    return time;
                }
            }
        }
        timers.Add(time);
        return time;
    }
    public RealFuncTimer InsertFuncTimer(RealFuncTimer time)
    {
        funcTimers.Add(time);
        time.OnTimerEnd += () => {
            funcTimers.Remove(time);
        };

        InsertTimer(time);
        time.function(time.getT(0));
        return time;
    }
    public static RealTimer AddTimer(float time)
    {
        RealTimer t = new RealTimer(time);
        main.InsertTimer(t);
        return t;

    }
    public static RealFuncTimer AddFuncTimer(float time, tfunc function, float startRange = 0, float endRange = 1)
    {
        RealFuncTimer t = new RealFuncTimer(time, function, false, startRange, endRange);
        main.InsertFuncTimer(t);
        return t;

    }
    public static RealTimer AddFuncTimer(float time, AnimationCurve curve, tfunc function, float startRange = 0, float endRange = 1)
    {
        RealFuncTimer t = new RealFuncTimer(time, curve, function, false, startRange, endRange);
        main.InsertFuncTimer(t);
        return t;

    }

    public static IEnumerator<float> CustomTween(float time, tfunc function, float startRange = 0, float endRange = 1)
    {
        for (float i = 0; i < time; i += Time.unscaledDeltaTime)
        {
            function(MathEXT.realfLerp(startRange, endRange, i / time));
            yield return 0;

        }
        yield return 0;
        function(endRange);

    }
    public static float MECCustomTween(float time, tfunc function, float startRange = 0, float endRange = 1)
    {
        return Coroutines.Hold(CustomTween(time, function, startRange, endRange));

    }
    //public static IEnumerator<float> CustomTween(float time, AnimationCurve curve, tfunc function, float startRange = 0, float endRange = 1)
    //{
    //    for (float i = 0; i < time; i += Time.unscaledDeltaTime)
    //    {

    //        function(MathEXT.realfLerp(startRange, endRange, curve.Evaluate(i / time)));
    //        yield return RealTiming.WaitForOneFrame;
    //    }
    //    function(curve.Evaluate(1) * endRange);

    //}
    //public static float MECCustomTween(float time, AnimationCurve curve, tfunc function, float startRange = 0, float endRange = 1)
    //{
    //    return RealTiming.Hold(CustomTween(time, curve, function, startRange, endRange));

    //}

    //public static IEnumerator<float> CustomTweenPause(float time, AnimationCurve curve, tfunc function, float startRange = 0, float endRange = 1)
    //{
    //    for (float i = 0; i < time; i += Time.unscaledDeltaTime)
    //    {
    //        if (Pause) i -= Time.unscaledDeltaTime;
    //        function(MathEXT.realfLerp(startRange, endRange, curve.Evaluate(i / time)));
    //        yield return RealTiming.WaitForOneFrame;
    //    }
    //    function(curve.Evaluate(1) * endRange);

    //}
    //public static float MECCustomTweenPause(float time, AnimationCurve curve, tfunc function, float startRange = 0, float endRange = 1)
    //{
    //    return RealTiming.Hold(CustomTweenPause(time, curve, function, startRange, endRange));

    //}

    //public static IEnumerator<float> CustomTweenPause(float time, tfunc function, float startRange = 0, float endRange = 1)
    //{
    //    for (float i = 0; i < time; i += Time.unscaledDeltaTime)
    //    {
    //        if (Pause)
    //        {
    //            i -= Time.unscaledDeltaTime;
    //            print("pausing");
    //        }
    //        function(MathEXT.realfLerp(startRange, endRange, i / time));
    //        yield return RealTiming.WaitForOneFrame;

    //    }
    //    yield return RealTiming.WaitForOneFrame;
    //    function(endRange);

    //}
    //public static float MECCustomTweenPause(float time, tfunc function, float startRange = 0, float endRange = 1)
    //{
    //    return RealTiming.Hold(CustomTweenPause(time, function, startRange, endRange));

    //}
}

public class RealTimer
{
    public bool Completed;
    public float TimeEnd;
    protected float TimeStart;
    protected float duration;
    public bool CanAct => (!Check || CheckBy != null) && !Cancelled;
    public float TimePlaying => TimeKeeperReal.realTime - TimeStart;
    public float PercentageComplete => Mathf.Clamp(TimePlaying / duration, 0, 1);
    public Action OnTimerEnd;
    public bool repeat { get; protected set; }
    public RealTimer(float time, bool repeat = false)
    {
        duration = time;
        TimeEnd = TimeKeeperReal.realTime + time;
        TimeStart = TimeKeeperReal.realTime;
        OnTimerEnd = new Action(() => { });
        this.repeat = repeat;
    }
    public RealTimer(float time, GameObject tieTo, bool repeat = false)
    {
        duration = time;
        TimeEnd = TimeKeeperReal.realTime + time;
        TimeStart = TimeKeeperReal.realTime;
        OnTimerEnd = new Action(() => { });
        this.repeat = repeat;
        CheckBy = tieTo;
        Check = true;
    }

    public GameObject CheckBy;
    public bool Check;
    public bool Cancelled;

    public RealTimer TieTo(GameObject TieTo)
    {
        CheckBy = TieTo;
        Check = true;
        return this;
    }
}
[Serializable]
public class RealFuncTimer : RealTimer
{

    public tfunc function;

    public AnimationCurve curve;
    public float start;
    public float end;
    public float getT(float t)
    {
        return start + (end - start) * t;
    }
    public RealFuncTimer(float time, tfunc f, bool repeat = false, float startRange = 0, float endRange = 1) : base(time)
    {
        start = startRange;
        end = endRange;
        duration = time;
        TimeEnd = TimeKeeperReal.realTime + time;
        TimeStart = TimeKeeperReal.realTime;
        OnTimerEnd = new Action(() => { });
        this.repeat = repeat;
        function = f;
    }
    public RealFuncTimer(float time, AnimationCurve c, tfunc f, bool repeat = false, float startRange = 0, float endRange = 1) : base(time)
    {
        start = startRange;
        end = endRange;
        duration = time;
        TimeEnd = TimeKeeperReal.realTime + time;
        TimeStart = TimeKeeperReal.realTime;
        OnTimerEnd = new Action(() => { });
        this.repeat = repeat;
        function = f;
        curve = c;
    }
}