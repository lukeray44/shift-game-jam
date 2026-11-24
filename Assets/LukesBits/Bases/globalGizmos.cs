using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

public class globalGizmos : AdditiveSingleton<globalGizmos>
{
    List<drawnGizmo> gizmos = new List<drawnGizmo>(10);
     List<drawnGizmo> last = new List<drawnGizmo>(10);
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        foreach (drawnGizmo g in last)
        {
            g.drawGizmo();
        }
        gizmos = new List<drawnGizmo>(10);
    }

    public static void Draw(drawnGizmo g)
    {
        main.gizmos.Add(g);
    }
    public static void DrawPoint(Vector3 pos, Color c)
    {
        main.gizmos.Add(new circleGizmo(pos) { color = c });
    }
    public static void DrawCircle(Vector3 pos, float r, Color c)
    {
        main.gizmos.Add(new circleGizmo(pos, r) { color = c });
    }
    public static void Text(Vector3 pos, string t)
    {

#if UNITY_EDITOR
        if (Application.isPlaying)
            main.gizmos.Add(new textGizmo(pos, t));
#endif
    }

    public static void DrawPoint(Vector3 pos)
    {
        main.gizmos.Add(new circleGizmo(pos, 0.1f) { color = Color.red });
    }
    private void LateUpdate()
    {
        last = gizmos;
        gizmos = new List<drawnGizmo>(10);
    }
}

[System.Serializable]
public abstract class drawnGizmo
{
    public Color color = Color.cyan;
    public void drawGizmo()
    {
        Gizmos.color = color;
        draw();
    }
    public abstract void draw();
}
[System.Serializable]
public class lineGizmo : drawnGizmo
{
    Vector3 p1;
    Vector3 p2;
    public lineGizmo (Vector3 p1, Vector3 p2)
    {
        this.p1 = p1;
        this.p2 = p2;
    }

    public override void draw()
    {
        Gizmos.DrawLine(p1, p2);
    }
}
#if UNITY_EDITOR
[System.Serializable]
public class textGizmo : drawnGizmo
{
    Vector3 p1;
    string text;
    public textGizmo(Vector3 p1, string text)
    {
        this.p1 = p1;
        this.text = text;
    }

    public override void draw()
    {
        Handles.Label(p1, text);
    }
}
#endif
[System.Serializable]
public class circleGizmo : drawnGizmo
{
    Vector3 p1;
    float radius;
    public circleGizmo(Vector3 p1, float radius = 0.2f)
    {
        this.radius = radius;
        this.p1 = p1;
    }

    public override void draw()
    {
        Gizmos.DrawSphere(p1, radius);
    }
}
[System.Serializable]
public class boxGizmo : drawnGizmo
{
    Vector3 p1;
    Vector3 size;
    public boxGizmo(Vector3 p1, Vector3 size)
    {
        this.size = size;
        this.p1 = p1;
    }
    public static boxGizmo corners(Vector3 p1, Vector3 p2)
    {
        return new boxGizmo(Vector3.Lerp(p1, p2, 0.5f), p2 - p1);
    }
    public static boxGizmo corners(Vector3 p1, Vector3 p2, Color c)
    {
        return new boxGizmo(Vector3.Lerp(p1, p2, 0.5f), p2 - p1) { color = c };
    }
    public override void draw()
    {
        Gizmos.DrawWireCube(p1, size);
    }
}
[System.Serializable]
public class solidBoxGizmo : drawnGizmo
{
    Vector3 p1;
    Vector3 size;
    public solidBoxGizmo(Vector3 p1, Vector3 size)
    {
        this.size = size;
        this.p1 = p1;
    }
    public static solidBoxGizmo corners(Vector3 p1, Vector3 p2)
    {
        return new solidBoxGizmo(Vector3.Lerp(p1, p2, 0.5f), p2 - p1);
    }
    public static solidBoxGizmo corners(Vector3 p1, Vector3 p2, Color c)
    {
        return new solidBoxGizmo(Vector3.Lerp(p1, p2, 0.5f), p2 - p1) { color = c };
    }
    public override void draw()
    {
        Gizmos.DrawCube(p1, size);
    }
}