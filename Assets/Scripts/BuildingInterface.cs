using System;
using UnityEngine;

public class BuildingInterface : MonoBehaviour
{
    public BuildingManager Manager;


    public Building Placing;

    public Building[] TempToolbar;
    int selectedIndex;
    Vector2Int lastDir = Vector2Int.right;
    void LoadIndex(int index)
    {
       
        selectedIndex = (int)Mathf.Repeat(index, TempToolbar.Length);

        SetPlacing(TempToolbar[selectedIndex]);
    }
    void ReloadIndex()
    {
        Placing = null;
        LoadIndex(selectedIndex);
    }
    public void SetPlacing(Building prefab)
    {
        if (Placing != null)
        {
            Destroy(Placing.gameObject);
        }

        Placing = Instantiate(prefab);
        Placing.gameObject.SetActive(false); 
        Placing.SetRotation(lastDir);
        setDirty();
    }


    Vector2Int lastPos;
    bool dirty;
    void setDirty()
    {
        dirty = true;
        Debug.Log(dirty);
    }

    public SpriteRenderer cursor;

    public void HighlightTile(Vector2Int tile, Vector2Int size)
    {
        cursor.gameObject.SetActive(true);
        Vector3 worldSize = new Vector3(size.x * Manager.Grid.cellSize.x, size.y * Manager.Grid.cellSize.y);
        cursor.transform.position = Manager.LocalToWorld(tile) + worldSize * 0.5f - Manager.Grid.cellSize * 0.5f;

        cursor.size = worldSize;
    }
    public void HighlightBuilding(Building b)
    {
        HighlightTile(b.GridPos, b.GridSize);
    }

    Building selected;
    bool valid;
    void Update()
    {
        Vector3 input = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2Int mouseTile = Manager.WorldToLocal(input);


        bool tileChanged = lastPos != mouseTile || dirty;

        if (tileChanged)
        {
            dirty = false;
            lastPos = mouseTile;
            cursor.gameObject.SetActive(false);
            cursor.color = Color.yellow;
            selected = Manager.GetBuildingAtPos(mouseTile);
        }



        if (selected != null)
        {

            if (tileChanged)
            {
                if (Placing != null) Placing.gameObject.SetActive(false);
                HighlightBuilding(selected);
                cursor.color = Color.red;
            }

            if (Input.GetMouseButtonDown(1))
            {
                Manager.DeleteBuilding(selected);
                setDirty();
                return;
            }
        }
        else if (Placing != null)
        {
            if (tileChanged)
            {
                Placing.gameObject.SetActive(true);
                Placing.GridPos = mouseTile;
                Placing.Position(mouseTile);
                HighlightBuilding(Placing); 

                valid = Placing.CanPlaceHere(mouseTile);

                if (!valid) cursor.color = Color.red;

            }




            if (valid && Input.GetMouseButtonDown(0))
            {
                Manager.AddBuilding(Placing);
                Placing = null;
                ReloadIndex();
                setDirty();
                return;
            }


        }
        else
        {
            HighlightTile(mouseTile, Vector2Int.one);
        }


        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            LoadIndex(selectedIndex - 1);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            LoadIndex(selectedIndex + 1);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Placing.Rotate();
            lastDir = Placing.Direction;
        }
    }


}