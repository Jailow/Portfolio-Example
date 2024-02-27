using UnityEngine;
using Firebase;
using Firebase.Database;
using System;
using Firebase.Extensions;
using Firebase.Auth;
using Firebase.RemoteConfig;
using System.Collections.Generic;
using System.Collections;
using Firebase.Installations;
using System.Threading.Tasks;
using Firebase.Analytics;

namespace CaveMiner
{
    public class FirebaseManager : Singleton<FirebaseManager>
    {
        private FirebaseApp _app;
        private FirebaseUser _user;
        private DatabaseReference _databaseRef;
        private FirebaseAuth _firebaseAuth;
        private PhoneAuthProvider _provider;
        private GameManager _gameManager;

        public Dictionary<string, object> RemoteConfig { get; private set; }

        public bool IsInitialized { get; private set; }
        public bool RemoteConfigLoaded { get; private set; }
        public string AppVersion { get; private set; }
        public FirebaseUser User => _user;
        public string UserId => SystemInfo.deviceUniqueIdentifier;
        public bool IsAuthenticated { get; private set; }
        public byte[] Data { get; private set; }

        public void Init()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    _app = FirebaseApp.DefaultInstance;
                    _firebaseAuth = FirebaseAuth.DefaultInstance;
                    _provider = PhoneAuthProvider.GetInstance(_firebaseAuth);

                    _databaseRef = FirebaseDatabase.GetInstance(_app).RootReference;

                    FirebaseInstallations.DefaultInstance.GetTokenAsync(true).ContinueWith(task =>
                    {
                        if (!(task.IsCanceled || task.IsFaulted) && task.IsCompleted)
                        {
                            Debug.Log(String.Format("Installations token {0}", task.Result));
                        }
                    });

                    FetchRemoteConfig();

                    IsInitialized = true;
                }
                else
                {
                    Debug.LogError(string.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                }
            });
        }

        public void SignIn(Action<bool> onCompleted)
        {
            Debug.Log("Firebase SignIn Anonymous");

            _firebaseAuth.SignInAnonymouslyAsync().ContinueWith(task => 
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInAnonymously was canceled.");
                    onCompleted?.Invoke(false);
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SignInAnonymously encountered an error: " + task.Exception);
                    onCompleted?.Invoke(false);
                    return;
                }

                AuthResult result = task.Result;
                _user = result.User;
                IsAuthenticated = true;
                Debug.LogFormat("User signed in successfully: {0} ({1})", result.User.DisplayName, result.User.UserId);
                onCompleted?.Invoke(true);
            });
        }

        public void OpenSavedGame(Action onCompleted)
        {
            Debug.Log("OpenSavedGame (Firebase)");
            _databaseRef.Child($"Users/{UserId}").GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (!task.IsFaulted && task.IsCompleted)
                {
                    var saveJson = (string)task.Result.GetValue(true);
                    if (!string.IsNullOrEmpty(saveJson))
                    {
                        Data = Convert.FromBase64String(saveJson);
                        Debug.Log("OpenSavedGame Completed (Firebase)");
                    }
                }
                else
                {
                    Debug.Log("OpenSavedGame Failed (Firebase)");
                }

                onCompleted?.Invoke();
            });
        }

        public void SaveGame(byte[] byteArray)
        {
            Debug.Log("Save Game CLOUD (Firebase)");

            string byteString = Convert.ToBase64String(byteArray);
            _databaseRef.Child($"Users/{UserId}").SetValueAsync(byteString).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log("Save Game CLOUD (Firebase) Failed: " + task.Exception.Message);
                }
                else if (task.IsCompleted)
                {
                    Debug.Log("Save Game CLOUD (Firebase) Completed");
                }
            });
        }

        public void LoadAppVersion(Action onLoaded)
        {
            _databaseRef.Child("Settings/AppVersion").GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (!task.IsFaulted && task.IsCompleted)
                {
                    AppVersion = (string)task.Result.GetValue(true);
                }

                onLoaded?.Invoke();
            });
        }

        private void FetchRemoteConfigComplete(Task fetchTask)
        {
            if (!fetchTask.IsCompleted)
            {
                return;
            }

            RemoteConfig = new Dictionary<string, object>();

            var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
            var info = remoteConfig.Info;
            if(info.LastFetchStatus != LastFetchStatus.Success)
            {
                Debug.Log("RemoteConfig Set Default");
                RemoteConfig.Add("block_destroy_visual", "animated");
                RemoteConfigLoaded = true;
                return;
            }

            remoteConfig.ActivateAsync().ContinueWithOnMainThread(task =>
            {
                Debug.Log("RemoteConfig Loaded");
                RemoteConfig.Add("block_destroy_visual", remoteConfig.GetValue("block_destroy_visual").StringValue);
                RemoteConfigLoaded = true;
            });
        }

        private Task FetchRemoteConfig()
        {
            Task fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
            return fetchTask.ContinueWithOnMainThread(FetchRemoteConfigComplete);
        }

        public void LogReplayUploaded(string fileName, string name)
        {
            if (!IsInitialized)
                return;

            var parameters = new List<Parameter>
            {
                new Parameter("file_name", fileName),
                new Parameter("replay_name", name)
            };

            FirebaseAnalytics.LogEvent("replay_uploaded", parameters.ToArray());
        }
    }
}