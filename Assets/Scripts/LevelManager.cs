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
    public ItemComposite target;
    public BuildingInterface Building;

    public Action HideHUD;

    float startSimulationSpeed;
    public MusicPlayer Player;
    public LevelSuccess WinScreen;

    void Awake()
    {
        base.Awake();
        target = ItemManager.main.GetStateStructure(Target);

    }
    void Start()
    {
        Output.OnItemEntered += ItemGot;
        Output.ShowPreview(target);

        Building.OnPlace += a => ChangeMoney(-a.Cost);
        Building.OnDestroy += a => ChangeMoney(a.Data.Cost);
        Building.OnDestroy += a => ItemGot(null);

        Level = LevelMeta.main.Cur;
        LevelData = LevelMeta.main.CurData;
        Money = LevelData.Money;

        TimeKeeper.AddTimer(0.1f).OnTimerEnd += () => CutoutTransition.main.TransitionIn(Vector3.zero, 1);

        OnMoneyChanged?.Invoke(Money);
        OnQuotaChange?.Invoke(Quota);

        startSimulationSpeed = SimulationManager.main.SimulationTickPeriod;
        
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
        if (item != null && item.state == target)
        {
            current++;

            if (current == 3)
            {
                SetSpeedUp();
            }

            if (current == Quota)
            {
                Win();
            }
        }
        else
        {
            current = 0;
            CancelSpeedUp();
        }
        OnQuotaChange?.Invoke(Quota - current);
    }

    Timer t;
    void SetSpeedUp()
    {
        t = TimeKeeper.AddFuncTimer(SimulationManager.main.SimulationTickPeriod * 2 * 7, f =>
        {
            SimulationManager.main.SetPeriod(Mathf.Lerp(startSimulationSpeed, startSimulationSpeed * 0.5f, f));

        });

        Player.Swap();
    }

    void CancelSpeedUp()
    {
        if (t != null) t.Cancelled = true;
        float oldTime = SimulationManager.main.SimulationTickPeriod;
        if (oldTime == startSimulationSpeed) return;
        t = TimeKeeper.AddFuncTimer(SimulationManager.main.SimulationTickPeriod * 0.5f, f =>
       {
           SimulationManager.main.SetPeriod(Mathf.Lerp(oldTime, startSimulationSpeed * 0.5f, 1 - f));

       });

        Player.Play();
    }


    [Button]
    private void Win()
    {
        Output.OnItemEntered -= ItemGot;

        HideHUD?.Invoke();

        WinScreen.Show(Money > 0, Money > LevelMeta.main.Data.Data[LevelMeta.main.Cur].BestMoney);

        if (Money < 0 || !LevelMeta.main.isNextLevel) WinScreen.Next.gameObject.SetActive(false);
        else WinScreen.Next.Clicked += () => LevelMeta.main.LoadLevel(LevelMeta.main.Cur + 1);

        WinScreen.Restart.Clicked += () => LevelMeta.main.LoadLevel(LevelMeta.main.Cur);

        WinScreen.Exit.Clicked += () => LevelMeta.main.LoadScene("Level Select");


        LevelMeta.main.Win(Money);

        TimeKeeper.AddFuncTimer(SimulationManager.main.SimulationTickPeriod * 2 * 7, f =>
        {
            SimulationManager.main.SetPeriod(Mathf.Lerp(startSimulationSpeed, startSimulationSpeed * 0.5f, 1 - f));

        });
    }
}
