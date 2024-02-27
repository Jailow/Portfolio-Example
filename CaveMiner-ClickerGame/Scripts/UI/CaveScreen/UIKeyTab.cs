using UnityEngine;
using TMPro;

public enum RarityType
{
    Common,
    Rare,
    Epic,
    Mythical,
    Legendary,
}

namespace CaveMiner.UI
{
    public class UIKeyTab : MonoBehaviour
    {
        [SerializeField] private RarityType _rarityType;
        [SerializeField] private TextMeshProUGUI _countText;

        public RarityType RarityType => _rarityType;

        public void SetCount(int count)
        {
            if (count <= 999)
            {
                _countText.text = count.ToString();
            }
            else
            {
                _countText.text = "999+";
            }
        }
    }
}