using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelPanel : MonoBehaviour
{
    public TextMeshProUGUI Title, Completion, Best;

    public CoolButton Play;

    public RawImage LevelPreview;

    public ThreeStarDisplay Stars;

    public void LoadLevel(int index = -1)
    {
        if (index == -1)
        {
            Title.text = "Select a Level";
            Completion.text = "";
            Best.text = "";
            Play.gameObject.SetActive(false);
            LevelPreview.gameObject.SetActive(false);
            Stars.gameObject.SetActive(false);
        }
        else
        {
            LevelData data = LevelMeta.main.Levels.Levels[index];
            LevelSaveData save = LevelMeta.main.Data.Data[index];
            Debug.Log(save == null);

            Title.text = LevelMeta.main.Levels.LevelTitle(index);
            Play.gameObject.SetActive(true);
            LevelPreview.gameObject.SetActive(true);
            LevelPreview.texture = data.LevelScreenshot;

            

            if (save != null && save.Beaten)
            {
                Completion.text = "Level Completed!";
                Best.text = $"Best: ${save.BestMoney}<size=70%>/${data.Money}";
                Stars.gameObject.SetActive(true);
                Stars.Init(data, save.BestMoney);
            }
            else
            {
                  Stars.gameObject.SetActive(false);
                Completion.text = "Not Completed";
                Best.text = "";

            }
        }
    }
}
