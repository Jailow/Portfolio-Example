using System.Collections;
using UnityEngine;
using CaveMiner.UI;

namespace CaveMiner
{
    public class TopPanelTutorial : TutorialBase
    {
        [SerializeField] private UITutorialTab _topPanels;

        protected override IEnumerator Tutorial()
        {
            _topPanels.ShowAnimation();

            yield return null;

            CompleteTutorial();
        }
    }
}