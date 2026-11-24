using UnityEngine;
using UnityEditor;

public static class ScriptableObjectFromInstance
{
    [MenuItem("Tools/Object from Instance %i")]
    private static void CreateScriptableObjectFromInstance()
    {
        // Get the selected object in the Project window
        UnityEngine.Object selectedObject = Selection.activeObject;

        // Check if the selected object is a script
        if (selectedObject is MonoScript)
        {
            // Get the type of the script
            System.Type scriptType = (selectedObject as MonoScript).GetClass();

            // Check if the script is a valid scriptable object
            if (scriptType != null && scriptType.IsSubclassOf(typeof(ScriptableObject)))
            {
                // Create an instance of the scriptable object
                ScriptableObject instance = ScriptableObject.CreateInstance(scriptType);

                // Create the asset file for the scriptable object
                string assetPath = AssetDatabase.GetAssetPath(selectedObject);
                assetPath = assetPath.Replace(".cs", ".asset");
                AssetDatabase.CreateAsset(instance, assetPath);
                AssetDatabase.SaveAssets();

                Debug.Log("Scriptable object instance created from " + scriptType.Name + " at: " + assetPath);
            }
            else
            {
                Debug.LogWarning("Selected script is not a valid ScriptableObject script");
            }
        }
        else
        {
            Debug.LogWarning("Selected object is not a script");
        }
    }
}