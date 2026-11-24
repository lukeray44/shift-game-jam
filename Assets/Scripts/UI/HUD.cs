using System;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public TextMeshProUGUI Money, LevelName, QuotaNumber;

    public BuildingInterface BuildingControls;
    public LevelManager Level;

    void Start()
    {
        LevelName.text = LevelMeta.main.LevelTitle;

        BuildingControls.OnSetPlacing += a => UpdateMoneyDisplay();
        Level.OnMoneyChanged += a => UpdateMoneyDisplay();

        Level.OnQuotaChange += UpdateQuota;
        UpdateQuota(Level.Quota);
        UpdateMoneyDisplay();
    }

    private void UpdateQuota(int obj)
    {
        QuotaNumber.text = obj.ToString() + "x";
    }

    public Color Normal = Color.yellow;
    public Color Negative = Color.red;
    public void UpdateMoneyDisplay()
    {
        string moneyText(float money)
        {
            Money.color = money < 0 ? Negative : Normal;
            return $"${money}";
        }
        if (BuildingControls.CurrentlyPlacing != null)
        {
            float buildingCost = BuildingControls.CurrentlyPlacing.Cost;
            Money.text = $"{moneyText(Level.Money - buildingCost)} <size=70%><color=black>-${buildingCost}";
        } else
        {
            Money.text = $"{moneyText(Level.Money)}";
        }
    }
}
