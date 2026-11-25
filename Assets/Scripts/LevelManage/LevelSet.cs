
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;


#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;
#endif

public class LevelSet : ScriptableObject
{
    public LevelData[] Levels;

    public string LevelTitle(int i) =>
        $"Level {i + 1}: \"{Levels[i].LevelName}\"";

#if UNITY_EDITOR
    
[Button]
public async void TakePhotos()
{
    const string folder = "Assets/Level/LevelScreenshots";

    if (!Directory.Exists(folder))
        Directory.CreateDirectory(folder);

    var previousScene = SceneManager.GetActiveScene().path;

    for (int i = 0; i < Levels.Length; i++)
    {
        string scenePath = $"Assets/Scenes/Level{i}.unity";

        if (!File.Exists(scenePath))
        {
            Debug.LogError($"Scene not found: {scenePath}");
            continue;
        }

        EditorSceneManager.OpenScene(
            scenePath,
            OpenSceneMode.Single
        );

        Camera.main.cullingMask ^= (1 << 5);

        string filename = $"Level{i}.png";
        string fullPath = Path.Combine(folder, filename);

        ScreenCapture.CaptureScreenshot(fullPath);

        await Task.Delay(500);

        Camera.main.cullingMask ^= (1 << 5);

        // Import screenshot into Unity
        AssetDatabase.ImportAsset(fullPath);

        // Load imported texture
        Texture2D screenshot = AssetDatabase.LoadAssetAtPath<Texture2D>(
            fullPath
        );

        if (screenshot == null)
        {
            Debug.LogError($"Failed to load screenshot: {fullPath}");
            continue;
        }

        // Assign to LevelData
        Levels[i].LevelScreenshot = screenshot;

        // Mark LevelData as dirty
        EditorUtility.SetDirty(Levels[i]);

        Debug.Log($"Saved screenshot for {Levels[i].LevelName}");
    }

    // Save all asset changes
    EditorUtility.SetDirty(this);
    AssetDatabase.SaveAssets();

    // Restore previous scene
    EditorSceneManager.OpenScene(
        previousScene,
        OpenSceneMode.Single
    );

    Debug.Log("Finished taking level screenshots!");
}

#endif
}
