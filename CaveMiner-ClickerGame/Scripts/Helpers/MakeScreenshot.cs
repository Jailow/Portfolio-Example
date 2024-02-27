using System;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace CaveMiner.Helpers
{
    public class MakeScreenshot : MonoBehaviour
    {
        [MenuItem("Helpers/Screenshot")]
        private static void Screenshot()
        {
            var dateTime = DateTime.Now;
            ScreenCapture.CaptureScreenshot($"screenshot_{dateTime.Year}_{dateTime.Month}_{dateTime.Day}_{dateTime.Minute}_{dateTime.Second}.png");
        }
    }
}
#endif