using UnityEngine;

public class Building : MonoBehaviour
{
    public Vector2Int GridPos;
    public Vector2Int GridSize = new Vector2Int(1, 1);

    public bool AutoAdd;

    public Vector2Int Direction = Vector2Int.right;

    public int DirectionX => (int)Mathf.Sign(Direction.x);

    void Start()
    {
        if (AutoAdd)
        {
            BuildingManager.main.AddBuilding(this);
        }
    }
    void OnDrawGizmos()
    {
        #if UNITY_EDITOR

        if (BuildingManager.Editor != null)
        {
            GridPos = BuildingManager.Editor.WorldToLocal(transform.position);
            transform.position = BuildingManager.Editor.LocalToWorld(GridPos);
            BuildingManager.CellAreaGizmo(GridPos, GridSize);
        }

        #endif
    }

    public Machine[] Simulators;

    public Machine MainSimulator => (Simulators == null || Simulators.Length == 0) ? null : Simulators[0];
}
