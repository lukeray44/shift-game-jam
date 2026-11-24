using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using System.Collections.Generic;


[ExecuteAlways]
public class TimeKeeper : AdditiveSingleton<TimeKeeper>
{
    public void SetAnchor()
    {
        if (Anchor == null) Anchor = new GameObject("Anchor");
    }
    public void NewAnchor(Scene s, LoadSceneMode l)
    {
        Anchor = new GameObject("Anchor");
        SceneManager.MoveGameObjectToScene(Anchor, s);
    }
    protected override void onInitialise()
    {
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += NewAnchor;
    }
    public List<Timer> timers = new List<Timer>();
    public List<Timer> realtimeTimers = new List<Timer>();
    public List<FuncTimer> funcTimers = new List<FuncTimer>(15);

    public GameObject Anchor;
    public static int fixedFrameCount => main.t_fixedFrameCount;
    public int t_fixedFrameCount;
    int failedIndex;
    int failedType; //0 - none, 1 - func, 2 - timer
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
        if (failedType == 1)
        {
            funcTimers.RemoveAt(failedIndex);
        } else if (failedIndex == 2)
        {
            timers.RemoveAt(0);
        }
        failedType = 1;
        failedIndex = -1;
        if (funcTimers.Count != 0)
        {
            foreach (FuncTimer f in funcTimers)
            {
                failedIndex++;
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
        failedType = 2;
        failedIndex = 0;
        while (timers.Count > 0)
        {

            Timer t = timers[0];
            bool canAct = t.CanAct;
            if (t.PercentageComplete == 1 || !canAct)
            {
                t.Completed = true;
                timers.RemoveAt(0);
                t.Finish();
                if (t.repeat)
                {
                    t.Completed = false;
                    t.Cancelled = false;
                    InsertTimer(t);
                }
            }
            else
            {
                break;
            }
        }

        failedType = 0;

    }
    public Timer InsertTimer(Timer time)
    {
        SetAnchor();
        time.TieTo(Anchor);
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
    public Timer InsertFuncTimer(FuncTimer time)
    {
        if (!time.CanAct) return time;

        funcTimers.Add(time);
        time.OnTimerEnd += () => {
            funcTimers.Remove(time);
        };

        InsertTimer(time);
        time.function(time.getT(0));
        return time;
    }
    public static Timer AddTimer(float time)
    {
        Timer t = new Timer(time);
        main.InsertTimer(t);
        return t;

    }
    public static Timer AddFuncTimer(float time, tfunc function, float startRange = 0, float endRange = 1)
    {
        FuncTimer t = new FuncTimer(time, function, false, startRange, endRange);
        main.InsertFuncTimer(t);
        return t;

    }
    public static Timer AddFuncTimer(float time, AnimationCurve curve, tfunc function, float startRange = 0, float endRange = 1)
    {
        FuncTimer t = new FuncTimer(time, curve,  function, false, startRange, endRange);
        main.InsertFuncTimer(t);
        return t;

    }
    public static IEnumerator<float> CustomTween(float time, tfunc function, float startRange = 0, float endRange = 1)
    {
        for (float i = 0; i < time; i += Time.deltaTime)
        {
            function(MathEXT.realfLerp(startRange, endRange, i / time));
            yield return -1;
        }
        yield return -1;
        function(endRange);

    }
    public static float MECCustomTween(float time, tfunc function, float startRange = 0, float endRange = 1)
    {
        return Coroutines.Hold(CustomTween(time, function, startRange, endRange));

    }
    public static float MECCustomTweenCancelWith(float time, tfunc function, GameObject g, float startRange = 0, float endRange = 1)
    {
        return Coroutines.Hold(CustomTween(time, function, startRange, endRange).CancelWith(g));

    }
    public static IEnumerator<float> CustomTween(float time, AnimationCurve curve, tfunc function, float startRange = 0, float endRange = 1)
    {
        for (float i = 0; i < time; i += Time.deltaTime)
        {

            function(MathEXT.realfLerp(startRange, endRange, curve.Evaluate(i / time)));
            yield return -1;
        }
        function(curve.Evaluate(1) * endRange);

    }
    public static float MECCustomTween(float time, AnimationCurve curve, tfunc function, float startRange = 0, float endRange = 1)
    {
        return Coroutines.Hold(CustomTween(time, curve, function, startRange, endRange));

    }

    public static IEnumerator<float> WaitForTimeOrCondition(float time, Func<bool> condition)
    {
        float endTime = Time.time + time;
        while (!condition())
        {
            if (endTime < Time.time)
            {
                break;
            }
            yield return -1;
        }

    }

    public static string SecondsToHoursMinutesSeconds(int seconds)
    {
        int hours = Mathf.FloorToInt(seconds / 3600);
        int minutes = Mathf.FloorToInt((seconds - hours * 3600) / 60);
        seconds -= hours * 3600 + minutes * 60;
        string toReturn = "";
        if (hours != 0) toReturn = hours + ":";
        return $"{toReturn}{minutes.ToString().MakeOfLength(2, "0")}:{seconds.ToString().MakeOfLength(2, "0")}";
    }
    public static string SecondsToMinutesSecondsMilliseconds(float seconds)
    {
        int minutes = Mathf.FloorToInt((seconds) / 60);
        seconds -=  minutes * 60;
        float milliseconds = Mathf.Floor(Mathf.Repeat(seconds * 100, 100));
        seconds -= milliseconds / 100f;
        seconds = Mathf.Round(seconds);
        return $"{minutes.ToString().MakeOfLength(2, "0")}:{Mathf.FloorToInt(seconds).ToString().MakeOfLength(2, "0")}.{milliseconds.ToString().MakeOfLength(2, "0")}";
    }
}

[Serializable]
public class Timer
{
    public bool Completed;
    public float TimeEnd;
    protected float TimeStart;
    public float duration;
    public bool CanAct => (!Check || CheckBy != null) && !Cancelled;
    public float TimePlaying => Time.time - TimeStart;
    public float TimeLeft => duration - TimePlaying;
    public float PercentageComplete => Mathf.Clamp(TimePlaying / duration, 0, 1);
    public Action OnTimerEnd;
    public bool repeat;
    public Timer(float time, bool repeat = false)
    {
        duration = time;
        TimeEnd = Time.time + time;
        TimeStart = Time.time;

        this.repeat = repeat;
    }
    public Timer(float time, GameObject tieTo, bool repeat = false)
    {
        duration = time;
        TimeEnd = Time.time + time;
        TimeStart = Time.time;

        this.repeat = repeat;
        CheckBy = tieTo;
        Check = true;
    }

    public virtual void Finish()
    {
        if (!CanAct) return;
        Completed = true;
        Cancelled = true;
        OnTimerEnd?.Invoke();

        //reset in case of re-insertion
        TimeEnd = Time.time + duration;
        TimeStart = Time.time;
    }
    public void DefaultFinish()
    {
        if (!CanAct) return;
        Completed = true;
        Cancelled = true;
        OnTimerEnd?.Invoke();

        //reset in case of re-insertion
        TimeEnd = Time.time + duration;
        TimeStart = Time.time;
    }
    public UnityEngine.Object CheckBy;
    public bool Check;
    public bool Cancelled;

    public Timer TieTo(UnityEngine.Object TieTo)
    {
        CheckBy = TieTo;
        Check = true;
        return this;
    }
    public Timer DontDestroyOnLoad()
    {
        CheckBy = null;
        Check = false;
        return this;
    }
}
[Serializable]
public class FuncTimer : Timer
{
    
    public tfunc function;

    public AnimationCurve curve;
    public float start;
    public float end;
    public float getT(float t)
    {
        return start + (end - start) * t;
    }
    public FuncTimer(float time, tfunc f, bool repeat = false, float startRange = 0, float endRange = 1) : base(time)
    {
        start = startRange;
        end = endRange;
        duration = time;
        TimeEnd = Time.time + time;
        TimeStart = Time.time;
        this.repeat = repeat;
        function = f;
    }
    public FuncTimer(float time, AnimationCurve c, tfunc f, bool repeat = false, float startRange = 0, float endRange = 1) : base(time)
    {
        start = startRange;
        end = endRange;
        duration = time;
        TimeEnd = Time.time + time;
        TimeStart = Time.time;
        this.repeat = repeat;
        function = f;
        curve = c;
    }
    public override void Finish()
    {
        if (!CanAct) return;
       
        function(end);
        base.Finish();
        
    }
}
public delegate void tfunc(float t);

