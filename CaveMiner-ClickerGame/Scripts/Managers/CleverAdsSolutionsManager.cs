using CAS;
using CAS.AdObject;
using System;
using UnityEngine;

namespace CaveMiner
{
    public class CleverAdsSolutionsManager : Singleton<CleverAdsSolutionsManager>
    {
        [SerializeField] private float _showInterstitialEverySeconds;

        private ManagerAdObject _managerAdObject;
        private InterstitialAdObject _interstitialAd;
        private RewardedAdObject _rewardedAd;

        private Action _onReward;
        private float _time;
        private bool _interstitialShowSoonNotificated;
        private bool _isInitialized;
        private bool _enableTimer;

        public bool RewardedAdReady => _rewardedAd.isAdReady;

        public bool EnableTimer
        {
            set { _enableTimer = value; }
        }

        public bool NoAds { get; set; }

        public Action onInterstitialShowSoonStarting;
        public Action onInterstitialShowSoonEnded;
       

        protected override void Awake()
        {
            base.Awake();

            _managerAdObject = GetComponent<ManagerAdObject>();
            _interstitialAd = GetComponentInChildren<InterstitialAdObject>();
            _rewardedAd = GetComponentInChildren<RewardedAdObject>();

            _rewardedAd.OnReward.AddListener(OnReward);
            _managerAdObject.OnInitialized.AddListener(OnInitialized);
        }

        private void Update()
        {
            if (!_enableTimer || !_isInitialized || NoAds)
                return;

            _time += Time.deltaTime;

            if(!_interstitialShowSoonNotificated && _time >= _showInterstitialEverySeconds - 3f)
            {
                _interstitialShowSoonNotificated = true;
                onInterstitialShowSoonStarting?.Invoke();
            }

            if(_time >= _showInterstitialEverySeconds)
            {
                _time = 0;
                ShowInterstitial();
            }
        }

        private void OnInitialized()
        {
            _enableTimer = true;
            _isInitialized = true;
        }

        private void OnReward()
        {
            _onReward?.Invoke();
        }

        public void ShowRewarded(Action onReward)
        {
            if (!_rewardedAd.isAdReady)
                return;

            _onReward = onReward;
            _rewardedAd.Present();
        }

        public void ShowInterstitial()
        {
            _interstitialShowSoonNotificated = false;
            onInterstitialShowSoonEnded?.Invoke();

            if (!_interstitialAd.isAdReady)
               return;

            _interstitialAd.Present(); 
        }
    }
}