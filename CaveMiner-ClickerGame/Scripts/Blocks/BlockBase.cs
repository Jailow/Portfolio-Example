using System;
using System.Collections;
using UnityEngine;

namespace CaveMiner
{
    public abstract class BlockBase : MonoBehaviour
    {
        private Action<BlockBase> _onDisabled;

        protected bool _isDestroyed;
        protected float _timeToDisable;
        private Transform _tr;

        public bool IsDestroyed => _isDestroyed;
        public Transform Transform
        {
            get
            {
                if (_tr == null)
                {
                    _tr = transform;
                }

                return _tr;
            }
        }

        public void Init(Action<BlockBase> onDisabled)
        {
            _onDisabled = onDisabled;
        }

        public abstract void SetDestroy(float value, bool explosion);

        public void Update()
        {
            if (!_isDestroyed)
                return;

            _timeToDisable -= Time.deltaTime;

            if (_timeToDisable >= 0f)
                return;

            gameObject.SetActive(false);

            _onDisabled?.Invoke(this);
        }

        public virtual void ResetVisual()
        {

        }

        public virtual void ResetObject()
        {
            _timeToDisable = -1;
        }

        public void DestroyObject()
        {
            _timeToDisable = -1;
            _isDestroyed = true;
        }
    }
}