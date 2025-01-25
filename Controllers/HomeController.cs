using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Data;
using UrlShortener.Models;

namespace UrlShortener.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private ApplicationDbContext _db;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    public IActionResult Index()
    {
        var urls = _db.UrlMaps.OrderByDescending(u => u.Created).ToList();
        var viewModel = new UrlMapViewModel
        {
            Urls = urls,
            ErrorMessage = string.Empty,
            NewUrl = string.Empty
        };
        return View(viewModel);
    }

    [Authorize]
    public IActionResult RedirectByShortenUrl(string ShortUrl)
    {
        var targetUrl = _db.UrlMaps.FirstOrDefault(item => item.ShortUrl == ShortUrl);
        
        targetUrl.ClicksCount++;
        _db.SaveChanges();

        return Redirect(targetUrl.LongUrl);
    }

    [Authorize]
    [HttpPost]
    public IActionResult ShortenUrl(UrlMapViewModel viewModel)
    {
        Uri uri;
        var isUrlValid = Uri.TryCreate(viewModel.NewUrl, UriKind.Absolute, out uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

        if (!isUrlValid)
        {
            viewModel.ErrorMessage = "URL is not valid";
            return View("Index", viewModel);
        }

        var isUrlExists = _db.UrlMaps.Any(item => item.LongUrl == viewModel.NewUrl);

        if (isUrlExists)
        {
            viewModel.ErrorMessage = "URL is already exists";
            return View("Index", viewModel);
        }

        var shortUrl = string.Empty;

        do
        {
            shortUrl = UrlShortenator.GenerateShortUrl(viewModel.NewUrl);

            var isShortUrlExistsInDb = _db.UrlMaps.Any(item => item.ShortUrl == shortUrl);

            if (!isShortUrlExistsInDb) break;

        } while (true);

        var url = new UrlMap
        {
            ShortUrl = "do.main/" + shortUrl,
            LongUrl = viewModel.NewUrl,
            Created = DateTime.UtcNow,
            ClicksCount = 0
        };

        _db.UrlMaps.Add(url);
        _db.SaveChanges();

        return View("Index", viewModel);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
