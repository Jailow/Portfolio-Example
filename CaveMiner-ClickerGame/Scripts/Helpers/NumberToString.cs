using System.Globalization;
using System.Numerics;
using UnityEngine;

namespace CaveMiner.Helpers
{
    public class NumberToString : MonoBehaviour
    {
        private static string[] _abbreviations = new string[]
        {
            "K",
            "M",
            "B",
            "t",
            "q",
            "Q",
            "s",
            "S",
            "o",
            "n",
            "d",
            "U",
            "D",
            "T",
            "Qt",
            "Qd",
            "Sd",
            "St",
            "O",
            "N",
            "v",
            "c",
        };


        public static string Convert(double num)
        {
            string numString = string.Empty;
            int indexOfDot = -1;
            if (num < 1000)
            {
                numString = num.ToString("0.##", CultureInfo.InvariantCulture);
                indexOfDot = numString.IndexOf('.');
                if (indexOfDot != -1 && numString.Length > indexOfDot + 3)
                {
                    numString = numString.Substring(0, indexOfDot + 3);
                }

                return numString;
            }

            int step = -1;
            while(num >= 1000)
            {
                num /= 1000;
                step++;

                if (num < 1000)
                    break;
            }

            numString = num.ToString("0.##", CultureInfo.InvariantCulture);
            indexOfDot = numString.IndexOf('.');
            if (indexOfDot != -1 && numString.Length > indexOfDot + 3)
            {
                numString = numString.Substring(0, indexOfDot + 3);
            }

            return $"{numString}{_abbreviations[step]}";
        }
    }
}