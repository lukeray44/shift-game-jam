using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSuccess : MonoBehaviour
{
    public TextMeshProUGUI SuccessText, MoneyCounter;

    public CanvasGroup SuccessScreen, FailureScreen, ScoreScreen;

    public CanvasGroup alpha;
    public Image Panel;

    public Color successColor = Color.yellow;
    public Color failureColor = Color.red;
    public Color normalColour = Color.grey;

    public ThreeStarDisplay Stars;


    public CursorButtonGroup Buttons;
    public UIButton Next, Restart, Exit;
    public Coroutine Show(bool success)
    {
        return Coroutines.RunCoroutine(Sequence(success));
    }
    public IEnumerator<float> Sequence(bool success)
    {
        yield return Coroutines.WaitForSeconds(0.3f);

        SuccessText.text = success ? "Victory!" : "Failure";

        SuccessScreen.gameObject.SetActive(true);

        gameObject.SetActive(true);
        alpha.alpha = 0.5f;
        alpha.LeanAlpha(1, 0.1f).setEaseOutCubic();
        transform.position += Vector3.up * 2;
        transform.LeanMove(transform.position + Vector3.down * 2, 0.1f).setEaseOutCubic();

        transform.localScale *= 1.1f;
        transform.LeanScale(transform.localScale / 1.1f, 0.1f).setEaseOutCubic();

        Panel.color = success ? successColor : failureColor;
        
        yield return Coroutines.WaitForSeconds(0.1f);

        yield return Coroutines.WaitForSeconds(0.5f);

        void changeScreen(CanvasGroup to)
        {
            Color start = Panel.color;
            TimeKeeperReal.AddFuncTimer(0.2f, f =>
            {
                Panel.color = Color.Lerp(start, normalColour, f);
            });

            SuccessScreen.LeanAlpha(0, 0.2f).setEaseOutCubic();
            to.gameObject.SetActive(true);
            to.alpha = 0;
            to.LeanAlpha(1, 0.2f).setEaseOutCubic();

            SuccessScreen.transform.LeanMove(SuccessScreen.transform.position + Vector3.left * 2, 0.2f).setEaseOutCubic();
            to.transform.position += Vector3.right * 2;
            to.transform.LeanMove(to.transform.position + Vector3.left * 2, 0.2f).setEaseOutCubic();
        }

        changeScreen(success ? ScoreScreen : FailureScreen);
        MoneyCounter.text = "You saved: $0";

        Stars.Init(LevelMeta.main.CurData, 0);

        yield return Coroutines.WaitForSeconds(0.2f);

        if (success)
        {
            

            float moneySaved = LevelManager.main.Money;

            float lastMoney = 0;
            void setText(float proportion)
            {
                float money = Mathf.RoundToInt(moneySaved * proportion);

                Stars.AnimateCompletion(money);

                lastMoney = money;

                MoneyCounter.text = "You saved: $" + money.ToString();
            }

            yield return TimeKeeperReal.MECCustomTween(3, t =>
            {
               setText(LeanTween.easeOutCubic(0, 1, t)); 
            });
        }

        yield return Coroutines.WaitForSeconds(2f);

        Buttons.Show();
    }
}
