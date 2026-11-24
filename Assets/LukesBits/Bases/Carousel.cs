using System.Collections;
using UnityEngine;


public class Carousel<T>
{
    public int Amount;
    public T[] Contents;
    int current;
    int currentGet;
    public T Get(int index)
    {
        return (Contents[index]);
    }
    public T GetNext()
    {
        return (Contents[++currentGet % Amount]);
    }
    public T GetCurrent(int index = 0)
    {
        return (Contents[(currentGet + index) % Amount]);
    }
    public void Add(T obj)
    {
        Contents[++current % Amount] = obj;
    }

    public Carousel(int Amount)
    {
        Contents = new T[Amount];
        this.Amount = Amount;
    }
    public Carousel(T[] initialContent) // won't copy - so be careful
    {
        Contents = initialContent;
        this.Amount = initialContent.Length;
    }
}