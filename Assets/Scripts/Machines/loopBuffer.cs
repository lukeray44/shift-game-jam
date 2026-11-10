using UnityEngine;

public class loopBuffer : Machine
{
    public override void Animate(float percentage)
    {
        
    }

    protected override void Tick()
    {
        if (MainOutput != null) TransferItem(0, MainOutput, (Vector2)MainOutput.Building.Direction);
    }

    protected override void OnInitialiseOutputs()
    {
       
    }
    public override void Initialise() 
    {
        InitialiseInventory(1);
    }

    public override Vector3 GetItemTarget(Vector3 direction)
    {
        return MainOutput.GetItemTarget(direction);
    }

    public override bool RecieveItem(Item i, Vector3 direction)
    {
        return TrySetItemInInventory(0, i);
    } 

    public override void OnInputRemoved(Machine input)
    {
        
        SimulationManager.main.DeleteMachine(this);
    }

    public override void OnOutputRemoved(Machine input)
    { 
        SimulationManager.main.DeleteMachine(this);
    }
}
