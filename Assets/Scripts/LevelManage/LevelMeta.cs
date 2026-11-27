using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class LevelMeta : Singleton<LevelMeta>
{
    public LevelSet Levels;
    public int Cur;
    public LevelData CurData => Levels.Levels[Cur];

    public AudioMixerGroup SFX, Music;
    bool isAnimating;

    public bool isNextLevel => Cur < Levels.Levels.Length - 1;

    void Awake()
    {
        Sound.set(Music, SFX);
        base.Awake();
    }
    protected override void onInitialise()
    {
        base.onInitialise();
        Load();

        DontDestroyOnLoad(gameObject);
    }
    public void LoadLevel(int level, Vector3? zoomPos = null)
    {

        Cur = level;

        async void loadLevel()
        {
            await SceneManager.LoadSceneAsync($"Level " + level.ToString());
            isAnimating = false;
        }

        if (zoomPos == null) CutoutTransition.main.FadeOut(0.15f).OnTimerEnd += () => loadLevel();
        else
        {
            TimeKeeper.AddFuncTimer(1.5f, f => Sound.sound.main.volume = 1 - f).OnTimerEnd += () =>
            {
                Sound.sound.main.volume = 0;
                Sound.sound.main.Stop();
            };

            CutoutTransition.main.TransitionOut(zoomPos.Value, 1.5f).OnTimerEnd += () => loadLevel();
        }
    }

    public void LoadScene(string scene)
    {
        if (isAnimating) return;
        isAnimating = true;
        CutoutTransition.main.FadeOut(0.3f).OnTimerEnd += () =>
        {
            SceneManager.LoadScene(scene);
            isAnimating = false;
        };
    }

    public string LevelTitle => Levels.LevelTitle(Cur);

    public SaveData Data;

    string path => Application.persistentDataPath + "/save.dat";
    public void Load()
    {
        try
        {
            Data = BinaryUtility.Load(path) as SaveData;
            Data.EnsureLength(Levels.Levels.Length);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            Data = new SaveData(Levels.Levels.Length);

        }
    }

    public void Save()
    {
        BinaryUtility.Save(Data, path);
    }

    public void Win(float money)
    {
        LevelSaveData s = Data.Data[Cur];

        s.BestMoney = Mathf.Max(s.BestMoney, money);
        s.Beaten = true;
        Save();
    }

    public void DeleteData()
    {
        Data = new SaveData(Levels.Levels.Length);
        Save();
    }
}

[System.Serializable]
public class SaveData
{
    public LevelSaveData[] Data;

    public SaveData(int count)
    {
        Data = new LevelSaveData[count];
        for (int i = 0; i < count; i++)
        {
            Data[i] = new LevelSaveData();
        }
    }
    public void EnsureLength(int length)
    {
        LevelSaveData[] newData = new LevelSaveData[length];

        int count = Mathf.Min(length, Data.Length);
        for (int i = 0; i < count; i++)
        {
            newData[i] = Data[i];
        }
        Data = newData;
    }
}
[System.Serializable]
public class LevelSaveData
{
    public bool Beaten;
    public float BestMoney = 0;
}

public static class BinaryUtility
{

    // usage: Save(PlayerData,  Application.persistentDataPath + "/playerdata.dat")
    public static void Save(object o, string path)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, o);
        stream.Close();
    }

    public static object Load(string path)
    {
        if (File.Exists(path))
        {

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            try
            {
                object o = formatter.Deserialize(stream);
                stream.Close();
                return o;
            }
            catch
            {
                stream.Close();
                throw new System.Exception("Corrupt");
            }


        }
        else
        {

            throw new System.Exception("Invalid path: " + path);

        }
    }
}
