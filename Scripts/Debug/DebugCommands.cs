using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DebugStuff
{
    [System.Serializable]
    public static class DebugCommands
    {
        #region fields & properties

        #endregion fields & properties

        #region methods
        public static bool IsApplicationPlaying()
        {
            if (!Application.isPlaying)
            {
                Log("Enter Play Mode First");
                return false;
            }
            return true;
        }
        public static void BenchmarkRepeat(int repeatTimes, System.Action action, string testName)
        {
            float startTime = Time.realtimeSinceStartup;
            for (int i = 0; i < repeatTimes; ++i)
            {
                action.Invoke();
            }
            float endTime = Time.realtimeSinceStartup;
            Log($"{testName} Test Success. \nAverage time for iteration = {(endTime - startTime) / (float)repeatTimes}");
        }
        private static void Log(string message)
        {
            Debug.Log(message);
        }
        #endregion methods
    }
}