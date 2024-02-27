using GD.MinMaxSlider;
using UnityEngine;

namespace CaveMiner
{
    [CreateAssetMenu(fileName = "New Cave", menuName = "CaveMiner/Cave")]
    public class CaveData : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private Sprite _icon;
        [SerializeField] private BlockDataPercentage[] _blockDatas;
        [SerializeField] private CaveLevelData[] _caveLevels;
        [SerializeField] private float _blockExperienceMultiplierPerLevel;

        public string Id => _id;
        public Sprite Icon => _icon;
        public BlockDataPercentage[] BlocksDatas => _blockDatas;
        public int MaxLevel => _caveLevels.Length;

        public BlockData GetRandomBlockData()
        {
            if (_blockDatas.Length <= 0)
                return null;

            float percent = Random.Range(0f, 100f);
            for(int i = 0; i < _blockDatas.Length; i++)
            {
                if (percent >= _blockDatas[i].SpawnPercentage.x && percent < _blockDatas[i].SpawnPercentage.y)
                {
                    return _blockDatas[i].BlockData;
                }
            }

            return null;
        }

        public BlockData GetRichestExperienceBlockData()
        {
            if (_blockDatas.Length <= 0)
                return null;

            BlockData blockData = null;

            for (int i = 0; i < _blockDatas.Length; i++)
            {
                if (blockData == null)
                {
                    blockData = _blockDatas[i].BlockData;
                }
                else
                {
                    if(_blockDatas[i].BlockData.ExperienceCount > blockData.ExperienceCount)
                    {
                        blockData = _blockDatas[i].BlockData;
                    }
                }
            }

            return blockData;
        }

        public double GetAveragePricePerBlock(int caveLevel)
        {
            double value = 0;

            foreach(var block in _blockDatas)
            {
                double percent = (block.SpawnPercentage.y - block.SpawnPercentage.x) / 100f;
                value += (double)block.BlockData.ExperienceCount * percent;
            }

            return value * GetExperienceMultiplier(caveLevel);
        }

        public double GetAverageHealthPerBlock()
        {
            double value = 0f;

            foreach (var block in _blockDatas)
            {
                double percent = (block.SpawnPercentage.y - block.SpawnPercentage.x) / 100f;
                value += (double)block.BlockData.ExperienceCount * percent;
            }

            return value;
        }

        public CaveLevelData GetLevelData(int caveLevel)
        {
            return _caveLevels[caveLevel - 1];
        }

        public float GetExperienceMultiplier(int caveLevel)
        {
            return 1f + ((caveLevel - 1) * _blockExperienceMultiplierPerLevel);
        }

        private void OnValidate()
        {
            if (_blockDatas.Length <= 0)
                return;

            float lastValue = 0f;
            for(int i = 0; i < _blockDatas.Length; i++)
            {
                if (i == 0)
                {
                    _blockDatas[i].SetMinChance(0f);
                }
                else
                {
                    _blockDatas[i].SetMinChance(lastValue);

                    if(_blockDatas[i].SpawnPercentage.x > _blockDatas[i].SpawnPercentage.y)
                    {
                        _blockDatas[i].SetMaxChance(lastValue);
                    }

                    if (i == _blockDatas.Length - 1)
                    {
                        _blockDatas[i].SetMaxChance(100f);
                    }
                }

                lastValue = _blockDatas[i].SpawnPercentage.y;
            }
        }
    }

    [System.Serializable]
    public class CaveLevelData
    {
        [SerializeField] private CaveLevelItemData[] _itemsToUnlock;
        [SerializeField] private int _mineralsCountToUnlock;
        [SerializeField] private int _experienceCountToUnlock;

        public CaveLevelItemData[] ItemsToUnlock => _itemsToUnlock;
        public int MineralsCountToUnlock => _mineralsCountToUnlock;
        public int ExperienceCountToUnlock => _experienceCountToUnlock;
    }

    [System.Serializable]
    public class CaveLevelItemData
    {
        [SerializeField] private ItemData _itemData;
        [SerializeField] private int _count;

        public ItemData ItemData => _itemData;
        public int Count => _count;
    }

    [System.Serializable]
    public class BlockDataPercentage
    {
        [SerializeField] private BlockData _blockData;
        [SerializeField, MinMaxSlider(0f, 100f)] private Vector2 _spawnPercentage;

        public BlockData BlockData => _blockData;
        public Vector2 SpawnPercentage => _spawnPercentage;

        public void SetMinChance(float value)
        {
            _spawnPercentage.x = value;
        }

        public void SetMaxChance(float value)
        {
            _spawnPercentage.y = value;
        }
    }
}