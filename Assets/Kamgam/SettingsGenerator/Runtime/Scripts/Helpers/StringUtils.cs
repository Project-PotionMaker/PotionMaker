namespace Kamgam.SettingsGenerator
{
    public static class StringUtils
    {
        public static string InsertSpaceBeforeUpperCase(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Regular expression to match an uppercase letter that is not
            // preceded by a number or another uppercase letter
            string pattern = @"(?<![\dA-Z])([A-Z])";

            return System.Text.RegularExpressions.Regex.Replace(input, pattern, " $1");
        }
    }
}