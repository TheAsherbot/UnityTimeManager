using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

namespace TheAshBotAssets.TimeTracker
{
    public class PreferencesWindow
    {

        private static Preferences preferences;



        [SettingsProvider]
        private static SettingsProvider CustomPreferencesGUI()
        {
            SettingsProvider provider = new SettingsProvider("Preferences/Time Tracker", SettingsScope.User)
            {
                // By default the last token of the path is used as display name if no label is provided.
                label = "Time Tracker",
                // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
                guiHandler = (searchContext) =>
                {
                    preferences.timesShown = (Preferences.TimesShown)EditorGUILayout.EnumFlagsField("Times Shown", preferences.timesShown);


                    EditorGUILayout.HelpBox("Having \"Allow Individual Tracking\" enabled will allow you to track how much time you contribute seperatly then others working on the project. By doing this you also allow this program you look at your Unity Conect settings to find your username. This info is only kept on your local machine, but can be distributed to others working on the same project.", MessageType.Warning);
                    preferences.allowIndividualTracking = EditorGUILayout.Toggle(new GUIContent("Allow Individual Tracking", "Allows this program to track how much time you contribute to the project seperatly from others."), preferences.allowIndividualTracking);


                    if (preferences.isPaused)
                    {
                        if (GUILayout.Button(new GUIContent("Resume Time Tracking", "Tempararly pauses the tracking of time. Time tracking will resume when the button is clicked again, or on reopening of Unity Editor.")))
                        {
                            preferences.isPaused = false;
                        }
                    }
                    else
                    {
                        if (GUILayout.Button(new GUIContent("Pause Time Tracking", "Tempararly pauses the tracking of time. Time tracking will resume when the button is clicked again, or on reopening of Unity Editor.")))
                        {
                            preferences.isPaused = true;
                        }
                    }
                },

                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = new HashSet<string>(new[] { "Time Tracker" })
            };

            return provider;
            
        }
    }
}
