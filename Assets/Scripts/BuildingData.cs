using UnityEngine;


[CreateAssetMenu(menuName = "Building")]
public class BuildingData : ScriptableObject
{
    public string DisplayName;

    [TextArea(3, 5)]
    public string Tooltip;
    public float Cost;
    public Building Prefab;

    public bool DragPlace;
}
