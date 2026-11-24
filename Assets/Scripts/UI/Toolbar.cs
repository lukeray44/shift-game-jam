using System.Collections.Generic;
using UnityEngine;

public class Toolbar : UIPanel
{
    public UICursor Cursor;
    public BuildingInterface BuildingControls;
    public ToolbarButton NotchPrefab;

    public List<ToolbarButton> Children;

    public Tooltip Tooltip;

    public BuildingData[] debug_toAdd;
    void Start()
    {
        base.Start();
        foreach (BuildingData b in debug_toAdd)
        {
            AddButton(b);
        }
    }
    public KeyCode[] numbers = new[]{KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0};
    public void AddButton(BuildingData building)
    {
        ToolbarButton added = Instantiate(NotchPrefab, transform);
        added.Load(building);

        Children.Add(added);

        added.OnHoverSet += a => OnChildHovered(added, a);
        added.Clicked += () => OnChildClicked(added);
    }


    ToolbarButton currentlySelected;

    ToolbarButton tooltipShowing;

    public void ShowTooltip(ToolbarButton b)
    {
        if (b != tooltipShowing)
        {
            tooltipShowing = b;
            Tooltip.Show(b.Data, b.transform.position + Vector3.up);
        }
    }
    public void HideTooltip(ToolbarButton b)
    {
        if (b == tooltipShowing)
        {
            tooltipShowing = null;
            Tooltip.Hide();
        }
    }
    public void OnChildHovered(ToolbarButton child, bool isHovered)
    {
        if (isHovered)
        {
            ShowTooltip(child);
        } else
        {
            HideTooltip(child);
        }
    }

    void deselectCurrent()
    {
        if (currentlySelected == null) return;

        currentlySelected.SetSelected(false);
        currentlySelected = null;
        BuildingControls.SetPlacing(null);
    }
    public void OnChildClicked(ToolbarButton child)
    {
        
        if (currentlySelected == child)
        {
            OnEscape();
            return;
        }

        deselectCurrent();

        currentlySelected = child;
        Cursor.Select(child.transform.position);
        BuildingControls.SetPlacing(child.Data);
        child.SetSelected(true);
    }

    public void OnEscape()
    {
        Cursor.Hide();
        deselectCurrent();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnEscape();
        }

        int ind = 0;
        foreach (KeyCode k in numbers)
        {
            if (Input.GetKeyDown(k))
            {
                OnChildClicked(Children[ind]);
                break;
            }
            ind++;
        }

        base.Update();
    }
}
