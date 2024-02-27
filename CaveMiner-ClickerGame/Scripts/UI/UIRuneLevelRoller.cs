using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CaveMiner.UI
{
    public class UIRuneLevelRoller : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textPrefab;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private float _switchLevelSpeed;
        [SerializeField] private AnimationCurve _switchCurve;

        private List<TextMeshProUGUI> _levelTexts;
        private float _switchAmount;

        public void Init()
        {
            _levelTexts = new List<TextMeshProUGUI>();
            _textPrefab.gameObject.SetActive(false);
        }

        public void NextLevel()
        {
            StopAllCoroutines();
            StartCoroutine(SetLevelAnimation(_scrollRect.horizontalNormalizedPosition + _switchAmount));
        }

        public void PrevLevel()
        {
            StopAllCoroutines();
            StartCoroutine(SetLevelAnimation(_scrollRect.horizontalNormalizedPosition - _switchAmount));
        }

        public void ZeroLevel()
        {
            StopAllCoroutines();
            StartCoroutine(SetLevelAnimation(0f));
        }

        private IEnumerator SetLevelAnimation(float pos)
        {
            float time = 0f;
            float prevPos = _scrollRect.horizontalNormalizedPosition;

            while(time <= 1f)
            {
                time += Time.deltaTime * _switchLevelSpeed;
                _scrollRect.horizontalNormalizedPosition = Mathf.Lerp(prevPos, pos, _switchCurve.Evaluate(time));

                yield return null;
            }

            _scrollRect.horizontalNormalizedPosition = pos;
        }

        public void Set(int maxLevel, int currentLevel)
        {
            foreach(var text in _levelTexts)
            {
                text.gameObject.SetActive(false);
            }

            for(int i = 0; i < maxLevel; i++)
            {
                if(i >= _levelTexts.Count)
                {
                    var text = Instantiate(_textPrefab, _scrollRect.content, false);
                    _levelTexts.Add(text);
                }

                _levelTexts[i].gameObject.SetActive(true);
                _levelTexts[i].text = (i + 1).ToString();
            }

            _switchAmount = 1f / maxLevel;
            _scrollRect.horizontalNormalizedPosition = currentLevel * _switchAmount;
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }
    }
}