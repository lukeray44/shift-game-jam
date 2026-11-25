using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
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

    public Action HideHUD;

    public LevelSuccess WinScreen;
    void Start()
    {
        Output.OnItemEntered += ItemGot;
        target = ItemManager.main.GetStateStructure(Target);

        Building.OnPlace += a => ChangeMoney(-a.Cost);
        Building.OnDestroy += a => ChangeMoney(a.Data.Cost);

        Level = LevelMeta.main.Cur;
        LevelData = LevelMeta.main.CurData;
        Money = LevelData.Money;

        TimeKeeper.AddTimer(0.1f).OnTimerEnd += () => CutoutTransition.main.TransitionIn(Vector3.zero, 1);
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

    [Button]
    private void Win()
    {
        Output.OnItemEntered -= ItemGot;

        HideHUD?.Invoke();

        WinScreen.Show(Money > 0);
        
        if (Money < 0 || !LevelMeta.main.isNextLevel) WinScreen.Next.gameObject.SetActive(false);
        else WinScreen.Next.Clicked += () => LevelMeta.main.LoadLevel(LevelMeta.main.Cur + 1);

        WinScreen.Restart.Clicked += () => LevelMeta.main.LoadLevel(LevelMeta.main.Cur);

        WinScreen.Exit.Clicked += () => LevelMeta.main.LoadScene("Level Select");

        
        LevelMeta.main.Win(Money);
    }
}
