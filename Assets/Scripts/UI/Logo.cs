using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class Logo : MonoBehaviour
{
    public SpriteRenderer[] Letters;

    [Button]
    void get()
    {
        Letters = gameObject.GetComponentsInChildren<SpriteRenderer>();

        for (int x = 0; x < 3; x++)
        {
            for (int i = 0; i < Letters.Length; i++)
            {
                SpriteRenderer prev = Letters[i];
                int next = Random.Range(0, Letters.Length);

                Letters[i] = Letters[next];
                Letters[next] = prev;
            }
        }
    }
    public Color[] Colors;


    public void Randomise()
    {
        foreach (SpriteRenderer i in Letters)
        {
            i.color = Colors.Choose();
            i.transform.localEulerAngles = Vector3.forward * Random.Range(-5f, 5f);
            i.transform.localScale = Vector3.one * Random.Range(0.95f, 1.05f);
        }
    }

    void Start()
    {
        Randomise();
        AnimateAppearance();
    }


    public AnimationCurve BounceInX;
    public AnimationCurve BounceInY;
    void animateLetter(SpriteRenderer letter)
    {
        letter.gameObject.SetActive(true);
        float time = 0.5f;
        Vector3 v = letter.transform.localScale;
        letter.transform.localScale = Vector3.zero;
        letter.gameObject.LeanScaleX(v.x, time).setEase(BounceInX);
        letter.gameObject.LeanScaleY(v.y, time).setEase(BounceInY);
    }

    public float Frequency = 3;
    bool finishedAnimating;

    int tick;
    void Update()
    {
        if (!finishedAnimating) return;
        int randomTick = Mathf.FloorToInt(Time.time * Frequency);
        if (randomTick != tick) Randomise();
        tick = randomTick;
    }
    public void AnimateAppearance()
    {
        
        TimeKeeper.AddTimer(0.1f).OnTimerEnd += () =>
        {
            CutoutTransition.main.TransitionIn(transform.position, 1.5f, 1.5f);
            Coroutines.RunCoroutine(Appear());
        };
    }
    IEnumerator<float> Appear()
    {
        
        foreach (SpriteRenderer s in Letters)
        {
            s.gameObject.SetActive(false);
        }
         yield return Coroutines.WaitForSeconds(0.3f);

         int ind = 0;
         float time = 0.05f;
        foreach (SpriteRenderer i in Letters)
        {
            animateLetter(i);
            yield return Coroutines.WaitForSeconds(time);

            ind++;

            if (ind == 3)
            {
                 yield return Coroutines.WaitForSeconds(0.2f);
                 time *= 3 / 5;
            }
        }
        yield return Coroutines.WaitForSeconds(0.3f);
        finishedAnimating = true;
    }
}
