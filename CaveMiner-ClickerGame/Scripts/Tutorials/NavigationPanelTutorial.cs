using System.Collections;
using UnityEngine;
using CaveMiner.UI;

namespace CaveMiner
{
    public class NavigationPanelTutorial : TutorialBase
    {
        [SerializeField] private UITutorialTab _navigationPanel;

        protected override IEnumerator Tutorial()
        {
            _navigationPanel.ShowAnimation();

            yield return new WaitForSeconds(1f);

            CompleteTutorial();
        }
    }
}