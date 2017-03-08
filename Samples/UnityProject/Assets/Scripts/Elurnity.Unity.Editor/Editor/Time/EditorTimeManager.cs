using UnityEditor;
using System.Collections;

namespace Elurnity.TimeManager
{
    [InitializeOnLoad]
    public static class EditorTimeManager
    {
        private static TimeManager timeManager = new TimeManager();
        private static double lastTime;

        public static void Add(IEnumerator coroutine)
        {
            if (timeManager.Count == 0)
            {
                lastTime = EditorApplication.timeSinceStartup;
                EditorApplication.update += Update;
            }
            timeManager.Add(new Coroutine(coroutine));
        }

        private static void Update()
        {
            var timeSinceStartup = EditorApplication.timeSinceStartup;
            var deltaTime = timeSinceStartup - lastTime;
            lastTime = timeSinceStartup;

            timeManager.Update((float)deltaTime);

            if (timeManager.Count == 0)
            {
                EditorApplication.update -= Update;
            }
        }
    }
}