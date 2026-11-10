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

    public void Tick()
    {
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
        All.Add(m);
        m.Initialise();

    }

    public void RemoveBuilding(Building b)
    {
        foreach (Machine s in b.Simulators)
        {
            RemoveMachine(s);
        }

    }
    public void RemoveMachine(Machine m)
    {
        All.Remove(m);
        BuildingManager.main.OnBuildingAdd -= m.OnBuildingAdded;

        while (m.InputMachines.Count > 0)
        {
            m.InputMachines[0].RemoveOutput(m);
        }

        if (m.IsRoot) RemoveRoot(m);
        if (m.IsLoopRoot) LoopRoots.Remove(m);
    }

    public void CheckForLoop(Machine m)
    {
        if (m is loopBuffer) return;
        if (m.CheckForLoop(m, new HashSet<Machine>(), out Machine previous))
        {

            loopBuffer loopRoot = new GameObject("Loop").AddComponent<loopBuffer>();
            loopRoot.IsLoopRoot = true;
            
            previous.OutputMachines.Remove(m);
            previous.AddOutput(loopRoot);

            m.InputMachines.Remove(previous);
            loopRoot.AddOutput(m);
            

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
}
