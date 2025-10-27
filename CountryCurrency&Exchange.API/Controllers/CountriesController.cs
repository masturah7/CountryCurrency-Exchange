using CountryCurrency_Exchange.API.Model.CountryModel;
using CountryCurrency_Exchange.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CountryCurrency_Exchange.API.Model.Context;

namespace CountryCurrency_Exchange.API.Controllers
{
    [ApiController]
    [Route("countries")]
    public class CountriesController : ControllerBase
    {
        private readonly CountryService _countryService;
        private readonly CountryCurrencyDbContext _context;

        public CountriesController(CountryService countryService, CountryCurrencyDbContext context)
        {
            _countryService = countryService;
            _context = context;
        }

        // ✅ POST /countries/refresh
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshCountries()
        {
            try
            {
                var countries = await _countryService.RefreshCountriesAsync();
                return Ok(new { message = "Countries refreshed successfully", total = countries.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(503, new { error = "External data source unavailable", details = ex.Message });
            }
        }

        // ✅ GET /countries
        [HttpGet]
        public async Task<IActionResult> GetCountries(
            [FromQuery] string? region,
            [FromQuery] string? currency,
            [FromQuery] string? sort)
        {
            var query = _context.Countries.AsQueryable();

            if (!string.IsNullOrEmpty(region))
                query = query.Where(c => c.Region.ToLower() == region.ToLower());

            if (!string.IsNullOrEmpty(currency))
                query = query.Where(c => c.CurrencyCode.ToLower() == currency.ToLower());

            // Sorting by GDP or Population
            if (!string.IsNullOrEmpty(sort))
            {
                sort = sort.ToLower();
                if (sort == "gdp")
                    query = query.OrderByDescending(c => c.EstimatedGdp);
                else if (sort == "population")
                    query = query.OrderByDescending(c => c.Population);
            }

            var result = await query.ToListAsync();
            return Ok(result);
        }

        // ✅ GET /countries/{name}
        [HttpGet("{name}")]
        public async Task<IActionResult> GetCountry(string name)
        {
            var country = await _context.Countries.FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
            if (country == null)
                return NotFound(new { error = "Country not found" });

            return Ok(country);
        }

        // ✅ DELETE /countries/{name}
        [HttpDelete("{name}")]
        public async Task<IActionResult> DeleteCountry(string name)
        {
            var country = await _context.Countries.FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
            if (country == null)
                return NotFound(new { error = "Country not found" });

            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"{name} deleted successfully" });
        }

        // ✅ GET /countries/image
        [HttpGet("image")]
        public IActionResult GetSummaryImage()
        {
            var imagePath = Path.Combine("cache", "summary.png");
            if (!System.IO.File.Exists(imagePath))
                return NotFound(new { error = "Image not found. Please refresh countries first." });

            var imageBytes = System.IO.File.ReadAllBytes(imagePath);
            return File(imageBytes, "image/png");
        }
    }
}
