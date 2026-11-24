using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public CanvasGroup Alpha;

    public TextMeshProUGUI Title, Description, Price;

    public void Show(BuildingData Building, Vector3 pos)
    {
        gameObject.SetActive(true);
        if (Building != null)
        {
            Title.text = Building.DisplayName;
            Description.text = Building.Tooltip;
            Price.text = "$" + Building.Cost.ToString();
        } else
        {
            Title.text = "???";
            Description.text = "Hidden";
            Price.text = "???";
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        transform.position = pos;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
