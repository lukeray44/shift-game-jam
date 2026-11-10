using UnityEngine;

public class Conveyor : Machine
{
    public override void Initialise()
    {
        InitialiseInventory(1);
        base.Initialise();
    }
    public override void Animate(float percentage)
    {
        Item i = GetItemInInventory(0);
        if (i != null && !ItemIsStuck(0))
        {
            Vector3 end = MainOutput == null ? GetItemTarget(Vector3.left * Building.DirectionX) : MainOutput.GetItemTarget(Vector3.right * Building.DirectionX);

            i.transform.position = Vector3.Lerp(i.PreviousPosition, end, percentage);

        }
    } 
 
    protected override void Tick()
    {
        TransferItem(0, MainOutput, Vector3.right * Building.DirectionX); 
    }

    public override Vector3 GetItemTarget(Vector3 direction) 
    {
        Vector3 facing = transform.right;
        float sign = Building.DirectionX;
        if (direction.x == -Building.DirectionX) sign *= -1; 
        return transform.position + Vector3.up * 0.25f - facing * 0.5f * sign - facing * Building.DirectionX * 0.2f;
    }

    public override bool RecieveItem(Item i, Vector3 direction)
    { 
        Debug.Log(direction.x + " vs " + Building.Direction.x);
        if (direction.x == Building.Direction.x || direction.x == 0) 
        {
            bool success = TrySetItemInInventory(0, i); 
            if (success) LoopInputs();
            return success;
        } 
        return false;
    }

}