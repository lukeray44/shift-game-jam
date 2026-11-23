using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public InputChute[] Inputs;
    public OutputChute Output;

    public ItemByRows Target;
    public int Quota = 20;
    int current = 0;
    ItemComposite target;

    void Start()
    {
        Output.OnItemEntered += ItemGot;
        target = ItemManager.main.GetStateStructure(Target);
    }

    private void ItemGot(Item item)
    {
        if (item.state == target)
        {
            current++;

            Debug.Log("SUCCESS");

            if (current == Quota)
            {
                Win();
            }
        } else
        {
            Debug.Log("INCORRECT");
            current = 0;
        }
    }

    private void Win()
    {
        Debug.Log("VICTORY");
        Output.OnItemEntered -= ItemGot;
    }
}
