using UnityEngine;

public class stacker : Conveyor
{
    public Transform targetPos;

    public override void Initialise()
    {
        base.Initialise();

        InitialiseInventory(3);
    }


    public override bool RecieveItem(Item i, Vector3 direction)
    {

        if (direction == Vector3.down) return TrySetItemInInventory(1, i);
        return base.RecieveItem(i, direction);
    }


    protected override void Tick()
    {
        TransferItem(2, MainOutput, Vector3.right * Building.DirectionX);

        Item input1 = GetItemInInventory(0);
        Item input2 = GetItemInInventory(1);

        if (input1 != null && input2 != null)
        {
            RemoveItemInInventory(0);
            RemoveItemInInventory(1);

            Item combined = ItemManager.main.Merge(input1, input2, Vector2Int.up);
            combined.PositionChildren();
            TrySetItemInInventory(2, combined);
        }
        else
        {
            if (input1 != null) SetItemIsStuck(0);
            if (input2 != null) SetItemIsStuck(1);
        }
    }

    public override Vector3 GetItemTarget(Vector3 direction)
    {
        if (direction == Vector3.down) return targetPos.position;
        return base.GetItemTarget(direction);
    }

    public override void Animate(float percentage)
    {
        Vector3 inputPos = GetItemTarget(Vector3.right * Building.DirectionX);
        Vector3 outputPos = GetItemTarget(Vector3.left * Building.DirectionX);

        AnimateEachItem(percentage, (i, t, item) =>
        {
            if (i == 0 || i == 2)
            {
                float completion = (percentage + (i == 2 ? 1 : 0)) * 0.5f;
                item.transform.position = Vector3.Lerp(inputPos, outputPos, completion);
            } else
            {
                item.transform.position = Vector3.Lerp(targetPos.position, targetPos.position + Vector3.down * 0.5f, t);
            }
        });
    }

    void OnDrawGizmos()
    {
         Vector3 inputPos = GetItemTarget(Vector3.right * Building.DirectionX);
        Vector3 outputPos = GetItemTarget(Vector3.left * Building.DirectionX);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(inputPos, outputPos);
    }
}
