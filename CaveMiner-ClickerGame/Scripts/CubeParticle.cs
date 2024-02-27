using UnityEngine;
using DG.Tweening;
using CaveMiner.UI;
using System;
using Random = UnityEngine.Random;

namespace CaveMiner
{
    public class CubeParticle : MonoBehaviour
    {
        [SerializeField] private AnimationCurve _curve;
        [SerializeField] private AnimationCurve _secondCurve;

        private Transform _tr;
        private UIManager _uiManager;
        private GameManager _gameManager;
        private MeshRenderer _meshRenderer;
        private Action _onAnimationFinished;

        public bool IsInitialized { get; private set; }

        public void Init(GameManager gameManager, UIManager uiManager)
        {
            _gameManager = gameManager;
            _uiManager = uiManager;

            _tr = GetComponent<Transform>();
            _meshRenderer = GetComponent<MeshRenderer>();

            IsInitialized = true;
        }

        public void PlayAnimation(Material material, Vector3 startPos, Vector3 endPos, Action onAnimationFinished)
        {
            _onAnimationFinished = onAnimationFinished;

            _meshRenderer.material = material;

            _tr.localScale = Vector3.zero;
            _tr.position = startPos + Vector3.back * 50f;

            var pos = _tr.position + GetRandomSpherePosition();
            _tr.localEulerAngles = -pos * 2.5f;

            DOTween.Sequence()
                .Join(DOTween.Sequence().Append(_tr.DOMove(pos, 0.5f)).Join(_tr.DOScale(Random.Range(8.5f, 10f), 0.25f)))
                .AppendInterval(0.4f)
                .Join(DOTween.Sequence().Append(_tr.DOMove(endPos, 0.5f).SetEase(_curve)).Join(_tr.DOScale(0f, 0.5f).SetEase(_secondCurve)))
                .AppendCallback(Hide);
        }

        private Vector3 GetRandomSpherePosition()
        {
            Vector3 pos = ClampDistance(Random.insideUnitSphere * 75f, 50f, 75f);
            pos.z = 0;
            return pos;
        }

        private Vector3 ClampDistance(Vector3 vector, float min, float max)
        {
            float magnitude = vector.magnitude;

            if (magnitude < min)
            {
                return vector.normalized * min;
            }
            else if (magnitude > max)
            {
                return vector.normalized * max;
            }
            else
            {
                return vector;
            }
        }

        private void Hide()
        {
            ObjectPoolManager.Instance.GetObject(PoolName.AudioShot).GetComponent<AudioShot>().Play(_gameManager.SoundData.GetBoule());

            gameObject.SetActive(false);

            _onAnimationFinished?.Invoke();
        }
    }
}