using CodeStage.AntiCheat.Detectors;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using System;
using UnityEngine;

namespace CaveMiner.Secure
{
    public class AntiCheatManager : Singleton<AntiCheatManager>
    {
        public ObscuredBool CheatDetected { get; private set; }

        public static readonly ObscuredString Hash = "EXAMPLE PROJECT";

        public Action onCheatDetected;

        protected override void Awake()
        {
            base.Awake();

            SpeedHackDetector.StartDetection(OnCheatDetected);
            ObscuredCheatingDetector.StartDetection(OnCheatDetected);
            ObscuredPrefs.NotGenuineDataDetected += OnCheatDetected;
        }

        public void OnCheatDetected()
        {
            Debug.LogError("CHEAT DETECTED");

            CheatDetected = true;
            onCheatDetected?.Invoke();
        }
    }
}