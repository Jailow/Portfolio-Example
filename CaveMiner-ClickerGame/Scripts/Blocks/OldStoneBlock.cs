using CaveMiner.Helpers;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CaveMiner
{
    public class OldStoneBlock : BlockBase
    {
        [SerializeField] private Rigidbody[] _parts;
        [SerializeField] private Image[] _partsImg;
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

            _parts = new Rigidbody[allChild.Length - 1];
            _partsImg = new Image[allChild.Length - 1];
            _positions = new Vector3[allChild.Length - 1];

            for (int i = 1; i < allChild.Length; i++)
            {
                var img = allChild[i].GetComponent<Image>();
                var rb = allChild[i].GetComponent<Rigidbody>();
                var spriteRenderer = allChild[i].GetComponent<SpriteRenderer>();
                var sphereCollider = allChild[i].GetComponent<SphereCollider>();

                GameObject obj = allChild[i].gameObject;

                if (img == null)
                    img = obj.AddComponent<Image>();

                if (rb == null)
                    rb = obj.AddComponent<Rigidbody>();

                if (sphereCollider == null)
                    sphereCollider = obj.AddComponent<SphereCollider>();

                sphereCollider.center = Vector3.zero;
                sphereCollider.radius = 1f;

                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
                rb.isKinematic = true;

                _parts[i - 1] = rb;
                _partsImg[i - 1] = img;
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
                    _parts[_currentIndex].useGravity = true;
                    _parts[_currentIndex].isKinematic = false;
                    _parts[_currentIndex].transform.position += CachedVector3.Back * 10;

                    if (explosion)
                    {
                        _parts[_currentIndex].AddForce(new Vector3(Random.Range(-15000f, 15000f), Random.Range(15000f, 15000f), -1500f), ForceMode.Acceleration);
                        _parts[_currentIndex].AddTorque(CachedVector3.Back * Random.Range(-500f, 500f), ForceMode.Acceleration);
                    }
                    else
                    {
                        _parts[_currentIndex].AddForce(new Vector3(Random.Range(-2000f, 2000f), Random.Range(1000f, 4000f), -1500f), ForceMode.Acceleration);
                        _parts[_currentIndex].AddTorque(CachedVector3.Back * Random.Range(-100f, 100f), ForceMode.Acceleration);
                    }
                }
                else // Второй тип частей
                {
                    if (explosion)
                    {
                        _partsImg[_currentIndex].DOFade(0f, 0.3f);
                    }
                    else
                    {
                        _partsImg[_currentIndex].DOFade(0f, 0.5f);
                        tr.DOShakePosition(0.5f, 50f);
                        tr.DOScale(0.3f, 0.5f);
                        tr.DOLocalRotate(CachedVector3.Forward * Random.Range(-30f, 30f), 0.5f);
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
                _timeToDisable = 2f;
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

                _parts[i].isKinematic = true;
                _parts[i].useGravity = false;

                var color = _partsImg[i].color;
                color.a = 1f;
                _partsImg[i].color = color;
            }
        }

        private void OnEnable()
        {
            ResetVisual();
        }
    }
}