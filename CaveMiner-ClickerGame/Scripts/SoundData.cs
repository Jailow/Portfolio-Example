using UnityEngine;

namespace CaveMiner
{
    [CreateAssetMenu(fileName = "SoundData", menuName = "CaveMiner/SoundData")]
    public class SoundData : ScriptableObject
    {
        [SerializeField] private AudioClip[] _buttonClicks;
        [SerializeField] private AudioClip _levelUp;
        [SerializeField] private AudioClip _mineralsDrop;
        [SerializeField] private AudioClip _commonKeyDrop;
        [SerializeField] private AudioClip _taskCompleted;
        [SerializeField] private AudioClip _treasuryRune;
        [SerializeField] private AudioClip[] _dynamiteExplosives;
        [SerializeField] private AudioClip[] _criticalDamage;
        [SerializeField] private AudioClip[] _boule;
        [Header("Block Hits")]
        [SerializeField] private AudioClip[] _stoneBlockHits;
        [SerializeField] private AudioClip[] _glassBlockHits;
        [SerializeField] private AudioClip[] _sandBlockHits;
        [SerializeField] private AudioClip[] _grassBlockHits;
        [SerializeField] private AudioClip[] _waterBlockHits;

        public AudioClip LevelUp => _levelUp;
        public AudioClip TaskCompleted => _taskCompleted;
        public AudioClip MineralsDrop => _mineralsDrop;
        public AudioClip CommonKeyDrop => _commonKeyDrop;
        public AudioClip TreasuryRune => _treasuryRune;

        public AudioClip GetBoule()
        {
            return _boule[Random.Range(0, _boule.Length)];
        }

        public AudioClip GetDynamiteExplosive()
        {
            return _dynamiteExplosives[Random.Range(0, _dynamiteExplosives.Length)];
        }

        public AudioClip GetCriticalDamage()
        {
            return _criticalDamage[Random.Range(0, _criticalDamage.Length)];
        }

        public AudioClip GetBlockHit(BlockSurface surface)
        {
            switch (surface)
            {
                case BlockSurface.Stone:
                    return _stoneBlockHits[Random.Range(0, _stoneBlockHits.Length)];
                case BlockSurface.Glass:
                    return _glassBlockHits[Random.Range(0, _glassBlockHits.Length)];
                case BlockSurface.Sand:
                    return _sandBlockHits[Random.Range(0, _sandBlockHits.Length)];
                case BlockSurface.Grass:
                    return _grassBlockHits[Random.Range(0, _grassBlockHits.Length)];
                case BlockSurface.Water:
                    return _waterBlockHits[Random.Range(0, _waterBlockHits.Length)];
            }

            return null;
        }

        public AudioClip GetButtonClick()
        {
            return _buttonClicks[Random.Range(0, _buttonClicks.Length)];
        }
    }
}