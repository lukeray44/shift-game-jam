using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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
    public Tilemap Ground;
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

    bool BuildingsOverlap(Building b1, Building b2)
    { 
        Vector2Int min1 = b1.GridPos;
        Vector2Int max1 = b1.GridPos + b1.GridSize;

        Vector2Int min2 = b2.GridPos;
        Vector2Int max2 = b2.GridPos + b2.GridSize;

        if (Vector2IntInArea(min2, min1, max1)) return true;
        if (Vector2IntInArea(max2, min1, max1)) return true;

        if (Vector2IntInArea(min1, min2, max2)) return true;
        if (Vector2IntInArea(max1, min2, max2)) return true;

        return false;
    }
    public Building FindOverlappingBuilding(Building b)
    {
        foreach (Building b2 in AllBuildings)
        {
            if (b == b2) continue;

            if (BuildingsOverlap(b2, b)) return b2;
        }
        return null;
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

    public TileBase Ramp;

    public bool IsSolidTile(Vector2Int pos)
    {
        TileBase t = Ground.GetTile(Ground.WorldToCell(LocalToWorld(pos)));

        return t != null && t != Ramp;
    } 
    public bool IsAnyTile(Vector2Int pos)
    {
        TileBase t = Ground.GetTile(Ground.WorldToCell(LocalToWorld(pos)));

        return t != null;
    }
    public bool IsRamp(Vector2Int pos)
    {
        TileBase t = Ground.GetTile(Ground.WorldToCell(LocalToWorld(pos)));

        return t == Ramp;
    }

    // adding buildings
    public Action<Building> OnBuildingAdd;
    public Action<Building> OnBuildingRemove;
    public void AddBuilding(Building b) // assume transform and gridPos set
    {
        AllBuildings.Add(b);
        OnBuildingAdd?.Invoke(b);
    }
 
    public void DeleteBuilding(Building b)
    {
        AllBuildings.Remove(b);
        OnBuildingRemove?.Invoke(b);
        
        Destroy(b.gameObject);
    }
    

    public Vector3 GetAreaCentre(Vector2Int pos, Vector2Int sizeInt)
    {
        Vector3 root = LocalToWorld(pos);
        Vector3 offset = new Vector3((sizeInt.x - 1)  * Grid.cellSize.x, (sizeInt.y - 1) * Grid.cellSize.y);
        return root + offset * 0.5f;
    }
    public Vector2Int GetAreaPositionLocal(Vector3 worldPos, Vector2Int sizeInt)
    {
        Vector3 offset =  new Vector3((sizeInt.x - 1)  * Grid.cellSize.x, (sizeInt.y - 1) * Grid.cellSize.y);
        Vector2Int pos = WorldToLocal(worldPos - offset * 0.5f);
        return pos;
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
