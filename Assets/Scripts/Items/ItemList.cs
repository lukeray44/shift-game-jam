using UnityEngine;

[CreateAssetMenu(menuName = "Item List")]
public class ItemList : ScriptableObject
{
    public ItemType[] Types;
}

[System.Serializable]
public class ItemType
{
    public Sprite sprite;
    public SymmetryType symmetry;

}

public enum SymmetryType
{
    NoSymmetry = 4,
    OneSymetry = 2,
    FullSymetry = 1
}