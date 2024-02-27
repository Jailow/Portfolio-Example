using System;
using UnityEngine;

namespace CaveMiner.Helpers
{
    public class ByteConverter : MonoBehaviour
    {
        public static string ToString(byte[] bytes)
        {
            return string.Join(string.Empty, Array.ConvertAll(bytes, b => b.ToString("X2")));
        }
    }
}