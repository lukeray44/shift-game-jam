using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    public UIButton Play, Settings, Credits, Exit;

    public CursorButtonGroup PlayMenu, SettingsMenu, CreditsMenu;


    public Slider MainVol, MusicVol, SFXVol;
    public AudioMixerGroup MainGroup, MusicGroup, SFXGroup;
    public UIButton ResetData;

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

        void handleGroup(Slider s, AudioMixerGroup group)
        {
            string varName = s.transform.parent.name + "Vol";
            group.audioMixer.GetFloat(varName, out float vol);

            s.maxValue = 10;
            s.minValue = -30f;
            s.value = vol;

            void snap(float to, float eps = 1f)
            {
                if (Mathf.Abs(s.value - to) < eps) s.value = to;
            }

            s.onValueChanged.AddListener(a =>
            {
                snap(0);
                snap(s.minValue);
                snap(s.maxValue);

                float val = s.value;
                if (val == s.minValue) val = -80f;
                group.audioMixer.SetFloat(varName, val);
            });
        }

        handleGroup(MainVol, MainGroup);
        handleGroup(SFXVol, SFXGroup);
        handleGroup(MusicVol, MusicGroup);

        ResetData.Clicked += () => LevelMeta.main.DeleteData();

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            Exit.gameObject.SetActive(false);
        } else
        {
            Exit.Clicked += () => Application.Quit();
        }
    }
}
