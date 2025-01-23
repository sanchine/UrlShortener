namespace UrlShortener.Models
{
    public class UrlMap
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public string LongUrl { get; set; } = string.Empty;
        public string ShortUrl { get; set; } = string.Empty;
        public int ClicksCount { get; set; }

        public UrlMap()
        {}
    }
}