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
    public override IEnumerator<float> ClickAnim()
    { 
       
        transform.localScale *= 0.9f;
        gameObject.LeanScale(transform.localScale / 0.9f, 0.1f).setEaseOutCubic();
        yield return TimeKeeperReal.MECCustomTween(0.1f, t =>
        {
            Backing.color = Color.Lerp(Color.white, MainColor, t);
        });
    }



    public override IEnumerator<float> HoverAnim()
    {
        float time = 0.07f;
        gameObject.LeanScale(transform.localScale * 1.1f, time).setEaseOutCubic();
        gameObject.LeanRotateZ(rotateBy, time).setEaseOutCubic();
        yield return TimeKeeperReal.MECCustomTween(time, t =>
        {
            Backing.color = Color.Lerp(stayColoured ? MainColor : BoringColor, MainColor, t);
        });
    }

    public override IEnumerator<float> UnHoverAnim()
    {
        float time = 0.07f;
        gameObject.LeanScale(transform.localScale / 1.1f, time).setEaseOutCubic();
        gameObject.LeanRotateZ(0, time).setEaseOutCubic();
        yield return TimeKeeperReal.MECCustomTween(time, t =>
        {
            Backing.color = Color.Lerp(stayColoured ? MainColor : BoringColor, MainColor, 1 - t);
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
