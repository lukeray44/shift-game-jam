using System;
using UnityEngine;

public class OutputChute : Machine
{
    public Transform outputPos;


    public Action<Item> OnItemEntered;

    public override bool CanRecieveInDirection(Vector3 direction)
    {
        return direction == Vector3.down;
    }
    public override Vector3 GetItemTarget(Vector3 direction)
    {
        return outputPos.position;
    }
    public override bool RecieveItem(Item i, Vector3 direction)
    {
        OnItemEntered?.Invoke(i);
        Destroy(i.gameObject);
        return true;
    }
    public override void Animate(float percentage)
    {
       
    }

    protected override void Tick()
    {
        
    }

    public override bool CanRecieveFromPosition(Vector3 direction, Vector3 pos)
    {
        return pos.y > outputPos.position.y && base.CanRecieveFromPosition(direction, pos);
    }
}
