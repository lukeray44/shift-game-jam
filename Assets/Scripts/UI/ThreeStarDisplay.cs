using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThreeStarDisplay : MonoBehaviour
{
    public Image[] Stars;
    public TextMeshProUGUI[] Text;

    public Color Success, Normal;

    public LevelData Level;

    void animateStar(int i)
    {
        
        if (i >= 3) return;
        Stars[i].transform.LeanScale(Vector3.one* 1.1f, 0.1f).setEaseOutCubic();

        TimeKeeper.AddFuncTimer(0.1f, f =>
        {
            Stars[i].color = Color.Lerp(Normal, Success, Mathf.Sqrt(f));
        });
    }
    int visible;
    void showStar(int i)
    {
        
        Stars[i].color = Success;
        Stars[i].transform.localScale = Vector3.one * 1.1f;
    }

    public void Init(LevelData data, float money)
    {
        Level = data;
        visible = 0;
        for (int cur = 0; cur < 3; cur++)
        {
            float target = data.GetMoneyTarget(cur);
            Debug.Log(cur + " " + target);
            Stars[cur].color = Normal;
            Stars[cur].transform.localScale = Vector3.one;

            if (target < money)
            {
                showStar(cur);
                visible++;
            }
            
            Text[cur].text = "$" + target;
        }
    }


    public void AnimateCompletion(float money)
    {
        if (visible >= 3) return;

        float target = Level.GetMoneyTarget(visible);
        
        if (target < money)
        {
            animateStar(visible);
            visible++;
        }
    }

}
