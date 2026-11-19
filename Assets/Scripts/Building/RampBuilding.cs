using UnityEngine;

public class RampBuilding : SimpleTileBuilding
{
    public override bool CanPlaceHere(Vector2Int pos)
    {
        bool tile = base.CanPlaceHere(pos);
        if (tile) return true;

        if (BuildingManager.main.IsRamp(pos, out Vector3 normal))
        {
            return normal.y > 0;
        }

        return false;
    }

    public override bool Rotate()
    {
        SetRotation(Direction * -1);
        return true;
    }

    public override void SetRotation(Vector2Int rotation)
    {
        if (rotation == Direction) return;
        
        if (rotation.y != 0 && rotation.x == 0)
        {
            Direction = Vector2Int.right;
        }

        Direction = rotation;

        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * Mathf.Sign(rotation.x), transform.localScale.y);

        if (rotation.y != 0)
        {
            bool isRotated = rotation.x == rotation.y; // if true: 45 deg
            transform.localEulerAngles = Vector3.forward * (isRotated ? 1 : -1) * 45f;
        }
        else
        {
            transform.localEulerAngles = Vector3.zero;
        }



    }

    public override void Position(Vector2Int position)
    {
         base.Position(position);

        if (BuildingManager.main.IsRamp(position, out Vector3 normal))
        {
            if (normal.y > 0)
            {
                Vector2Int dir = new Vector2Int(1, -(int)normal.x) * DirectionX;
                SetRotation(dir);
                transform.position +=  normal * 0.4f;
            }
        } else
        {
            SetRotation(DirectionX * Vector2Int.right);
        }
       
    }

}
