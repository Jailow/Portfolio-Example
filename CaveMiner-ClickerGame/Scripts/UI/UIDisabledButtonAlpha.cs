using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CaveMiner.UI
{
    public class UIDisabledButtonAlpha : MonoBehaviour
    {
        [SerializeField] private float _disableAlpha;

        private Image[] _images;
        private TextMeshProUGUI[] _texts;
        private Button _btn;

        private Color[] _imagesNormalColors;
        private Color[] _textsNormalColors;
        private Color[] _imagesDisabledColors;
        private Color[] _textsDisabledColors;

        public bool Interactable
        {
            get { return _btn.interactable; }
            set { SetInteractable(value); }
        }

        public void Init()
        {
            _btn = GetComponent<Button>();

            _images = GetComponentsInChildren<Image>(true);
            _texts = GetComponentsInChildren<TextMeshProUGUI>(true);

            _imagesNormalColors = new Color[_images.Length];
            _imagesDisabledColors = new Color[_images.Length];
            _textsNormalColors = new Color[_texts.Length];
            _textsDisabledColors = new Color[_texts.Length];

            for(int i = 0; i < _images.Length; i++)
            {
                _imagesNormalColors[i] = _images[i].color;
                _imagesDisabledColors[i] = _images[i].color;
                _imagesDisabledColors[i].a = _disableAlpha;
            }

            for(int i = 0; i < _texts.Length; i++)
            {
                _textsNormalColors[i] = _texts[i].color;
                _textsDisabledColors[i] = _texts[i].color;
                _textsDisabledColors[i].a = _disableAlpha;
            }
        }

        private void SetInteractable(bool value)
        {
            _btn.interactable = value;

            for (int i = 0; i < _images.Length; i++)
            {
                _images[i].color = value ? _imagesNormalColors[i] : _imagesDisabledColors[i];
            }

            for(int i = 0; i < _texts.Length; i++)
            {
                _texts[i].color = value ? _textsNormalColors[i] : _textsDisabledColors[i]; 
            }
        }
    }
}