using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PhysicsMachine : Machine
{
    public Machine Excess;

    itemSimulation simulation;

    protected override void OnInitialiseOutputs()
    {

    }
    public override void Initialise()
    {

    }

    public override int ExtraOutputs()
    {
        return base.ExtraOutputs() + 1;
    }
    public override void OnAddToSimulation()
    {
        base.OnAddToSimulation();

        SimulationManager.main.AddMachine(Excess);
    }


    public override Vector3 GetItemTarget(Vector3 direction)
    {
        return transform.position;
    }

    protected override void Tick()
    {
        if (TryIncrementItems())
        {
            if (MainOutput == null || !TransferItem(0, MainOutput, simulation.GetOutputDir()))
            {
                TransferItem(0, Excess, Vector3.zero);
            }
        }


        if (MainInput == null && ItemCount == 0)
        {
            SimulationManager.main.DeleteMachine(this);
        }
    }

    public override bool RecieveItem(Item i, Vector3 direction)
    {
        return TrySetItemInInventory(0, i);
    }

    public override void OnRemoveFromSimulation()
    {
        base.OnRemoveFromSimulation();
        Excess.InputMachines.Remove(this);
        SimulationManager.main.RemoveMachine(Excess);
    }

    Vector3 startPos;
    Vector3 startVelocity;
    Building startCause;
    public void Simulate(Building cause, Vector3 pos, Vector3 velocity)
    {
        startCause = cause;
        startPos = pos;
        startVelocity = velocity;
        BuildingManager build = BuildingManager.main;
        transform.position = pos;
        simulation = new itemSimulation(pos, velocity);

        if (MainOutput != null)
        {
            RemoveOutput(MainOutput);
        }

        simulation.TryFindBuilding(cause, null, (b, m) =>
        {
            AddOutput(m);
            Excess.transform.position = simulation.GetPositionByPercentage(1);
        });

        int itemCount = Mathf.CeilToInt(simulation.maxTime);

        InitialiseInventory(itemCount);
    }

    public override void OnBuildingAdded(Building b)
    {
        base.OnBuildingAdded(b);
        if (MainInput == null) return;
        if (buildingOnPath(b))
        {
            MainInput.RemoveOutput(this);
        }
    }

    public override void OnOutputRemoved(Machine input)
    {
        base.OnOutputRemoved(input);
        if (MainInput == null) return;
        Machine lastInput = MainInput;
        MainInput.RemoveOutput(this);

        if (lastInput.MainOutput is PhysicsMachine p)
        {
            p.TransferInventory(this);
        }
    }

    bool buildingOnPath(Building b)
    {
        foreach (Vector2Int tile in simulation.tileSpaces)
        {
            if (b.ContainsTile(tile))
            {
                return true;
            }
        }
        return false;
    }
    public override void Animate(float percentage)
    {
        int length = InventoryLength;
        bool easeDestroy = MainOutput == null;
        AnimateEachItem(percentage, (ind, t, item) =>
        {
            item.transform.position = simulation.GetPositionByPercentage(t);

            if (easeDestroy && ind == InventoryLength - 1)
            {
                item.transform.localScale = Vector3.one * Mathf.Lerp(1, 0, percentage * percentage);
            }
        });
    }


    public float maxTime = 2;

    public override void Visualise(Machine previous, bool foundFuture = false)
    {
        if (MainInput != null && previous != MainInput) MainInput.Visualise(this);

        simulation.Visualise();
    }

    public override void VisualiseIfOutput(Building b)
    {
        if (buildingOnPath(b))
        {
            Visualiser.VisualisePhysics(startCause, startPos, startVelocity, b);
        }


    }
}


class itemSimulation
{
    public List<Vector2Int> tileSpaces = new List<Vector2Int>();

    public List<Vector3> positions = new List<Vector3>();

    public const float timeStep = 0.1f;
    public const float gravity = 9f;

    public float maxTime => (positions.Count - 1) * timeStep;
    bool wasInTileLastTick;
    float currentTime;
    public Vector3 lastVelocity;
    Vector3 currentVelocity;
    Vector3 currentPosition;
    Vector2Int currentCell;


    public float Distance;

