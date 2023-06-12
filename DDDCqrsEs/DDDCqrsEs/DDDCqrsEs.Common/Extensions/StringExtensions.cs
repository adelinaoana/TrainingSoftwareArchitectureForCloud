using System.Text.RegularExpressions;

namespace DDDCqrsEs.Common.Extensions
{   
    public static class StringUtils
    {
        public static bool LowerStartsWith(string searchText, string text)
        {
            return text.ToLower().StartsWith(searchText);
        }
        public static bool MatchPasswordComplexity(this string password)
        {
            return Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\$%\^&\*])(?=.{8,})");
        }
    }
}
