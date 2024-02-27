using UnityEngine;

namespace CaveMiner.Helpers
{
    public class Timestamp : MonoBehaviour
    {
        public static string ToHoursTimer(int timestamp)
        {
            int hours = timestamp / 3600; // 3600 ������ � ����
            int minutes = (timestamp % 3600) / 60; // 60 ������ � ������
            int seconds = timestamp % 60; // ������� - �������

            string timeString = string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
            return timeString;
        }
    }
}