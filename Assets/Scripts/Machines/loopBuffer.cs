using UnityEngine;

public class loopBuffer : Machine
{
    public override void Animate(float percentage)
    {
        
    }

    protected override void Tick()
    {
        TransferItem(0, MainOutput, (Vector2)MainOutput.Building.Direction);
    }

    protected override void OnInitialiseOutputs()
    {
        if (MainOutput == null) SimulationManager.main.RemoveMachine(this);
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
}
