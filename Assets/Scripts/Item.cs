using System;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemState PreviousState;
    public Machine parent;

    public SpriteRenderer Sprite;

    public Vector3 PreviousPosition => PreviousState.position;
    void Start()
    {
        PreviousState = new ItemState(this);
    }
    
    public void SavePosition()
    {
        PreviousState = new ItemState(this);
    }
}

[Serializable]
public struct ItemState
{
    public Vector3 position;
    public Vector3 rotation;
    public Color color;

    public ItemState(Vector3 position)
    {
        this.position = position;
        rotation = new Vector3();
        color = Color.white;
    }
    public ItemState(Vector3 position, Vector3 rotation)
    {
        this.position = position;
        this.rotation = rotation;
         color = Color.white;
    }

    public ItemState(Item t)
    {
        position = t.transform.position;
        rotation = t.transform.localEulerAngles;
        color = t.Sprite.color;
    }


}