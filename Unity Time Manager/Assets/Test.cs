using System.Threading.Tasks;

using UnityEditor;

using UnityEngine;
//  
[InitializeOnLoad]
public class Test
{

    static Test()
    {
        Task task = Task.Run(() =>
        {
            while (true)
            {
                Debug.Log("Hello World");
                Task.Delay(1000).Wait();
            }
        });
    }
    
}
