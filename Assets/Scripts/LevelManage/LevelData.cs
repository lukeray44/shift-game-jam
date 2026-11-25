using UnityEngine;

[CreateAssetMenu(menuName = "Level Data")]
public class LevelData : ScriptableObject
{
    public string LevelName;

    public float Money;
    public float ThreeStarsMoney;
    public float OneStarsMoney;

    public Texture2D LevelScreenshot; 
}
