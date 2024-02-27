using UnityEngine;

namespace CaveMiner
{
    [CreateAssetMenu(fileName = "Craft Item", menuName = "CaveMiner/Craft Item")]
    public class CraftItemData : ScriptableObject
    {
        [SerializeField] private Sprite _icon;
        [SerializeField] private string _id;
        [SerializeField] private ResourceItemData _resourceToCraft;
        [SerializeField] private int _countToCraft;
        [SerializeField] private int _experiencePerSecond;

        public Sprite Icon => _icon;
        public string Id => _id;
        public ResourceItemData ResourceToCraft => _resourceToCraft;
        public int CountToCraft => _countToCraft;
        public int ExperiencePerSecond => _experiencePerSecond;
    }
}