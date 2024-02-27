using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CaveMiner.UI
{
    public class UIOfflineIncomeDropItem : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _count;

        public void Set(Sprite icon, int count)
        {
            _icon.sprite = icon;
            _count.text = count.ToString();
        }
    }
}