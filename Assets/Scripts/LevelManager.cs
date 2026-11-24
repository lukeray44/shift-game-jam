using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int Level;
    public LevelData LevelData;

    public InputChute[] Inputs;
    public OutputChute Output;

    public ItemByRows Target;
    public int Quota = 20;

    public float Money;

    int current = 0;
    ItemComposite target;
    public BuildingInterface Building;
    void Start()
    {
        Output.OnItemEntered += ItemGot;
        target = ItemManager.main.GetStateStructure(Target);

        Building.OnPlace += a => ChangeMoney(-a.Cost);
        Building.OnDestroy += a => ChangeMoney(a.Data.Cost);

        Level = LevelMeta.main.Cur;
        LevelData = LevelMeta.main.CurData;

        CutoutTransition.main.TransitionIn(Vector3.zero, 1);
    }


    public Action<float> OnMoneyChanged;
    public Action<int> OnQuotaChange;

    public void ChangeMoney(float money)
    {
        Money += money;
        OnMoneyChanged?.Invoke(Money);
    }
    private void ItemGot(Item item)
    {
        if (item.state == target)
        {
            current++;
            if (current == Quota)
            {
                Win();
            }
        } else
        {
            
            current = 0;
        }
        OnQuotaChange?.Invoke(Quota - current);
    }

    private void Win()
    {
        Debug.Log("VICTORY");
        Output.OnItemEntered -= ItemGot;
    }
}
