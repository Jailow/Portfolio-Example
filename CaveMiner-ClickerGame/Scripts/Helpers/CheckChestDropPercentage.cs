using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace CaveMiner.Editor
{
    public class CheckChestDropPercentage : MonoBehaviour
    {
        [MenuItem("Helpers/Get Chest Drop Percentage/Common", priority = 1)]
        private static void CommonChestCheck()
        {
            var gameData = Resources.Load<GameData>("GameData");
            CalculatePercent(gameData.CommonChestDrops.ItemDrops);
        }

        [MenuItem("Helpers/Get Chest Drop Percentage/Rare", priority = 2)]
        private static void RareChestCheck()
        {
            var gameData = Resources.Load<GameData>("GameData");
            CalculatePercent(gameData.RareChestDrops.ItemDrops);
        }

        [MenuItem("Helpers/Get Chest Drop Percentage/Epic", priority = 3)]
        private static void EpicChestCheck()
        {
            var gameData = Resources.Load<GameData>("GameData");
            CalculatePercent(gameData.EpicChestDrops.ItemDrops);
        }

        [MenuItem("Helpers/Get Chest Drop Percentage/Mythical", priority = 4)]
        private static void MythicalChestCheck()
        {
            var gameData = Resources.Load<GameData>("GameData");
            CalculatePercent(gameData.MythicalChestDrops.ItemDrops);
        }

        [MenuItem("Helpers/Get Chest Drop Percentage/Legendary", priority = 5)]
        private static void LegendaryChestCheck()
        {
            var gameData = Resources.Load<GameData>("GameData");
            CalculatePercent(gameData.LegendaryChestDrops.ItemDrops);
        }

        private static void CalculatePercent(ItemData[] itemDatas)
        {
            float[] normalizedValue = new float[itemDatas.Length];
            float valueSum = 0f;
            for(int i = 0; i < itemDatas.Length; i++)
            {
                normalizedValue[i] = 1f / itemDatas[i].Value;
                valueSum += normalizedValue[i];
            }

            string finalText = string.Empty;

            for (int i = 0; i < itemDatas.Length; i++)
            {
                finalText += $"{I2.Loc.LocalizationManager.GetTranslation($"Items/{itemDatas[i].Id}")} - {(normalizedValue[i] / valueSum) * 100f}\n";
            }

            Debug.Log(finalText);
        }
    }
}
#endif