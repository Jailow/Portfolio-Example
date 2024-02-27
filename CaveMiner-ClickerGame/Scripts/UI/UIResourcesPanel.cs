using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CaveMiner.Helpers;
using System.Threading.Tasks;

namespace CaveMiner.UI
{
    public class UIResourcesPanel : MonoBehaviour
    {
        [SerializeField] private UIFolderTab _folderTabPrefab;
        [SerializeField] private UIResourceItemTab _resourceItemPrefab;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private Button _storageButton;
        [SerializeField] private TextMeshProUGUI _experiencePerSecondText;

        private GameManager _gameManager;
        private UIManager _uiManager;
        private Dictionary<string, UIFolderTab> _folders;

        public void Init(GameManager gameManager, UIManager uiManager)
        {
            _gameManager = gameManager;
            _uiManager = uiManager;

            _gameManager.onStorageLevelChanged += UpdateStorage;

            _folders = new Dictionary<string, UIFolderTab>();

            _storageButton.onClick.AddListener(() =>
            {
                uiManager.ButtonClickSound();
                _uiManager.ShowStoragePanel();
            });

            UpdateItems();
        }

        private void UpdateStorage()
        {
            var storageState = _gameManager.GameState.PlayerState.StorageState;

            string currentExperiencePerSecond = NumberToString.Convert(storageState.ExperiencePerSecond);
            string maxExperiencePerSecond = NumberToString.Convert(_gameManager.GetStorageMaxExperiencePerSecond(storageState.Level));
            _experiencePerSecondText.text = $"{currentExperiencePerSecond}/{maxExperiencePerSecond} {I2.Loc.LocalizationManager.GetTranslation("Inventory/eps")}";
        }

        private async void UpdateItems()
        {
            var playerState = _gameManager.GameState.PlayerState;

            foreach (var itemState in playerState.ResourceItems)
            {
                var item = _gameManager.ResourceItems.FirstOrDefault(e => e.Id == itemState.Id);

                if (item == null)
                    continue;

                if (!_folders.ContainsKey(item.CaveData.Id)) // Если папки не существует
                {
                    var newFolder = Instantiate(_folderTabPrefab, _scrollRect.content, false);
                    newFolder.Init(_uiManager);
                    _folders.Add(item.CaveData.Id, newFolder);
                }

                var folder = _folders[item.CaveData.Id];
                folder.gameObject.SetActive(true);
                folder.Title.text = I2.Loc.LocalizationManager.GetTranslation($"CaveSelection/Cave/{item.CaveData.Id}");

                if (folder.Items.FirstOrDefault(e => (e as UIResourceItemTab).ItemState.Id == itemState.Id) != null)
                    continue;

                var itemTab = Instantiate(_resourceItemPrefab, folder.Content, false);
                itemTab.Init(folder);
                itemTab.Init(_gameManager, itemState);
                folder.Items.Add(itemTab);
            }

            _scrollRect.verticalNormalizedPosition = 1f;

            await Task.Delay(10);

            LayoutRebuilder.ForceRebuildLayoutImmediate(_scrollRect.content);
        }

        private void OnEnable()
        {
            _gameManager.onExperiencePerSecondChanged += UpdateStorage;

            foreach (var folder in _folders.Values)
            {
                if (folder.IsOpened)
                    folder.SwitchInstante();
            }

            UpdateItems();
            UpdateStorage();
        }

        private void OnDisable()
        {
            _gameManager.onExperiencePerSecondChanged -= UpdateStorage;
        }
    }
}