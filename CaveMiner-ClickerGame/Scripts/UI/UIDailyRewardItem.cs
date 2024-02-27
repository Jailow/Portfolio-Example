using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CaveMiner.UI
{
    public enum DailyRewardState
    {
        Active,
        Taken,
        Closed,
    }

    public enum DailyRewardType
    {
        Minerals,
        Experience,
        CommonKey,
        RareKey,
        EpicKey,
        MythicalKey,
        LegendaryKey,
        Item,
    }

    public class UIDailyRewardItem : MonoBehaviour
    {
        [SerializeField] private DailyRewardType _rewardType;
        [SerializeField] private string _value;

        [SerializeField] private Image _icon;
        [SerializeField] private Button _takeButton;
        [SerializeField] private GameObject _slotHighlightObj;
        [SerializeField] private GameObject _takenIcon;
        [SerializeField] private Image _line;
        [SerializeField] private Sprite _activeLineSprite;
        [SerializeField] private Sprite _deactiveLineSprite;
        [SerializeField] private Image _slot;
        [SerializeField] private Sprite _activeSlotSprite;
        [SerializeField] private Sprite _deactiveSlotSprite;
        [SerializeField] private Sprite _takenSlotSprite;

        private Color _disabledColor;

        public DailyRewardType RewardType => _rewardType;
        public string Value => _value;

        public void Init(UIManager uiManager, Action<UIDailyRewardItem> onTakeReward)
        {
            _takeButton.onClick.AddListener(() =>
            {
                uiManager.ButtonClickSound();
                onTakeReward?.Invoke(this);
            });

            _disabledColor = new Color(1f, 1f, 1f, 0.3f);
        }

        public void SetSlotState(DailyRewardState state)
        {
            _slotHighlightObj.SetActive(false);
            _takenIcon.SetActive(false);
            _takeButton.gameObject.SetActive(false);

            switch (state)
            {
                case DailyRewardState.Active:
                    _slot.sprite = _activeSlotSprite;
                    _slotHighlightObj.SetActive(true);
                    _takeButton.gameObject.SetActive(true);
                    _icon.color = Color.white;
                    break;
                case DailyRewardState.Taken:
                    _slot.sprite = _takenSlotSprite;
                    _takenIcon.SetActive(true);
                    _icon.color = _disabledColor;
                    break;
                case DailyRewardState.Closed:
                    _slot.sprite = _deactiveSlotSprite;
                    _icon.color = _disabledColor;
                    break;
            }
        }

        public void LineActive(bool active)
        {
            if (_line == null)
                return;

            _line.sprite = active ? _activeLineSprite : _deactiveLineSprite;
        }
    }
}