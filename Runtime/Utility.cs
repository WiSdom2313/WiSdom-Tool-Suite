using System;
using System.Text;
using UnityEngine;

namespace WiSdom
{
    public static class Utility
    {
        // Helper method to generate a more secure file name
        public static string EncodeStringToByte(string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            string encodedString = Convert.ToBase64String(bytes);

            return encodedString;
        }

        // Convert a Base64 string back to a normal string
        public static string DecodeByteFromString(string input)
        {
            Debug.Log("DecodeByteFromString: " + input);
            byte[] bytes = Convert.FromBase64String(input);
            string decodedString = Encoding.UTF8.GetString(bytes);

            return decodedString;
        }
    }
}