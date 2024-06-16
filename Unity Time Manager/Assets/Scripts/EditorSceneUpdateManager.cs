using UnityEditor;
using UnityEditor.SceneManagement;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheAshBotAssets.TimeTracker
{
    public class EditorSceneUpdateManager : AssetPostprocessor
    {
        public delegate void OnSceneImportedCallBack(Scene importedScene, string name);
        public delegate void OnSceenDeletedCallBack(Scene deletedScene, string name);
        public delegate void OnSceneMovedCallBack(Scene movedScene, string oldPath, string newPath);
        public delegate void OnSceneRenamedCallBack(Scene renamedScene, string oldName, string newName);



        public static event OnSceneImportedCallBack OnSceneImported;
        public static event OnSceenDeletedCallBack OnSceenDeleted;
        public static event OnSceneMovedCallBack OnSceneMoved;
        public static event OnSceneRenamedCallBack OnSceneRenamed;




        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
        {
            HandleDeletedAssets(deletedAssets);

            bool hasAssetMoved = HandleMovedAssets(movedAssets, movedFromAssetPaths);

            if (!hasAssetMoved)
            {
                HandleImportedAssets(importedAssets);
            }
        }

        private static void HandleImportedAssets(string[] importedAssets)
        {
            foreach (string importedAsset in importedAssets)
            {
                if (IsAssetPathAScene(importedAsset))
                {
                    string sceneName = GetFileNameFromPath(importedAsset);

                    OnSceneImported?.Invoke(EditorSceneManager.GetSceneByPath(sceneName), sceneName);
                    Debug.Log("Reimported Asset: " + importedAsset);
                }
            }
        }

        private static void HandleDeletedAssets(string[] deletedAssets)
        {
            foreach (string deletedAsset in deletedAssets)
            {
                if (IsAssetPathAScene(deletedAsset))
                {
                    string sceneName = GetFileNameFromPath(deletedAsset);

                    OnSceenDeleted?.Invoke(EditorSceneManager.GetSceneByPath(sceneName), sceneName);
                    Debug.Log("Deleted Asset: " + deletedAsset);
                }
            }
        }

        private static bool HandleMovedAssets(string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (movedAssets.Length == 0)
            {
                return false;
            }
            for (int i = 0; i < movedAssets.Length; i++)
            {
                if (IsAssetPathAScene(movedAssets[i]))
                {
                    string newSceneName = GetFileNameFromPath(movedAssets[i]);
                    string oldSceneName = GetFileNameFromPath(movedFromAssetPaths[i]);

                    // Must have moved the file
                    if (newSceneName == oldSceneName)
                    {
                        OnSceneMoved?.Invoke(EditorSceneManager.GetSceneByName(newSceneName), movedFromAssetPaths[i].Split('.')[0], movedAssets[i].Split('.')[0]);
                        Debug.Log("Scene Moved from \"" + movedFromAssetPaths[i].Split('.')[0] + "\" to \"" + movedAssets[i].Split('.')[0] + "\"");
                    }
                    // Must have renamed the file
                    else
                    {
                        OnSceneRenamed?.Invoke(EditorSceneManager.GetSceneByName(newSceneName), oldSceneName, newSceneName);
                        Debug.Log("Scene Renamed from \"" + oldSceneName + "\" to \"" + newSceneName + "\"");
                    }
                }
            }
            return true;
        }


        private static bool IsAssetPathAScene(string path)
        {
            return path.EndsWith(".unity");
        }

        private static string GetFileNameFromPath(string path)
        {
            string[] partsOfPath = path.Split('/');
            return partsOfPath[partsOfPath.Length - 1].Split('.')[0]; // reveomving the file name extention.
        }

    }
}
