using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    public UIButton Play, Settings, Credits, Exit;

    public CursorButtonGroup PlayMenu, SettingsMenu, CreditsMenu;

    CursorButtonGroup current;
    public void ChangeMenu(CursorButtonGroup to, float dir = 1)
    {
        if (current != null)
        {
            current.Hide(dir);
        }

        current = to;
        current.Show(dir);
    }
    public void SubscribeMenus(UIButton link, CursorButtonGroup from, CursorButtonGroup to)
    {
        link.Clicked += () => ChangeMenu(to); 
        to.Quit.Clicked += () => ChangeMenu(from, -1); 
    }

    void Start()
    {
        SubscribeMenus(Settings, PlayMenu, SettingsMenu);
        SubscribeMenus(Credits, PlayMenu, CreditsMenu);
        Play.Clicked += () => LevelMeta.main.LoadScene("Level Select");

        TimeKeeper.AddTimer(1f).OnTimerEnd += () => ChangeMenu(PlayMenu);
    }
}
