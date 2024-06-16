using System;

namespace TheAshBotAssets.TimeTracker
{
    [Serializable]
    public struct SaveData
    {
        [Serializable]
        public struct UserData
        {
            public string userName;


            public string startTime;
            public SceneTime[] sceneTimes;
            public Time32Bit elapsedSessionTime;
            public Time32Bit todaysElapsedTime;
            public Time32Bit totalUserElapsedTime;
            public string lastUsedTime;
            public bool isFirstOpen;
        }

        [Serializable]
        public struct SceneTime
        {
            public SceneTime(string sceneName)
            {
                this.sceneName = sceneName;
                elaspedScenetime = new Time32Bit();
            }
            public SceneTime(string sceneName, Time32Bit elaspedScenetime)
            {
                this.sceneName = sceneName;
                this.elaspedScenetime = elaspedScenetime;
            }

            public string sceneName;
            public Time32Bit elaspedScenetime;
        }


        public UserData[] userDataArray;


        public Time32Bit totalElapsedTime;
    }
}