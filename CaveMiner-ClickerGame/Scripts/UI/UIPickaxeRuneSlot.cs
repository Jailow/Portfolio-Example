using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CaveMiner.UI
{
    public class UIPickaxeRuneSlot : MonoBehaviour
    {
        [SerializeField] private GameObject _plusIcon;
        [SerializeField] private Image _icon;

        private Button _btn;
        private GameManager _gameManager;
        private UIManager _uiManager;
        private int _index;

        private const string THIS_RUNE_PLACED_KEY = "this_rune_placed";
        private const string CLOSE_KEY = "close";

        public void Init(GameManager gameManager, UIManager uiManager, int index)
        {
            _gameManager = gameManager;
            _uiManager = uiManager;
            _index = index;

            _btn = GetComponent<Button>();

            _btn.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            var playerState = _gameManager.GameState.PlayerState;

            if(string.IsNullOrEmpty(playerState.PickaxeState.Runes[_index].Id)) // Если руны в слоте ещё нет
            {
                _uiManager.InventorySelectionPanel.Show(OnItemTake, new ItemType[1] { ItemType.Rune }, null);
            }
            else // если руна уже есть в слоте
            {
                _uiManager.RuneUpgradePanel.Show(_index);
            }
        }

        public void UpdateSlotVisual()
        {
            var playerState = _gameManager.GameState.PlayerState;
            bool haveRune = !string.IsNullOrEmpty(playerState.PickaxeState.Runes[_index].Id);

            _plusIcon.SetActive(!haveRune);
            _icon.gameObject.SetActive(haveRune);

            if (!haveRune)
                return;

            var item = _gameManager.Items.FirstOrDefault(e => e.Id == playerState.PickaxeState.Runes[_index].Id);

            if (item == null)
                return;

            _icon.sprite = item.Icon;
        }

        private void OnItemTake(ItemState itemState)
        {
            var playerState = _gameManager.GameState.PlayerState;

            foreach(var rune in playerState.PickaxeState.Runes)
            {
                if(rune.Id == itemState.Id)
                {
                    _uiManager.InfoPanel.Show(THIS_RUNE_PLACED_KEY, string.Empty, CLOSE_KEY, null, null, new string[0]);
                    return;
                }
            }

            itemState.Count--;

            if (itemState.Count <= 0)
                playerState.Items.Remove(itemState);

            bool isBroken = Random.Range(0, 100) > int.Parse(itemState.CustomValue.ToString().TrimEnd('%'));

            var item = _gameManager.Items.FirstOrDefault(e => e.Id == itemState.Id);
            _uiManager.PlaceRuneAnimation.Show(_index, isBroken, item.Icon, UpdateSlotVisual);

            if (!isBroken)
            {
                playerState.PickaxeState.Runes[_index].Id = itemState.Id;
                playerState.PickaxeState.Runes[_index].Level = 1;
            }
            else
            {
                playerState.Stats.BreakRuneCount++;

                AchievementManager.Instance.CheckBreakRuneCountCondition(playerState.Stats.BreakRuneCount);
            }

            _gameManager.SaveGame();

            #region PlaceRuneEvent
            Dictionary<string, object> properties = new Dictionary<string, object>();

            properties.Add("CaveLevel", playerState.CaveLevel);
            properties.Add("RebirthCount", playerState.Stats.RebirthCount);
            properties.Add("PlaceChance", itemState.CustomValue);
            properties.Add("RuneType", (item as RuneItemData).RuneType);
            properties.Add("IsBroken", isBroken);

            AmplitudeManager.Instance.Event(AnalyticEventKey.PLACE_RUNE, properties);
            #endregion

            AchievementManager.Instance.CheckMaxRuneCountCondition(playerState.PickaxeState);
        }

        private void OnEnable()
        {
            UpdateSlotVisual();
        }
    }
}