using UnityEngine;
using UnityEngine.UI;

public class ToolbarButton : CoolButton
{
    public Image Icon;

    public BuildingData Data;

    public void Load(BuildingData toPlace)
    {
        Data = toPlace;
        SetSprite(Data.Icon);
    }
    public void SetSprite(Sprite icon)
    {
        Vector3 size = Icon.rectTransform.sizeDelta;
        Icon.sprite = icon;
        Icon.SetNativeSize();

        float scalar = Mathf.Min(size.x / Icon.rectTransform.sizeDelta.x, size.y / Icon.rectTransform.sizeDelta.y);
        Icon.transform.localScale = Vector3.one * scalar;
    }
}
