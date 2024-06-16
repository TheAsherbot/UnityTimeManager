using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEditor.SceneManagement;

namespace TheAshBotAssets.TimeTracker
{
    public static class WindowUpdater
    {

        // ====================================================================================== WINDOWS ONLY ======================================================================================


#if UNITY_EDITOR_WIN

        private delegate bool EnumWindowProc(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumWindows(EnumWindowProc lpEnumFunc, IntPtr lParam);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        // Import Functions  following.
        [DllImport("user32.dll", EntryPoint = "SetWindowText")]
        public static extern bool SetWindowText(IntPtr hwnd, string lpString);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);


        private static string BaseTitleName
        {
            get
            {
                return Application.productName + " - " + EditorSceneManager.GetActiveScene().name + " - Windows, Mac, Linux - Unity " + Application.unityVersion + (EditorSceneManager.GetActiveScene().isDirty ? '*' : "") + " <DX11>";
            }
        }
        private static IntPtr unityApplicationHandle;

        private static bool isInitiated = false;

        private static async Task<IntPtr> GetUnityWindowHandle()
        {
            Process process = Process.GetCurrentProcess();

            for (int i = 0; i < 60; i++)
            {
                IEnumerable<IntPtr> windowHandles = GetAllWindowHandlesForProcess(process.Id);
                // Print each handle.
                foreach (IntPtr handle in windowHandles)
                {
                    StringBuilder buffer = new StringBuilder(256);
                    GetWindowText(handle, buffer, buffer.Capacity);
                    if (buffer.ToString().Contains("Unity " + Application.unityVersion))
                    {
                        return handle;
                    }
                }
                await Task.Delay(1000);
            }

            return IntPtr.Zero;
        }

        public static IEnumerable<IntPtr> GetAllWindowHandlesForProcess(int processId)
        {
            List<IntPtr> handles = new List<IntPtr>();

            EnumWindows((hWnd, lParam) =>
            {
                GetWindowThreadProcessId(hWnd, out uint pid);

                if (pid == (uint)processId)
                {
                    handles.Add(hWnd);
                }

                return true;
            }, IntPtr.Zero);

            return handles;
        }
#endif 


        public static async void UpdateTitle(string title)
        {
            if (!isInitiated)
            {
                await Initiate();
            }

#if UNITY_EDITOR_WIN
            // TO DO!!! Unity overrides what I have when the scene is switch from is dirty to is not dirty and the other way around.
            SetWindowText(unityApplicationHandle, BaseTitleName + title);
#endif
#if UNITY_EDITOR_MAC
            // TO DO!!! Unity overrides what I have when the scene is switch from is dirty to is not dirty and the other way around.
            SetWindowText(unityApplicationHandle, BaseTitleName + title);
#endif
        }


        private static async Task Initiate()
        {
#if UNITY_EDITOR_WIN
            unityApplicationHandle = await GetUnityWindowHandle();
#endif
        }

    }
}
