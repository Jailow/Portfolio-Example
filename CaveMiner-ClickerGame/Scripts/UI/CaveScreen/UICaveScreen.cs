using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

namespace CaveMiner.UI
{
    public class UICaveScreen : MonoBehaviour
    {
        [SerializeField] private Image _blockImage;
        [SerializeField] private GameObject _nextBlockFade;
        [SerializeField] private UIKeysPanel _keysPanel;
        [SerializeField] private Button _dynamiteButton;
        [SerializeField] private TextMeshProUGUI _dynamiteCountText;
        [SerializeField] private Transform _boosterNotificationParent;
        [SerializeField] private UIBoosterNotification _boosterNotificationPrefab;
        [SerializeField] private Button _upCaveBtn;
        [SerializeField] private Button _settingsBtn;
        [SerializeField] private Button _leaderboardsBtn;
        [SerializeField] private Button _tasksBtn;
        [SerializeField] private Button _chatBtn;
        [SerializeField] private Button _cavesBtn;
        [SerializeField] private Button _chestsBtn;
        [SerializeField] private Button _changeKeyBtn;
        [SerializeField] private Animator _pickaxeAnimator;
        [SerializeField] private Transform _tapTutorialText;
        [SerializeField] private Button _giftBtn;
        [SerializeField] private Button _noAdsBtn;
        [SerializeField] private Transform _blocksParent;
        [SerializeField] private float _timeToShowGift;
        [SerializeField] private UIBonusBox _bonusBox;
        [SerializeField] private float _timeToShowBonusBoxMin;
        [SerializeField] private float _timeToShowBonusBoxMax;
        [SerializeField] private RectTransform _blockParticleSpawnAnchor;

        private float _timeToShowBonusBox;
        private float _bonusBoxTime;
        private float _giftTime;
        private bool _haveGift;
        private bool _canShowGift;
        private Tween _tapTutorialTextTween;
        private Tween _giftTween;
        private UIManager _uiManager;
        private GameManager _gameManager;
        private Image _pickaxeImg;
        private Animator _upCaveAnimator;
        private List<UIBoosterNotification> _boosterNotifications;

        public Image BlockImage => _blockImage;
        public Animator PickaxeAnimator => _pickaxeAnimator;
        public Transform BlocksParent => _blocksParent;
        public RectTransform BlockParticleSpawnAnchor => _blockParticleSpawnAnchor;
        public bool CanShowGift
        {
            get { return _canShowGift; }
            set { _canShowGift = value; }
        }

