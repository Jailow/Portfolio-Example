using CaveMiner.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CaveMiner
{
    public class BreakAnimationBlock : BlockBase
    {
        [SerializeField] private int _newPartsFromIndex;
        [SerializeField] private Rigidbody[] _parts;
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
            _positions = new Vector3[allChild.Length - 1];

            for (int i = 1; i < allChild.Length; i++)
            {
                var img = allChild[i].GetComponent<Image>();
                var rb = allChild[i].GetComponent<Rigidbody>();
                var sphereCollider = allChild[i].GetComponent<SphereCollider>();
                var spriteRenderer = allChild[i].GetComponent<SpriteRenderer>();

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

            int destroyCount = _newPartsFromIndex - Mathf.CeilToInt((float)_newPartsFromIndex * value);
            destroyCount -= _currentIndex;

            for (int i = 0; i < destroyCount; i++)
            {
                if (_currentIndex >= _parts.Length)
                    return;

                _parts[_currentIndex].gameObject.SetActive(true);
                _currentIndex++;
            }

            if (value <= 0f)
            {
                _isDestroyed = true;
                _timeToDisable = 1.25f;

                for(int i = 0; i < _parts.Length; i++)
                {
                    if (i < _newPartsFromIndex)
                    {
                        _parts[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        _parts[i].useGravity = true;
                        _parts[i].isKinematic = false;
                        _parts[i].transform.position += CachedVector3.Back * 10;

                        if (explosion)
                        {
                            _parts[i].AddForce(new Vector3(Random.Range(-15000f, 15000f), Random.Range(15000f, 15000f), -1500f), ForceMode.Acceleration);
                            _parts[i].AddTorque(CachedVector3.Back * Random.Range(-500f, 500f), ForceMode.Acceleration);
                        }
                        else
                        {
                            _parts[i].AddForce(new Vector3(Random.Range(-2000f, 2000f), Random.Range(1000f, 4000f), -1500f), ForceMode.Acceleration);
                            _parts[i].AddTorque(CachedVector3.Back * Random.Range(-100f, 100f), ForceMode.Acceleration);
                        }
                    }
                }
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
                if (i < _newPartsFromIndex)
                {
                    _parts[i].gameObject.SetActive(false);
                }
                else
                {
                    var tr = _parts[i].transform;
                    tr.localPosition = _positions[i];
                    tr.localEulerAngles = CachedVector3.Zero;

                    _parts[i].isKinematic = true;
                    _parts[i].useGravity = false;
                }
            }
        }

        private void OnEnable()
        {
            ResetVisual();
        }
    }
}