using UnityEngine;

public class HUDElement : MonoBehaviour
{
    public CanvasGroup Canvas;
    public LevelManager Manager;
    void Start()
    {
        Manager.HideHUD += Hide;
    }
    void Hide()
    {
        Vector3 dir = (transform.position - Camera.main.transform.position).td().normalized;
        gameObject.LeanMove(transform.position + dir, 0.2f).setEaseOutExpo();
        transform.LeanScale(transform.localScale * 1.1f, 0.2f).setEaseOutExpo().setOnComplete(() => gameObject.SetActive(false));
        Canvas.LeanAlpha(0, 0.2f).setEaseInExpo();
    }
}
