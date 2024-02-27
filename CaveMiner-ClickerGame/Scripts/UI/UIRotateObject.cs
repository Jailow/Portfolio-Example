using CaveMiner.Helpers;
using UnityEngine;

namespace CaveMiner.UI
{
    public class UIRotateObject : MonoBehaviour
    {
        [SerializeField] private float _rotateSpeed;

        private RectTransform _tr;

        private void Awake()
        {
            _tr = GetComponent<RectTransform>();
        }

        private void Update()
        {
            _tr.Rotate(_rotateSpeed * Time.deltaTime * CachedVector3.Forward);
        }
    }
}