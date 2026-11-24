using UnityEngine;

public class LevelSet : ScriptableObject
{
    public LevelData[] Levels;

    public string LevelTitle(int i) =>  $"Level {i + 1}: \"{Levels[i].LevelName}\"";
}
