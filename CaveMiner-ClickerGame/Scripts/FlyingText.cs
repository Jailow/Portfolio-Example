using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CaveMiner
{
    public class FlyingText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;

        private RectTransform _rectTr;
        public RectTransform RectTr
        {
            get
            {
                if (_rectTr == null)
                    _rectTr = GetComponent<RectTransform>();

                return _rectTr;
            }
        }

        public void Set(string text)
        {
            _text.text = text;
        }
    }
}