using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public Machine parent;

    public Vector3 PreviousPosition;
    public void SavePosition()
    {
        PreviousPosition = transform.position;
    }

    public ItemComposite state;
    public List<ItemElement> Elements = new List<ItemElement>();

    public const float ElementRadius = 0.6f;

    public Vector3 GetLocalPosition(ItemElement e)
    {
        return e.state.localPosition * ElementRadius;
    }

    public void PositionChildren()
    {
        foreach (var e in Elements)
        {
            PositionChild(e);
        }
    }

    public void PositionChild(ItemElement e)
    {
        e.transform.position = transform.position + GetLocalPosition(e);
        e.transform.localEulerAngles = e.state.GetRotation();
    }

    public void AddElement(ItemElement e)
    {
        e.Parent = this;
        Elements.Add(e);
        e.transform.parent = transform;
    }
}

[System.Serializable]
public class ItemState
{
    public ItemType ID;
    public int Rotation;

    public Vector2 localPosition;

    public ItemState(ItemType ID, Vector2 localPosition)
    {
        this.ID = ID;
        this.localPosition = localPosition;
    }

    public override int GetHashCode()
    {
        return ID.GetHashCode() + Mathf.Repeat(Rotation, (int)ID.symmetry).GetHashCode() * 20 + ItemComposite.GetDoubledInt2(localPosition).GetHashCode();
    }

    public Vector3 GetRotation()
    {
        return Vector3.forward * Rotation * 90;
    }

    public void SetRotation(int i)
    {
        Rotation = i;;
    }
}
[System.Serializable]
public class ItemComposite
{
    public HashSet<int> ItemCodes = new HashSet<int>();
    public List<ItemState> Items = new List<ItemState>();

    public Vector2 minLocal, maxLocal;

    public Vector3 size => maxLocal - minLocal + Vector2.one;
    public Vector3 centre => (minLocal + maxLocal) * 0.5f;

    public static Vector3 GetRotation(int Rotation)
    {
        return Vector3.forward * Rotation * 90;
    }
    public static Vector2Int GetDoubledInt2(Vector2 round)
    {
        return new Vector2Int(Mathf.RoundToInt(round.x * 2), Mathf.RoundToInt(round.y * 2));
    }
    public void Add(ItemType i, Vector2 position)
    {
        ItemState state = new ItemState(i, position);

        Add(state);
    }
    public void Add(ItemState state)
    {
        minLocal = Vector2.Min(minLocal, state.localPosition);
        maxLocal = Vector2.Max(maxLocal, state.localPosition);

        Items.Add(state);
        ItemCodes.Add(state.GetHashCode());
    }

    public void UpdateItem(ItemState s, System.Action<ItemState> act)
    {
        ItemCodes.Remove(s.GetHashCode());
        act(s);
        ItemCodes.Add(s.GetHashCode());
    }

    public void Rotate(int i)
    {
        Quaternion rotateBy = Quaternion.Euler(0, 0, i * 90f);

        foreach (ItemState e in Items)
        {
            UpdateItem(e, s =>
            {
               s.localPosition = rotateBy * s.localPosition;
               s.SetRotation(s.Rotation + i);
            });
        }
    }

    // won't update hash map, assume discard
    public void Translate(Vector2 pos)
    {
        minLocal += pos;
        maxLocal += pos;

        foreach (ItemState s in Items)
        {
            s.localPosition += pos;
        }
    }

    public static ItemComposite CreateByRows(params ItemType[][] rows)
    {
        ItemComposite c = new ItemComposite();

        float getAxis(float count, int index)
        {
            if (count == 1) return 0;

            count -= 1f;
            return Mathf.Lerp(-count * 0.5f, count * 0.5f, index / (count));
        }

        float spany = rows.Length;
        int y = 0;
        foreach (ItemType[] idRow in rows)
        {
            float ypos = y;


            int x = 0;
            float spanx = idRow.Length;
            foreach (ItemType id in idRow)
            {
                float xpos = getAxis(spanx, x);

                c.Add(id, new Vector2(xpos, ypos));

                x++;
            }

            y++;
        }

        return c;
    }


    // c1 is joined to c2 in direction dir
    // eg if dir = up, c2 is attached to the top of c2
    public static ItemComposite Merge(ItemComposite c1, ItemComposite c2, Vector2Int dir)
    {
        Vector2 multiply(Vector2 v1, Vector2 v2) => new Vector2(v1.x * v2.x, v1.y * v2.y);

        c2.Translate(multiply(dir, c1.size));

        ItemComposite composite = new ItemComposite();

        foreach (ItemState s in c1.Items)
        {
            composite.Add(s);
        }

        foreach (ItemState s in c2.Items)
        {
            composite.Add(s);
        }

        return composite;
    }


    public override bool Equals(object obj)
    {
        if (obj is ItemComposite c)
        {
            return c == this;
        }
        return false;
    }
    public static bool operator ==(ItemComposite o1, ItemComposite o2)
    {
        foreach (ItemState c in o1.Items)
        {
            if (!o2.ItemCodes.Contains(c.GetHashCode()))
            {
                return false;
            }
        }

        foreach (ItemState c in o2.Items)
        {
            if (!o1.ItemCodes.Contains(c.GetHashCode()))
            {
                return false;
            }
        }

        return true;
    }

    public static bool operator !=(ItemComposite c1, ItemComposite c2)
    {
        return !(c1 == c2);
    }

    public override int GetHashCode()
    {
        int h = 0;
        foreach (var c in Items)
        {
            h += c.GetHashCode();
        }
        return h;
    }

}