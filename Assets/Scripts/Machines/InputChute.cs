using UnityEngine;

public class InputChute : Machine
{
    public int SpawnPeriod;
    public ItemByRows ToSpawn;
    

    public Transform dropFrom;

    int timer;

    public override void Initialise()
    {
        base.Initialise();
        InitialiseInventory(SpawnPeriod);
    }
    protected override void OnInitialiseOutputs()
    {
        SimulationManager.CreatePhysics(this, dropFrom.position, Vector2.zero);
    }

    public override void OnAddToSimulation()
    {
        base.OnAddToSimulation();
        SimulationManager.main.AddMachine(MainOutput);
    }

    public override void OnBuildingAdded(Building b)
    {
         
    }
    protected override void Tick()
    {
        if (TryIncrementItems())
        {
            if (!TransferItem(0, MainOutput, Vector2.zero))
            {
                DestroyItemInInventory(0);
            }
        }


        timer += 1;
        if (timer >= SpawnPeriod)
        {
            Item i = ItemManager.main.CreateByRows(ToSpawn);
            i.transform.position = dropFrom.position + Vector3.up;
            if (!RecieveItem(i, Vector3.zero))
            {
                Destroy(i.gameObject);
            } else 
            {
                 timer = 0;
            }

        }
    }
    public override bool RecieveItem(Item i, Vector3 direction)
    {
        return TrySetItemInInventory(0, i);
    }

    public override void Animate(float percentage)
    {
        AnimateEachItem(percentage, (ind, t, i) =>
        {
             i.transform.position = Vector3.Lerp(dropFrom.position + Vector3.up, dropFrom.position, Mathf.Sqrt(t));
        });
    }

    
}
