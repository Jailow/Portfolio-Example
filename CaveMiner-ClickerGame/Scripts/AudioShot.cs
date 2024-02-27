using UnityEngine;

namespace CaveMiner
{
    public class AudioShot : MonoBehaviour
    {
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void Play(AudioClip audioClip)
        {
            _audioSource.PlayOneShot(audioClip);
            Invoke(nameof(Disable), audioClip.length);
        }

        private void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}