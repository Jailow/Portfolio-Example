using UnityEngine;

namespace CaveMiner
{
    public class PoolObject : MonoBehaviour
    {
        public System.Action<PoolObject> onMoveToPool;
        public Transform Tr { get; private set; }

        private void Awake()
        {
            Tr = GetComponent<Transform>();
        }

        private void OnDisable()
        {
            onMoveToPool?.Invoke(this);
        }
    }
}