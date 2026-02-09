using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace PdfTemplateSystem.Services;

public class PdfGenerationService
{
    private static IBrowser? _browser;
    private static readonly SemaphoreSlim _browserLock = new(1, 1);
    private readonly ILogger<PdfGenerationService> _logger;

    public PdfGenerationService(ILogger<PdfGenerationService> logger)
    {
        _logger = logger;
    }

    public async Task<byte[]> GeneratePdfFromHtmlAsync(string html)
    {
        var browser = await GetBrowserAsync();
        await using var page = await browser.NewPageAsync();

        await page.SetContentAsync(html);

        var pdfOptions = new PdfOptions
        {
            Format = PaperFormat.A4,
            PrintBackground = true,
            MarginOptions = new MarginOptions
            {
                Top = "1cm",
                Bottom = "1cm",
                Left = "1cm",
                Right = "1cm"
            }
        };

        var pdfBytes = await page.PdfDataAsync(pdfOptions);
        await page.CloseAsync();

        return pdfBytes;
    }

    private async Task<IBrowser> GetBrowserAsync()
    {
        if (_browser != null && _browser.IsConnected)
        {
            return _browser;
        }

        await _browserLock.WaitAsync();
        try
        {
            if (_browser != null && _browser.IsConnected)
            {
                return _browser;
            }

            _logger.LogInformation("Downloading Chromium browser...");
            
            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();

            _logger.LogInformation("Launching browser...");
            
            _browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" }
            });

            _logger.LogInformation("Browser launched successfully");

            return _browser;
        }
        finally
        {
            _browserLock.Release();
        }
    }

    public static async Task DisposeBrowserAsync()
    {
        if (_browser != null)
        {
            await _browser.CloseAsync();
            _browser.Dispose();
            _browser = null;
        }
    }
}