        public void Init(GameManager gameManager, CaveController caveController, UIManager uiManager)
        {
            _uiManager = uiManager;
            _gameManager = gameManager;

            _keysPanel.Init(gameManager);
            _bonusBox.Init(gameManager, uiManager);
            _timeToShowBonusBox = Random.Range(_timeToShowBonusBoxMin, _timeToShowBonusBoxMax);

            _boosterNotifications = new List<UIBoosterNotification>();

            _upCaveAnimator = _upCaveBtn.GetComponent<Animator>();

            _dynamiteButton.onClick.AddListener(() =>
            {
                #region UseDynamite
                AmplitudeManager.Instance.Event(AnalyticEventKey.USE_DYNAMITE);
                #endregion

                _gameManager.GameState.PlayerState.Stats.UseDynamiteCount++;

                if (_gameManager.GameState.SoundsIsOn)
                {
                    var audioShot = ObjectPoolManager.Instance.GetObject(PoolName.AudioShot).GetComponent<AudioShot>();
                    audioShot.Play(_gameManager.SoundData.GetDynamiteExplosive());
                }

                caveController.UseDynamite(_gameManager.GameData.DynamiteBreakBlocksCount);

                ObjectPoolManager.Instance.GetObject(PoolName.DynamiteVFX);

                ItemData dynamiteItem = _gameManager.Items.FirstOrDefault(e => e.ItemType == ItemType.Dynamite);
                _gameManager.RemoveItemFromInventory(dynamiteItem.Id, 1);

                UpdateDynamiteButton();
            });

            _noAdsBtn.onClick.AddListener(() =>
            {
                IAPController.Instance.Purchase("no_ads");
            });

            _upCaveBtn.onClick.AddListener(() =>
            {
                _uiManager.ButtonClickSound();
                _uiManager.CaveSelectionPanel.UnlockCave();
            });

            _settingsBtn.onClick.AddListener(() =>
            {
                _uiManager.ButtonClickSound();
                _uiManager.OpenSettings();
            });

            _leaderboardsBtn.onClick.AddListener(() =>
            {
                Social.ShowLeaderboardUI();
            });

            _tasksBtn.onClick.AddListener(() =>
            {
                _uiManager.ButtonClickSound();
                _uiManager.OpenTasksPanel();
            });

            _cavesBtn.onClick.AddListener(() =>
            {
                _uiManager.ButtonClickSound();
                _uiManager.OpenCaveSelection();
            });

            _chestsBtn.onClick.AddListener(() =>
            {
                _uiManager.ButtonClickSound();
                _uiManager.OpenChestsPanel();
            });

            _changeKeyBtn.onClick.AddListener(() => 
            {
                _uiManager.ButtonClickSound();
                _uiManager.OpenChangeKeyPanel();
            });

            _giftBtn.onClick.AddListener(() =>
            {
                OnTakeGift();
            });

            _pickaxeImg = _pickaxeAnimator.GetComponent<Image>();

            _gameManager.onGraphicsQualityChanged += (GraphicsQuality quality) => UpdateGraphicsQuality(); 

            caveController.onBlockSpawned += OnBlockSpawned;
            TutorialController.Instance.onTutorialCompleted += OnTutorialCompleted;

            UpdateBoostersNotifications();
            UpdateDynamiteButton();

            if (!_gameManager.GameState.PlayerState.CompletedTutorialsIds.Contains("top_panel_tutorial"))
            {
                _tapTutorialText.gameObject.SetActive(true);
                _tapTutorialTextTween = DOTween.Sequence().Append(_tapTutorialText.DOScale(1.15f, 0.175f).SetEase(Ease.Linear)).Append(_tapTutorialText.DOScale(0.95f, 0.175f).SetEase(Ease.Linear)).SetLoops(-1);
            }

            var playerState = _gameManager.GameState.PlayerState;
            _canShowGift = playerState.CaveStates.Count >= 2;
        }

        private void Update()
        {
            _bonusBoxTime += Time.deltaTime;

            if(_bonusBoxTime >= _timeToShowBonusBox)
            {
                _bonusBoxTime = 0;
                _timeToShowBonusBox = Random.Range(_timeToShowBonusBoxMin, _timeToShowBonusBoxMax);

                if (_gameManager.GameState.PlayerState.CompletedTutorialsIds.Contains("cave_upgrade_tutorial"))
                {
                    ShowBonusBox();
                }
            }

            if (_haveGift || !_canShowGift)
                return;

            _giftTime += Time.deltaTime;
            if (_giftTime >= _timeToShowGift)
            {
                _giftTime = 0f;
                _haveGift = true;

                _giftBtn.gameObject.SetActive(true);
                _giftBtn.transform.localScale = Helpers.CachedVector3.One;
                _giftTween = DOTween.Sequence().Append(_giftBtn.transform.DOScale(1.15f, 0.25f).SetEase(Ease.OutSine)).Append(_giftBtn.transform.DOScale(0.95f, 0.25f).SetEase(Ease.OutSine)).SetLoops(-1);
            }
        }

        private void OnTakeGift()
        {
            _uiManager.OpenGiftPanel();

            _haveGift = false;

            _giftBtn.gameObject.SetActive(false);
            _giftTween?.Kill();
        }

        private void OnTutorialCompleted(string tutorialId)
        {
            if(tutorialId == "top_panel_tutorial")
            {
                DOTween.Sequence().Append(_tapTutorialText.DOScale(0f, 0.25f)).AppendCallback(() => _tapTutorialText.gameObject.SetActive(false));
                _tapTutorialTextTween?.Kill();
                _tapTutorialTextTween = null;
            }
        }

        private void OnExperienceChanged(int count)
        {
            var playerState = _gameManager.GameState.PlayerState;

            if (playerState.CaveLevel >= _gameManager.Caves.Length)
            {
                _upCaveBtn.gameObject.SetActive(false);
                return;
            }

            int needCount = _gameManager.GetExperienceCountToNextLevel();

            bool showAnimation = count >= needCount && !_upCaveBtn.gameObject.activeSelf;

            _upCaveAnimator.enabled = false;
            _upCaveBtn.gameObject.SetActive(count >= needCount);
            _upCaveAnimator.enabled = true;

            if (count >= needCount && showAnimation)
            {
                _upCaveAnimator.Play("Show");

                if (_gameManager.GameState.SoundsIsOn && _uiManager.NavigationPanel.CurrentSelectedNavigationButton == NavigationButtonType.Cave)
                {
                    var audioShot = ObjectPoolManager.Instance.GetObject(PoolName.AudioShot).GetComponent<AudioShot>();
                    audioShot.Play(_gameManager.SoundData.LevelUp);
                }
            }
        }

