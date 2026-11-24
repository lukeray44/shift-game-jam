using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EfficientList<T> : IEnumerator<T>
{
    public int max;
    public int last { get; protected set; }
    public int length;
    public bool full => length == max;



    T[] contents;
    Buffer<int> spacesToFill;
    public EfficientList(int limit)
    {
        contents = new T[limit];
        spacesToFill = new Buffer<int>(limit);
        max = limit;
    }

    public void add(T toAdd)
    {
        if (!full)
        {
            if (!spacesToFill.Empty)
            {
                contents[spacesToFill.nextBuffer()] = toAdd;
                length++;
            } else
            {
                contents[last] = toAdd;
                last++;
                length++;
            }
        }
    }

    public void removeFirst(T toRemove)
    {
        int indexToNullify = -1;
        int i = 0;
        foreach (T obj in contents)
        {
            if ((object)obj == (object)toRemove)
            {
                indexToNullify = i;
                break;
            }
            i++;
        }
        
        if (indexToNullify != -1)
        {
            length--;
            contents[indexToNullify] = default(T);
            spacesToFill.addBuffer(indexToNullify);
        }
    }
    public T Current => contents[position];

    object IEnumerator.Current
    {
        get
        {
                try
                {
                    return contents[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
        }
    }
    public IEnumerator GetEnumerator()
    {
        foreach (T t in contents)
        {
           
            if ((object)t != (object)default(T))
            {
                
                yield return t;
            }
        }
    }
    public T getIndex(int i)
    {
        int x = 0;
        foreach (T obj in this)
        {
            if (x == i)
                return obj;
            x++;
        }
        return default(T);
    }


    public int position = -1;
    public bool MoveNext()
    {
        position++;
        for (int i = position; i < length; i++)
        {
            if (contents[i] != null)
            {
                break;
            }
        }
        return position < length;
    }

    public void Reset()
    {
        position = -1;
    }

    public void Dispose()
    {
        contents = new T[length];
    }

    public T this[int index]
    {
        get => getIndex(index);
        set => contents[index] = value;
    }
}