    int timeToIndex(float time)
    {
        return Mathf.FloorToInt(time / timeStep);
    }

    Vector3 outputDir;
    public Vector3 GetOutputDir()
    {
        return outputDir;

    }
    public Vector3 Position() => currentPosition;
    public void ShiftEnd(Vector3 pos, int shiftMax = -1)
    {

        Vector3 addVelocity = GetOutputDir().y < 0 ? Vector3.up * 5 * timeStep : Vector3.zero;

        if (shiftMax == -1) shiftMax = positions.Count - 2;
        Vector3 last = positions[positions.Count - 2]; // second to last
        Vector3 lastDelta = positions[positions.Count - 1] - last;

        int min = positions.Count / 2;
        float lastYDelta = -1000;
        for (min = shiftMax; min >= 2; min--)
        {
            Vector3 cur = positions[min];
            Vector3 delta = last - cur;

            if (Vector3.Dot(delta, lastDelta) < 0) break;

            float yComp = delta.y;
            if (Mathf.Abs(yComp) > 0.5f)
            {
                if (lastYDelta != -1000)
                {
                    if (Mathf.Sign(lastYDelta) != Mathf.Sign(yComp))
                    {
                        break;
                    }
                }
                lastYDelta = yComp;
            }

            last = cur;
            lastDelta = delta;

        }



        Vector3 origin = positions[min];
        Vector3 add = pos - positions[positions.Count - 1];


        int count = positions.Count - 1;
        positions[positions.Count - 1] = pos;
        for (int i = positions.Count - 2; i >= min; i--)
        {
            float completion = (count - i) / (float)(count - min);

            float adjustedScalar = 1 - Mathf.Clamp01(completion * 2);
            positions[i] = positions[i] + add * (1 - completion) + addVelocity * adjustedScalar;
        }
    }
    public Vector3 GetPosition(float time)
    {

        if (positions.Count == 0) return Vector3.zero;

        int ind = timeToIndex(time);
        int max = positions.Count;
        if (ind >= max - 1) return positions[max - 1];

        Vector3 p1 = positions[ind];
        Vector3 p2 = positions[ind + 1];

        return Vector3.Lerp(p1, p2, (time % timeStep) / timeStep);
    }
    public Vector3 GetPositionByPercentage(float t)
    {
        return GetPosition(t * (positions.Count - 1) * timeStep);
    }

    public itemSimulation(Vector3 position, Vector3 velocity)
    {
        currentPosition = position;
        currentVelocity = velocity;
        currentCell = BuildingManager.main.WorldToLocal(position);
        wasInTileLastTick = true;
        positions.Add(currentPosition);
    }

    public Action<Vector2Int> NewTile;

    public void Break()
    {
        breakImmediate = true;
    }
    bool breakImmediate = false;

    public void TryFindBuilding(Building cause, Building LookFor, Action<Building, Machine> OnFindBuilding)
    {
        BuildingManager build = BuildingManager.main;
        bool isConveyor = cause.MainSimulator.GetType() == typeof(Conveyor);
        NewTile += tile =>
        {
            bool tryGetBuilding(out Building b)
            {
                if (LookFor != null && LookFor.ContainsTile(tile))
                {
                    b = LookFor;
                    return true;
                }
                return build.PositionMap.TryGetValue(tile, out b);
            }

            if (tryGetBuilding(out Building b))
            {

                if (b != null && b != cause)
                {
                    Machine m = b.GetPhysicsOutput(Position(), lastVelocity);

                    if (m is Conveyor && isConveyor)
                    {
                        int offset = b.GridPos.x - cause.GridPos.x;

                        if (offset == 2 || offset == -2)
                        {
                            ShiftEnd(currentPosition + Vector3.left * Mathf.Sign(currentPosition.y - positions[0].x) * 0.2f);
                            Break();
                            return;
                        }

                    }

                    Vector3? closest(Vector3 v1, Vector3 v2)
                    {
                        Vector3 p1 = m.GetItemTarget(v1);

                        Vector3 p2 = m.GetItemTarget(v2);

                        bool p1Valid = b.MainSimulator.CanRecieveFromPosition(v1, Position());
                        bool p2Valid = b.MainSimulator.CanRecieveFromPosition(v2, Position());

                        if (p1Valid && (Vector3.Distance(p1, currentPosition) < Vector3.Distance(p2, currentPosition) || !p2Valid))
                        {
                            outputDir = v1;
                            return p1;
                        }

                        if (!p2Valid) return null;

                        outputDir = v2;
                        return p2;
                    }

                    Vector3? endPos = closest(Vector3.down, Vector3.right * Mathf.Sign(currentVelocity.x));
                   
                    if (endPos == null) return;

                    ShiftEnd(endPos.Value);
                    Break();

                    OnFindBuilding?.Invoke(b, m);



                }
            }
        };

        SimulateTime(build);
    }
    public void SimulateTime(BuildingManager build, float time = 25)
    {

        int ind = timeToIndex(time);

        for (int i = 0; i < ind; i++)
        {
            if (breakImmediate) break;
            SimulateStep(build);

            if (deadTime > 1.5f || currentPosition.y < -10)
            {

                break;
            }
        }

    }
    float deadTime = 0;
    int collisions;
    float radius = 0.3f;

