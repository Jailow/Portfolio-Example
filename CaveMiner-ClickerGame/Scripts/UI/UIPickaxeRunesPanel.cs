using UnityEngine;

namespace CaveMiner.UI
{
    public class UIPickaxeRunesPanel : MonoBehaviour
    {
        [SerializeField] private UIPickaxeRuneSlot[] _runeSlots;

        public void Init(GameManager gameManager, UIManager uiManager)
        {
            for(int i = 0; i < _runeSlots.Length; i++)
            {
                _runeSlots[i].Init(gameManager, uiManager, i);
            }
        }

        public void UpdateSlots()
        {
            foreach(var rune in _runeSlots)
            {
                rune.UpdateSlotVisual();
            }
        }
    }
}