        private void OnBlockSpawned(BlockData blockData)
        {
            //switch (_gameManager.GameState.GraphicsQuality)
            //{
            //    case GraphicsQuality.Low:
            //        _blockImage.sprite = _gameManager.CaveController.CurrentBlockData.Sprite;
            //        break;
            //    case GraphicsQuality.High:
            //        _blockImage.sprite = _gameManager.CaveController.NextBlockData.Sprite;
            //        break;
            //}
        }

        private void UpdatePickaxeIcon()
        {
            int pickaxeLevel = _gameManager.PickaxeState.DamageLevel - 1;
            pickaxeLevel = Mathf.Clamp(pickaxeLevel, 0, _gameManager.GameData.PickaxeSprites.Length - 1);

            _pickaxeImg.sprite = _gameManager.GameData.PickaxeSprites[pickaxeLevel];
        }

        private void UpdateBoostersNotifications()
        {
            foreach(var boosterNotification in _boosterNotifications)
            {
                boosterNotification.gameObject.SetActive(false);
            }

            var playerState = _gameManager.GameState.PlayerState;

            for(int i = 0; i < playerState.BoosterStates.Count; i++)
            {
                var boosterItemData = _gameManager.Items.FirstOrDefault(e => e.Id == playerState.BoosterStates[i].Id) as BoosterItemData;

                if (boosterItemData == null)
                    continue;

                if (i >= _boosterNotifications.Count)
                {
                    var boosterNotification = Instantiate(_boosterNotificationPrefab, _boosterNotificationParent, false);
                    boosterNotification.Init(_gameManager);
                    _boosterNotifications.Add(boosterNotification);
                }

                _boosterNotifications[i].gameObject.SetActive(true);
                _boosterNotifications[i].Set(playerState.BoosterStates[i], boosterItemData, boosterItemData.MoneyMultiplier > 0f ? BoosterType.Money : BoosterType.Experience);

                switch (boosterItemData.BoosterType)
                {
                    case BoosterType.Combo:
                        if (i + 1 >= _boosterNotifications.Count)
                        {
                            var boosterNotification = Instantiate(_boosterNotificationPrefab, _boosterNotificationParent, false);
                            boosterNotification.Init(_gameManager);
                            _boosterNotifications.Add(boosterNotification);
                        }

                        if (i < playerState.BoosterStates.Count)
                        {
                            _boosterNotifications[i + 1].gameObject.SetActive(true);
                            _boosterNotifications[i + 1].Set(playerState.BoosterStates[i], boosterItemData, BoosterType.Experience);
                        }
                        break;
                }
            }
        }

        private void UpdateDynamiteButton()
        {
            int dynamiteCount = 0;

            ItemData dynamiteItem = _gameManager.Items.FirstOrDefault(e => e.ItemType == ItemType.Dynamite);

            _gameManager.GameState.PlayerState.Items.ForEach(e =>
            {
                if (dynamiteItem.Id == e.Id)
                {
                    dynamiteCount += e.Count;
                }
            });

            _dynamiteCountText.text = dynamiteCount.ToString();
            _dynamiteButton.gameObject.SetActive(dynamiteCount > 0);
        }

        private void OnItemAdded(ItemState itemState)
        {
            var item = _gameManager.Items.FirstOrDefault(e => e.Id == itemState.Id);

            if (item != null && item.ItemType == ItemType.Dynamite)
                UpdateDynamiteButton();
        }

