using UnityEngine;

public class UICursor : MonoBehaviour
{
    public RectTransform rect;
    public float alternateFrequency;
    public float growSize;

    Vector3 size;

    bool flipped;

    void Start()
    {
        size = rect.sizeDelta;
        SetHidden();
    }

    public void Update()
    {
        flipped = Mathf.FloorToInt(Time.time * alternateFrequency) % 2 == 0;
        rect.sizeDelta = size + (flipped ? growSize : 0) * Vector3.one;
    }

    public CanvasGroup alpha;

    bool hidden;

    public void Select(Vector3 focus)
    {
        gameObject.LeanCancel();
        if (hidden)
        {
            transform.position = focus;
            alpha.LeanAlpha(1, 0.05f);
            gameObject.transform.localScale = Vector3.one * 0.95f;
            gameObject.LeanScale(Vector3.one, 0.05f).setEaseOutExpo();
            hidden = false;
        } else
        {
            alpha.alpha = 1;
            gameObject.LeanMove(focus, 0.05f).setEaseOutExpo();
        }
    }
    
    public void Hide()
    {
        hidden = true;
        gameObject.LeanScale(transform.localScale * 0.95f, 0.05f);
        alpha.LeanAlpha(0, 0.05f);
    }

    public void SetHidden()
    {
        hidden = true;
        alpha.alpha = 0;
    }
}
