using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public static SimulationManager main;

    public static int TickIndex;


    public List<Machine> Roots = new List<Machine>();
    public List<Machine> LoopRoots = new List<Machine>();
    public List<Machine> All = new List<Machine>();

    void Start()
    {
        TickIndex = 0;


    }

    void Awake()
    {
        main = this;
        BuildingManager.main.OnBuildingAdd += _buildingAdded;
        BuildingManager.main.OnBuildingRemove += RemoveBuilding;
    }

    private void _buildingAdded(Building building)
    {
        foreach (Machine m in building.Simulators)
        {
            AddMachine(m);
        }
    }

    bool isTick;
    public void Tick()
    {
        isTick = true;
        TickIndex++;

        foreach (Machine m in LoopRoots)
        {
            if (m == null) continue;
            m.TryTickRecursive();
        }

        foreach (Machine m in Roots)
        {
            if (m == null) continue;
            m.TryTickRecursive();
        }

        foreach (Machine m in LoopRoots)
        {
            if (m == null) continue;
            m.TickSelf();
        }
        isTick = false;

        foreach (Machine m in toRemove)
        {
            RemoveMachine(m);
        }
        toRemove.Clear();
    }
    public void Animate(float percentage)
    {
        foreach (Machine m in All)
        {
            if (m == null) continue;
            m.Animate(percentage);
        }
    }

    public void AddRoot(Machine m)
    {

        Roots.Add(m);
        m.IsRoot = true;
    }
    public void RemoveRoot(Machine m)
    {

        Roots.Remove(m);
        m.IsRoot = false;
    }
    public void AddMachine(Machine m)
    {
        BuildingManager.main.OnBuildingAdd += m.OnBuildingAdded;
        BuildingManager.main.OnBuildingVisualise += m.VisualiseIfOutput;
        All.Add(m);
        
        m.Initialise();
        m.UpdateIsRoot();
        m.OnAddToSimulation();
        m.Added = true;

    }
    List<Machine> toRemove = new List<Machine>();
    public void RemoveBuilding(Building b)
    {
        foreach (Machine s in b.Simulators)
        {
            RemoveMachine(s);
        }

    }
    public void DeleteMachine(Machine m)
    {
        if (m == null) return;
        RemoveMachine(m);
    }
    public void RemoveMachine(Machine m)
    {

        if (isTick)
        {
            toRemove.Add(m);
            return;
        }
        
        m.Added = false;
        All.Remove(m);
        if (m.IsRoot)
        {
            RemoveRoot(m);
           
        }
        if (m.IsLoopRoot) LoopRoots.Remove(m);

        BuildingManager.main.OnBuildingAdd -= m.OnBuildingAdded;
        BuildingManager.main.OnBuildingVisualise -= m.VisualiseIfOutput;

        while (m.InputMachines.Count > 0)
        {
            m.InputMachines[0].RemoveOutput(m);
        }

        foreach (Machine outmm in m.OutputMachines)
        {
            outmm.InputMachines.Remove(m);
            outmm.OnInputRemoved(m);
        }

        m.OnRemoveFromSimulation();
        if (m.Building == null) Destroy(m.gameObject);



    }

    public void CheckForLoop(Machine m)
    {
        if (m is loopBuffer) return;
        if (m.CheckForLoop(m, new HashSet<Machine>(), out Machine previous))
        {

            loopBuffer loopRoot = new GameObject("Loop").AddComponent<loopBuffer>();
            loopRoot.IsLoopRoot = true;
            loopRoot.SimulationPriority = 1000;

            m.InputMachines.Remove(previous);
            loopRoot.AddOutput(m);

            previous.OutputMachines.Remove(m);
            previous.AddOutput(loopRoot);

            AddMachine(loopRoot);
            LoopRoots.Add(loopRoot);

        }
    }

    float simulationTimer;

    public float SimulationTickPeriod;

    float tickPercentage => 1 - simulationTimer / SimulationTickPeriod;

    void Update()
    {
        simulationTimer -= Time.deltaTime;

        if (simulationTimer <= 0)
        {
            simulationTimer = SimulationTickPeriod;
            Animate(1);
            Tick();
            Animate(0);
        }
        else
        {
            Animate(tickPercentage);
        }
    }

    public PhysicsMachine PhysicsPrefab;
    public static void CreatePhysics(Machine cause, Vector3 pos, Vector3 velocity)
    {
        PhysicsMachine physics = cause.MainOutput as PhysicsMachine;
        Building build = cause.Building;

        if (physics != null)
        {
            if (!physics.Added)
            {
                physics.Simulate(build, pos, velocity);
                return;
            }
        }

        PhysicsMachine m = Instantiate(main.PhysicsPrefab);
        m.transform.parent = cause.transform;
        m.Simulate(build, pos, velocity);
        cause.AddOutput(m);
        physics = m;

        if (cause.Added)
        {
            main.AddMachine(physics);
        }


    }
}
