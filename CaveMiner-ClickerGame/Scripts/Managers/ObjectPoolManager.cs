using System.Collections.Generic;
using UnityEngine;

namespace CaveMiner
{
    public enum PoolName
    {
        BreakBlockCube,
        AudioShot,
        ImpulseVFX,
        DynamiteVFX,
        CriticalDamageVFX,
        TreasuryRuneVFX,
    }

    [System.Serializable]
    public class ObjectPool
    {
        [SerializeField] private PoolName poolName;
        [SerializeField] private PoolObject objectPrefab;
        [SerializeField] private int maxCount;

        [HideInInspector] public Queue<PoolObject> allObjects = new Queue<PoolObject>();
        [HideInInspector] public List<PoolObject> globalAllObjects = new List<PoolObject>();

        public PoolName PoolName => poolName;
        public PoolObject ObjectPrefab => objectPrefab;
        public int MaxCount => maxCount;

        public void OnMoveToPool(PoolObject obj)
        {
            allObjects.Enqueue(obj);
        }
    }

    public class ObjectPoolManager : Singleton<ObjectPoolManager>
    {
        [SerializeField] private ObjectPool[] _allObjectPools;

        private Transform _tr;

        private void Start()
        {
            _tr = GetComponent<Transform>();
        }

        public void ResetAllObjects()
        {
            foreach (ObjectPool pool in _allObjectPools)
            {
                foreach (PoolObject obj in pool.globalAllObjects)
                {
                    obj.gameObject.SetActive(false);
                    obj.Tr.SetParent(_tr);
                }
            }
        }

        public void ResetAllObjects(PoolName poolName)
        {
            foreach (ObjectPool pool in _allObjectPools)
            {
                if (pool.PoolName == poolName)
                {
                    foreach (PoolObject obj in pool.globalAllObjects)
                        obj.gameObject.SetActive(false);
                }
            }
        }

        public GameObject GetObject(PoolName poolName, bool active = true)
        {
            foreach (ObjectPool pool in _allObjectPools)
            {
                if (pool.PoolName == poolName)
                {
                    if (pool.allObjects.Count > 0)
                    {
                        // Get object
                        pool.allObjects.Peek().gameObject.SetActive(active);
                        return pool.allObjects.Dequeue().gameObject;
                    }
                    else
                    {
                        // Create new object

                        if (pool.MaxCount < 0 || pool.globalAllObjects.Count < pool.MaxCount)
                        {
                            PoolObject poolObject = Instantiate(pool.ObjectPrefab);
                            pool.globalAllObjects.Add(poolObject);
                            poolObject.onMoveToPool += pool.OnMoveToPool;
                            return poolObject.gameObject;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }

            return null;
        }
    }
}