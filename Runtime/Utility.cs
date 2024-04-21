using System;
using System.Text;

namespace WiSdom
{
    public static class Utility
    {
        // Helper method to generate a more secure file name
        public static string GetSecureFileName(string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(bytes);
        }

        // Convert a Base64 string back to a normal string
        public static string DecodeFileName(string input)
        {
            byte[] bytes = Convert.FromBase64String(input);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}