using UnityEngine;

public class Trebuchet : Conveyor
{


    protected override void OnInitialiseOutputs()
    {
         SimulationManager.CreatePhysics(this, transform.position, velocity);
    }

    Vector3 velocity => new Vector3(1 * Building.DirectionX, 1.5f) * 6;

    public override bool RecieveItem(Item i, Vector3 direction)
    {
        return base.TrySetItemInInventory(0, i);
    }

    public override void VisualiseIfOutput(Building b)
    {
        
    }


    public override void VisualiseSelf(bool foundOutput = false)
    {
        if (MainOutput == null)
        {
            Visualiser.VisualisePhysics(Building, transform.position, velocity);
        }
    }
}
