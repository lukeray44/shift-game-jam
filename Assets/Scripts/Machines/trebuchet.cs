using UnityEngine;

public class Trebuchet : Machine
{
    public AnimationCurve Arc;
    public float DistPerItem;
    new public void Start()
    {

        float dist = Vector3.Distance(transform.position, MainOutput.transform.position);
        int items = Mathf.CeilToInt(dist / DistPerItem) + 1;
        InitialiseInventory(items);
        base.Start();
    }
    public override void Animate(float percentage)
    {
        Vector3 target = MainOutput.GetItemTarget(Vector3.down);
        Vector3 start =  transform.position + Vector3.up * 0.5f;
        
        AnimateEachItem(percentage, (ind, t, item) =>
        { 
            if (ind == 0)
            {
                item.transform.position = Vector3.Lerp(item.PreviousPosition, start, percentage);
            }
            else
            {
                if (Piecewise(t, 1.5f, out float p))
                {  
                    item.transform.position = Vector3.Lerp(start, target, p)
                        + Arc.Evaluate(p) * Vector3.up * 7;
                    item.transform.localEulerAngles = item.PreviousState.rotation + Vector3.forward * 360 * p * Mathf.Ceil(0.5f * (InventoryLength - 1));
                }
                

            }
        });
    } 

    public override Vector3 GetItemTarget(Vector3 direction)
    {
        return transform.position +- transform.right * 0.5f + Vector3.up * 0.2f;
    }
    
    protected override void Tick()
    {
        if (TryIncrementItems())
        {
            if (!TransferItem(0, MainOutput, Vector3.down)) 
            {
                UndoIncrementAndCompress();    
            }
        }
    }

    public override bool RecieveItem(Item i, Vector3 direction)
    { 
        return TrySetItemInInventory(0, i);
    }

    
}