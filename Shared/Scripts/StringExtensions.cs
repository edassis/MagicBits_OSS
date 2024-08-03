using UnityEngine;

namespace MagicBits_OSS.Shared.Scripts
{
    /// <summary>
    /// Utilitário para formatar strings.
    /// Fonte: https://forum.unity.com/threads/easy-text-format-your-debug-logs-rich-text-format.906464/
    /// </summary>
    public static class StringExtensions
    {
        public static string Bold(this string str) => "<b>" + str + "</b>";
        
        public static string Color(this string str, string clr)
        {
            if (Application.platform != RuntimePlatform.WebGLPlayer)
            {
                return string.Format("<color={0}>{1}</color>", clr, str);
            }

            return str;
        }

        public static string Italic(this string str)
        {
            if (Application.platform != RuntimePlatform.WebGLPlayer)
            {
                return "<i>" + str + "</i>";
            }

            return str;
        }
        public static string Size(this string str, int size)
        {
            if (Application.platform != RuntimePlatform.WebGLPlayer)
            {
                return string.Format("<size={0}>{1}</size>", size, str);
            }

            return str;
        }
    }
}
