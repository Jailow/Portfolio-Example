using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace CaveMiner.UI
{
    public class UIKeysPanel : MonoBehaviour
    {
        private Dictionary<RarityType, UIKeyTab> _keyTabs;

        private GameManager _gameManager;
        private RectTransform _tr;

        public void Init(GameManager gameManager)
        {
            _gameManager = gameManager;

            _tr = GetComponent<RectTransform>();
            _keyTabs = new Dictionary<RarityType, UIKeyTab>();

            var keyTabs = GetComponentsInChildren<UIKeyTab>(true);

            foreach(var keyTab in keyTabs)
            {
                _keyTabs.Add(keyTab.RarityType, keyTab);
            }
        }

        private void OnKeyChanged()
        {
            UpdateKeyStates();
        }

        private void UpdateKeyStates()
        {
            var playerState = _gameManager.GameState.PlayerState;

            foreach (var keyTab in _keyTabs.Values)
            {
                switch (keyTab.RarityType)
                {
                    case RarityType.Common:
                        keyTab.SetCount(playerState.CommonKeyCount);
                        break;
                    case RarityType.Rare:
                        keyTab.SetCount(playerState.RareKeyCount);
                        break;
                    case RarityType.Epic:
                        keyTab.SetCount(playerState.EpicKeyCount);
                        break;
                    case RarityType.Mythical:
                        keyTab.SetCount(playerState.MythicalKeyCount);
                        break;
                    case RarityType.Legendary:
                        keyTab.SetCount(playerState.LegendaryKeyCount);
                        break;
                }
            }
        }

        private void OnEnable()
        {
            _gameManager.onKeyChanged += OnKeyChanged;

            UpdateKeyStates();
        }

        private void OnDisable()
        {
            _gameManager.onKeyChanged -= OnKeyChanged;
        }
    }
}