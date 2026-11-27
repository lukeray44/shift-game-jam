using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager main;
    void Awake()
    {
        main = this;
    }

    public Item RootPrefab;
    public ItemElement ElementPrefab, ElementPrefabUI;

    public ItemList Index;

    public ItemType Get(int ID)
    {
        return Index.Types[ID];
    }

    public Item Create(ItemComposite composite, Transform UIParent = null, bool centred = false)
    {

        Item root = Instantiate(RootPrefab, UIParent);
        root.state = composite;
        
        foreach (ItemState s in composite.Items)
        {
            ItemElement element;
            if (UIParent != null) element = Instantiate(ElementPrefabUI, UIParent);
            else element = Instantiate(ElementPrefab);
            element.LoadState(s);
            root.AddElement(element);
        }

        if (centred) composite.Translate(Vector2.down * (composite.size.y - 1) * 0.5f);

        root.PositionChildren();

        if (centred) composite.Translate(Vector2.up * (composite.size.y - 1) * 0.5f);

        
        return root;

    }

    public Item Merge(Item keep, Item destroy, Vector2Int dir)
    {
        ItemComposite newComp = ItemComposite.Merge(keep.state, destroy.state, dir);
        keep.state = newComp;
        foreach (var i in destroy.Elements)
        {
            keep.AddElement(i);
        }
        Destroy(destroy.gameObject);
        return keep;
    }

    public Item CreateByRows(params int[][] IDs)
    {
        ItemType[][] IDList = new ItemType[IDs.Length][];
        for (int i = 0; i < IDList.Length; i++)
        {
            IDList[i] = new ItemType[IDs[i].Length];

            for (int y = 0; y < IDList[i].Length; y++)
            {
                IDList[i][y] = Get(IDs[i][y]);
            }
        }

        return CreateByRows(IDList);
    } 
    public Item CreateByRows(params ItemType[][] IDs)
    {
        ItemComposite composite = ItemComposite.CreateByRows(IDs);
        return Create(composite);
    } 

    public Item CreateByRows(ItemByRows items)
    {
    
        return CreateByRows(GetStructure(items));
    } 
    
    public ItemComposite GetStateStructure(ItemByRows items)
    {
        return ItemComposite.CreateByRows(GetStructure(items));
    }

    public ItemType[][] GetStructure(ItemByRows items)
    {
         ItemType[][] IDList = new ItemType[items.Rows.Length][];

        for (int i = 0; i < IDList.Length; i++)
        {
            IDList[i] = new ItemType[items.Rows[i].IDs.Length];

            for (int y = 0; y < IDList[i].Length; y++)
            {
                IDList[i][y] = Get(items.Rows[i].IDs[y]);
            }
        }
        return IDList;
    }
}


[System.Serializable]
public class ItemByRows
{
    public ItemRow[] Rows;

    public ItemByRows(params int[][] ids)
    {
        Rows = new ItemRow[ids.Length];

        for (int i = 0; i < ids.Length; i++)
        {
            Rows[i] = new ItemRow(){IDs = ids[i]};
        }
    }
}

[System.Serializable]
public class ItemRow
{
    public int[] IDs;
}