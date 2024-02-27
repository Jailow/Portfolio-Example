using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace CaveMiner
{
    [System.Serializable]
    public struct ServerTimeData
    {
        public int unixtime;
    }

    public class ServerTimeManager : Singleton<ServerTimeManager>
    {
        private const string _serverUri = "http://worldtimeapi.org/api/ip";
        private float _initializationServerTime;
        private int _currentTime;

        public int ServerTime => _currentTime + (int)(Time.realtimeSinceStartup - _initializationServerTime);

        public void SendRequest(Action onComplete)
        {
            StartCoroutine(SendRequest_Coroutine(onComplete));
        }

        private IEnumerator SendRequest_Coroutine(Action onComplete)
        {
            while (true)
            {
                var request = UnityWebRequest.Get(_serverUri);

                yield return request.SendWebRequest();

                bool success = !request.isNetworkError && !request.isHttpError && request.responseCode == 200;

                if (success)
                {
                    var json = request.downloadHandler.text;
                    var responseData = JsonUtility.FromJson<ServerTimeData>(json);

                    _currentTime = responseData.unixtime;
                    _initializationServerTime = Time.time;
                    onComplete?.Invoke();
                }
                else
                {
                    Debug.LogError($"ServerTime error: {request.error}");
                }

                request.Dispose();

                if (success)
                {
                    yield break;
                }

                yield return new WaitForSecondsRealtime(1f);
            }
        }
    }
}
