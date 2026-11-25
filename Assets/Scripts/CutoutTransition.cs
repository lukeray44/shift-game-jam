using UnityEngine;
using UnityEngine.UI;

public class CutoutTransition : Singleton<CutoutTransition>
{
    public Image Cutout;
    public CanvasGroup Fade;

    public AnimationCurve Appear, Disappear;

    void Awake()
    {
        base.Awake();
        Fade.gameObject.SetActive(true);
        Fade.alpha = 1;
    }
    public RealTimer TransitionIn(Vector3 pos, float time = 2, float distMulti = 1)
    {
        Cutout.gameObject.SetActive(true);
        Fade.gameObject.SetActive(false);
        Cutout.material.SetVector("_WorldPos", pos);
        
        float dist = getDistToCameraCorner(pos) * distMulti;
        return TimeKeeperReal.AddFuncTimer(time, t =>
        {
            Cutout.material.SetFloat("_Dist", Mathf.Lerp(0, dist, Appear.Evaluate(t)));
        });
    }

     public RealTimer FadeIn(float time = 2)
    {
        Fade.gameObject.SetActive(true);
        return TimeKeeperReal.AddFuncTimer(time, t =>
        {
            Fade.alpha = LeanTween.easeInExpo(1, 0, t);
        });
    }

     public RealTimer FadeOut(float time = 2)
    {
        Fade.gameObject.SetActive(true);
        return TimeKeeperReal.AddFuncTimer(time, t =>
        {
            Fade.alpha = LeanTween.easeInExpo(0, 1, t);
        });
    }

    float getDistToCameraCorner(Vector3 pos)
    {
        Vector3 camSize = Camera.main.CamSize() * 0.5f;
        Vector3 corner1 = Camera.main.transform.position + Vector3.up * camSize.y + Vector3.right * camSize.x;
        Vector3 corner2 = Camera.main.transform.position + Vector3.down * camSize.y + Vector3.right * camSize.x;
        Vector3 corner3 = Camera.main.transform.position + Vector3.up * camSize.y + Vector3.left * camSize.x;
        Vector3 corner4 = Camera.main.transform.position + Vector3.down * camSize.y + Vector3.left * camSize.x;
        
        
        return Mathf.Max(
            Vector3.Distance(corner1, pos),
            Vector3.Distance(corner2, pos),
            Vector3.Distance(corner3, pos),
            Vector3.Distance(corner4, pos)
        );
    }
    public RealTimer TransitionOut(Vector3 pos, float time = 2)
    {
        Fade.gameObject.SetActive(false);
        Cutout.gameObject.SetActive(true);
        Cutout.material.SetVector("_WorldPos", pos);

        float dist = getDistToCameraCorner(pos);
        return TimeKeeperReal.AddFuncTimer(time, t =>
        {
            Cutout.material.SetFloat("_Dist", Mathf.Lerp(dist, 0, Disappear.Evaluate(t)));
        });
    }
}
