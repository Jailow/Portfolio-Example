using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace CaveMiner.UI
{
    public class UICategoryButton : MonoBehaviour
    {
        [SerializeField] private Image _iconImg;
        [SerializeField] private Sprite _normalIcon;
        [SerializeField] private Sprite _activeIcon;
        [SerializeField] private Sprite _normalButton;
        [SerializeField] private Sprite _activeButton;
        [SerializeField] private float _buttonNormalPosY;
        [SerializeField] private float _buttonActivePosY;
        [SerializeField] private float _iconNormalPosY;
        [SerializeField] private float _iconActivePosY;
        [SerializeField] private float _animationTime;

        private Button _btn;
        private RectTransform _rectTr;
        private RectTransform _iconRectTr;
        private UIManager _uiManager;

        public void Init(UIManager uiManager, Action<UICategoryButton> onSelect)
        {
            _uiManager = uiManager;
            _btn = GetComponent<Button>();
            _rectTr = GetComponent<RectTransform>();
            _iconRectTr = _iconImg.GetComponent<RectTransform>();

            _btn.onClick.AddListener(() =>
            {
                _uiManager.ButtonClickSound();
                onSelect?.Invoke(this);
            });
        }

        public virtual void Select()
        {
            _iconImg.sprite = _activeIcon;
            _btn.image.sprite = _activeButton;
            _rectTr.DOAnchorPosY(_buttonActivePosY, _animationTime).SetEase(Ease.Linear).SetAutoKill(true);
            _iconRectTr.DOAnchorPosY(_iconActivePosY, _animationTime).SetEase(Ease.Linear).SetAutoKill(true);
        }

        public virtual void Deselect()
        {
            _iconImg.sprite = _normalIcon;
            _btn.image.sprite = _normalButton;
            _rectTr.DOAnchorPosY(_buttonNormalPosY, _animationTime).SetEase(Ease.Linear).SetAutoKill(true);
            _iconRectTr.DOAnchorPosY(_iconNormalPosY, _animationTime).SetEase(Ease.Linear).SetAutoKill(true);
        }
    }
}