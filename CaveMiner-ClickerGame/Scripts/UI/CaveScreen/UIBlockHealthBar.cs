using UnityEngine;
using UnityEngine.UI;

namespace CaveMiner.UI
{
    public class UIBlockHealthBar : MonoBehaviour
    {
        [SerializeField] private Image _healthFill;

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Set(float amount)
        {
            _healthFill.fillAmount = amount;
        }
    }
}