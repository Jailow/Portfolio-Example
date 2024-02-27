using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using DG.Tweening;
using CaveMiner.Helpers;
using System.Globalization;

namespace CaveMiner.UI
{
    public class UIStoragePanel : MonoBehaviour
    {
        [SerializeField] private Button _exitButton;
        [SerializeField] private GameObject _upgradeTabs;
        [SerializeField] private Button _upgradeButton;
        [SerializeField] private RectTransform _storageObj;
        [SerializeField] private TextMeshProUGUI _priceText;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _experiencePerSecondText;
        [SerializeField] private Color _defaultTextColor;
        [SerializeField] private Color _warningTextColor;

        public bool IsOpened { get; private set; }

        private Tween _storageTween;
        private UIDisabledButtonAlpha _upgradeDisabledButton;
        private bool _isPlacingItems;
        private GameManager _gameManager;
        private UIManager _uiManager;

        public void Init(GameManager gameManager, UIManager uiManager)
        {
            _gameManager = gameManager;
            _uiManager = uiManager;

            _gameManager.onStorageLevelChanged += UpdateVisual;

            _upgradeDisabledButton = _upgradeButton.GetComponent<UIDisabledButtonAlpha>();
            _upgradeDisabledButton.Init();

            _exitButton.onClick.AddListener(() =>
            {
                if (!_isPlacingItems)
                {
                    _uiManager.ButtonClickSound();
                    Hide();
                }
            });

            _upgradeButton.onClick.AddListener(() =>
            {
                _uiManager.ButtonClickSound();
                UpgradeStorage();
            });
        }

        private void UpgradeStorage()
        {
            var playerState = _gameManager.GameState.PlayerState;
            var storageState = playerState.StorageState;

            var price = _gameManager.GetStorageUpgradePrice(storageState.Level);

            _gameManager.AddMoney(-price);
            _gameManager.UpgradeStorage();
        }

        private void UpdateVisual()
        {
            var playerState = _gameManager.GameState.PlayerState;
            var storageState = playerState.StorageState;
            _levelText.text = $"{I2.Loc.LocalizationManager.GetTranslation("Inventory/level")} {storageState.Level}";

            string currentExperiencePerSecond = NumberToString.Convert(storageState.ExperiencePerSecond);
            string maxExperiencePerSecond = NumberToString.Convert(_gameManager.GetStorageMaxExperiencePerSecond(storageState.Level));
            _experiencePerSecondText.text = $"{currentExperiencePerSecond}/{maxExperiencePerSecond} {I2.Loc.LocalizationManager.GetTranslation("Inventory/eps")}";

            var upgradePrice = _gameManager.GetStorageUpgradePrice(storageState.Level);
            bool haveMoney = playerState.Money >= upgradePrice;
            _upgradeTabs.SetActive(storageState.Level < _gameManager.GameData.MaxStorageLevel);
            _upgradeDisabledButton.Interactable = storageState.Level < _gameManager.GameData.MaxStorageLevel && haveMoney;
            _priceText.text = $"{NumberToString.Convert(playerState.Money)}/{NumberToString.Convert((int)upgradePrice)}";
            _priceText.color = haveMoney ? _defaultTextColor : _warningTextColor;
        }

        private IEnumerator OnResourceItemTake(ResourceItemState resourceItemState, int count, Vector3 pos)
        {
            _isPlacingItems = true;

            var blockData = _gameManager.BlockDatas.FirstOrDefault(e => e.ResourceItemData.Id == resourceItemState.Id);

            var playerState = _gameManager.GameState.PlayerState;
            var storageState = playerState.StorageState;

            int maxExperiencePerSecond = _gameManager.GetStorageMaxExperiencePerSecond(storageState.Level);

            var maxAddedCount = (float)(maxExperiencePerSecond - storageState.ExperiencePerSecond);
            var addedCount = (blockData.ResourceItemData.ExperiencePerSecond * _gameManager.ExperiencePerSecondMultiplier) * count;

            if (addedCount > maxAddedCount)
            {
                var overCount = Mathf.Abs(maxAddedCount - addedCount);
                int overResourceCount = (int)(overCount / (blockData.ResourceItemData.ExperiencePerSecond * _gameManager.ExperiencePerSecondMultiplier));
                count -= overResourceCount;
            }

            if (storageState.ExperiencePerSecond >= maxExperiencePerSecond)
                count = 0;

            playerState.Stats.PlacedItemsToStorage += count;

            AchievementManager.Instance.CheckPlacedItemsToStorageCountCondition(count);

            _gameManager.AddStorageExperiencePerSecond(addedCount);
            _gameManager.AddResourceItem(resourceItemState.Id, -count);
            
            UpdateVisual();

            int blocksCount = Mathf.Clamp(count, 0, 25);

            var waitSecond = new WaitForFixedUpdate();

            for (int i = 0; i < blocksCount; i++)
            {
                var cubeParticle = ObjectPoolManager.Instance.GetObject(PoolName.BreakBlockCube)?.GetComponent<CubeParticle>();
                if (cubeParticle != null)
                {
                    if (!cubeParticle.IsInitialized)
                        cubeParticle.Init(_gameManager, _uiManager);

                    cubeParticle.PlayAnimation(blockData.BreakBlockParticleMaterial, pos, _storageObj.position, PlayStorageAnimation);
                }

                yield return waitSecond;
            }

            yield return new WaitForSeconds(1f);

            _isPlacingItems = false;
        }

        private void PlayStorageAnimation()
        {
            _storageTween?.Complete();
            _storageTween = DOTween.Sequence()
                .Append(_storageObj.DOScaleX(1.15f, 0.05f).SetEase(Ease.InSine))
                .Append(_storageObj.DOScaleX(1f, 0.05f).SetEase(Ease.OutSine));
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            IsOpened = true;

            _storageTween?.Complete();
            _uiManager.ResourceSelectionPanel.Show((itemState, count, pos) => StartCoroutine(OnResourceItemTake(itemState, count, pos)), false);

            UpdateVisual();
        }

        private void OnDisable()
        {
            IsOpened = false;

            _uiManager.ResourceSelectionPanel.Hide();
        }
    }
}