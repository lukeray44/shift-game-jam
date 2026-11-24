using System.Collections;
using UnityEngine;


public abstract class Pool<T> : Singleton<Pool<T>> where T : Component
{
    public T Prefab;
    public T[] Base;
    public Buffer<T> Buffer;
    private void Start()
    {
        Buffer = new Buffer<T>(Base);
    }
    public T Next()
    {
        T toReturn = Buffer.nextBuffer(); ;
        toReturn.gameObject.SetActive(true);
        return toReturn;
    }
    public void Insert(T t)
    {
        t.gameObject.SetActive(false);
        Buffer.addBuffer(t);
    }
}
#if UNITY_EDITOR
namespace UnityEditor
{
    public abstract class CustomPoolEditorClass<T> : Editor where T : Component
    {
        int spawn;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Pool<T> source = target as Pool<T>;


            GUILayout.BeginHorizontal();
            spawn = EditorGUILayout.IntField(spawn, GUILayout.Width(50));
            EditorGUI.BeginDisabledGroup(spawn == 0);
            if (GUILayout.Button("Create"))
            {
                for (int i = 0; i < source.Base.Length; i++)
                {
                    DestroyImmediate(source.Base[i].gameObject);
                }
                source.Base = new T[spawn];
                for (int i = 0; i < spawn; i++)
                {
                    source.Base[i] = PrefabUtility.InstantiatePrefab(source.Prefab, source.transform) as T;
                    source.Base[i].gameObject.SetActive(false);
                }
                spawn = 0;
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();
        }
    }
}
#endif