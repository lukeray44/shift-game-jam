using UnityEngine;

public class LevelSelect : MonoBehaviour
{
    public CoolButton Prefab, Back;

    public Transform ButtonParent;
    public LevelPanel Panel;
    CoolButton selected;

    public UICursor cursor;

    void Start()
    {
        for (int i = 0; i < LevelMeta.main.Levels.Levels.Length; i++)
        {
            AddLevel(i);
        }

        CutoutTransition.main.FadeIn(0.2f);
        Panel.LoadLevel(-1);

        Panel.Play.Clicked += () =>
        {
            LevelMeta.main.LoadLevel(curLevel);
        };

        Back.Clicked += () => LevelMeta.main.LoadScene("Title");

    }

    void AddLevel(int level)
    {
        CoolButton b = Instantiate(Prefab, ButtonParent);
        LevelData data = LevelMeta.main.Levels.Levels[level];
        b.Text.text = $"{level + 1} - {data.LevelName}";

        b.Clicked += () => OnSelect(b, level);
    }
 
    void OnSelect(CoolButton b, int level)
    {
        if (selected == b)
        {
            Deselect();
            cursor.Hide();
        } else
        {
            Deselect();
            selected = b;
            b.SetSelected(true);
            cursor.Select(b.transform.position);
            LoadInfo(level);
        }
    }
    int curLevel;
    public void LoadInfo(int level)
    {
        curLevel = level;
        Panel.LoadLevel(level);
    }

    void Deselect()
    {
        if (selected != null)
        {
            selected.SetSelected(false);
            selected = null;
            Panel.LoadLevel(-1);
        }
    }
    

}
