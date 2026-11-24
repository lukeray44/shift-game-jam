using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;

public class MiscEditorScripts : ScriptableObject
{
    [MenuItem("Assets/Create Instance From Script")]
    static void CreateInstance()
    {
        int i = 0;
        foreach (UnityEngine.Object obj in Selection.objects)
        {
            if (!string.IsNullOrEmpty(Selection.assetGUIDs[i]))
            {
                Debug.Log(obj.name);

                string path = AssetDatabase.GetAssetPath(obj);
                path = path.Substring(0, path.Length - (obj.name.Length + 3));
                Type objType = TypeEXT.GetType(obj.name);

                ScriptableObject temp = CreateInstance(objType);
                AssetDatabase.CreateAsset(temp, path + $"/{obj.name}.asset");
            }
        }
    }
   
    [MenuItem("Tools/Misc/Move ALl")]
    public static void MoveBack()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            if (obj.TryGetComponent<SpriteRenderer>(out SpriteRenderer s))
            {
                s.sortingOrder -= 10;
            }
        }
    }
    public static ScriptableObject CreateInstancePublic(string name, string path, string pathExtra = "")
    {
        UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
        if (obj == null) return null;
        path = path.Substring(0, path.Length - (obj.name.Length + 3));
        Type objType = TypeEXT.GetType(obj.name);
        if (objType == null) return null;

        ScriptableObject temp = CreateInstance(objType);
        temp.name = name;
        AssetDatabase.CreateAsset(temp, path + pathExtra + $"/{name}.asset");
        return temp;
    }

    public static void DrawIconOnBase(Texture2D baseTex, Texture2D icon)
    {
        if (baseTex == null || icon == null)
        {
            Debug.LogError("Both baseTex and icon must be non-null.");
            return;
        }

        // Ensure the icon fits within the base texture
        if (icon.width > baseTex.width || icon.height > baseTex.height)
        {
            Debug.LogError("Icon texture is too large to fit in the base texture.");
            return;
        }

        // Copy icon pixels onto the base texture in the top-left corner
        for (int y = 0; y < icon.height; y++)
        {
            for (int x = 0; x < icon.width; x++)
            {
                Color iconColor = icon.GetPixel(x, y);
                if (iconColor.a > 0) // Respect transparency
                {
                    baseTex.SetPixel(x, baseTex.height - icon.height + y, iconColor);
                }
            }
        }

        // Apply changes to the base texture
        baseTex.Apply();
    }

    public static void DrawIconOnBaseRotated(Texture2D baseTex, Texture2D icon)
    {
        if (baseTex == null || icon == null)
        {
            Debug.LogError("Both baseTex and icon must be non-null.");
            return;
        }

        // Ensure the icon fits within the base texture
        if (icon.width > baseTex.width || icon.height > baseTex.height)
        {
            Debug.LogError("Icon texture is too large to fit in the base texture.");
            return;
        }

        // Copy icon pixels onto the base texture in the top-left corner
        for (int y = 0; y < icon.height; y++)
        {
            for (int x = 0; x < icon.width; x++)
            {
                Color iconColor = icon.GetPixel(x, icon.height - 1 - y);
                if (iconColor.a > 0) // Respect transparency
                {
                    baseTex.SetPixel(baseTex.width - x - 1, y, iconColor);
                }
            }
        }

        // Apply changes to the base texture
        baseTex.Apply();
    }


}

