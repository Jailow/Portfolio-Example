using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using CaveMiner.Helpers;
using System.Collections;
using System.Linq;

namespace CaveMiner.UI
{
    public class UIBonusBox : MonoBehaviour
    {
        [SerializeField] private int _minResourceCount;
        [SerializeField] private int _maxResourceCount;
        [SerializeField] private Material _material;
        [SerializeField] private Image _shine;
        [SerializeField] private Image _secondShine;
        [SerializeField] private Transform _box;

        public RectTransform RectTr { get; private set; }

        private Animator _anim;
        private Tween _boxTween;
        private Tween _shineTween;
        private UIManager _uiManager;
        private GameManager _gameManager;
        private Button _btn;
        private Color _clearColor;
        private Tween _inventoryButtonTween;

        public void Init(GameManager gameManager, UIManager uiManager)
        {
            _gameManager = gameManager;
            _uiManager = uiManager;

            _anim = GetComponent<Animator>();
            RectTr = GetComponent<RectTransform>();
            _btn = GetComponent<Button>();

            _btn.onClick.AddListener(() =>
            {
                StartCoroutine(Collect());
            });

            _clearColor = new Color(1f, 1f, 1f, 0f);
        }

        private void PlayInventoryButtonAnimation()
        {
            _inventoryButtonTween?.Complete();
            _inventoryButtonTween = _uiManager.NavigationPanel.InventoryIconTr.DOShakeScale(0.12f, 0.15f, 5, 40, true, ShakeRandomnessMode.Harmonic);
        }

        private IEnumerator Collect()
        {
            var currentCave = _gameManager.Caves.FirstOrDefault(e => e.Id == _gameManager.GameState.PlayerState.CurrentCave);

            if (currentCave == null)
                Hide();

            _btn.enabled = false;

            _shine.gameObject.SetActive(false);
            _secondShine.gameObject.SetActive(false);
            _box.gameObject.SetActive(false);

            for (int i = 0; i < Random.Range(_minResourceCount, _maxResourceCount + 1); i++)
            {
                var cubeParticle = ObjectPoolManager.Instance.GetObject(PoolName.BreakBlockCube)?.GetComponent<CubeParticle>();
                if (cubeParticle != null)
                {
                    if (!cubeParticle.IsInitialized)
                        cubeParticle.Init(_gameManager, _uiManager);

                    var blockData = currentCave.BlocksDatas[Random.Range(0, currentCave.BlocksDatas.Length)].BlockData;
                    cubeParticle.PlayAnimation(blockData.BreakBlockParticleMaterial, RectTr.position, _uiManager.NavigationPanel.InventoryIconTr.position, PlayInventoryButtonAnimation);

                    if (blockData.ResourceItemData != null)
                    {
                        _gameManager.AddResourceItem(blockData.ResourceItemData.Id, 1);
                    }

                }

                yield return new WaitForFixedUpdate();
            }

            AmplitudeManager.Instance.Event(AnalyticEventKey.COLLECT_BONUS_BOX);

            Hide();
        }

        public void Show()
        {
            _btn.enabled = true;
            _anim.enabled = true;

            _shine.gameObject.SetActive(true);
            _secondShine.gameObject.SetActive(true);
            _box.gameObject.SetActive(true);
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private IEnumerator HideDelay()
        {
            yield return new WaitForSeconds(6f);

            _btn.enabled = false;
            _anim.enabled = false;
            RectTr.DOScale(0f, 0.5f).SetEase(Ease.InBounce);

            yield return new WaitForSeconds(0.5f);

            Hide();
        }

        private void OnEnable()
        {
            _boxTween?.Kill();
            _shineTween?.Kill();

            _shine.color = _clearColor;
            _secondShine.color = _clearColor;
            _box.localScale = CachedVector3.Zero;

            _shine.DOFade(1f, 0.3f);
            _secondShine.DOFade(1f, 0.3f);
            _box.DOScale(1f, 0.3f).SetEase(Ease.OutBounce);

            _box.localEulerAngles = CachedVector3.Back * 17.5f;

            _boxTween = DOTween.Sequence()
                .Append(_box.DOLocalRotate(CachedVector3.Forward * 17.5f, 1f).SetEase(Ease.InOutSine))
                .Append(_box.DOLocalRotate(CachedVector3.Back * 17.5f, 1f).SetEase(Ease.InOutSine))
                .SetLoops(-1);

            _shineTween = _shine.transform.DOLocalRotate(CachedVector3.Forward * 360f, 5f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);

            StartCoroutine(HideDelay());
        }

        private void OnDisable()
        {
            _boxTween?.Kill();
            _shineTween?.Kill();

            StopAllCoroutines();
        }
    }
}