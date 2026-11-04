using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public static int TickIndex;


    public List<Machine> Roots = new List<Machine>();
    public List<Machine> All = new List<Machine>();

    void Start()
    {
        TickIndex = 0;
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
