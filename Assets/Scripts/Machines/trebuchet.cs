using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class Trebuchet : Conveyor
{

    public Transform ShootFrom;
    protected override void OnInitialiseOutputs()
    {
         SimulationManager.CreatePhysicsAndAdd(this, ShootFrom.position, velocity);
    }

    Vector3 velocity => new Vector3(1 * Building.DirectionX, 1.5f) * 6;

    public override bool RecieveItem(Item i, Vector3 direction)
    {
        
        return base.TrySetItemInInventory(0, i);
    }

    public override void VisualiseIfOutput(Building b)
    {
        
    }
    public override bool CanRecieveInDirection(Vector3 direction)
    {
        return true;
    }

    protected override void Tick()
    {
        if (!IsEmptyInInventory(0)) Activate();
        base.Tick();
    }

    public override void VisualiseSelf(bool foundOutput = false)
    {
        if (MainOutput == null)
        {
            Visualiser.VisualisePhysics(Building, ShootFrom.position, velocity);
        }
    }

    public override void Animate(float percentage)
    {
        if (!IsEmptyInInventory(0))
        {
            Item i = GetItemInInventory(0);

            if (Piecewise(percentage, 0f, 0.35f, out float p))
            {
                i.transform.position = Vector3.Lerp(i.PreviousPosition, transform.position, p);
            } else if (Piecewise(percentage, 0.35f, 0.7f, out float p2))
            {
                i.transform.position = Vector3.Lerp(transform.position, ShootFrom.position, p2);
            }
            
        }

        ActivateAnim(percentage);
    }

    public override Vector3 GetItemTarget(Vector3 direction)
    {
        if (direction == Vector3.down) return Vector3.Lerp(GetItemTarget(Vector3.right), GetItemTarget(Vector3.left), 0.5f);
        return base.GetItemTarget(direction);
    }
}
