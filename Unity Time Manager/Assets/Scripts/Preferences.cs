using System;
using System.IO;

using UnityEngine;

namespace TheAshBotAssets.TimeTracker
{
    public static class Preferences
    {

        private static string PATH
        {
            get
            {
                return Application.consoleLogPath.Remove(Application.consoleLogPath.Length - 17) + "Assets/TheAsherBots Assets/Time Tracker";
            }
        }
        private static readonly string FILE_NAME = "Preferences";
        private static string FULL_PATH
        {
            get
            {
                return PATH + '/' + FILE_NAME;
            }
        }


        [Flags]
        public enum TimesShown
        {
            ElapsedSessionTime = 2,
            UserElapsedTimeToday = 4,
            UserSceneElaspedTime = 8,
            UserElaspedTime = 16,

            TotalElapsedTimeToday = 32,
            TotalSceneElaspedTime = 64,
            TotalElaspedTime = 128,
        }
        [Serializable]
        public struct SaveData
        {
            public TimesShown timesShown;
            public bool allowIndividualTracking;

            public static bool operator ==(SaveData left, SaveData right)
            {
                return left.timesShown == right.timesShown && left.allowIndividualTracking == right.allowIndividualTracking;
            }
            public static bool operator !=(SaveData left, SaveData right)
            {
                return !(left.timesShown == right.timesShown && left.allowIndividualTracking == right.allowIndividualTracking);
            }
        }

        public static SaveData saveData;
        public static bool isPaused;



        public static void Save()
        {
            if (!File.Exists(PATH))
            {
                Directory.CreateDirectory(PATH);
            }

            File.WriteAllText(FULL_PATH, JsonUtility.ToJson(saveData));

            Debug.Log("SAVE!");
        }
        public static bool Load(out SaveData preferences)
        {
            if (!File.Exists(FULL_PATH))
            {
                preferences = default(SaveData);
                return false;
            }

            preferences = JsonUtility.FromJson<SaveData>(File.ReadAllText(FULL_PATH));
            saveData = preferences;
            return true;
        }

    }
}
