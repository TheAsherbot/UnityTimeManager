using System;

using UnityEditor;
using UnityEditor.PackageManager.UI;

using UnityEngine;

namespace TheAshBotAssets.TimeTracker
{
    public struct Preferences
    {
        

        private static bool hasInstance = false;
        private static Preferences instance; 
        public static Preferences Instance 
        {
            get
            {
                // Must creat first instance
                if (!hasInstance)
                {

                    hasInstance = true;
                }
                return instance;
            }
        }



        private static void Save()
        {

        }
        private static bool Load(out Preferences preferences)
        {
            preferences = default(Preferences);
            return false;
            // UnityEditor.PackageManager.Client.
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


        
        public TimesShown timesShown;
        public bool allowIndividualTracking;
        public bool isPaused;



    }
}
