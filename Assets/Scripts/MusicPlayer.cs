using Sirenix.OdinInspector;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip Intro, Main, Alternate;

    public bool PlayOnStart = true;
    public bool ForceIntro = false;
    void Start()
    {
        if (PlayOnStart) Play();
    }

    [Button]
    public void Play()
    {
        if (Sound.sound != null && Sound.sound.main != null && Alternate != null && Sound.sound.main.clip == Alternate) Sound.sound.Replace(Main);
        else Sound.setTrack(Intro == null ? Main : Intro, Intro == null ? null : Main, true, ForceIntro);
    }

    [Button]
    public void Swap()
    {
        Sound.sound.Replace(Alternate);
    }
}
