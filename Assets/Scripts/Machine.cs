using System;
using System.Collections.Generic;
using UnityEngine;


public abstract class Machine : MonoBehaviour
{

    int lastTick;
    public void TryTickRecursive()
    {
        if (lastTick != SimulationManager.TickIndex)
        {
            lastTick = SimulationManager.TickIndex;
            
            Tick();

            // recursively tick from end to start
            foreach (Machine m in InputMachines)
            {
                m.TryTickRecursive();
            }
        }
    }

    protected abstract void Tick();

    public abstract void Animate(float percentage);


    public void AnimateEachItem(float percentage, Action<int, float, Item> act) // index, percentage, item
    {
        for (int i = 0; i < Inventory.Length; i++)
        {
            float t = (percentage + i) / (float)Inventory.Length;

            Item item = GetItemInInventory(i);
            if (item != null && !ItemIsStuck(i))
            {
               
                act(i, t, item);
            }
            
        }
    }

    public List<Machine> InputMachines = new List<Machine>(); // machines that provide input
    public Machine MainInput => InputMachines.Count == 0 ? null : InputMachines[0];

    public List<Machine> OutputMachines = new List<Machine>(); // machines where this outputs
    public Machine MainOutput => OutputMachines.Count == 0 ? null : OutputMachines[0];

    [SerializeField]
    Item[] Inventory;

    public int InventoryLength => Inventory.Length;

    bool[] IsStuck;

    int inventoryOffset;

    // adjust index of each item, returning
    public bool TryIncrementItems()
    {
        inventoryOffset -= 1;
        inventoryOffset = (int)Mathf.Repeat(inventoryOffset, Inventory.Length);


        for (int i = 0; i < IsStuck.Length; i++)
        {
            IsStuck[i] = false;
        } 

        return GetItemInInventory(0) != null;
    }
    public void UndoIncrementAndCompress()
    {
       
        inventoryOffset -= 1;
         SetItemIsStuck(-1);
        CompressItems(); 
    }

    // iterate through list moving each up one if there's an empty space
    public void CompressItems()
    {
         
        for (int i = Inventory.Length - 1; i > 0; i--)
        {
            if (IsEmptyInInventory(i))
            {
                Item prev = GetItemInInventory(i - 1);
                if (prev != null)
                {
                    SetItemInInventory(i, prev);
                    SetItemInInventory(i  - 1, null);
                    SetItemIsStuck(i);
                }
                
            }

        }

        if (!IsEmptyInInventory(0)) SetItemIsStuck(0);

    }


    public void InitialiseInventory(int length)
    {
        if (Inventory.Length != length) Inventory = new Item[length];
        IsStuck = new bool[length];
    }
    public bool ItemIsStuck(int index)
    {
        return IsStuck[GetInventoryIndex(index)];
    }
    public void SetItemIsStuck(int index, bool stuck = true)
    {
        IsStuck[GetInventoryIndex(index)] = stuck;
    }
    public int GetInventoryIndex(int index)
    {
        return (int)Mathf.Repeat(index + inventoryOffset, Inventory.Length);
    }
    public Item GetItemInInventory(int index)
    {
        return Inventory[GetInventoryIndex(index)];
    }
    public bool IsEmptyInInventory(int index)
    {
        return GetItemInInventory(index) == null;
    }
    public void SetItemInInventory(int index, Item i)
    {
        int ind = GetInventoryIndex(index);
        Inventory[ind] = i;
         
    }
     public bool TrySetItemInInventory(int index, Item i)
    {

        if (GetItemInInventory(index) != null) return false;

        i.SavePosition();
        Inventory[GetInventoryIndex(index)] = i;
        if (i == null) IsStuck[GetInventoryIndex(index)] = false;
        else i.parent = this;
        return true;
    }

    public virtual bool RecieveItem(Item i, Vector3 direction)
    {
        return false;
    }
    public virtual Vector3 GetItemTarget(Vector3 direction)
    {
        return transform.position;
    }
    public void DestroyItemInInventory(int i)
    {
        Destroy(GetItemInInventory(i).gameObject);
        SetItemInInventory(i, null);
    }

    public bool TransferItem(int index, Machine target, Vector3 direction)
    {
        if (IsEmptyInInventory(index)) return false;

        Item toMove = GetItemInInventory(index);
        if (target != null && target.RecieveItem(toMove, direction))
        {
            SetItemInInventory(index, null);
            SetItemIsStuck(index, false);
            return true;
        }

        SetItemIsStuck(index, true);
        return false;
    }



    // debug
    public Item[] Debug_StartWith;

    public void Start()
    {
        int ind = 0;
        foreach (Item i in Debug_StartWith)
        {
            if (i == null) continue;
            Vector3 dir = Vector2.right;
            if (ind == 1) dir = Vector2.down;
            RecieveItem(i, dir);

            ind++;
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        UnityEditor.Handles.color = Color.green;


        void drawArrow(Vector3 start, Vector3 end, float headWidth = 0.2f, float headLength = 0.2f)
        {
            Vector3 dir = (end - start).normalized; // start -> end
            Vector3 perp = new Vector3(dir.y, -dir.x);

            UnityEditor.Handles.DrawLine(start, end, 3);
            UnityEditor.Handles.DrawLine(end, end - dir * headLength + perp * headWidth, 3);
            UnityEditor.Handles.DrawLine(end, end - dir * headLength - perp * headWidth, 3);
        }

        foreach (Machine m in OutputMachines)
        {
            drawArrow(transform.position, m.transform.position);
        }

        UnityEditor.Handles.color = Color.red;
        foreach (Machine m in InputMachines)
        {
            drawArrow(m.transform.position, transform.position);
        }
    }
#endif

    public bool Piecewise(float t, float startIndex, out float p)
    {
       return Piecewise(t, startIndex, InventoryLength, out p);
    }
    public bool Piecewise(float t, float startIndex, float endIndex, out float p)
    {
        t *= InventoryLength;
        t -= startIndex;
        t /= (float)(endIndex - startIndex);

        p = t;

        return p > 0 && p <= 1;
    }
    
}

