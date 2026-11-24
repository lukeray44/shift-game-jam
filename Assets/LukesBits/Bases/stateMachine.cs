using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Reflection;

[System.Serializable]
public class stateMachine : IEquatable<stateMachine>
{
    public Buffer<state> reserve = new Buffer<state>(15);
    public state current = null;


    public bool Add(state state, bool overrideAdd = false)
    {
        recycle();
        if (state == null || state == current) return false;

        state.frameAdded = Time.frameCount;
        if (current == null || current.priority <= state.priority || overrideAdd)
        {

            state temp = current;
            current = state;
            if (temp != null) temp.remove(state);
            if (current == state)
            {
                state.add();
                return true;
            }
        }
        state.frameAdded = Time.frameCount;
        reserve.addBuffer(state);
        return false;
    }
    public bool Add(string stateName, float statePriority, bool overrideAdd = false)
    {
        return Add(new state(stateName, statePriority), overrideAdd);

    }
    public void Remove(state state)
    {
        recycle();
        if (current == state)
        {
            state last = current;
            current = null;           

            if (reserve.currentBuffer != null)
            {
                
                state temp = reserve.takeWhere(s => s.frameAdded == Time.frameCount);
                current = last;
                Add(temp, true);

            } 
            if (last != null) last.remove(null);
            return;
        }
    }
    public void Remove(string stateName, float statePriority)
    {
        Remove(new state(stateName, statePriority));
    }
    public void recycle()
    {
        if (reserve.currentBuffer != null)
        {
            if (reserve.currentBuffer.frameAdded != Time.frameCount)
            {
                //remove every buffer until it finds an empty slot, and stays there
                reserve.takeWhere(s => false);
            }
        }
    }

    public override int GetHashCode() => base.GetHashCode();
    
    //operators
    public override bool Equals(object obj) { return Equals(obj as stateMachine); }
    public bool Equals(stateMachine s)
    {
        if (s is null) return false;

        if (ReferenceEquals(this, s))
        {
            return true;
        }
        if (GetType() != s.GetType())
        {
            return false;
        }
        return s.current == current;
    }

    public static bool operator ==(stateMachine lhs, state rhs)
    {
        if (lhs is null)
        {
            if (rhs is null)
            {
                return true;
            }

            return false;
        }

        return lhs.current == rhs;
    }

    public static bool operator !=(stateMachine lhs, state rhs) => !(lhs == rhs);

    public static stateMachine operator +(stateMachine a, state b)
    {
        a.Add(b);
        return a;
    }
    public static stateMachine operator -(stateMachine a, state b)
    {
        a.Remove(b);
        return a;
    }
}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(stateMachine))]
public class stateMachineDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.LabelField(position.subdivide(new Vector2(1, 0.5f)), label);

        var prop = property.FindPropertyRelative("current");
        if (prop != null)
        {
            EditorGUI.LabelField(position.subdivide(new Vector2(0.4f, 0.5f), new Vector2(0f, 0.5f)), "Current State:");
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(position.subdivide(new Vector2(0.5f, 0.5f), new Vector2(0.3f, 0.5f)), prop.FindPropertyRelative("name"), GUIContent.none);
            EditorGUI.PropertyField(position.subdivide(new Vector2(0.1f, 0.5f), new Vector2(0.9f, 0.5f)), prop.FindPropertyRelative("priority"), GUIContent.none);
            EditorGUI.EndDisabledGroup();
        } else
        {
            EditorGUI.LabelField(position.subdivide(new Vector2(1, 0.5f), new Vector2(0, 1)), "No state set");
        }

    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 50;
    }
}
#endif



public static class rectEXT
{
    public static Rect subdivide(this Rect source, Vector2 sizePercent, Vector2 offsetPercent = new Vector2())
    {
        Vector3 pos = (Vector3)source.position + Vector3Extensions.multiply(source.size, offsetPercent);
        Vector3 size = Vector3Extensions.multiply(source.size, sizePercent);
        return new Rect(pos, size);
    }
}

[System.Serializable]
public class state : IEquatable<state>
{
    public string name;
    public float priority;
    [HideInInspector]
    public int frameAdded;
    public int techOverride;
    public float timeAdded;
    public float timeRunning => Time.time - timeAdded;
    [HideInInspector]
    public Action onSet = new Action(() => { });
    [HideInInspector]
    public Action onCancel = new Action(() => { });

    public List<stateTransition> stateTransitions;

    public state(string name, float priority = 0)
    {
        this.name = name;
        this.priority = priority;
    }
    public virtual void remove(state to)
    {
        techOverride = 0;
        if (onCancel != null) onCancel.Invoke();
        if (stateTransitions != null)
        {
            stateTransition s = stateTransitions.Find(s => s.stateTo == to);
            if (s != null) s.intermediary.Invoke();
        }
    }
    public void ResetTimer()
    {
        timeAdded = Time.time;
    }
    public virtual void add()
    {
        if (onSet != null) onSet.Invoke();
        
        timeAdded = Time.time; 
    }
    public stateTransition addTransition(state state)
    {
        if (stateTransitions == null) stateTransitions = new List<stateTransition>();
        stateTransition s = new stateTransition() { stateTo = state };
        stateTransitions.Add(s);
        return s;
    }
    public override int GetHashCode() => base.GetHashCode();
    public override string ToString()
    {
        return name;
    }
    //operators
    public override bool Equals(object obj) => Equals(obj as state);
    public bool Equals(state s)
    {
        if (s is null) return false;

        if (ReferenceEquals(this, s))
        {
            return true;
        }
        if (GetType() != s.GetType())
        {
            return false;
        }
        return s.name == name;
    }

    public static bool operator ==(state lhs, state rhs)
    {
        if (lhs is null)
        {
            if (rhs is null)
            {
                return true;
            }

            return false;
        }

        return lhs.Equals(rhs);
    }

    public static bool operator !=(state lhs, state rhs) => !(lhs == rhs);
    public static implicit operator state(string s) => new state(s, 0);

}


public class stateTransition
{
    public state stateTo;
    public Action intermediary = new Action(() => { });
}