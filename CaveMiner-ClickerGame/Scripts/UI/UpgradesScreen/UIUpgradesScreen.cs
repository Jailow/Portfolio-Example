using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CaveMiner.UI
{
    public class UIUpgradesScreen : MonoBehaviour
    {
        [SerializeField] private Image _pickaxeImg;
        [SerializeField] private UIPickaxeRunesPanel _runesPanel;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private AnimationCurve _scrollRectShowCurve;
        [SerializeField] private Button _combineRuneButton;

        private UIPickaxeUpgradeTabBase[] _upgradeTabs;

        private GameManager _gameManager;
        private Image _pickaxeOutlineImg;

        public UIPickaxeRunesPanel RunesPanel => _runesPanel;

        public void Init(GameManager gameManager, UIManager uiManager)
        {
            _gameManager = gameManager;

            _runesPanel.Init(gameManager, uiManager);

            _combineRuneButton.onClick.AddListener(() =>
            {
                uiManager.ButtonClickSound();
                uiManager.ShowCombineRunePanel();
            });

            _upgradeTabs = GetComponentsInChildren<UIPickaxeUpgradeTabBase>();
            foreach (var upgradeTab in _upgradeTabs)
            {
                upgradeTab.Init(gameManager, uiManager);
            }

            _gameManager.Pickaxe.onSpeedChanged += UpdatePickaxeIcon;

            _pickaxeOutlineImg = _pickaxeImg.transform.GetChild(0).GetComponent<Image>();
        }

        private void UpdatePickaxeIcon()
        {
            int pickaxeLevel = _gameManager.PickaxeState.DamageLevel - 1;
            pickaxeLevel = Mathf.Clamp(pickaxeLevel, 0, _gameManager.GameData.PickaxeSprites.Length - 1);

            _pickaxeImg.sprite = _gameManager.GameData.PickaxeSprites[pickaxeLevel];
            _pickaxeOutlineImg.sprite = _gameManager.GameData.PickaxeSprites[pickaxeLevel];
        }

        private void OnEnable()
        {
            _scrollRect.verticalNormalizedPosition = 1f - (1f / 4) * 3f;
            _scrollRect.DOVerticalNormalizedPos(1f, 1f).SetEase(_scrollRectShowCurve).SetAutoKill(true);

            UpdatePickaxeIcon();
        }
    }
}