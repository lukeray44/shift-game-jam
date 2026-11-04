using UnityEngine;

public class painter : Conveyor
{
    public override void Animate(float percentage)
    {
        base.Animate(percentage);
        
        if (Piecewise(percentage, 0.45f, 0.55f, out float p))
        {
            Item i = GetItemInInventory(0);

            if (i != null)
            {
                Debug.Log(i.name + " " + p);
                i.Sprite.color = Color.Lerp(i.PreviousState.color, Color.red, p);
            }
        }
    }
    protected override void Tick()
    {
        // set colour flag
        base.Tick();
    }
}
