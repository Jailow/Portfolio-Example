using CaveMiner.Helpers;
using UnityEngine;
using DG.Tweening;

namespace CaveMiner
{
    public class DragonScalesBlock : BlockBase
    {
        [SerializeField] private Transform[] _parts;
        [SerializeField] private Vector3[] _positions;

        private int _currentIndex;

        private void Reset()
        {
            var canvas = GetComponent<Canvas>();
            if (canvas == null)
                canvas = gameObject.AddComponent<Canvas>();

            canvas.overrideSorting = true;
            canvas.sortingOrder = 51;

            var allChild = GetComponentsInChildren<Transform>();

            _parts = new Transform[allChild.Length - 1];
            _positions = new Vector3[allChild.Length - 1];

            for (int i = 1; i < allChild.Length; i++)
            {
                GameObject obj = allChild[i].gameObject;

                _parts[i - 1] = obj.transform;
                _positions[i - 1] = obj.transform.localPosition;
            }
        }

        public override void SetDestroy(float value, bool explosion)
        {
            if (IsDestroyed)
                return;

            int destroyCount = _parts.Length - Mathf.CeilToInt((float)_parts.Length * value);
            destroyCount -= _currentIndex;

            for (int i = 0; i < destroyCount; i++)
            {
                if (_currentIndex >= _parts.Length)
                    return;

                _parts[_currentIndex].transform.position += CachedVector3.Back * 10;

                if (explosion)
                {
                    _parts[_currentIndex].DOLocalMoveY(_positions[_currentIndex].y + 3000, 0.8f);
                }
                else
                {
                    _parts[_currentIndex].DOLocalMoveY(_positions[_currentIndex].y + 3000, 1.4f);
                }

                _currentIndex++;
            }

            if (value <= 0f)
            {
                _isDestroyed = true;
                _timeToDisable = 1.4f;
            }
        }

        public override void ResetObject()
        {
            base.ResetObject();

            _currentIndex = 0;
            _isDestroyed = false;

            ResetVisual();
        }

        public override void ResetVisual()
        {
            for (int i = _currentIndex; i < _parts.Length; i++)
            {
                DOTween.Kill(_parts[_currentIndex]);

                var tr = _parts[i].transform;
                tr.localPosition = _positions[i];
                tr.localEulerAngles = CachedVector3.Zero;
            }
        }

        private void OnEnable()
        {
            ResetVisual();
        }
    }
}