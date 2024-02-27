using System.Collections;
using UnityEngine;
using TMPro;

namespace CaveMiner
{
    public class UIShowInterstitialSoonScreen : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _title;

        private WaitForSeconds _second;

        private void Awake()
        {
            _second = new WaitForSeconds(1f);
        }

        private IEnumerator Animation()
        {
            string txt = I2.Loc.LocalizationManager.GetTranslation("Info/show_interstitial_soon").Replace("\\n", "\n");
            _title.text = string.Format(txt, 3);
            yield return _second;
            _title.text = string.Format(txt, 2);
            yield return _second;
            _title.text = string.Format(txt, 1);
        }

        private void OnEnable()
        {
            StartCoroutine(Animation());
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}