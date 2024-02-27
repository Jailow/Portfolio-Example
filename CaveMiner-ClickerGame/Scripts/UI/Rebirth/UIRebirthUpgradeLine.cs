using UnityEngine;
using UnityEngine.UI;

namespace CaveMiner.UI
{
    public class UIRebirthUpgradeLine : MonoBehaviour
    {
        [SerializeField] private Sprite _activeSprite;
        [SerializeField] private Sprite _inactiveSprite;

        private Image _img;

        public void SetState(bool active)
        {
            if(_img == null)
                _img = GetComponent<Image>();

            _img.sprite = active ? _activeSprite : _inactiveSprite;
        }
    }
}