using CountryCurrency_Exchange.API.Model.Context;
using CountryCurrency_Exchange.API.Model.CountryModel;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Drawing;

namespace CountryCurrency_Exchange.API.Services
{
    public class CountryService
    {
        private readonly CountryCurrencyDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly Random _random = new();

        public CountryService(CountryCurrencyDbContext context, HttpClient httpClient, IConfiguration config)
        {
            _context = context;
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<List<Country>> RefreshCountriesAsync()
        {
            string countriesUrl = "https://restcountries.com/v2/all?fields=name,capital,region,population,flag,currencies";
            string exchangeUrl = "https://open.er-api.com/v6/latest/USD";

            try
            {
                // Fetch data from both APIs
                var countriesResponse = await _httpClient.GetStringAsync(countriesUrl);
                var exchangeResponse = await _httpClient.GetStringAsync(exchangeUrl);

                var countriesData = JArray.Parse(countriesResponse);
                var exchangeData = JObject.Parse(exchangeResponse)["rates"];

                if (countriesData == null || exchangeData == null)
                    throw new Exception("Failed to fetch external data");

                // Clear old data to prevent duplicates
                _context.Countries.RemoveRange(_context.Countries);
                await _context.SaveChangesAsync();

                foreach (var item in countriesData)
                {
                    string name = item["name"]?.ToString() ?? "Unknown";
                    long population = item["population"]?.Value<long>() ?? 0;
                    string capital = item["capital"]?.ToString() ?? "";
                    string region = item["region"]?.ToString() ?? "";
                    string flagUrl = item["flag"]?.ToString() ?? "";

                    var currency = item["currencies"]?.FirstOrDefault();
                    string currencyCode = currency?["code"]?.ToString() ?? "USD";

                    // Get exchange rate (default to 1 if not found)
                    decimal exchangeRate = exchangeData?[currencyCode]?.Value<decimal>() ?? 1;

                    // GDP = population * random(1000-2000) ÷ exchange_rate
                    decimal estimatedGdp = (exchangeRate > 0)
                        ? (population * _random.Next(1000, 2000)) / exchangeRate
                        : 0;

                    var newCountry = new Country
                    {
                        Name = name,
                        Capital = capital,
                        Region = region,
                        Population = population,
                        CurrencyCode = currencyCode,
                        ExchangeRate = exchangeRate,
                        EstimatedGdp = estimatedGdp,
                        FlagUrl = flagUrl,
                        LastRefreshedAt = DateTime.UtcNow
                    };

                    _context.Countries.Add(newCountry);
                }

                await _context.SaveChangesAsync();

                // Generate summary image after refreshing
                await GenerateSummaryImageAsync();

                return await _context.Countries.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"External data source unavailable: {ex.Message}");
            }
        }

        public async Task GenerateSummaryImageAsync()
        {
            var total = await _context.Countries.CountAsync();
            var top5 = await _context.Countries
                .OrderByDescending(c => c.EstimatedGdp)
                .Take(5)
                .ToListAsync();

            // Create simple summary image
            using var bmp = new Bitmap(600, 400);
            using var graphics = Graphics.FromImage(bmp);
            graphics.Clear(Color.White);
            var font = new Font("Arial", 14);

            graphics.DrawString($"Total Countries: {total}", font, Brushes.Black, 20, 20);
            graphics.DrawString("Top 5 by GDP:", font, Brushes.Black, 20, 60);

            int y = 100;
            foreach (var c in top5)
            {
                graphics.DrawString($"{c.Name} - {c.EstimatedGdp:F2}", font, Brushes.Black, 40, y);
                y += 30;
            }

            graphics.DrawString($"Last Refresh: {DateTime.UtcNow}", font, Brushes.Gray, 20, y + 20);

            Directory.CreateDirectory("cache");
            bmp.Save("cache/summary.png", System.Drawing.Imaging.ImageFormat.Png);
        }
    }
}
