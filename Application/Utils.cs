using System.Text.RegularExpressions;

namespace Application;

public static class Utils
{
    public static bool IsBase64String(string base64)
    {
        if (string.IsNullOrWhiteSpace(base64))
            return false;

        base64 = base64.Trim();

        if (base64.Length % 4 != 0)
            return false;

        if (!Regex.IsMatch(base64, @"^[a-zA-Z0-9\+/]*={0,2}$", RegexOptions.None))
            return false;

        try
        {
            _ = Convert.FromBase64String(base64);
            return true;
        }
        catch
        {
            return false;
        }
    }
}