    Vector3 lastProjection;
    bool dontGravity;
    Vector3 lastAdded;
    public void SimulateStep(BuildingManager build)
    {
        if (!dontGravity)
        {
            currentVelocity += Vector3.down * gravity * timeStep;
            if (currentVelocity.y < -gravity) currentVelocity.y = -gravity;
        }
        if (currentVelocity.y > 0.2f) dontGravity = false;

        Vector3 toMove = currentVelocity * timeStep;
        currentTime += timeStep;

        Vector3 radiusOffset = toMove.normalized * radius;
        if (toMove.magnitude < 0.1f) radiusOffset = lastProjection;
        lastProjection = radiusOffset;


        Vector3 lastPos = currentPosition;
        Vector2Int cell = build.WorldToLocal(currentPosition + toMove + radiusOffset);

        bool newTile = false;
        if (cell != currentCell || wasInTileLastTick)
        {
            if (cell != currentCell) newTile = true;
            wasInTileLastTick = build.IsAnyTile(cell);


            if (build.RaycastTiles(currentPosition, toMove + radiusOffset, currentCell, cell, out Vector3 collisionPoint, out Vector3 normal))
            {
                collisions++;


                currentVelocity = Vector2.Reflect(currentVelocity, normal.normalized) * 0.8f;


                if (normal.x == 0) currentVelocity.x *= 0.9f;
                if (normal.x == 0 && normal.y == 1 && currentVelocity.y < 0.2f && Mathf.Abs(currentVelocity.x) < 0.2f)
                {
                    currentVelocity.y = 0;
                    dontGravity = true;
                }

                Vector3 last = currentPosition;
                currentPosition = collisionPoint - radiusOffset;
                if (Mathf.Abs(last.x - currentPosition.x) < 0.15f) currentPosition.x = last.x;
                toMove = new Vector3();

                if (Vector3.Distance(lastAdded, currentPosition) < 0.1f && currentVelocity.magnitude > 0.1f)
                {
                    lastAdded = Vector3.negativeInfinity;
                    return;
                }




            }
            else
            {
                tileSpaces.Add(cell);
            }
            currentCell = cell;

        }

        currentPosition += toMove;
        float dist = Vector3.Distance(currentPosition, lastAdded);

        if (dist < 0.25f || dist > 100)
        {
            deadTime += timeStep;

        }
        else
        {
            lastVelocity = currentVelocity;
            deadTime = 0;

        }





        if (dist < 100) Distance += dist;

        lastAdded = currentPosition;
        positions.Add(currentPosition);
        if (newTile) NewTile?.Invoke(cell); ;

    }

    public void Gizmo()
    {
        for (int i = 0; i < positions.Count - 1; i++)
        {
            Vector3 pos = positions[i];
            Vector3 pos2 = positions[i + 1];

            Gizmos.DrawLine(pos, pos2);
            Gizmos.DrawSphere(pos, 0.02f);
        }

        // foreach (Vector2Int pos in tileSpaces)
        // {
        //     BuildingManager.CellGizmo(pos);
        // } 
    }
    public void Visualise()
    {
        Visualiser.DrawLine(positions.ToArray());
    }
}

