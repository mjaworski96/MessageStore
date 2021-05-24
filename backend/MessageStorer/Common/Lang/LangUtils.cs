namespace Common.Lang
{
    public static class LangUtils
    {
        public static string Translate(this string src)
        {
            var translated = Lang.ResourceManager.GetString(src);
            if (string.IsNullOrEmpty(translated))
            {
                return src;
            }
            return translated;
        }
    }
}
