using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

namespace CaveMiner.UI
{
    public class UILauncherScreen : MonoBehaviour
    {
        [SerializeField] private Image _fade;
        [SerializeField] private TextMeshProUGUI _loadingText;
        [SerializeField] private TextMeshProUGUI _versionText;
        [SerializeField] private Image _fillImg;

        public Image FillImg => _fillImg;
        public Image Fade => _fade;

        private void Awake()
        {
            _versionText.text = Application.version;

            StartCoroutine(LoadingAnimation());
        }

        private IEnumerator LoadingAnimation()
        {
            string loadingText = I2.Loc.LocalizationManager.GetTranslation("Launcher/loading");

            var waitSecond = new WaitForSeconds(0.5f);

            _loadingText.text = $"{loadingText}";

            yield return new WaitForEndOfFrame();

            _loadingText.GetComponent<ContentSizeFitter>().enabled = false;

            while (true)
            {
                _loadingText.text = $"{loadingText}";
                yield return waitSecond;
                _loadingText.text = $"{loadingText}.";
                yield return waitSecond;
                _loadingText.text = $"{loadingText}..";
                yield return waitSecond;
                _loadingText.text = $"{loadingText}...";
                yield return waitSecond;
            }
        }

        private void OnEnable()
        {
            _fade.DOFade(0f, 1.5f).SetEase(Ease.Linear);
        }
    }
}