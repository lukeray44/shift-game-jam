using UnityEngine;

public class Bouncer : Building
{
    public float BounceFactor = 10;
    public override Machine GetPhysicsOutput(Vector3 endPos, Vector3 velocity)
    {
        velocity *= 0.6f;
        velocity += new Vector3(-Direction.y, Direction.x) * BounceFactor;

        return SimulationManager.CreatePhysics(MainSimulator, transform.position, velocity);
    }

    public override void VisualisePhysics(Vector3 endPos, Vector3 velocity, Vector3 dir)
    {
        velocity *= 0.6f;
        velocity += new Vector3(-Direction.y, Direction.x) * BounceFactor;


        Debug.Log(velocity);
        Visualiser.VisualisePhysics(this, transform.position, velocity);
    }

    public override bool CanPlaceHere(Vector2Int pos)
    {
        Vector2Int perp = new Vector2Int(Direction.y, -Direction.x);
        Vector2Int dir = new Vector2Int(Mathf.Abs(Direction.x), Mathf.Abs(Direction.y));


        if (!BuildingManager.main.IsSolidTile(pos + perp)) return false;
        if (!BuildingManager.main.IsSolidTile(pos + dir + perp)) return false;
        
        if (BuildingManager.main.IsSolidTile(pos)) return false;
        if (BuildingManager.main.IsSolidTile(pos + dir)) return false;


        return base.CanPlaceHere(pos);
    }

    public override bool Rotate()
    {
        SetRotation((Vector2Int)(Quaternion.Euler(0, 0, 90) * (Vector2)Direction).Round());

        return true;
    }


    public override void SetRotation(Vector2Int rotation)
    {
        Direction = rotation;
        GridSize = new Vector2Int(2, 1);
        float angle = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        if (Mathf.Repeat(angle, 180) == 90) GridSize = new Vector2Int(1, 2);

        Position(GridPos);
        transform.localEulerAngles = angle * Vector3.forward;
    }

    public override void Position(Vector2Int position)
    {
        transform.position = BuildingManager.main.GetAreaCentre(position, GridSize);
    }

}
