using UnityEngine;

namespace CaveMiner
{
    public class CaveObject : MonoBehaviour
    {
        [SerializeField] private CaveData _caveData;

        public CaveData CaveData => _caveData;
    }
}