using System.Collections.Generic;
using UnityEngine;

namespace CaveMiner
{
    public static class AnalyticEventKey
    {
        public const string UPGRADE_CAVE = "uprade_cave";
        public const string REBIRTH = "rebirth";
        public const string KEY_COUNT = "key_count";
        public const string OPEN_CHEST = "open_chest";
        public const string UPDATE_GAME_SHOP = "update_game_shop";
        public const string PLACE_RUNE = "place_rune";
        public const string UPGRADE_RUNE = "upgrade_rune";
        public const string DESTROY_RUNE = "destroy_rune";
        public const string UPGRADE_PICKAXE_SPEED = "upgrade_pickaxe_speed";
        public const string UPGRADE_PICKAXE_WEALTH = "upgrade_pickaxe_wealth";
        public const string UPGRADE_PICKAXE_EXPERIENCE = "upgrade_pickaxe_experience";
        public const string UPGRADE_PICKAXE_AUTO_CLICK = "upgrade_pickaxe_auto_click";
        public const string IN_APP_PURCHASE = "in_app_purchase";
        public const string USE_DYNAMITE = "use_dynamite";
        public const string REVIEW_STARS = "review_stars";
        public const string CHANGE_KEY = "change_key";
        public const string REBIRTH_UPGRADE = "rebirth_upgrade";
        public const string COMBINE_RUNES = "combine_runes";
        public const string START_GAME_GRAPHICS = "start_game_graphics";
        public const string BUY_GAME_SHOP_ITEM = "buy_game_shop_item";
        public const string COLLECT_BONUS_BOX = "collect_bonus_box";
        public const string WATCH_REWARDED_AD = "watch_rewarded_ad";
    }

    public class AmplitudeManager : Singleton<AmplitudeManager>
    {
        private const string AMPLITUDE_API_KEY = "EXAMPLE PROJECT";
        private Amplitude _amplitude;

        protected override void Awake()
        {
            base.Awake();

#if !DEVELOPMENT_BUILD
            _amplitude = Amplitude.getInstance();
            _amplitude.logging = true;
            _amplitude.enableCoppaControl();
            _amplitude.trackSessionEvents(true);
            _amplitude.init(AMPLITUDE_API_KEY);

            Debug.Log("Amplitude Initialization");
#endif
        }

        public void Event(string id)
        {
#if !DEVELOPMENT_BUILD
            _amplitude.logEvent(id);
#endif
        }

        public void Event(string id, Dictionary<string, object> properties)
        {
#if !DEVELOPMENT_BUILD
            _amplitude.logEvent(id, properties);
#endif
        }

        public void EventRevenue(string productId, int count, double price)
        {
#if !DEVELOPMENT_BUILD
            _amplitude.logRevenue(productId, count, price);
#endif
        }
    }
}