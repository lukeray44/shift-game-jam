using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class soundManager : MonoBehaviour
{
    public AudioMixerGroup sounds;
    public AudioMixerGroup music;
    public AudioClip secondlastIntro;
    AudioClip lastIntro;
    public AudioClip last;
    public string currentSongName;
    public float dipBefore = -999;
    public int Amount;
    private void Awake()
    {
        this.set();
    }

    public void play(AudioClip track, float pitch, float volume)
    {
        pitch = pitch.RoundTo(0.2f);
        foreach (CachedSource source in cachedSources)
        {
            if ((source.source.pitch == pitch && source.source.volume == 1) || source.time < Time.time)
            {
                if (source.time != -1)
                {
                    source.time = -1;
                }
                source.source.pitch = pitch;
                source.source.volume = 0.9f;
                source.source.PlayOneShot(track, volume);
                source.time = Mathf.Max(source.time, track.length + Time.time);
                return;
            }
        }
        
        AudioSource au = gameObject.AddComponent<AudioSource>();
        Amount += 1;
        au.outputAudioMixerGroup = sounds;
        au.pitch = pitch;
        au.volume =1f;
        au.PlayOneShot(track, volume);

        cachedSources.add(new CachedSource()
        {
            clip = track,
            time = Time.time + track.length,
            source = au
        });
    }
    public void playDip(AudioClip track, float padding, float pitch, float volume)
    {

        play(track, pitch, volume);
        dipBefore = Time.unscaledTime + track.length - padding;
    }
    public AudioSource GetOwnSource()
    {
        Amount += 1;
        AudioSource au =  gameObject.AddComponent<AudioSource>();
        au.outputAudioMixerGroup = sounds;
        return au;
    }
    public AudioSource playWithOwnSource(AudioClip track, float pitch, float volume)
    {

        foreach (CachedSource source in cachedSources)
        {
            if (source.time < Time.time)
            {
                if (source.time != -1)
                {
                    source.time = -1;
                }
                source.source.pitch = pitch;
                source.source.volume = volume;
                source.source.clip = track;
                source.source.Play();
                if (source.time < track.length + Time.time) source.time = Time.time + track.length;
                return source.source;
            }
        }
        Amount += 1;

        AudioSource au = gameObject.AddComponent<AudioSource>();
        au.outputAudioMixerGroup = sounds;
        au.pitch = pitch;
        au.volume = volume;
        au.clip = track;
        au.Play();

        cachedSources.add(new CachedSource()
        {
            clip = track,
            time = Time.time + track.length,
            source = au
        });

        return au;
    }

    public void AddOverLayer(AudioClip layer)
    {
        if (mainOverlayer == null)
        {
            mainOverlayer = gameObject.AddComponent<AudioSource>();
            mainOverlayer.outputAudioMixerGroup = music;

        }
        mainOverlayer.clip = layer;
        
        mainOverlayer.volume = 0;
        mainOverlayer.Play();
        mainOverlayer.timeSamples = main.timeSamples;
        mainOverlayer.loop = true;
        mainOverlayer.priority = 0;
        TimeKeeperReal.AddFuncTimer(0.3f, f => mainOverlayer.volume = f * 1f);
    }
    public void AddOverLayerOverride(AudioClip layer, GameObject parent)
    {
        mainOverlayer = parent.AddComponent<AudioSource>();
        mainOverlayer.outputAudioMixerGroup = music;
        mainOverlayer.clip = layer;
        mainOverlayer.priority = 0;
        mainOverlayer.volume = 0;
        mainOverlayer.Play();
        mainOverlayer.timeSamples = main.timeSamples;
        mainOverlayer.loop = true;
        TimeKeeperReal.AddFuncTimer(0.3f, f => mainOverlayer.volume = f * 1f);
    }
    public void Replace(AudioClip layer)
    {
        if (nextScheduledAt > AudioSettings.dspTime) // clip is in future
        {
            main.clip = layer;
            return;
        }

        mainOverlayer = gameObject.AddComponent<AudioSource>();
        mainOverlayer.outputAudioMixerGroup = music;
        mainOverlayer.clip = layer;
        currentSongName = layer.name;
        mainOverlayer.volume = 0;
        mainOverlayer.Play();
        mainOverlayer.timeSamples = main.timeSamples;
        mainOverlayer.loop = true;
        TimeKeeperReal.AddFuncTimer(1f, f =>
        {
            enabled = false;
            mainOverlayer.volume = f * 1f;
            main.volume = 1f - f;
        }).OnTimerEnd += () =>
        {
            enabled = true;
            main.Stop();
            main.clip = layer;
            main.timeSamples = mainOverlayer.timeSamples;
            main.Play();
            mainOverlayer.Stop();
        };
    }
    public AudioSource main;
    public AudioSource mainOverlayer;
    public AudioClip trackToAdd;
    public float time;
    public AudioClip afterIntro;
    RealTimer cur;
    public void setTrack(AudioClip toAdd, AudioClip tafterIntro, bool loop = true, bool force = false)
    {

        if (main == null)
        {
            main = gameObject.AddComponent<AudioSource>();
            main.outputAudioMixerGroup = music;

        }
        if (toAdd != trackToAdd && (tafterIntro != main.clip || tafterIntro == null) || force)
        {
            currentSongName = toAdd.name;
            secondlastIntro = lastIntro;
            last = main.clip;
            if (tafterIntro)
            {
                lastIntro = toAdd;

            }


            trackToAdd = toAdd;
            time = toAdd.length;
            afterIntro = tafterIntro;

            main.loop = loop && tafterIntro == null;
            if (cur != null) cur.Cancelled = true;
        }

    }


    public void endTrack()
    {
        if (main != null)
        {
            main.clip = null;
            main.Stop();
        }
    }
    public void SetAfterIntro()
    {
        trackToAdd = afterIntro;
        main.clip = afterIntro;
        afterIntro = null;
        main.Play();
    }


    double nextScheduledAt = 0;
    private void Update()
    {
       // if (mainOverlayer != null) mainOverlayer.timeSamples = main.timeSamples;
        if (main == null)
        {
            main = gameObject.AddComponent<AudioSource>();
            main.outputAudioMixerGroup = music;
            main.priority = 0;
        }


        if (trackToAdd != main.clip && trackToAdd != null)
        {
            last = main.clip;
            main.volume += (0 - main.volume) * 0.15f;
            if (mainOverlayer != null)
                mainOverlayer.volume = main.volume;

            //changing track
            if (main.volume < 0.1f)
            {
                main.Stop();
                if (mainOverlayer != null) mainOverlayer.Stop();
                if (afterIntro == null)
                {
                    main.clip = trackToAdd;
                    main.timeSamples = 0;
                    main.Play();
                } else
                {

                    main.timeSamples = 0;
                    main.PlayOneShot(trackToAdd);
                    trackToAdd = null;
                    main.clip = afterIntro;
                    afterIntro = null;
                    nextScheduledAt = AudioSettings.dspTime + time;
                    main.PlayScheduled(AudioSettings.dspTime + time);
                    main.loop = true;
                }
                trackToAdd = null;
            }
        }
        else
        {
            float vol = 1f;
            if (Time.unscaledTime < dipBefore)
            {
                vol = 0.1f;
            }
            main.volume += (vol - main.volume) * 0.15f;

        }


    }
    private EfficientList<CachedSource> cachedSources = new EfficientList<CachedSource>(15);

}
[System.Serializable]
public class LoopSource
{
    public GameObject parent;
    public AudioSource au;
    public float fade;
    public float distanceMulti = 1;
    float vol;
    bool proximity;
    Transform cam;
    public LoopSource(GameObject parent, AudioClip ac, bool prox = true)
    {
        this.parent = parent;
        cam = Camera.main.transform;
        au = parent.AddComponent<AudioSource>();
        au.outputAudioMixerGroup = Sound.sound.sounds;
        au.clip = ac;
        au.loop = true;
        proximity = prox;
        au.volume = 0;
        au.Play();

    }
    public void fadeIn()
    {
        TimeKeeper.AddFuncTimer(0.4f, f =>
        {
            fade = f;
            SetVol();
        }).TieTo(parent);
    }
    public void fadeOut()
    {
        TimeKeeper.AddFuncTimer(0.4f, f =>
        {
            fade = f;
            SetVol();
        }, 1, 0).TieTo(parent);
    }
    public void SetVol(float v)
    {
        if (parent == null) return;
        if (proximity)
        {
            float t = MathEXT.realfLerp(1, 0, Mathf.Min(Vector2.Distance(cam.position, parent.transform.position) / 15f, 1));
            v *= t;
        }
        vol = v;
        au.volume = v * fade;
    }
    public void SetVol()
    {
        if (parent == null) return;
        float v = vol * fade;
        if (proximity && cam != null && parent != null)
        {

            float t = MathEXT.realfLerp(0, 1, Mathf.Min(Vector2.Distance(cam.position, parent.transform.position) / 15f * distanceMulti, 1));
            v *= t;
        }
        au.volume = v;
    }
}
public class CachedSource
{
    public AudioSource source;
    public float time;
    public AudioClip clip;
}
public static class Sound
{
    public static Camera c;
    public static Camera cam
    {
        get
        {
            if (c == null) c = Camera.main;

            return c;
        }
    }
    public static void setMainVolume(float volume)
    {
        sound.main.volume = volume;
    }
    public static Dictionary<string, AudioClip> record = new Dictionary<string, AudioClip>();
    public static soundManager sound;
    public static void set(this soundManager m)
    {
        if (sound == m) return;
        if (sound != null)
        {
            GameObject.Destroy(m);
            return;
        }

        sound = m;
        GameObject.DontDestroyOnLoad(m);
    }
    public static void set(  AudioMixerGroup music, AudioMixerGroup effects)
    {
        if (sound == null)
        {
            sound = new GameObject("~soundManager").AddComponent<soundManager>();
        }

        sound.music = music;
        sound.sounds = effects;
        GameObject.DontDestroyOnLoad(sound);
    }
    public static void PlayWithDefaultVariation(AudioClip track, float volume = 1)
    {
        PlayWithVariation(track, 0.8f, 1.3f, volume);
    }
    public static void PlayWithVariation(AudioClip track, float minPitch = 0.8f, float maxPitch = 1.3f, float volume = 1)
    {
        Play(track, Mathf.Round( UnityEngine.Random.Range(minPitch, maxPitch) * 10) / 10, volume);
    }
    public static void Play(AudioClip track, float pitch = 1, float volume = 1)
    {
        if (sound == null)
        {
            sound = new GameObject("~soundManager").AddComponent<soundManager>();
            sound.set();
        }
        if (track == null) return;

        sound.play(track, pitch, volume);

    }
    public static void PlayStinger(AudioClip track, float padding, float pitch = 1, float volume = 1)
    {
        if (sound == null)
        {
            sound = new GameObject("~soundManager").AddComponent<soundManager>();
            sound.set();
        }
        if (track == null) return;

        sound.playDip(track, pitch, volume, padding);

    }
    public static AudioSource PlayWithOwnSource(AudioClip track, float pitch = 1, float volume = 1)
    {
        if (sound == null)
        {
            sound = new GameObject("~soundManager").AddComponent<soundManager>();
            sound.set();
        }
        if (track == null) return null;

        return sound.playWithOwnSource(track, pitch, volume);

    }
    static float fadeOut(float t)
    {
        return MathEXT.realfLerp(1, -2, Mathf.Clamp(t - 0.2f, 0, 1));
    }
    public static void PlayProximity(AudioClip track, Transform t, float pitch = 1, float volume = 1)
    {
        if (sound == null)
        {
            sound = new GameObject("~soundManager").AddComponent<soundManager>();
            sound.set();
        }
        if (track == null) return;

        float f = MathEXT.realfLerp(1, -2, Mathf.Clamp01(Vector2.Distance(t.position, cam.transform.position) / 15));

        sound.play(track, pitch, f * volume);

    }
    public static AudioClip setTrack(AudioClip au, AudioClip afterAu = null, bool looping = false, bool force = false)
    {
        if (sound == null)
        {
            sound = new GameObject("~soundManager").AddComponent<soundManager>();
            sound.set();
        }


        sound.setTrack(au, afterAu, looping, force);

        return au;
    }

    public static void RevertToLast()
    {
        Debug.Log("Happening");
        if (sound.secondlastIntro != null)
        {
            sound.setTrack(sound.secondlastIntro, sound.last);
        } else
        {
            Debug.Log(sound.last == null);
            sound.setTrack(sound.last, null);
        }
        
    }

}