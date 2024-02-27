using UnityEngine;

namespace CaveMiner
{
    public enum BlockSurface
    {
        Stone,
        Glass,
        Sand,
        Grass,
        Water,
    }

    [CreateAssetMenu(fileName = "New Block", menuName = "CaveMiner/Block")]
    public class BlockData : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private float _health;
        [SerializeField] private int _experienceCount;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private Material _breakBlockParticleMaterial;
        [SerializeField] private BlockSurface _surface;
        [SerializeField] private ResourceItemData _resourceItemData;
        [SerializeField] private BlockBase _blockPrefab;

        public string Id => _id;
        public float Health => _health;
        public Sprite Sprite => _sprite;
        public BlockSurface Surface => _surface;
        public int ExperienceCount => _experienceCount;
        public BlockBase BlockPrefab => _blockPrefab;
        public ResourceItemData ResourceItemData => _resourceItemData;
        public Material BreakBlockParticleMaterial => _breakBlockParticleMaterial;
    }
}