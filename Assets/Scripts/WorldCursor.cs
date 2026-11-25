using UnityEngine;

public class WorldCursor : MonoBehaviour
{
    public SpriteRenderer C1, C2, C3, C4;

    public void SetColor(Color c)
    {
        C1.color = C2.color = C3.color = C4.color = c;
    }

    public void SetPosition(Vector3 pos, Vector3 size)
    {
        size *= 0.5f;
        C1.transform.position = pos + new Vector3(-1 * size.x, 1 * size.y);
        C2.transform.position = pos + new Vector3(1 * size.x, 1 * size.y);
        C3.transform.position = pos + new Vector3(1 * size.x, -1 * size.y);
        C4.transform.position = pos + new Vector3(-1 * size.x, -1 * size.y);
    }
}
