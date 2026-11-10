using UnityEngine;

public class painter : Conveyor
{
    public override void Animate(float percentage)
    {
        base.Animate(percentage);

        if (Piecewise(percentage, 0.65f, 0.8f, out float p))
        {
            Item i = GetItemInInventory(0);

            if (i != null)
            {
                i.Sprite.color = Color.Lerp(i.PreviousState.color, Color.red, p);
            }
        }
    }
    protected override void Tick()
    {
        // set colour flag

        Item i = GetItemInInventory(0);

        if (i != null)
        {
            i.Sprite.color = Color.red;
            i.SavePosition();
        }

        base.Tick();
    }
}
