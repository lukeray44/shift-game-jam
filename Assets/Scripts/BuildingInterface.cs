using System;
using UnityEngine;

public class BuildingInterface : MonoBehaviour
{
    public BuildingManager Manager;

    public BuildingData CurrentlyPlacing;
    public Building Placing;

    public BuildingData[] TempToolbar;
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
        SetPlacing(lastAdded);
    }
    BuildingData lastAdded;

    public Action<BuildingData> OnSetPlacing;
    public Action<BuildingData> OnPlace;
    public Action<Building> OnDestroy;
    public void SetPlacing(BuildingData data)
    {

        lastAdded = data;
        if (Placing != null)
        {
            Destroy(Placing.gameObject);
        }

        CurrentlyPlacing = data;
        if (data != null)
        {
            if (data.Prefab == null)
            {

                return;
            }
            Placing = Instantiate(data.Prefab);
            Placing.gameObject.SetActive(false);
            Placing.SetRotation(lastDir);
            Placing.Data = data;
        }

        OnSetPlacing?.Invoke(data);
        setDirty();
    }


    Vector2Int lastPos;
    bool dirty;
    void setDirty()
    {
        dirty = true;

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
    bool holdingRightClick;

    void OnDisable()
    {
        Cursor.visible = true;
    }
    void Update()
    {
        if (UIManager.Hovered)
        {
            if (Placing != null) Placing.gameObject.SetActive(false);
            setDirty();
            cursor.gameObject.SetActive(false);
            Cursor.visible = true;
            return;
        }

        Cursor.visible = false;
        if (float.IsInfinity(Input.mousePosition.x)) return;
        Vector3 input = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2Int mouseTile = Manager.WorldToLocal(input);


        bool tileChanged = lastPos != mouseTile || dirty;

        if (tileChanged)
        {
            dirty = false;
            lastPos = mouseTile;
            cursor.gameObject.SetActive(false);
            cursor.color = Color.white;
            selected = Manager.GetBuildingAtPos(mouseTile);
            Visualiser.HideAll();
        }

        if (Input.GetMouseButtonUp(1)) holdingRightClick = false;


        if (selected != null)
        {
            if (Input.GetMouseButtonDown(1)) holdingRightClick = true;

            if (holdingRightClick && selected.Destroyable)
            {
                OnDestroy?.Invoke(selected);
                Manager.DeleteBuilding(selected);
                setDirty();
                return;
            }

            if (tileChanged)
            {
                BuildingManager.main.Visualise(selected);
                HighlightBuilding(selected);
                cursor.color = Color.white;

            }
        }

        if (Placing != null)
        {
            if (tileChanged)
            {
                Placing.gameObject.SetActive(true);
                Placing.GridPos = mouseTile;
                Placing.Position(mouseTile);

                if (selected == null) HighlightBuilding(Placing);

                valid = Placing.CanPlaceHere(mouseTile);

                if (!valid) cursor.color = Color.red;
                else BuildingManager.main.PreVisualise(Placing);

            }




            if (valid && (Input.GetMouseButtonDown(0) || (Input.GetMouseButton(0) && CurrentlyPlacing.DragPlace)))
            {
                OnPlace?.Invoke(CurrentlyPlacing);
                Manager.AddBuilding(Placing);
                CurrentlyPlacing = null;
                Placing = null;
                ReloadIndex();
                setDirty();

                return;
            }


        }
        else if (selected == null)
        {
            HighlightTile(mouseTile, Vector2Int.one);
        }

        if (Input.GetKeyDown(KeyCode.R) && Placing != null)
        {
            Placing.Rotate();
            lastDir = Placing.Direction;
            setDirty();
        }

    }


}