        private void TryShowTutorial()
        {
            if (!_uiManager.HaveOverlayPanel() && _uiManager.NavigationPanel.CurrentSelectedNavigationButton == NavigationButtonType.Cave && !TutorialController.Instance.TutorialPlaying)
            {
                #region CaveUpgrade Tutorial
                bool caveUpgradeTutorialCompleted = _gameManager.GameState.PlayerState.CompletedTutorialsIds.Contains("cave_upgrade_tutorial");

                if (!caveUpgradeTutorialCompleted && _gameManager.GameState.PlayerState.Experience >= _gameManager.GetExperienceCountToNextLevel())
                {
                    TutorialController.Instance.StartTutorial("cave_upgrade_tutorial");
                }
                #endregion

                bool navigationPanelTutorialCompleted = _gameManager.GameState.PlayerState.CompletedTutorialsIds.Contains("navigation_panel_tutorial");

                #region NavigationPanel Tutorial
                bool keyPanelTutorialCompleted = _gameManager.GameState.PlayerState.CompletedTutorialsIds.Contains("key_panel_tutorial");
                if (!navigationPanelTutorialCompleted)
                {
                    if (keyPanelTutorialCompleted)
                    {
                        TutorialController.Instance.StartTutorial("navigation_panel_tutorial");
                    }
                }
                #endregion

                #region Inventory Tutorial
                if (navigationPanelTutorialCompleted)
                {
                    bool inventoryTutorialCompleted = _gameManager.GameState.PlayerState.CompletedTutorialsIds.Contains("inventory_tutorial");
                    if (!inventoryTutorialCompleted)
                    {
                        TutorialController.Instance.StartTutorial("inventory_tutorial");
                    }
                }
                #endregion
            }
        }

        private void UpdateGraphicsQuality()
        {
            //_nextBlockFade.SetActive(_gameManager.GameState.GraphicsQuality == GraphicsQuality.High);

            //switch (_gameManager.GameState.GraphicsQuality)
            //{
            //    case GraphicsQuality.Low:
            //        _blockImage.sprite = _gameManager.CaveController.CurrentBlockData.Sprite;
            //        break;
            //    case GraphicsQuality.High:
            //        _blockImage.sprite = _gameManager.CaveController.NextBlockData.Sprite;
            //        break;
            //}
        }

        private void ShowBonusBox()
        {
            _bonusBox.RectTr.anchoredPosition = new Vector2(Random.Range(-275, 275), Random.Range(-575, 575));
            _bonusBox.Show();
        }

        private void OnPurchaseCompleted(Product product)
        {
            if (product.definition.id == "no_ads")
                _noAdsBtn.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            IAPController.Instance.onPurchaseCompleted += OnPurchaseCompleted;

            Invoke(nameof(TryShowTutorial), 0.02f);

            _gameManager.onItemAdded += OnItemAdded;
            _gameManager.onExperienceChanged += OnExperienceChanged;
            _gameManager.Pickaxe.onSpeedChanged += UpdatePickaxeIcon;
            _gameManager.onActiveBoostersChanged += UpdateBoostersNotifications;

            _uiManager.NavigationPanel.HealthBar.Show();
            _uiManager.NavigationPanel.DepthBar.Show();

            UpdatePickaxeIcon();
            UpdateBoostersNotifications();
            UpdateDynamiteButton();
            Invoke(nameof(UpdateGraphicsQuality), 0.02f);

            var playerState = _gameManager.GameState.PlayerState;

            var noAdsProduct = IAPController.Instance.Controller.products.WithID("no_ads");

            if(playerState.CaveStates != null && playerState.CaveStates.Count >= 6 && !noAdsProduct.hasReceipt)
            {
                _noAdsBtn.gameObject.SetActive(true);
            }

            if (playerState.CaveLevel >= _gameManager.Caves.Length)
            {
                _upCaveBtn.gameObject.SetActive(false);
                return;
            }

            int needCount = _gameManager.GetExperienceCountToNextLevel();
            _upCaveBtn.gameObject.SetActive(playerState.Experience >= needCount);
        }

        private void OnDisable()
        {
            IAPController.Instance.onPurchaseCompleted -= OnPurchaseCompleted;

            _gameManager.onItemAdded -= OnItemAdded;
            _gameManager.onExperienceChanged -= OnExperienceChanged;
            _gameManager.Pickaxe.onSpeedChanged -= UpdatePickaxeIcon;
            _gameManager.onActiveBoostersChanged -= UpdateBoostersNotifications;

            _uiManager.NavigationPanel.HealthBar.Hide();
            _uiManager.NavigationPanel.DepthBar.Hide();

            _upCaveBtn.gameObject.SetActive(false);
            _dynamiteButton.gameObject.SetActive(false);

            foreach (var boosterNotification in _boosterNotifications)
            {
                if (boosterNotification == null)
                    continue;

                boosterNotification.gameObject.SetActive(false);
            }
        }
    }
}