using UnityEngine;

public class producer : EaseAndDestroy
{
    public int SpawnPeriod;
    public Item Prefab;

    int timer;

    public override void Animate(float percentage)
    {
        base.Animate(percentage);
    }

    protected override void Tick()
    {
        base.Tick();

        timer += 1;
        if (timer >= SpawnPeriod)
        {
            Item i = Instantiate(Prefab);
            i.transform.position = transform.position;
            if (!RecieveItem(i, Vector3.down))
            {
                Destroy(i.gameObject);
            } else 
            {
                 timer = 0;
            }

           
        }
    }
}
