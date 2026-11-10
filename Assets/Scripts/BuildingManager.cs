using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager main;

    void Awake()
    {
        main = this;
    }
    void OnDestroy()
    {
        main = null;
    }


    public Grid Grid;
    public List<Building> AllBuildings;
    
    // transformations
    public Vector3 LocalToWorld(Vector2Int i)
    {
        return LocalToWorld(i, 0.5f);
    }
    public Vector3 LocalToWorld(Vector2Int i, float margin)
    {
        return LocalToWorld(i, margin, margin);
    }
    public Vector3 LocalToWorld(Vector2Int i, float marginx, float marginy)
    {
        return new Vector3((i.x + marginx) * Grid.cellSize.x , (i.y + marginy) * Grid.cellSize.y) + Grid.transform.position;
    }
    public Vector2Int WorldToLocal(Vector3 pos)
    {
        Vector3 local = pos - Grid.transform.position;
        return new Vector2Int(Mathf.FloorToInt(local.x / Grid.cellSize.x), Mathf.FloorToInt(local.y / Grid.cellSize.y));
    }

    // finding buildings
    bool Vector2IntInArea(Vector2Int pos, Vector2Int min, Vector2Int max)
    { 
        if (pos.x < min.x || pos.y < min.y) return false;
        if (pos.x >= max.x || pos.y >= max.y) return false;
        
        return true;
    }
    public bool TryGetBuildingAtPos(Vector2Int lPos, out Building b)
    {
        b = GetBuildingAtPos(lPos);
        return b != null;
    }
    public Building GetBuildingAtPos(Vector2Int lPos)
    {
        foreach (Building b in AllBuildings)
        {
            if (Vector2IntInArea(lPos, b.GridPos, b.GridPos + b.GridSize)) return b;
        }
        return null;
    }

    // adding buildings
    public Action<Building> OnBuildingAdd;
    public Action<Building> OnBuildingRemove;
    public void AddBuilding(Building b)
    {
        b.GridPos = WorldToLocal(b.transform.position);
        b.transform.position = LocalToWorld(b.GridPos);
        AllBuildings.Add(b);
        OnBuildingAdd?.Invoke(b);
    }

    public void DeleteBuilding(Building b)
    {
        OnBuildingRemove?.Invoke(b);
        AllBuildings.Remove(b);
        Destroy(b.gameObject);
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int pos = WorldToLocal(mouse);

            if (TryGetBuildingAtPos(pos, out var b)) DeleteBuilding(b);
        }
    }

    // editor
#if UNITY_EDITOR
    public static BuildingManager Editor
    {
        get
        {
            if (main == null) main = FindAnyObjectByType<BuildingManager>();
            return main;
        }
    }

    public static void CellGizmo(Vector2Int pos)
    {
        Vector3 wpos = Editor.LocalToWorld(pos);
        Vector3 size = Editor.Grid.cellSize;

        Gizmos.DrawWireCube(wpos, size);
    }
    public static void CellAreaGizmo(Vector2Int pos, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                CellGizmo(pos + new Vector2Int(x, y));
            }
        }
    }
    #endif
}
