using UnityEngine;

namespace CaveMiner
{
    [CreateAssetMenu(fileName = "Resource Item", menuName = "CaveMiner/Items/Resource")]
    public class ResourceItemData : ItemData
    {
        [SerializeField] private CaveData _caveData;
        [SerializeField] private float _recyclingTime;
        [SerializeField] private float _experiencePerSecond;

        public CaveData CaveData => _caveData;
        public float RecyclingTime => _recyclingTime;
        public float ExperiencePerSecond => _experiencePerSecond;
    }
}