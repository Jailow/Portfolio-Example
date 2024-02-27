using CaveMiner.Helpers;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CaveMiner
{
    public class BloodySlimeBlock : BlockBase
    {
        [SerializeField] private Image[] _parts;
        [SerializeField] private int _newPartsFromIndex;
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

            _parts = new Image[allChild.Length - 1];
            _positions = new Vector3[allChild.Length - 1];

            for (int i = 1; i < allChild.Length; i++)
            {
                var img = allChild[i].GetComponent<Image>();
                var spriteRenderer = allChild[i].GetComponent<SpriteRenderer>();

                GameObject obj = allChild[i].gameObject;

                if (img == null)
                    img = obj.AddComponent<Image>();

                _parts[i - 1] = img;
                _positions[i - 1] = obj.transform.localPosition;

                img.raycastTarget = false;
                img.maskable = false;

                if (spriteRenderer != null)
                {
                    img.sprite = spriteRenderer.sprite;
                    DestroyImmediate(spriteRenderer);
                }
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

                var tr = _parts[_currentIndex].transform;

                if (_currentIndex < _newPartsFromIndex) // Первый тип частей
                {
                    if (explosion)
                    {
                        _parts[_currentIndex].transform.DOLocalMoveY(_positions[_currentIndex].y - 2000, 0.5f);
                        _parts[_currentIndex].DOFade(0f, 0.5f);
                        _parts[_currentIndex].transform.DOScale(1.5f, 0.5f);
                    }
                    else
                    {
                        _parts[_currentIndex].transform.DOLocalMoveY(_positions[_currentIndex].y - 2000, 1.2f);
                        _parts[_currentIndex].DOFade(0f, 1.2f);
                        _parts[_currentIndex].transform.DOScale(1.5f, 1.2f);
                    }
                }
                else // Второй тип частей
                {
                    if (explosion)
                    {
                        _parts[_currentIndex].DOFade(0f, 0.25f);
                        _parts[_currentIndex].transform.DOScale(1.5f, 0.25f);
                    }
                    else
                    {
                        _parts[_currentIndex].DOFade(0f, 0.5f);
                        _parts[_currentIndex].transform.DOScale(1.5f, 0.5f);
                    }
                }

                var pos = tr.localPosition;
                pos.z = -50f;
                tr.localPosition = pos;

                _currentIndex++;
            }

            if (value <= 0f)
            {
                _isDestroyed = true;
                _timeToDisable = 1.3f;
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
                var tr = _parts[i].transform;
                tr.localPosition = _positions[i];
                tr.localScale = CachedVector3.One;
                tr.localEulerAngles = CachedVector3.Zero;

                var color = _parts[i].color;
                color.a = 1f;
                _parts[i].color = color;
            }
        }

        private void OnEnable()
        {
            ResetVisual();
        }
    }
}