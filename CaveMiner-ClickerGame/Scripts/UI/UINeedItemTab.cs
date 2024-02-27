using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CaveMiner.UI
{
    public class UINeedItemTab : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Image _icon;
        [SerializeField] private Color _defaultTextColor;
        [SerializeField] private Color _warningTextColor;

        public Image Icon => _icon;
        public TextMeshProUGUI Text => _text;
        public bool IsNotEnough
        {
            set
            {
                _text.color = value ? _warningTextColor : _defaultTextColor;
            }
        }
    }
}