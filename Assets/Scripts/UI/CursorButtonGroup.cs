using Sirenix.OdinInspector;
using UnityEngine;

public class CursorButtonGroup : UIPanel
{
    public UIButton[] Children;

    [OnInspectorInit]
    void getChildren()
    {
        Children = gameObject.GetComponentsInChildren<UIButton>();
    }

        public UIButton Quit;
    public UICursor cursor;

    public CanvasGroup alpha;
    float hideTimer;
    void Start()
    {
        foreach (UIButton b in Children)
        {
            b.OnHoverSet += a =>
            {
                if (a)
                {
                    hideTimer = 0;
                    cursor.Select(b.transform.position);
                }
                else
                {
                    hideTimer = 0.1f;
                }
            };
        }
    }

    void Update()
    {
        if (hideTimer > 0)
        {
            hideTimer -= Time.unscaledDeltaTime;
           
            if (hideTimer < 0)
            {
                cursor.Hide();
            }
        }
    }

    const float dist = 3;
    public void Show(float dir = 1)
    {
        alpha.alpha = 0;
        gameObject.SetActive(true);
        Vector3 pos = transform.position;
        transform.position += Vector3.left * dir * dist;
        gameObject.LeanMove(pos, 0.2f).setEaseOutExpo();

        transform.localScale *= 0.8f;
        transform.LeanScale(transform.localScale / 0.8f, 0.2f).setEaseOutExpo().setOnComplete(() =>
        {
            foreach (UIButton b in Children)
            {
                b.Active = true;
            }
        });

        alpha.LeanAlpha(1, 0.14f).setEaseOutExpo();
    }

    public void Hide(float dir = 1)
    {
        foreach (UIButton b in Children)
        {
            b.Active = false;
        }
        Vector3 pos = transform.position;
        gameObject.LeanMove(transform.position + Vector3.right * dir * dist, 0.2f).setEaseOutExpo().setOnComplete(() => transform.position = pos);

        transform.LeanScale(transform.localScale * 0.8f, 0.2f).setEaseOutExpo().setOnComplete(() =>
        {
            transform.localScale /= 0.8f;
            gameObject.SetActive(false);
        });

        alpha.LeanAlpha(0, 0.14f).setEaseOutExpo();
    }
}
