using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using CaveMiner.UI;
using DG.Tweening;

namespace CaveMiner
{
    public class LauncherController : MonoBehaviour
    {
        [SerializeField] private UINoInternetConnectionPanel _noConnectionPanel;
        [SerializeField] private UIUnsecuredEntrancePanel _unsecuredEntrancePanel;
        [SerializeField] private UILauncherScreen _launcherScreen;

        private const string GAME_SCENE_NAME = "Game";
        private const float _waitConnectionTime = 7.5f;

        private float _time;
        private bool _checkInternet = true;
        private bool _isConnected = false;

        private void Awake()
        {
            StartCoroutine(LoadGame());
        }

        private void Update()
        {
            if (_isConnected || !_checkInternet)
                return;

            _time += Time.deltaTime;
            if (_time > _waitConnectionTime)
            {
                _noConnectionPanel.Show();
                StopAllCoroutines();
            }
        }

        private IEnumerator LoadGame()
        {
            _launcherScreen.FillImg.fillAmount = 0f;
            _launcherScreen.gameObject.SetActive(true);

            yield return new WaitForSeconds(1.5f);

            if (RootChecker.isDeviceRooted())
            {
                _checkInternet = false;
                _unsecuredEntrancePanel.Show();
                yield break;
            }

            _launcherScreen.FillImg.DOFillAmount(0.25f, 0.2f);

            int loadCount = 1;

            ServerTimeManager.Instance.SendRequest(() =>
            {
                _isConnected = true;
                loadCount--;
            });

#if !UNITY_EDITOR
            loadCount++;
            Social.localUser.Authenticate(success =>
            {
                loadCount--;
                Debug.Log("Social Authenticate completed: " + success);
            });
#endif

            while (loadCount > 0)
                yield return null;

            _launcherScreen.FillImg.DOFillAmount(0.5f, 0.2f);

            FirebaseManager.Instance.Init();

            while (!FirebaseManager.Instance.IsInitialized || !FirebaseManager.Instance.RemoteConfigLoaded)
                yield return null;

            loadCount++;
            FirebaseManager.Instance.LoadAppVersion(() =>
            {
                loadCount--;
            });

            loadCount++;
            FirebaseManager.Instance.SignIn((success) =>
            {
                loadCount--;
            });

            while (loadCount > 0 || _launcherScreen.FillImg.fillAmount < 0.49f)
                yield return null;

            _launcherScreen.FillImg.DOFillAmount(0.65f, 0.2f);

            yield return new WaitForFixedUpdate();

#if !UNITY_EDITOR
            if (Social.localUser.authenticated)
            {
                loadCount++;
                GooglePlayGamesManager.Instance.OpenSavedGame("game_state", () =>
                {
                    loadCount--;
                });
            }
            else
            {
                loadCount++;
                FirebaseManager.Instance.OpenSavedGame(() =>
                {
                    loadCount--;
                });
            }
#endif

            while (loadCount > 0 || _launcherScreen.FillImg.fillAmount < 0.64f)
                yield return null;

            var loadScene = SceneManager.LoadSceneAsync(GAME_SCENE_NAME, LoadSceneMode.Single);
            loadScene.allowSceneActivation = false;

            while (loadScene.progress < 0.9f)
            {
                _launcherScreen.FillImg.fillAmount = Mathf.Lerp(0.65f, 1f, loadScene.progress);
                yield return null;
            }

            _launcherScreen.FillImg.DOFillAmount(1f, 0.1f);

            yield return new WaitForSeconds(0.1f);

            _launcherScreen.Fade.DOFade(1f, 0.2f);

            yield return new WaitForSeconds(0.2f);

            _checkInternet = false;
            loadScene.allowSceneActivation = true;
        }
    }
}