using UnityEngine;

public class Conveyor : Machine
{
    public void Start()
    {
        InitialiseInventory(1);
    }
    public override void Animate(float percentage)
    {
        Item i = GetItemInInventory(0);
        if (i != null && !ItemIsStuck(0))
        {
            Vector3 end = MainOutput == null ? GetItemTarget(Vector3.left) : MainOutput.GetItemTarget(Vector3.right);

            i.transform.position = Vector3.Lerp(GetItemTarget(Vector3.right), end, percentage);
  
             //i.transform.position = Vector3.Lerp(i.PreviousPosition, i.NextPosition, percentage);
        }
    } 

    protected override void Tick()
    {
        TransferItem(0, MainOutput, Vector3.right);
    }

    public override Vector3 GetItemTarget(Vector3 direction)
    {
        Vector3 facing = transform.right;
        float sign = Mathf.Sign(direction.x);
        return transform.position + Vector3.up * 0.25f - facing * 0.5f * sign - facing * 0.2f;
    }

    public override bool RecieveItem(Item i, Vector3 direction)
    { 
        return TrySetItemInInventory(0, i);
    }

}