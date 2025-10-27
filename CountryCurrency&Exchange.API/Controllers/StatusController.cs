using CountryCurrency_Exchange.API.Model.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CountryCurrency_Exchange.API.Controllers
{
    [ApiController]
    [Route("status")]
    public class StatusController : ControllerBase
    {
        private readonly CountryCurrencyDbContext _context;

        public StatusController(CountryCurrencyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetStatus()
        {
            var total = await _context.Countries.CountAsync();
            var last = await _context.Countries
                .OrderByDescending(c => c.LastRefreshedAt)
                .Select(c => c.LastRefreshedAt)
                .FirstOrDefaultAsync();

            return Ok(new
            {
                total_countries = total,
                last_refreshed_at = last.ToString("yyyy-MM-ddTHH:mm:ssZ")
            });
        }
    }
}
