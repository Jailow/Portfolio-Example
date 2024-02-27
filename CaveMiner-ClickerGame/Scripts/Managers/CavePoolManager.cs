using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CaveMiner
{
    public class CavePool
    {
        private int _maxBlockCount;
        private Transform _blockParent;
        private Dictionary<string, BlockPool> _blockPools = new Dictionary<string, BlockPool>();

        public CavePool(int maxBlockCount, Transform blockParent)
        {
            _maxBlockCount = maxBlockCount;
            _blockParent = blockParent;
        }

        public BlockBase GetBlockPrefab(BlockData blockData)
        {
            if (!_blockPools.ContainsKey(blockData.Id))
            {
                _blockPools.Add(blockData.Id, new BlockPool(_maxBlockCount, _blockParent));
            }

            var blockPool = _blockPools[blockData.Id];

            return blockPool.GetBlockPrefab(blockData);
        }
    }

    public class BlockPool
    {
        private int _maxBlockCount;
        private Transform _blockParent;
        private Queue<BlockBase> _availableBlocks = new Queue<BlockBase>();
        private List<BlockBase> _activeBlocks = new List<BlockBase>();

        public BlockPool(int maxBlockCount, Transform blockParent)
        {
            _maxBlockCount = maxBlockCount;
            _blockParent = blockParent;
        }

        public BlockBase GetBlockPrefab(BlockData blockData)
        {
            if (_availableBlocks.Count <= 0) // Если нет доступных блоков
            {
                if(_availableBlocks.Count + _activeBlocks.Count >= _maxBlockCount) // Если блоков больше чем разрешено
                {
                    if(_activeBlocks.Count > 0) // если существует активные блоки
                    {
                        var blockBase = _activeBlocks[0];
                        blockBase.ResetObject();

                        _activeBlocks.RemoveAt(0); // Переставляем обьект в конец листа
                        _activeBlocks.Add(blockBase);

                        return blockBase;
                    }
                }
                else // Если есть свободное место для нового блока
                {
                    var blockBase = MonoBehaviour.Instantiate(blockData.BlockPrefab, _blockParent);
                    blockBase.Init(OnBlockDisabled);

                    _activeBlocks.Add(blockBase);

                    return blockBase;
                }
            }
            else // Если есть доступный блок
            {
                var blockBase = _availableBlocks.Dequeue();
                blockBase.ResetObject();
                blockBase.gameObject.SetActive(true);

                _activeBlocks.Add(blockBase);

                return blockBase;
            }

            return null;
        }

        private void OnBlockDisabled(BlockBase blockBase)
        {
            _availableBlocks.Enqueue(blockBase);
            _activeBlocks.Remove(blockBase);
        }
    }

    public class CavePoolManager
    {
        private int _maxBlockCount;
        private Dictionary<string, CavePool> _cavePools;
        private Transform _blockParent;

        public CavePoolManager(Transform blockParent, int maxBlockCount)
        {
            _maxBlockCount = maxBlockCount;
            _blockParent = blockParent;

            _cavePools = new Dictionary<string, CavePool>();
        }

        public BlockBase GetBlockData(string caveId, BlockData blockData)
        {
            if (!_cavePools.ContainsKey(caveId))
            {
                _cavePools.Add(caveId, new CavePool(_maxBlockCount, _blockParent));
            }

            var cavePool = _cavePools[caveId];

            return cavePool.GetBlockPrefab(blockData);
        }
    }
}