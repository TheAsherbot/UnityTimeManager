using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEditor;

using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [MenuItem("Example/Get Package Cache Location")]
    static void GetCacheLocation()
    {
        GetAssetCacheLocation();

/*
        // Load the assembly containing your scripts
        Assembly assembly = Assembly.GetAssembly(typeof(Editor));

        // Iterate through all types in the assembly
        foreach (Type type in assembly.GetTypes())
        {
            // Iterate through all methods in the type
            foreach (MethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                // Check if the method has the SettingsProvider attribute
                var attributes = method.GetCustomAttributes(typeof(SettingsProviderAttribute), false);
                if (attributes.Length > 0)
                {
                    Debug.Log($"Found SettingsProvider in method: {method.Name} of type: {type.AssemblyQualifiedName}");
                }
            }
        }
*/
    }

    public static string GetAssetCacheLocation()
    {
        // Get the type of the PackageManagerPrefs class
        Assembly assembly = Assembly.GetAssembly(typeof(Editor));

        

        Debug.Log("assembly : " + assembly);
        Type packageManagerPrefsType = assembly.GetType("UnityEditor.PackageManager.UI.Internal.PackageManagerUserSettingsProvider");

        Debug.Log("packageManagerPrefsType: " + packageManagerPrefsType);


        Utills.ReflectType(packageManagerPrefsType, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        /////////////////////////////////

        Debug.Log("providerInstance: " + null);
        
        PropertyInfo propertyInfo = 
            packageManagerPrefsType
            .GetProperty("currentAssetStoreNormalizedPath");

        Debug.Log("propertyInfo: " + propertyInfo);

        object _o = propertyInfo.GetValue(null);


        // Utills.ReflectType(packageManagerPrefsType, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.CreateInstance | BindingFlags.InvokeMethod);




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
}


/*
UnityEditor.PackageManager.UI.Internal.PackageManagerUserSettingsProvider: currentAssetStoreNormalizedPath, type: System.String, readWritePermitions: ro, GetMethod: System.String get_currentAssetStoreNormalizedPath(), SetMethod: 
UnityEngine.Debug:Log (object) 
 */