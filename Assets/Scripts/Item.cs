using System;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemState PreviousState;
    public Machine parent;
    public Vector3 PreviousPosition => PreviousState.position;
    void Start()
    {
        PreviousState = new ItemState(transform);
    }
    
    public void SavePosition()
    {
        PreviousState = new ItemState(transform);
    }
}

[Serializable]
public struct ItemState
{
    public Vector3 position;
    public Vector3 rotation;

    public ItemState(Vector3 position)
    {
        this.position = position;
        rotation = new Vector3();
    }
    public ItemState(Vector3 position, Vector3 rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }

    public ItemState(Transform t)
    {
        position = t.position;
        rotation = t.localEulerAngles;
    }


}