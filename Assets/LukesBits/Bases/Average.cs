using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class AverageVector3
{
    public Vector3 value;
    Buffer<Vector3> list;
    public int amount;
    int limit;

    public AverageVector3(int limit)
    {
        this.limit = limit;
        list = new Buffer<Vector3>(limit);
        amount = 0;
    }

    public void AddVector(Vector3 t_value)
    {
        value *= amount;
        value += t_value;
        
        if (list.Full)
        {
            value -= list.nextBuffer();
        }
        else
        {
            amount++;
        }

        value /= amount;
        list.addBuffer(t_value);
    }

}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(AverageVector3))]
public class VecAveragePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //Rects
        Rect p1 = position.subdivide(new Vector2(0.3f, 1));
        Rect p2 = position.subdivide(new Vector2(0.2f, 1), new Vector2(0.3f, 0));
        Rect p3 = position.subdivide(new Vector2(0.5f, 1), new Vector2(0.5f, 0));

        //Draw Properties
        EditorGUI.LabelField(p1, label);
        EditorGUI.LabelField(p2, $"{property.FindPropertyRelative("amount").intValue} Samples");
        EditorGUI.PropertyField(p3, property.FindPropertyRelative("value"), GUIContent.none);

    }
}
#endif

