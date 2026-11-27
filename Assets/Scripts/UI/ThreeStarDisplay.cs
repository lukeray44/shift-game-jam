using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThreeStarDisplay : MonoBehaviour
{
    public Image[] Stars;
    public TextMeshProUGUI[] Text;

    public Color Success, Normal;

    public LevelData Level;

    bool[] isVisible = new bool[3];

    void animateStar(int i, bool visible = true)
    {

        if (i >= 3) return;
        if (isVisible[i] != visible)
        {
            isVisible[i] = visible;
            
            Stars[i].transform.LeanScale(Vector3.one * (visible ? 1.1f : 1), 0.1f).setEaseOutCubic();

            TimeKeeper.AddFuncTimer(0.1f, f =>
            {
                float t = Mathf.Sqrt(f);
                Stars[i].color = Color.Lerp(Normal, Success, visible ? t : 1 - t);
            });
        }

    }
    int visible;
    void showStar(int i)
    {
        isVisible[i] = true;
        Stars[i].color = Success;
        Stars[i].transform.localScale = Vector3.one * 1.1f;
    }

    public void Init(LevelData data, float money)
    {
        Level = data;
        visible = 0;
        isVisible = new bool[3];
        for (int cur = 0; cur < 3; cur++)
        {
            float target = data.GetMoneyTarget(cur);
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
        for (int i = 0; i < 3; i++)
        {
            float target = Level.GetMoneyTarget(i);

            animateStar(i, target < money);
        }

    }

}
