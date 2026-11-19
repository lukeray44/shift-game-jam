using UnityEngine;

public class Conveyor : adjacencyMachine
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
    protected override void OnInitialiseOutputs()
    {
        base.OnInitialiseOutputs();

        if (MainOutput == null)
        {
             SimulationManager.CreatePhysics(this, GetItemTarget(Vector3.left * Building.DirectionX), new Vector3(2 * Building.DirectionX, 0));
        }
        
    }

 
    protected override void Tick()
    {
        TransferItem(0, MainOutput, Vector3.right * Building.DirectionX); 
    }


    public override void OnAddToSimulation()
    {
        base.OnAddToSimulation();

        if (MainOutput is PhysicsMachine phys && !phys.Added)
        {
            SimulationManager.main.AddMachine(phys);
        }
    }

    public override void VisualiseSelf(bool foundOutput = false)
    {
        base.VisualiseSelf(foundOutput);

        if (MainOutput == null && ! foundOutput)
        {
            Visualiser.VisualisePhysics(Building, GetItemTarget(Vector3.left * Building.DirectionX), new Vector3(2 * Building.DirectionX, 0));
        }
    }

}