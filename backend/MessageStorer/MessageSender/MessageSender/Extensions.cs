namespace MessageSender
{
    public static class Extensions
    {
        public static bool ContainsOnlyDigits(this string str, int startIndex = 0)
        {
            for (int i = startIndex; i < str.Length; i++)
            {
                if (str[i] < '0' || str[i] > '9')
                    return false;
            }
            return true;
        }
    }
}
