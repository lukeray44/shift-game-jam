using System.Collections.Generic;
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

        TimeKeeper.AddTimer(0.1f).OnTimerEnd += () =>
        {


           
            
            

            if (LevelMeta.main.Cur != -1)
            {
                OnSelect(Children[LevelMeta.main.Cur], LevelMeta.main.Cur);

                 CutoutTransition.main.TransitionIn(Panel.LevelPreview.transform.position, 3f);
            } else
            {
                 CutoutTransition.main.FadeIn(0.2f);
            }
        };

        if (LevelMeta.main.Cur != -1)
        {
           
        }
        else
        {
            Panel.LoadLevel(LevelMeta.main.Cur);
        }
        Panel.Play.Clicked += () =>
        {
            LevelMeta.main.LoadLevel(curLevel, Panel.LevelPreview.transform.position);
        };

        Back.Clicked += () => LevelMeta.main.LoadScene("Title");

    }

    public List<CoolButton> Children = new List<CoolButton>();

    void AddLevel(int level)
    {
        CoolButton b = Instantiate(Prefab, ButtonParent);
        LevelData data = LevelMeta.main.Levels.Levels[level];
        b.Text.text = $"{level + 1} - {data.LevelName}";

        Children.Add(b);
        b.Clicked += () => OnSelect(b, level);
    }

    void OnSelect(CoolButton b, int level)
    {
        if (selected == b)
        {
            Deselect();
            cursor.Hide();
        }
        else
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
