using System;
using UnityEngine;
using UnityEngine.Rendering;

public class OutputChute : Machine
{
    public Transform outputPos;


    public Action<Item> OnItemEntered;

    public void ShowPreview(ItemComposite item)
    {
        Item i = ItemManager.main.Create(item, null, true);
        i.GetComponent<SortingGroup>().sortingOrder += 50;
        i.transform.position = outputPos.position + Vector3.down * 2;
    }
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
