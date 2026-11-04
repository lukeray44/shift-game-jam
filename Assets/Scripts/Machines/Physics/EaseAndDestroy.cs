using UnityEngine;

public class EaseAndDestroy : Machine
{
    public AnimationCurve Ease;
    public float DistPerItem;
    public Transform OverrideTarget;
    public float Rotations;
    new public void Start()
    {
        int items = 1;

        if (MainOutput != null)
        {
            float dist = Vector3.Distance(transform.position, OverrideTarget != null ? OverrideTarget.position : MainOutput.transform.position);
            if (dist > 0) items = Mathf.CeilToInt(dist / DistPerItem);
        }
       
        InitialiseInventory(items);
        base.Start();
    }
    public override void Animate(float percentage)
    {
        Vector3 target = OverrideTarget != null ? OverrideTarget.position : MainOutput.GetItemTarget(Vector3.down);
        Vector3 start =  transform.position;
        bool easeDestroy = MainOutput == null;
        AnimateEachItem(percentage, (ind, t, item) =>
        { 
            float x = Ease.Evaluate(t);
            item.transform.position = Vector3.Lerp(start, target, x);
            item.transform.localEulerAngles = Vector3.forward * 360 * Rotations * x;
            if (easeDestroy)
            { 
                item.transform.localScale = Vector3.one * Mathf.Lerp(1, 0, t * t);
            }
        });
    } 

    public override Vector3 GetItemTarget(Vector3 direction)
    {
        return transform.position;
    } 
    
    protected override void Tick()
    {
        if (TryIncrementItems())
        {
            if (MainOutput == null) DestroyItemInInventory(0);
            else if (!TransferItem(0, MainOutput, Vector3.down)) 
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
