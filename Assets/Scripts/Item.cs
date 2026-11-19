using System;
using UnityEngine;

public class Item : MonoBehaviour
{
    public Machine parent;
    public Vector3 PreviousPosition;

    public void SavePosition()
    {
        PreviousPosition = transform.position;
    }
}
