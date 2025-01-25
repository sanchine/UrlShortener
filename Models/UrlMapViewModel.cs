namespace UrlShortener.Models
{
    public class UrlMapViewModel
    {
        public List<UrlMap> Urls {get; set;}
        public string NewUrl {get; set;} = string.Empty;
        public string ErrorMessage {get; set;} = string.Empty;
    }
}