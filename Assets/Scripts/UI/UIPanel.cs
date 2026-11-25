using System;
using UnityEngine;

public class UIPanel : coroutineAnimator
{

    public bool WorkWhenPaused;
    public Action<bool> OnHoverSet;
    public bool Active = true;
    RectTransform rect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        rect = transform as RectTransform;
        UIManager.AddInstance();
    }

    bool inRect => RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition, Camera.main) && gameObject.activeInHierarchy && Active;

    // Update is called once per frame
    public void Update()
    {
        if (!WorkWhenPaused && Time.timeScale == 0) return;
        
        if (hovered && Input.GetMouseButtonDown(0) && Active) OnClick();

        if (inRect)
        {
            SetHovered(true);
        }
        else
        {
            SetHovered(false);
        }
    }

    protected bool hovered;
    public bool ShowCursor;
    public void SetHovered(bool hovered)
    {
        if (this.hovered != hovered)
        {
            this.hovered = hovered;
            OnHoverSet?.Invoke(hovered);

            if (hovered)
            {
                OnHover();
                UIManager.main.UIHoveredCount++;
                if (ShowCursor) UIManager.main.ButtonHovered(true);
            }
            else
            {
                OnUnHover();
                UIManager.main.UIHoveredCount--;
                if (ShowCursor) UIManager.main.ButtonHovered(false);
            }
        }
    }

    void OnDisable()
    {
        if (UIManager.InstanceExists) SetHovered(false);
    }
    public virtual void OnHover() { }
    public virtual void OnUnHover() { }

    public virtual void OnClick() { }
}
