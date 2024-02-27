using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

namespace CaveMiner.UI
{
    public class UIFolderTab : MonoBehaviour
    {
        [SerializeField] private RectTransform _content;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private Transform _arrow;

        public List<UIFolderItemBase> Items { get; private set; }
        public bool IsOpened { get; private set; }

        public RectTransform Content => _content;
        public TextMeshProUGUI Title => _title;

        private RectTransform _tr;
        private Tween _arrowTween;
        private Tween _contentTween;
        private UIManager _uiManager;

        public void Init(UIManager uiManager)
        {
            _uiManager = uiManager;

            _tr = GetComponent<RectTransform>();

            var btn = GetComponentInChildren<Button>();
            btn.onClick.AddListener(Switch);

            Items = new List<UIFolderItemBase>();

            IsOpened = true;
            SwitchInstante();
        }

        private void Switch()
        {
            _uiManager.ButtonClickSound();

            IsOpened = !IsOpened;
            _arrowTween = _arrow.DOScaleY(IsOpened ? 1f : -1f, 0.2f).SetEase(Ease.InOutSine).SetAutoKill(true);
            _contentTween = _content.DOScaleY(IsOpened ? 1f : 0f, 0.2f).SetEase(Ease.InOutSine).SetAutoKill(true);

            StopAllCoroutines();
            StartCoroutine(Rebuilder());
        }

        private IEnumerator Rebuilder()
        {
            yield return null;

            float time = 0.2f;
            while(time > 0)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(_tr);
                time -= Time.deltaTime;
                yield return null;
            }
        }

        public void SwitchInstante()
        {
            IsOpened = !IsOpened;
            _arrow.localScale = new Vector3(1f, IsOpened ? 1f : -1f, 1f);
            _content.localScale = new Vector3(1f, IsOpened ? 1f : 0f, 1f);

            LayoutRebuilder.ForceRebuildLayoutImmediate(_tr);
        }

        private void OnEnable()
        {
            StopAllCoroutines();
            StartCoroutine(Rebuilder());
        }

        private void OnDisable()
        {
            StopAllCoroutines();

            _arrowTween?.Kill();
            _contentTween?.Kill();

            IsOpened = !IsOpened;
            SwitchInstante();
        }
    }
}