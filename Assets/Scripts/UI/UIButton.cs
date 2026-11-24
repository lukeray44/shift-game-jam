using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIButton : UIPanel
{ 
    public Action Clicked;
    
    public abstract IEnumerator<float> HoverAnim();
    public abstract IEnumerator<float> UnHoverAnim();
    public abstract IEnumerator<float> ClickAnim();
    public override void OnHover()
    {
        PlayAnim(HoverAnim());
        UIManager.main.ButtonHovered(true);
        base.OnHover();
    }

    public override void OnUnHover()
    {
        PlayAnim(UnHoverAnim());
        UIManager.main.ButtonHovered(false);
        base.OnUnHover();
    }

    public override void OnClick()
    {
        Clicked?.Invoke();
        PlayAnim(ClickAnim());
        base.OnClick();
    }
}
