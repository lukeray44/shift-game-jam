using Sirenix.OdinInspector;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public LineRenderer Line;


    [Button]
    void setRadius(float radius)
    {
        Radius = radius;
    }
    public float Radius
    {
        set
        {
            Line.startWidth = Line.endWidth = value;
            Back.transform.localScale = Front.transform.localScale = Vector3.one * value;
        }
        get
        {
            return Back.transform.localScale.x;
        }
    }

    public SpriteRenderer Back, Front;


    public void SetPositions(params Vector3[] positions)
    {
        Line.positionCount = positions.Length;
        Line.SetPositions(positions);
        Back.transform.position = positions[0];
        Front.transform.position = positions[positions.Length - 1];
    }
}
