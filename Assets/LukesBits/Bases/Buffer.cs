using System.Collections;
using UnityEngine;
using System;

[Serializable]
public class Buffer<T>
{
    public T[] buffer;
    public bool[] isTaken;
    int curBuffer;
    int curLimit;
    int lastTaken;
    int lastAdded;
    public int LastTaken => lastTaken;
    public T LastAdded
    {
        get
        {
            return buffer[lastAdded];
        }
    }
    T checkAgainst;
    public Buffer(int bufferAmount)
    {
        buffer = new T[bufferAmount];
        isTaken = new bool[bufferAmount];
        checkAgainst = default(T);
    }
    public Buffer(int bufferAmount, T checkAgainst)
    {
        buffer = new T[bufferAmount];
        isTaken = new bool[bufferAmount];
        this.checkAgainst = checkAgainst;
    }
    public Buffer(T[] original)
    {
        buffer = original;
        isTaken = new bool[original.Length];
        for (int i = 0; i < isTaken.Length; i++)
        {
            isTaken[i] = true;
        }
    }
    public bool isBuffer => !isTaken[curBuffer];
    public bool Empty => !isTaken[curBuffer];
    public bool hasObjectAtIndex(int i) => isTaken[i];
    public int absoluteIndex(int relative)
    {
        return (relative + curBuffer) % bufferAmount;
    }
    public bool Full => isTaken[curLimit];
    public int bufferAmount => buffer.Length;
    public T nextBuffer()
    {
        
        T toReturn = buffer[curBuffer];
        buffer[curBuffer] = default(T);
        isTaken[curBuffer] = false;
        lastTaken = curBuffer;
        curBuffer = (curBuffer + 1) % bufferAmount;
        return toReturn;
    }
    public bool tryNext(out T toReturn)
    {
        toReturn = default(T);
        if (!isTaken[curBuffer]) return false;
        toReturn = nextBuffer();
        return true;
    }
    public T takeWhere(Func<T, bool> logic)
    {
        int i = 0;
        while (!Empty && i < buffer.Length)
        {
            T temp = nextBuffer();
            if (logic.Invoke(temp))
            {
                return temp;
            }
            i++;
        }
        return default(T);
    }

    //Does not run logic on null functions - if you want to check if there are any empties - use .Full
    public bool Exists(Func<T, bool> logic)
    {
        foreach (T i in buffer)
        {
            if (i != null)
            {
                if (logic.Invoke(i))
                {
                    return true;
                }
            }
        }
        return false;
    }
    public T currentBuffer => buffer[curBuffer];

    public void removeAll()
    {
        buffer = new T[bufferAmount];
        isTaken = new bool[bufferAmount];
        curBuffer = 0;
        curLimit = 0;
    }
    public bool addBuffer(T toAdd)
    {
        if (isTaken[curLimit]) return false;

        buffer[curLimit] = toAdd;
        isTaken[curLimit] = true;
        lastAdded = curLimit;
        curLimit = (curLimit + 1) % bufferAmount;
        return true;
    }
    public int? addBufferGetIndex(T toAdd)
    {
        if (isTaken[curLimit]) return null;

        buffer[curLimit] = toAdd;
        isTaken[curLimit] = true;
        int toReturn = curLimit;
        curLimit = (curLimit + 1) % bufferAmount;
        return toReturn;
    }
    public void setBuffer(T toAdd)
    {
        buffer[curLimit] = toAdd;
        isTaken[curLimit] = true;
        curLimit = (curLimit + 1) % bufferAmount;
    }

}