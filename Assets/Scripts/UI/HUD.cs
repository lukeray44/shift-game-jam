using System;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public TextMeshProUGUI Money, LevelName, QuotaNumber;

    public BuildingInterface BuildingControls;
    public LevelManager Level;

    public ThreeStarDisplay Stars;

    public Transform TargetRoot;

    void Start()
    {
        LevelName.text = LevelMeta.main.LevelTitle;

        BuildingControls.OnSetPlacing += a => UpdateMoneyDisplay();
        Level.OnMoneyChanged += a => UpdateMoneyDisplay();

        Level.OnQuotaChange += UpdateQuota;

        Stars.Init(LevelMeta.main.CurData, LevelMeta.main.CurData.Money);

        Item display = ItemManager.main.Create(Level.target, TargetRoot, true);

        float maxHeight = 2.4f;
        float scalar = maxHeight / display.state.size.y;
        TargetRoot.transform.localScale *= scalar;
        TargetRoot.transform.localEulerAngles += Vector3.forward * -5;

        if (LevelManager.main.Money != 0) UpdateMoneyDisplay();
        UpdateQuota(Level.Quota);
    }

    int lastQuota = -1;
    private void UpdateQuota(int obj)
    {
        bool changed = lastQuota != obj;


        if (changed && lastQuota != -1)
        {
            QuotaNumber.gameObject.LeanCancel();
            QuotaNumber.transform.localScale *= 1.2f;
            QuotaNumber.transform.LeanScale(Vector3.right + Vector3.up, 0.1f).setEaseOutCubic();

        }
        lastQuota = obj;
        QuotaNumber.text = obj.ToString() + "x";
    }

    public Color Normal = Color.yellow;
    public Color Negative = Color.red;

    float lastMoney = -1;
    public void UpdateMoneyDisplay()
    {
        string moneyText(float money)
        {
            Money.color = money < 0 ? Negative : Normal;
            return $"${money}";
        }
        bool changed = lastMoney != Level.Money;


        if (changed && lastMoney != -1)
        {
            Money.gameObject.LeanCancel();
            Money.transform.localScale *= 1.2f;
            Money.transform.LeanScale(Vector3.right * -1 + Vector3.up, 0.1f).setEaseOutCubic();

            Stars.AnimateCompletion(lastMoney);
        }
        lastMoney = Level.Money;
        if (BuildingControls.CurrentlyPlacing != null)
        {
            float buildingCost = BuildingControls.CurrentlyPlacing.Cost;
            Money.text = $"{moneyText(Level.Money)} <size=70%><color=black>-${buildingCost}";
        }
        else
        {
            Money.text = $"{moneyText(Level.Money)}";
        }
    }
}