public static class TypeEXT
{
    public static Type GetType(string typeName)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.LastPartOfTypeName() == typeName)
                {
                    return type;
                }
            }
        }
        return null;
    }

    public static string LastPartOfTypeName(this Type type)
    {
        string[] typeNameFull = type.FullName?.Split('.');
        string typeNameLast = typeNameFull?[typeNameFull.Length - 1];
        return typeNameLast;
    }


    [MenuItem("CONTEXT/MonoBehaviour/References/Fill")]
    public static void FillInReferences(MenuCommand command)
    {
        MonoBehaviour target = (MonoBehaviour)command.context;

        Type t = target.GetType();
        FieldInfo[] properties = t.GetFields().Where(prop => prop.IsDefined(typeof(Reference))).ToArray();

        foreach (FieldInfo p in properties)
        {
            object value = p.GetValue(target);
            if (value == null || value.ToString() == "null")
            {
                Debug.Log("Trying");
                Component m = target.gameObject.GetComponent(p.FieldType);
                Debug.Log(m == null ? "Found" : $"Found {p.FieldType}");
                p.SetValue(target, m);
            }


        }
        EditorUtility.SetDirty(target);
    }
    

    [MenuItem("Tools/Create Parent #C")]
    public static void CreateParent()
    {
        if (Selection.activeGameObject != null)
        {
            GameObject og = Selection.activeGameObject;
            Undo.RecordObject(og, "made parent");
            GameObject g = new GameObject(og.name);
            g.transform.parent = og.transform.parent;
            g.transform.position = og.transform.position;
            g.transform.rotation = og.transform.rotation;
            g.transform.localScale = og.transform.localScale;
            og.transform.parent = g.transform;
            Undo.RegisterCreatedObjectUndo(og, "made parent");
            
            og.transform.localScale = Vector3.one;
            Selection.activeGameObject = g;
        }
    }
    [MenuItem("CONTEXT/MonoBehaviour/References/Create")]
    public static void CreateFillInReferences(MenuCommand command)
    {
        MonoBehaviour target = (MonoBehaviour)command.context;

        Type t = target.GetType();
        FieldInfo[] properties = t.GetFields().Where(prop => prop.IsDefined(typeof(Reference))).ToArray();

        foreach (FieldInfo p in properties)
        {
            object value = p.GetValue(target);
            if (value == null || value.ToString() == "null")
            {
                Component m = target.gameObject.AddComponent(p.FieldType);


                Preset defaultPreset = Preset.GetDefaultForObject(m);
                if (defaultPreset != null)
                {
                    // Apply the preset
                    defaultPreset.ApplyTo(m);
                }

                p.SetValue(target, m);
            }


        }
        EditorUtility.SetDirty(target);
    }
    [MenuItem("CONTEXT/MonoBehaviour/References/CHAIN")]
    [MenuItem("CONTEXT/MonoBehaviour/CHAIN")]
    public static void ChainCreateFillInReferences(MenuCommand command)
    {
        MonoBehaviour target = (MonoBehaviour)command.context;
        Undo.RecordObject(target, "Reference Filled");
        Chain(target);
    }

    public static void Chain(MonoBehaviour target)
    {
        if (target == null) return;
        Type t = target.GetType();
        FieldInfo[] properties = t.GetFields().Where(prop => prop.IsDefined(typeof(Reference))).ToArray();

        foreach (FieldInfo p in properties)
        {
            object value = p.GetValue(target);
            Selection.activeObject = (UnityEngine.Object)value;
            if (value == null || value.ToString() == "null")
            {
                Component m = target.gameObject.GetComponent(p.FieldType);
                if (m == null)
                {

                    m = target.gameObject.GetComponentInChildren(p.FieldType);
                    if (m == null)
                    {
                        m = target.gameObject.AddComponent(p.FieldType);
                        Undo.RegisterCreatedObjectUndo(m, "Reference Filled");
                        Preset defaultPreset = Preset.GetDefaultForObject(m);
                        if (defaultPreset != null)
                        {
                            // Apply the preset
                            defaultPreset.ApplyTo(m);
                        }
                    }
                   

                    
                }

                p.SetValue(target, m);

                Chain(m as MonoBehaviour);
            }

        }
        EditorUtility.SetDirty(target);
    }

    [MenuItem("Tools/Add Shadow")]
    public static void AddDropShadow()
    {
        GameObject g = Selection.activeGameObject;
        if (g == null) return;

        SpriteRenderer s = new GameObject("drop shadow").AddComponent<SpriteRenderer>();
        s.transform.parent = g.transform;
        s.transform.position = g.transform.position;
        s.transform.localScale = new Vector3(1, 0.4f);
        s.color = new Color(0, 0, 0, 0.3f);
        s.sortingOrder = -99;
        s.sprite = AssetDatabase.LoadAssetAtPath("Assets/Art/Circle.png", typeof(Sprite)) as Sprite;
    }


}



[CustomPropertyDrawer(typeof(FineControlAttribute))]
public class FineControlDrawer : PropertyDrawer
{
    bool isDragging;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Event e = Event.current;

        // Check if the left mouse button is pressed
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            // Record the initial value when the drag operation starts
            isDragging = true;
        }
        // Check if the left mouse button is released
        else if (e.type == EventType.MouseUp && e.button == 0)
        {
            isDragging = false;
        }
        FineControlAttribute fineControl = (FineControlAttribute)attribute;

        if (property.propertyType == SerializedPropertyType.Float)
        {
            float newValue = EditorGUI.FloatField(position, label, property.floatValue);
            if (GUI.changed && isDragging)
            {
                float increment = fineControl.increment;
                newValue = (newValue - property.floatValue) * increment;
                property.floatValue += newValue;
            } else if (!isDragging)
            {
                property.floatValue = newValue;
            }
        }
        else if (property.propertyType == SerializedPropertyType.Vector3)
        {
            Vector3 newValue = EditorGUI.Vector3Field(position, label.text, property.vector3Value);
            if (GUI.changed && isDragging)
            {
                float increment = fineControl.increment;
                newValue = (newValue - property.vector3Value) * increment;
                property.vector3Value += newValue;
            }
            else if (!isDragging)
            {
                property.vector3Value = newValue;
            }
        }
        else
        {
            EditorGUI.LabelField(position, label.text, "Use [FineControl] with float.");
        }
    }
}