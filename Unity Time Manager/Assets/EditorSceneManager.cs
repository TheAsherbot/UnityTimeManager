using UnityEditor;

using UnityEngine;
using UnityEngine.SceneManagement;

public class EditorSceneManager : AssetPostprocessor
{
    public delegate void OnSceneImportedCallBack(Scene importedScene);
    public delegate void OnSceenDeletedCallBack(Scene deletedScene);
    public delegate void OnSceneMovedCallBack(Scene movedScene, string oldPath, string newPath);
    public delegate void OnSceneRenamedCallBack(Scene renamedScene, string oldName, string newName);



    public static event OnSceneImportedCallBack OnSceneImported;
    public static event OnSceenDeletedCallBack OnSceenDeleted;
    public static event OnSceneMovedCallBack OnSceneMoved;
    public static event OnSceneRenamedCallBack OnSceneRenamed;





    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
    {
        HandleImportedAssets(importedAssets);

        HandleDeletedAssets(deletedAssets);

        HandleMovedAssets(movedAssets, movedFromAssetPaths);
    }

    private static void HandleImportedAssets(string[] importedAssets)
    {
        foreach (string importedAsset in importedAssets)
        {
            if (IsPathToScene(importedAsset))
            {
                string sceneName = GetFileNameFromPath(importedAsset);

                OnSceneImported?.Invoke(SceneManager.GetSceneByPath(importedAsset));
                Debug.Log("Reimported Asset: " + importedAsset);
            }
        }
    }

    private static void HandleDeletedAssets(string[] deletedAssets)
    {
        foreach (string deletedAsset in deletedAssets)
        {
            if (IsPathToScene(deletedAsset))
            {
                string sceneName = GetFileNameFromPath(deletedAsset);

                OnSceenDeleted?.Invoke(SceneManager.GetSceneByPath(deletedAsset));
                Debug.Log("Deleted Asset: " + deletedAsset);
            }
        }
    }

    private static void HandleMovedAssets(string[] movedAssets, string[] movedFromAssetPaths)
    {
        for (int i = 0; i < movedAssets.Length; i++)
        {
            if (IsPathToScene(movedAssets[i]))
            {
                string newSceneName = GetFileNameFromPath(movedAssets[i]);
                string oldSceneName = GetFileNameFromPath(movedFromAssetPaths[i]);

                // Must have moved the file
                if (newSceneName == oldSceneName)
                {
                    OnSceneMoved?.Invoke(SceneManager.GetSceneByName(newSceneName), movedFromAssetPaths[i].Split('.')[0], movedAssets[i].Split('.')[0]);
                    Debug.Log("Scene Moved from \"" + movedFromAssetPaths[i].Split('.')[0] + "\" to \"" + movedAssets[i].Split('.')[0] + "\"");
                }
                // Must have renamed the file
                else
                {
                    OnSceneRenamed?.Invoke(SceneManager.GetSceneByName(newSceneName), oldSceneName, newSceneName);
                    Debug.Log("Scene Renamed from \"" + oldSceneName + "\" to \"" + newSceneName + "\"");
                }
            }
        }
    }


    private static bool IsPathToScene(string path)
    {
        return path.EndsWith(".unity");
    }

    private static string GetFileNameFromPath(string path)
    {
        string[] partsOfPath = path.Split('/');
        return partsOfPath[partsOfPath.Length - 1].Split('.')[0]; // reveomving the file name extention.
    }
}
