using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

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
    public List<Building> AllBuildings = new List<Building>();

    public Dictionary<Vector2Int, Building> PositionMap = new Dictionary<Vector2Int, Building>();

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
        return new Vector3((i.x + marginx) * Grid.cellSize.x, (i.y + marginy) * Grid.cellSize.y) + Grid.transform.position;
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
        if (Vector2IntInArea(max2- Vector2Int.one, min1, max1)) return true;

        if (Vector2IntInArea(min1, min2, max2)) return true;
        if (Vector2IntInArea(max1 - Vector2Int.one, min2, max2)) return true;

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
    public bool IsRamp(Vector2Int pos, out Vector3 normal)
    {
       

        Vector3Int cellPos = Ground.WorldToCell(LocalToWorld(pos));
        TileBase t = Ground.GetTile(cellPos);
        normal = Ground.GetTransformMatrix(cellPos).rotation * new Vector3(1, 1);
         
        return t == Ramp;
    }

    public bool RaycastTiles(Vector3 pos, Vector3 vel, Vector2Int tilePosOld, Vector2Int tilePosNew, out Vector3 collisionPoint, out Vector3 normal)
    {
        if (RaycastTile(pos, vel, tilePosOld, out collisionPoint, out normal)) return true;
        if (tilePosOld.x != tilePosNew.x && tilePosOld.y != tilePosNew.y) // when going diagonals it can skip adjacent tiles so check those
        {
            

            if (RaycastTile(pos, vel, new Vector2Int(tilePosOld.x, tilePosNew.y), out collisionPoint, out normal)) return true;
            if (RaycastTile(pos, vel, new Vector2Int(tilePosNew.x, tilePosOld.y), out collisionPoint, out normal)) return true;
        }
        return RaycastTile(pos, vel, tilePosNew, out collisionPoint, out normal);  
    }
 
    public bool RaycastTile(Vector3 pos, Vector3 vel, Vector2Int tilePos, out Vector3 collisionPoint, out Vector3 normal)
    {
        
        Vector3 nextPos = pos + vel;
        Vector3 tileCentre = LocalToWorld(tilePos);
        Vector3Int cellPos = Ground.WorldToCell(tileCentre);
        TileBase tile = Ground.GetTile(cellPos);
        
        bool ramp = tile == Ramp;
        bool block = tile != Ramp && tile != null;

 
        if (ramp)
        {
            Quaternion rotDeg = Ground.GetTransformMatrix(cellPos).rotation;
            Vector3 tangent = rotDeg * new Vector3(1, -1); // right down 
            Vector3 startPos = rotDeg * new Vector3(-0.5f, 0.5f) + tileCentre;
            normal = new Vector3(-tangent.y, tangent.x);   
            if (Vector3.Dot(-normal, vel) > 0 && LineEXT.TryIntersectLines(pos - vel * 2, pos + vel, startPos, startPos + tangent, out collisionPoint))
            {
                return true;
            }

            collisionPoint = new Vector3();
            normal = new Vector3();
            return false;
        }
        else if (block)
        {
            Vector3 yNormal = Vector3.up * -Mathf.Sign(vel.y);
            Vector3 xNormal = Vector3.right * -Mathf.Sign(vel.x);

            Vector3 squarePos = tileCentre;
 
            bool testSquareEdge(Vector3 normal, bool vertical, out Vector3 point)
            {
                Vector3 edgeCentre = squarePos + normal * 0.5f;
                float scaleFactor(float pos, float vel, float end)
                {
                    return (end - pos) / vel;
                }
                float scale;
                if (vertical)
                {
                    scale = scaleFactor(pos.y, vel.y, edgeCentre.y);
                }
                else 
                {
                    scale = scaleFactor(pos.x, vel.x, edgeCentre.x);
                }
                if (scale < 0 || scale > 1)
                {
                    point = new Vector3(); 
                    return false;
                }
                point = pos + vel * scale;

                Vector3 offset = point - edgeCentre;
                return (offset.y * offset.y + offset.x * offset.x) <= 0.25f * 1.1f;
            }

            
            if (testSquareEdge(yNormal, true, out collisionPoint))
            {
                normal = yNormal;

                return true;
            }
            if (testSquareEdge(xNormal, false, out collisionPoint))
            {
                normal = xNormal;

                return true;
            }

            normal = new Vector3();
            return false;
        }
        else
        {
            normal = new Vector3();
            collisionPoint = new Vector3();
            return false;
        }

    }



    // adding buildings
    public Action<Building> OnBuildingAdd;
    public Action<Building> OnBuildingRemove;
    public Action<Building> OnBuildingVisualise;
    public void AddBuilding(Building b) // assume transform and gridPos set
    {
        AllBuildings.Add(b);
        b.ForeachTile(a =>
        {
            PositionMap[a] = b;
        });

        OnBuildingAdd?.Invoke(b);

        
    }
    public void RemoveBuilding(Building b)
    {
        AllBuildings.Remove(b);
         b.ForeachTile(a =>
         {
             PositionMap.Remove(a);
         });
         
        OnBuildingRemove?.Invoke(b);
        
    }
    public void DeleteBuilding(Building b)
    {
        RemoveBuilding(b);
        Destroy(b.gameObject);
       
    }

    public void Visualise(Building b)
    {
        b.MainSimulator.Visualise(null);
    }
    public void PreVisualise(Building b)
    {
        b.MainSimulator.Visualise(null);
        OnBuildingVisualise?.Invoke(b);
    }

    public Vector3 GetAreaCentre(Vector2Int pos, Vector2Int sizeInt)
    {
        Vector3 root = LocalToWorld(pos);
        Vector3 offset = new Vector3((sizeInt.x - 1) * Grid.cellSize.x, (sizeInt.y - 1) * Grid.cellSize.y);
        return root + offset * 0.5f;
    }
    public Vector2Int GetAreaPositionLocal(Vector3 worldPos, Vector2Int sizeInt)
    {
        Vector3 offset = new Vector3((sizeInt.x - 1) * Grid.cellSize.x, (sizeInt.y - 1) * Grid.cellSize.y);
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


static class LineEXT
{
    static bool isInBounds(Vector3 start1, Vector3 end1, Vector3 intersection)
    {
        if ((intersection.x < start1.x) != (intersection.x < end1.x)     // Bounded in x by line 1
                && (intersection.y < start1.y) != (intersection.y < end1.y)) return true;
        
        if (start1.x == end1.x && intersection.x == start1.x) return true;
        if (start1.y == end1.y && intersection.y == start1.y) return true;

        return false;
    }
    public static bool TryIntersectLines(Vector3 start1, Vector3 end1, Vector3 start2, Vector3 end2, out Vector3 intersection)
    {
        // Append 1 to all points. (x, y) -> (x, y, 1)
        Vector3 toProjPlane(Vector3 p) => new Vector3(p.x, p.y, 1);

        // Get equations of lines using cross product & solve
        Vector3 l1 = Vector3.Cross(toProjPlane(start1), toProjPlane(end1));
        Vector3 l2 = Vector3.Cross(toProjPlane(start2), toProjPlane(end2));
        Vector3 sol = Vector3.Cross(l1, l2);

        if (sol.z == 0)
        {
            // No solution - result 'at infinity' in projective space.
            intersection = default(Vector3);
            return false; 
        }
        else 
        {
            intersection = new Vector3(sol.x / sol.z, sol.y / sol.z);
            
           bool valid = (
                intersection != start1 &&
                isInBounds(start1, end1, intersection) &&
                isInBounds(start2, end2, intersection)
            );
            
            return valid;
        }
    }
}