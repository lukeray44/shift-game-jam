using UnityEngine;


// placed on top of tiles
public class SimpleTileBuilding : Building
{
    public override bool CanPlaceHere(Vector2Int pos)
    {
        for (int x = 0; x < GridSize.x; x++)
        {
            if (!BuildingManager.main.IsSolidTile(pos + Vector2Int.right * x + Vector2Int.down)) return false;

            for (int y = 0; y < GridSize.y; y++)
            { 
                if (BuildingManager.main.IsAnyTile(pos + new Vector2Int(x, y))) return false;
            }
        }

        return true;

    }

    public override bool Rotate()
    {
        if (Direction == Vector2Int.right) SetRotation(Vector2Int.left);
        else SetRotation(Vector2Int.right);

        return true;
    }


    public override void SetRotation(Vector2Int rotation)
    {
        Direction = Vector2Int.right * (int)Mathf.Sign(rotation.x);

        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * Mathf.Sign(rotation.x), transform.localScale.y);
    }

    public override void Position(Vector2Int position)
    {
        transform.position = BuildingManager.main.GetAreaCentre(position, new Vector2Int(GridSize.x, 1));
    }

    public override Vector2Int PositionToCell()
    {
       return BuildingManager.main.GetAreaPositionLocal(transform.position, new Vector2Int(GridSize.x, 1));
    }
}