using UnityEngine;
using UnityEngine.UI;

namespace CaveMiner.UI
{
    public class UIDepthBarLine : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Image _line;
        [SerializeField] private GameObject _maxObj;

        private RectTransform _tr;

        public Image Icon => _icon;
        public Image Line => _line;
        public GameObject MaxObj => _maxObj;
        public RectTransform Tr
        {
            get
            {
                if (_tr == null)
                    _tr = GetComponent<RectTransform>();

                return _tr;
            }
        }
    }
}