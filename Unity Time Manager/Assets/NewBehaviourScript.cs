using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using TheAshBotAssets.TimeTracker;

using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

public class NewBehaviourScript : MonoBehaviour
{
    [MenuItem("Example/Get Package Cache Location")]
    static void GetCacheLocation()
    {

        Debug.Log(Application.consoleLogPath.Remove(Application.consoleLogPath.Length - 17) + "Assets/TheAsherBots Assets/Time Tracker/Preferences");

    }

    public static string GetAssetCacheLocation()
    {
        // Get the type of the PackageManagerPrefs class
        Assembly assembly = Assembly.GetAssembly(typeof(Editor));



        Debug.Log("assembly : " + assembly);
        Type packageManagerPrefsType = assembly.GetType("UnityEditor.PackageManager.UI.Internal.PackageManagerUserSettingsProvider");

        Debug.Log("packageManagerPrefsType: " + packageManagerPrefsType);

        // object packageManagerPrefs = Activator.CreateInstance(packageManagerPrefsType, true);
        // object packageManagerPrefs = assembly.CreateInstance(packageManagerPrefsType.FullName);

        // Utills.ReflectType(packageManagerPrefsType, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        /////////////////////////////////

        PropertyInfo propertyInfo = packageManagerPrefsType.GetProperty("k_ChangeLocation", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        Debug.Log("propertyInfo: " + propertyInfo);
        // Debug.Log("propertyInfo value: " + propertyInfo.GetValue(null));


        Test();

        // Debug.Log("propertyInfo: " + propertyInfo);

        // object _o = propertyInfo.GetValue(null);


        // Utills.ReflectType(packageManagerPrefsType, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.CreateInstance | BindingFlags.InvokeMethod);

        void Test()
        {
            Debug.Log("providerInstance: " + null);

            ConstructorInfo[] constructorInfo = packageManagerPrefsType.GetConstructors(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            for (int i = 0; i < constructorInfo.Length; i++)
            {
                string parameterString = "";
                ParameterInfo[] parameterInfo = constructorInfo[i].GetParameters();
                for (int j = 0; j < parameterInfo.Length; j++)
                {
                    parameterString += $"\nName: {parameterInfo[j].Name}, Type: {parameterInfo[j].ParameterType}, Defualt: {parameterInfo[j].DefaultValue}, isOut: {parameterInfo[j].IsOut}";
                }
                
                Debug.Log($"Constructor {i}\nName: {constructorInfo[i].Name}\nisPublic: {constructorInfo[i].IsPublic}\nIsPrivate: {constructorInfo[i].IsPrivate}\nIsStatic: {constructorInfo[i].IsStatic}\nIsStatic: {constructorInfo[i]}{parameterString}");

            }


            // object packageManagerPrefs = constructorInfo[0].Invoke(new object[] { "Hello WOrld!", new string[] { } });
            // Debug.Log("providerInstance: " + packageManagerPrefs);

            FieldInfo[] fieldInfo = packageManagerPrefsType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            for (int i = 0; i < fieldInfo.Length; i++)
            {
                Debug.Log($"field {i}\nName: {fieldInfo[i].Name}\nReturn: {fieldInfo[i].FieldType}\nisPublic: {fieldInfo[i].IsPublic}\nIsPrivate: {fieldInfo[i].IsPrivate}\nIsStatic: {fieldInfo[i].IsStatic}\nValue {fieldInfo[i].GetValue(null)}");
            }

            PropertyInfo[] propertyInfo = packageManagerPrefsType.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            for (int i = 0; i < propertyInfo.Length; i++)
            {
                Debug.Log($"field {i}\nName: {propertyInfo[i].Name}\nReturn: {propertyInfo[i].PropertyType}\nIsStatic: {fieldInfo[i].IsStatic}\nValue {fieldInfo[i].GetValue(null)}");
            }

            MethodInfo[] methodInfo = packageManagerPrefsType.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            for (int i = 0; i < methodInfo.Length; i++)
            {
               ;


                string parameterString = "";
                ParameterInfo[] parameterInfo = methodInfo[i].GetParameters();
                for (int j = 0; j < parameterInfo.Length; j++)
                {
                    parameterString += $"\nName: {parameterInfo[j].Name}, Type: {parameterInfo[j].ParameterType}, Defualt: {parameterInfo[j].DefaultValue}, isOut: {parameterInfo[j].IsOut}";
                }

                Debug.Log($"Method {i}\nName: {methodInfo[i].Name}\nReturn: {methodInfo[i].ReturnType}\nisPublic: {methodInfo[i].IsPublic}\nIsPrivate: {methodInfo[i].IsPrivate}\nIsStatic: {methodInfo[i].IsStatic}{parameterString}\n Has Atribute: {methodInfo[i].GetCustomAttributes(typeof(SettingsProvider), false).Length}");
            }
            // Debug.Log(methodInfo[0].Invoke(packageManagerPrefs, new object[] { }));
        }


        if (packageManagerPrefsType != null)
        {
            // Get the instance of the PackageManagerPrefs class
            // object packageManagerPrefsInstance = packageManagerPrefsType.GetProperty("instance", BindingFlags.Static | BindingFlags.Public).GetValue(null);

            // Get the value of the asset cache location
            // string assetCacheLocation = (string)packageManagerPrefsType.GetProperty("assetCacheLocation", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(packageManagerPrefsInstance);

            // return assetCacheLocation;
        }

        return null;
    }

    public static void GetAssetCacheLocationIfPreferencesIsOpenToAssetTab()
    {

        // Open the Preferences window
        Assembly asm = Assembly.GetAssembly(typeof(EditorWindow));
        Type preferencesWindowType = asm.GetType("UnityEditor.SettingsWindow");

        EditorApplication.delayCall += () =>
        {
            EditorWindow preferencesWindow;
            // Attempt to read some information from the Preferences window

            MethodInfo methodInfo = typeof(EditorWindow).GetMethod("HasOpenInstances", BindingFlags.Public | BindingFlags.Static);
            MethodInfo genericMethod = methodInfo.MakeGenericMethod(preferencesWindowType);
            if ((bool)genericMethod.Invoke(null, null))
            {
                preferencesWindow = EditorWindow.GetWindow(preferencesWindowType);

                VisualElement element = preferencesWindow.rootVisualElement.Query<VisualElement>("assetsCachePath").ToList()[0];

                Debug.Log(((UnityEngine.UIElements.Label)element).text);
            }
            else
            {
                MethodInfo showPreferencesWindowMethod = preferencesWindowType.GetMethod("OpenUserPreferences", BindingFlags.NonPublic | BindingFlags.Static);
                showPreferencesWindowMethod.Invoke(null, null);

                preferencesWindow = EditorWindow.GetWindow(preferencesWindowType);

                VisualElement element = preferencesWindow.rootVisualElement.Query<VisualElement>("assetsCachePath").ToList()[0];

                Debug.Log(((UnityEngine.UIElements.Label)element).text);

                preferencesWindow.Close();
            }
        };
    }

}


/*
UnityEditor.PackageManager.UI.Internal.PackageManagerUserSettingsProvider: currentAssetStoreNormalizedPath, type: System.String, readWritePermitions: ro, GetMethod: System.String get_currentAssetStoreNormalizedPath(), SetMethod: 
UnityEngine.Debug:Log (object) 
 */