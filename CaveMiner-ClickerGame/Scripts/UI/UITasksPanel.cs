using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CaveMiner.UI
{
    public class UITasksPanel : MonoBehaviour
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _backgroundCloseButton;
        [SerializeField] private UIFolderTab _folderTabPrefab;
        [SerializeField] private UITaskTab _taskTabPrefab;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private GameObject _noTasksObj;

        private Dictionary<string, UIFolderTab> _folders;
        private List<UITaskTab> _taskTabs;
        private GameManager _gameManager;
        private UIManager _uiManager;
        private RectTransform _tr;

        public bool IsOpened { get; private set; }

        public void Init(GameManager gameManager, UIManager uiManager)
        {
            _gameManager = gameManager;
            _uiManager = uiManager;

            _tr = GetComponent<RectTransform>();

            _folders = new Dictionary<string, UIFolderTab>();
            _taskTabs = new List<UITaskTab>();

            _closeButton.onClick.AddListener(() =>
            {
                uiManager.ButtonClickSound();
                Hide();
            });

            _backgroundCloseButton.onClick.AddListener(Hide);
        }

        private void UpdateItems()
        {
            var playerState = _gameManager.GameState.PlayerState;

            foreach (var folder in _folders.Values)
                folder.gameObject.SetActive(false);

            foreach (var taskTab in _taskTabs)
                taskTab.gameObject.SetActive(false);

            _noTasksObj.SetActive(playerState.Tasks.Count <= 0);

            if (playerState.Tasks.Count > 0)
            {
                for (int i = 0; i < playerState.Tasks.Count; i++)
                {
                    if (!_folders.ContainsKey(playerState.Tasks[i].Category))
                    {
                        var newFolder = Instantiate(_folderTabPrefab, _scrollRect.content, false);
                        newFolder.Init(_uiManager);
                        var gridLayoutGroup = newFolder.GetComponentInChildren<GridLayoutGroup>();
                        gridLayoutGroup.constraintCount = 1;
                        gridLayoutGroup.spacing = new Vector2(0f, 30f);
                        gridLayoutGroup.cellSize = new Vector2(854f, 237f);
                        _folders.Add(playerState.Tasks[i].Category, newFolder);
                    }

                    var folder = _folders[playerState.Tasks[i].Category];
                    folder.gameObject.SetActive(true);
                    folder.Title.text = I2.Loc.LocalizationManager.GetTranslation($"Tasks/Folders/{playerState.Tasks[i].Category}");

                    if (i >= _taskTabs.Count)
                    {
                        var newTaskTab = Instantiate(_taskTabPrefab, folder.Content, false);
                        newTaskTab.Init(_gameManager, _uiManager, UpdateItems);
                        _taskTabs.Add(newTaskTab);
                    }

                    var taskTab = _taskTabs[i];
                    taskTab.Init(folder);
                    taskTab.Set(playerState.Tasks[i]);
                    taskTab.transform.SetParent(folder.Content);
                    taskTab.gameObject.SetActive(true);
                    taskTab.transform.localScale = Helpers.CachedVector3.One;
                }

                _scrollRect.verticalNormalizedPosition = 1f;

                LayoutRebuilder.ForceRebuildLayoutImmediate(_tr);
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            UpdateItems();

            IsOpened = true;
        }

        private void OnDisable()
        {
            IsOpened = false;
        }
    }
}