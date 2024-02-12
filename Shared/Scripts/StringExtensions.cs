namespace MagicBits_OSS.Shared.Scripts
{
    /// <summary>
    /// Utilitário para formatar strings.
    /// Fonte: https://forum.unity.com/threads/easy-text-format-your-debug-logs-rich-text-format.906464/
    /// </summary>
    public static class StringExtensions
    {
        public static string Bold(this string str) => "<b>" + str + "</b>";
        
        // TODO: Não formatar para plataformas que não suportam (Web).
        public static string Color(this string str, string clr) => string.Format("<color={0}>{1}</color>", clr, str);
        public static string Italic(this string str) => "<i>" + str + "</i>";
        public static string Size(this string str, int size) => string.Format("<size={0}>{1}</size>", size, str);
    }
}
