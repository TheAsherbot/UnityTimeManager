using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Unity.EditorCoroutines.Editor;

using UnityEditor;
using UnityEditor.SceneManagement;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheAshBotAssets.TimeTracker
{
    [InitializeOnLoad]
    public class TimeTracker
    {
        
        static TimeTracker()
        {
            TimeTracker timeTracker = new TimeTracker();
        }



        private static readonly string TIME_SPENT_FILE_PATH = Application.dataPath + '/';
        private static readonly string TIME_SPENT_FILE_NAME = "TimeSpent.txt";

        private string currentUserName;

        private DateTime startSessionDateTime;
        private DateTime lastDatetime;
        private DateTime currentDatetime;

        private Time48Bit elapsedTime;
        private Time48Bit elapsedSceneTime;
        private Time48Bit elapsedSessionTime;
        private Time48Bit todaysElapsedTime;
        private Time48Bit totalElapsedTime;
        private Time48Bit totalUserElapsedTime;

        private List<SaveData.SceneTime> sceneTimeList;
        private int activeSceneIndex;

        private SaveData saveData;
        public int currentUserIndex;


        private static string GetFileNameFromPath(string path)
        {
            string[] partsOfPath = path.Split('/');
            return partsOfPath[partsOfPath.Length - 1].Split('.')[0]; // reveomving the file name extention.
        }


        private TimeTracker()
        {
            Initiate();
        }

        private async void Initiate()
        {
            AssemblyReloadEvents.beforeAssemblyReload += () =>
            {
                UpdateSaveData();
                Save();
            };

            EditorApplication.wantsToQuit += OnEditorClose;

            EditorSceneManager.activeSceneChangedInEditMode += EditorSceneManager_activeSceneChangedInEditMode;
            EditorSceneUpdateManager.OnSceneImported += EditorSceneManager_OnSceneImported;
            EditorSceneUpdateManager.OnSceneRenamed += EditorSceneManager_OnSceneRenamed;
            EditorSceneUpdateManager.OnSceenDeleted += EditorSceneManager_OnSceenDeleted;


            currentUserName = await GetCurrentUserName();

            startSessionDateTime = DateTime.Now;
            lastDatetime = startSessionDateTime;
            currentDatetime = startSessionDateTime;

            elapsedSessionTime = new Time48Bit();
            todaysElapsedTime = new Time48Bit();
            totalElapsedTime = new Time48Bit();
            totalUserElapsedTime = new Time48Bit();
            elapsedSceneTime = new Time48Bit();


            string[] allScenesPath = AssetDatabase.FindAssets("t:Scene").Select(guid => AssetDatabase.GUIDToAssetPath(guid)).ToArray();
            Scene[] allScenes = new Scene[allScenesPath.Length];

            for (int i = 0; i < allScenesPath.Length; i++)
            {
                allScenes[i] = EditorSceneManager.GetSceneByName(GetFileNameFromPath(allScenesPath[i]));
            }

            saveData = new SaveData();
            // Try getting data
            if (File.Exists(TIME_SPENT_FILE_PATH + TIME_SPENT_FILE_NAME))
            {
                saveData = Load();

                totalElapsedTime = (Time48Bit)saveData.totalElapsedTime;
                totalElapsedTime = (Time48Bit)saveData.totalElapsedTime;

                bool isFirstTimeUser = true;
                for (int i = 0; i < saveData.userDataArray.Length; i++)
                {
                    if (saveData.userDataArray[i].userName == currentUserName)
                    {
                        isFirstTimeUser = false;
                        currentUserIndex = i;
                        break;
                    }
                }

                if (isFirstTimeUser)
                {
                    SaveData.UserData[] userDataArray = new SaveData.UserData[saveData.userDataArray.Length + 1];

                    for (int i = 0; i < saveData.userDataArray.Length; i++)
                    {
                        userDataArray[i] = saveData.userDataArray[i];
                    }

                    CreateNewUser(ref userDataArray);

                    saveData.userDataArray = userDataArray;
                }
                else
                {
                    if (saveData.userDataArray[currentUserIndex].sceneTimes != null)
                    {
                        sceneTimeList = saveData.userDataArray[currentUserIndex].sceneTimes.ToList();
                    }
                    else
                    {
                        sceneTimeList = new List<SaveData.SceneTime>();

                        for (int i = 0; i < allScenes.Length; i++)
                        {
                            sceneTimeList.Add(new SaveData.SceneTime(SceneManager.GetSceneAt(i).name));
                        }
                    }

                    for (int sceneIndex = 0; sceneIndex < allScenes.Length; sceneIndex++)
                    {
                        Scene scene = allScenes[sceneIndex];
                        bool isSceneIncluded = false;

                        for (int sceneTimeIndex = 0; sceneTimeIndex < sceneTimeList.Count; sceneTimeIndex++)
                        {
                            if (scene.name == sceneTimeList[sceneTimeIndex].sceneName)
                            {
                                isSceneIncluded = true;
                                
                                if (scene.name == EditorSceneManager.GetActiveScene().name)
                                {
                                    elapsedSceneTime = (Time48Bit)sceneTimeList[sceneTimeIndex].elaspedScenetime;
                                    activeSceneIndex = sceneTimeIndex;
                                }

                                break;
                            }
                        }

                        // Need to add scene
                        if (!isSceneIncluded)
                        {
                            sceneTimeList.Add(new SaveData.SceneTime(scene.name));
                        }
                    }
                    


                    totalUserElapsedTime = (Time48Bit)saveData.userDataArray[currentUserIndex].totalUserElapsedTime;

                    DateTime88Bit lastUsedTime = DateTime88Bit.FromJson(saveData.userDataArray[currentUserIndex].lastUsedTime);

                    if (((DateTime)lastUsedTime).Date == DateTime.Now.Date)
                    {
                        todaysElapsedTime = (Time48Bit)saveData.userDataArray[currentUserIndex].todaysElapsedTime;
                    }


                    DateTime88Bit differenceBetweenLastSessionAndThisOne = startSessionDateTime - lastUsedTime;
                    if (!saveData.userDataArray[currentUserIndex].isFirstOpen) // As long as there is only Addend difference of less then 1 minute, and 59 seconds seconds
                    {
                        elapsedSessionTime = (Time48Bit)saveData.userDataArray[currentUserIndex].elapsedSessionTime;

                        elapsedSceneTime.seconds += Convert.ToSByte(differenceBetweenLastSessionAndThisOne.Second);
                        elapsedSessionTime.seconds += Convert.ToSByte(differenceBetweenLastSessionAndThisOne.Second);
                        todaysElapsedTime.seconds += Convert.ToSByte(differenceBetweenLastSessionAndThisOne.Second);
                        totalUserElapsedTime.seconds += Convert.ToSByte(differenceBetweenLastSessionAndThisOne.Second);
                        totalElapsedTime.seconds += Convert.ToSByte(differenceBetweenLastSessionAndThisOne.Second);
                    }
                }
            }
            else
            {
                sceneTimeList = new List<SaveData.SceneTime>();

                for (int i = 0; i < allScenes.Length; i++)
                {
                    sceneTimeList.Add(new SaveData.SceneTime(allScenes[i].name));
                }

                saveData.userDataArray = new SaveData.UserData[1];

                CreateNewUser(ref saveData.userDataArray);

            }

            Save();


            EditorCoroutineUtility.StartCoroutineOwnerless(UpdateLoop());
        }

        private void CreateNewUser(ref SaveData.UserData[] userDataArray)
        {
            currentUserIndex = userDataArray.Length - 1;

            userDataArray[currentUserIndex] = new SaveData.UserData();
            userDataArray[currentUserIndex].userName = currentUserName;
            userDataArray[currentUserIndex].startTime = ((DateTime88Bit)DateTime.Now).ToJson();
        }


        private void EditorSceneManager_activeSceneChangedInEditMode(Scene oldScene, Scene newActiveScene)
        {
            if (sceneTimeList == null)
            {
                return;
            }

            for (int i = 0; i < sceneTimeList.Count; i++)
            {
                if (sceneTimeList[i].sceneName == newActiveScene.name)
                {
                    elapsedSceneTime = (Time48Bit)sceneTimeList[i].elaspedScenetime;
                    activeSceneIndex = i;
                    return;
                }
            }
        }
        private void EditorSceneManager_OnSceenDeleted(Scene deletedScene, string name)
        {
            for (int i = 0; i < sceneTimeList.Count; i++)
            {
                if (sceneTimeList[i].sceneName == name)
                {
                    sceneTimeList.RemoveAt(i);

                    UpdateSaveData();
                    Save();

                    return;
                }
            }
        }
        private void EditorSceneManager_OnSceneImported(Scene importedScene, string name)
        {
            // If scene exits then return. Not need to add it as a new scene.
            for (int i = 0; i < sceneTimeList.Count; i++)
            {
                if (sceneTimeList[i].sceneName == name)
                {
                    return;
                }
            }

            sceneTimeList.Add(new SaveData.SceneTime(name));

            UpdateSaveData();
            Save();
        }
        private void EditorSceneManager_OnSceneRenamed(Scene renamedScene, string oldName, string newName)
        {
            for (int i = 0; i < saveData.userDataArray[currentUserIndex].sceneTimes.Length; i++)
            {
                if (saveData.userDataArray[currentUserIndex].sceneTimes[i].sceneName == oldName)
                {
                    saveData.userDataArray[currentUserIndex].sceneTimes[i].sceneName = newName;
                }
            }
        }


        private bool OnEditorClose()
        {
            UpdateSaveData(true);
            Save();

            return true;
        }


        private IEnumerator UpdateLoop()
        {
            while (true)
            {
                Update();
                yield return new EditorWaitForSeconds(1);
            }
        }
        private void Update()
        {
            currentDatetime = DateTime.Now;

            elapsedTime = Time48Bit.GetDifferenceFrom2DateTimes(currentDatetime, lastDatetime);
            elapsedSessionTime += elapsedTime;
            todaysElapsedTime += elapsedTime;
            totalElapsedTime += elapsedTime;
            elapsedSceneTime += elapsedTime;

            lastDatetime = currentDatetime;

            UpdateSaveData();
            Save();

            UpdateTitle();
        }


        private void UpdateTitle()
        {
            // TO DO!!! Unity overrides what I have when the scene is switch from is dirty to is not dirty and the other way around.
            WindowUpdater.UpdateTitle(" Elapsed Session Time: " + elapsedSessionTime.ToString() + " Todays Elapsed Time: " + todaysElapsedTime.ToString() + " Total Elapsed Time: " + totalElapsedTime.ToString() + " Elapsed Scene Time: " + elapsedSceneTime.ToString());
        }

        private async Task<string> GetCurrentUserName(int iteration = 0)
        {
            if (iteration < 60)
            {
                string userNmae = "";

                // Reflection Must be done on main thread, because the unityConnect is on the main thread;
                // So we are getting the main thread context to call this function again after the delay;
                SynchronizationContext mainThreadContext = SynchronizationContext.Current;
                mainThreadContext.Post(new SendOrPostCallback((object _object) =>
                {
                    // This will run the following code on the main thread;
                    Assembly assembly = Assembly.GetAssembly(typeof(EditorWindow));
                    object unityConnect = assembly.CreateInstance("UnityEditor.Connect.UnityConnect", false, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null, null);
                    unityConnect = unityConnect.GetType().GetProperty("instance").GetValue(unityConnect);
                    object userInfo = unityConnect.GetType().GetProperty("userInfo").GetValue(unityConnect);

                    userNmae = userInfo.GetType().GetProperty("displayName").GetValue(userInfo).ToString();


                }), null);

                // Has not finished reflection
                while (userNmae == "")
                {
                    await Task.Delay(100);
                }

                if (userNmae == "anonymous")
                {
                    await Task.Delay(1000);
                    return await GetCurrentUserName(iteration + 1);
                }

                return userNmae;
            }

            return "Anonymous";
        }


        private void UpdateSaveData(bool isFirstOpen = false)
        {
            if (sceneTimeList.Count != 0)
            {
                sceneTimeList[activeSceneIndex] = new SaveData.SceneTime(sceneTimeList[activeSceneIndex].sceneName, (Time32Bit)elapsedSceneTime);
            }

            saveData.userDataArray[currentUserIndex].startTime = ((DateTime88Bit)startSessionDateTime).ToJson();
            saveData.userDataArray[currentUserIndex].sceneTimes = sceneTimeList.ToArray();
            saveData.userDataArray[currentUserIndex].elapsedSessionTime = (Time32Bit)elapsedSessionTime;
            saveData.userDataArray[currentUserIndex].todaysElapsedTime = (Time32Bit)todaysElapsedTime;
            saveData.userDataArray[currentUserIndex].totalUserElapsedTime = (Time32Bit)totalUserElapsedTime;
            saveData.userDataArray[currentUserIndex].lastUsedTime = ((DateTime88Bit)currentDatetime).ToJson();
            saveData.userDataArray[currentUserIndex].isFirstOpen = isFirstOpen;
            saveData.totalElapsedTime = (Time32Bit)totalElapsedTime;
        }
        private void Save()
        {
            File.WriteAllText(TIME_SPENT_FILE_PATH + TIME_SPENT_FILE_NAME, JsonUtility.ToJson(saveData));
        }
        private static SaveData Load()
        {
            string fileData = File.ReadAllText(TIME_SPENT_FILE_PATH + TIME_SPENT_FILE_NAME);
            return JsonUtility.FromJson<SaveData>(fileData);
        }
    }
}
