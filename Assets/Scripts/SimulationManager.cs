using System;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public static SimulationManager main;
    
    public static int TickIndex;


    public List<Machine> Roots = new List<Machine>();
    public List<Machine> All = new List<Machine>();

    void Start()
    {
        TickIndex = 0;

        
    }

    void Awake()
    {
        main = this;
        BuildingManager.main.OnBuildingAdd += _buildingAdded;
    }

    private void _buildingAdded(Building building)
    {
        if (building.Simulator != null)
        {
            AddMachine(building.Simulator);
        }
    }

    public void Tick()
    {
        TickIndex++;
        foreach (Machine m in Roots)
        {
            if (m == null) continue;
            m.TryTickRecursive();
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

        if (m.MainOutput == null) 
        {
            AddRoot(m);
        }
    }

    public void CheckForLoop(Machine m)
    {
         if (m.CheckForLoop(m, new HashSet<Machine>()))
        {
            m.IsLoopRoot = true;
           AddRoot(m);
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
        } else
        {
            Animate(tickPercentage);
        }
    }
}
