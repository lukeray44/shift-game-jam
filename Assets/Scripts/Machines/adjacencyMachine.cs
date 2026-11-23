using UnityEngine;

public abstract class adjacencyMachine : Machine
{

    public override Vector3 GetItemTarget(Vector3 direction)
    {


        Vector3 right = transform.right * Building.DirectionX;
        Vector3 centre = transform.position + Vector3.up * 0.25f;

        float sign = Building.DirectionX;
        if (direction.x == -Building.DirectionX) sign *= -1;


        return centre + transform.right * sign * -0.5f * Building.GridSize.x;
    }

    public override bool RecieveItem(Item i, Vector3 direction)
    {

        bool success = TrySetItemInInventory(0, i);
        if (success) LoopInputs();
        return success;
    }

    public override bool CanRecieveInDirection(Vector3 direction)
    {
        if (direction.x != Building.Direction.x && direction.x != 0)
        {
            return false;
        }
        return base.CanRecieveInDirection(direction);
    }


    protected virtual Vector2Int OutputTile()
    {
        Vector2Int offset = new Vector2Int(0, 0);
        if (Building.Direction.x > 0)
        {
            offset.x = Building.GridSize.x - 1;
        }
        if (Building.Direction.y > 0)
        {
            offset.y = Building.GridSize.y - 1;
        }

        int adjust(int x)
        {
            if (x == 0) return 0;
            return x > 0 ? 1 : -1;
        }

        return Building.GridPos + offset + new Vector2Int(Building.Direction.x * adjust(Building.GridSize.x), Building.Direction.y * adjust(Building.GridSize.y));
    }

    protected override void OnInitialiseOutputs()
    {
        base.OnInitialiseOutputs();

        if (BuildingManager.main.TryGetBuildingAtPos(OutputTile(), out Building b))
        {
            AddOutput(b.MainSimulator);
        }
    }

    public override void OnBuildingAdded(Building b)
    {
        base.OnBuildingAdded(b);


        if ((MainOutput == null || MainOutput.Building == null) && b.GridPos == OutputTile())
        {
            if (MainOutput.Building == null)
            {
                RemoveOutput(MainOutput, true);
            }
            AddOutput(b.MainSimulator);
        }
    }

    public override void Visualise(Machine previous, bool foundFuture = false)
    {
        if (MainOutput == null)
        {
            if (BuildingManager.main.TryGetBuildingAtPos(OutputTile(), out Building b))
            {
                base.Visualise(previous, true);
                return;
            }
        }
        base.Visualise(previous, foundFuture);
    }

    public override void VisualiseIfOutput(Building b)
    {
        if (b.GridPos == OutputTile())
        {
            VisualiseSelf(true);
        }
    }

    public override void VisualiseSelf(bool foundOutput = false)
    {
        Vector3 p1 = GetItemTarget(Vector3.left * Building.DirectionX);
        Vector3 p2 = GetItemTarget(Vector3.right * Building.DirectionX);
        Vector3 delta = (p2 - p1).normalized;

        Visualiser.DrawLine(p2 - delta * 0.1f, p1 + delta * 0.1f);
    }

}