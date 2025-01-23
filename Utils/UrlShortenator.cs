

using System.Security.Cryptography;
using System.Text;
using UrlShortener.Data;

public static class UrlShortenator
{
    private const string CharSet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const int ShortUrlLength = 6;

    public static string GenerateShortUrl(string OriginalUrl)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(OriginalUrl));
            ulong hashValue = BitConverter.ToUInt64(hashBytes, 0);
            string shortUrl = ConvertToBase62(hashValue);
            shortUrl = shortUrl.Substring(0, Math.Min(ShortUrlLength, shortUrl.Length));
            return shortUrl;
        }
    }

    private static string ConvertToBase62(ulong value)
    {
        StringBuilder sb = new StringBuilder();
        do
        {
            ulong remainder = value % 62;
            sb.Insert(0, CharSet[(int) remainder]);
            value /= 62;
        } while (value > 0);
        return sb.ToString();
    }
}