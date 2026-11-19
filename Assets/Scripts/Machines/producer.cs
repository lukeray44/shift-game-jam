using UnityEngine;

public class producer : Machine
{
    public int SpawnPeriod;
    public Item Prefab;

    int timer;

    protected override void OnInitialiseOutputs()
    {
         
    }

    public override void OnBuildingAdded(Building b)
    {
         
    }
    protected override void Tick()
    {
 
        timer += 1;
        if (timer >= SpawnPeriod)
        {
            Item i = Instantiate(Prefab);
            i.transform.position = transform.position;
            if (!MainOutput.RecieveItem(i, Vector3.zero))
            {
                Destroy(i.gameObject);
            } else 
            {
                 timer = 0;
            }

           
        }
    }


    public override void Animate(float percentage)
    {
        
    }
}
