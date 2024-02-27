using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace CaveMiner
{
    public class FlyingTextController : MonoBehaviour
    {
        [SerializeField] private FlyingText _flyingTextPrefab;
        [SerializeField] private Transform _parent;
        [SerializeField] private AnimationCurve _flyCurve;
        [SerializeField] private Vector2 _startPos;
        [SerializeField] private Vector2 _endPos;
        [SerializeField] private float _flyTime;
        [SerializeField] private float _spawnDelay;
        [SerializeField] private int _maxQueueCount;

        private Queue<FlyingText> _flyingTexts = new Queue<FlyingText>();
        private Queue<string> _queue = new Queue<string>();
        private float _time;

        private void Update()
        {
            _time += Time.deltaTime;
            if(_time >= _spawnDelay && _queue.Count > 0)
            {
                _time = 0f;

                FlyingText flyingText = null;
                if (_flyingTexts.Count > 0)
                {
                    flyingText = _flyingTexts.Dequeue();
                    flyingText.gameObject.SetActive(true);
                }
                else
                {
                    flyingText = Instantiate(_flyingTextPrefab, _parent, false);
                }

                flyingText.RectTr.anchoredPosition = _startPos;
                flyingText.Set(_queue.Dequeue());

                DOTween.Sequence()
                    .Append(flyingText.RectTr.DOAnchorPos(_endPos, _flyTime).SetEase(_flyCurve))
                    .AppendCallback(() =>
                    {
                        _flyingTexts.Enqueue(flyingText);
                        flyingText.gameObject.SetActive(false);
                    });
            }
        }

        public bool Show(string text)
        {
            if (_queue.Count >= _maxQueueCount)
                return false;

            _queue.Enqueue(text);
            return true;
        }
    }
}