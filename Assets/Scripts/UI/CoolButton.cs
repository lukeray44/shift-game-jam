using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoolButton : UIButton
{
    public Image Backing;
    public TextMeshProUGUI Text;
    public Color MainColor;
    public Color BoringColor;

    public float rotateBy = 2;
    bool stayColoured;

    public AudioClip Hover, Click;
    public override IEnumerator<float> ClickAnim()
    { 
       Sound.Play(Click);
        transform.localScale *= 0.9f;
        gameObject.LeanScale(transform.localScale / 0.9f, 0.1f).setEaseOutCubic().setIgnoreTimeScale(true);
        yield return TimeKeeperReal.MECCustomTween(0.1f, t =>
        {
            Backing.color = Color.Lerp(Color.white, MainColor, t);
        });
    }



    public override IEnumerator<float> HoverAnim()
    {
        Sound.Play(Hover, 1.2f, 0.5f);
        float time = 0.07f;
        gameObject.LeanScale(Vector3.one * 1.1f, time).setEaseOutCubic().setIgnoreTimeScale(true);
        gameObject.LeanRotateZ(rotateBy, time).setEaseOutCubic().setIgnoreTimeScale(true);
        yield return TimeKeeperReal.MECCustomTween(time, t =>
        {
            if (Backing != null) Backing.color = Color.Lerp(stayColoured ? MainColor : BoringColor, MainColor, t);
        });
    }

    public override IEnumerator<float> UnHoverAnim()
    {
        float time = 0.07f;
        gameObject.LeanScale(Vector3.one, time).setEaseOutCubic().setIgnoreTimeScale(true);
        gameObject.LeanRotateZ(0, time).setEaseOutCubic().setIgnoreTimeScale(true);
        yield return TimeKeeperReal.MECCustomTween(time, t =>
        {
            if (Backing != null) Backing.color = Color.Lerp(stayColoured ? MainColor : BoringColor, MainColor, 1 - t);
        });
    }

    public void SetSelected(bool selected)
    {
        if (selected != stayColoured)
        {
            stayColoured = selected;

            if (!selected)
            {
                PlayAnim(FadeBack(true));
            } else if (!hovered)
            {
                PlayAnim(FadeBack(false));
            }
        }
    }

    public IEnumerator<float> FadeBack(bool fade)
    {
        yield return TimeKeeperReal.MECCustomTween(0.04f, t =>
        {
            Backing.color = Color.Lerp(BoringColor, MainColor, fade ? (1 - t) : t);
        });
    }
}
