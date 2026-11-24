using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class Coroutines : AdditiveSingleton<Coroutines>
{
    public Dictionary<int, Coroutine> pool = new Dictionary<int, Coroutine>();
    public int lastID;
    public int current;
    public bool[] UpdateCoroutines = new bool[64]; // whether it exists
    public int[] WaitingFor = new int[64]; // ID of coroutine waiting
    public float[] WaitingUntil = new float[64]; // list of times

    float lastt;


    //public void Destroy(Scene scene, Scene mode)
    //{
    //    Destroy(gameObject);
    //    SceneManager.activeSceneChanged -= Destroy;
    //}
    //protected override void onInitialise()
    //{
    //    SceneManager.activeSceneChanged += Destroy;
    //}
    private void Start()
    {
        DontDestroyOnLoad(this);
    }
    private void Update()
    {
        lastt = Time.time;
        for (int i = 0; i < 64; i++)
        {
            HandleCoroutine(i);
        }
    }
    public void Flush()
    {
        OverrideAddInstance();
    }
    public void HandleCoroutine(int index)
    {
        if (!UpdateCoroutines[index]) return;
        Coroutine c = pool[index];
        
        if (!c.Finished)
        {
            if (WaitingUntil[index] > Time.unscaledTime )
            {
                return;
            }
            if (pool.ContainsKey(WaitingFor[index]))
            {
                return;
            }

            current = index;

            if (!c.Routine.MoveNext())
            {
                c.Finished = true;
                c.End?.Invoke();

                pool.Remove(index);
                UpdateCoroutines[index] = false;
            }

        }
        else // must have been cancelled
        {
            pool.Remove(index);
            UpdateCoroutines[index] = false;
        }
    }

    public Coroutine AddCoroutine(IEnumerator<float> coroutine)
    {
        Coroutine c = new Coroutine();
        lastID += 1;
        lastID = (int)Mathf.Repeat(lastID - 1, 63) + 1;

        c.id = lastID;
        c.Routine = coroutine;

       
        UpdateCoroutines[lastID] = true;
        WaitingUntil[lastID] = Time.unscaledTime;
        WaitingFor[lastID] = -1;
        int cu = current;
        current = lastID;
        pool[lastID] = c;
        if (!coroutine.MoveNext())
        {
            return c;
        }
        current = cu;

   
        return c;
    }

    public void waitFor(Coroutine c)
    {
        WaitingFor[current] = c.id;
    }
    public void waitForSeconds(float f)
    {
        WaitingUntil[current] = Time.unscaledTime + f;
    }
    public class CurrentID
    {
        public int ID;
        public float time;
    }


    public static Coroutine RunCoroutine(IEnumerator<float> coroutine)
    {
       return main.AddCoroutine(coroutine);
    }

    public static float WaitFor(Coroutine c)
    {
        main.waitFor(c);
        return 0;
    }
    public static float Hold(IEnumerator<float> c)
    {
        main.waitFor(main.AddCoroutine(c));
        return 0;
    }
    public static float WaitUntil(Func<bool> condition)
    {
        main.waitFor(main.AddCoroutine(Wait(condition)));
        return 0;
    }

    public static IEnumerator<float> Wait(System.Func<bool> func)
    {
        while (!func())
        {
            yield return 0;
        }
    }

    public static float WaitForSeconds(float t)
    {
        main.waitForSeconds(t);
        return 0;
    }
}

public static class CoroutineEXT
{
    public static IEnumerator<float> CancelWith(this IEnumerator<float> coroutine, GameObject gameObject)
    {
        while (gameObject != null && coroutine.MoveNext())
            yield return 0;
    }
    public static IEnumerator<float> PauseIf(this IEnumerator<float> coroutine, System.Func<bool> condition)
    {

        while ( coroutine.MoveNext())
        {
            yield return 0;
            if (condition()) yield return Coroutines.WaitUntil(() => !condition());

        }
    }
    public static IEnumerator<float> CancelIf(this IEnumerator<float> coroutine, System.Func<bool> condition)
    {

        while ((!condition() && coroutine.MoveNext()))
            yield return 0;
    }

    public static IEnumerator<float> CancelWithCoroutine(this IEnumerator<float> coroutine, Coroutine obj)
    {
        while (obj != null && (!obj.Finished && coroutine.MoveNext()))
            yield return 0;
    }

}

public class Coroutine
{
    public int id;
    public IEnumerator<float> Routine;
    public bool Finished;
    public Action End;

    public void Cancel()
    {
        if (!Finished)
        {
            Finished = true;
            End?.Invoke();
        }
       
    }
}