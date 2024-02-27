using CaveMiner.UI;
using System.Collections;
using UnityEngine;

namespace CaveMiner
{
    public class SettingsButtonTutorial : TutorialBase
    {
        [SerializeField] private UITutorialTab _settingsButton;
        [SerializeField] private UITutorialTab _leaderboardsButton;

        protected override IEnumerator Tutorial()
        {
            _settingsButton.ShowAnimation();

            yield return new WaitForSeconds(1f);

            _leaderboardsButton.ShowAnimation();

            CompleteTutorial();
        }
    }
}