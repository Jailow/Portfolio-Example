using UnityEngine;

namespace CaveMiner.UI
{
    public class UILauncherScreenPickaxeAnimation : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particleSystem;

        public void PlayParticle()
        {
            _particleSystem.Play();
        }
    }
}