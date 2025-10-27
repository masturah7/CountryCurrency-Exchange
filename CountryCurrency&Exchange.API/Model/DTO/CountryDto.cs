using System.Text.Json.Serialization;

namespace CountryCurrency_Exchange.API.Model.DTOs
{
    public class CountryDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("capital")]
        public string Capital { get; set; } = string.Empty;

        [JsonPropertyName("region")]
        public string Region { get; set; } = string.Empty;

        [JsonPropertyName("population")]
        public long Population { get; set; }

        [JsonPropertyName("currency_code")]
        public string CurrencyCode { get; set; } = string.Empty;

        [JsonPropertyName("exchange_rate")]
        public decimal? ExchangeRate { get; set; }

        [JsonPropertyName("estimated_gdp")]
        public decimal? EstimatedGdp { get; set; }

        [JsonPropertyName("flag_url")]
        public string FlagUrl { get; set; } = string.Empty;

        [JsonPropertyName("last_refreshed_at")]
        public DateTime LastRefreshedAt { get; set; } = DateTime.UtcNow;
    }
}
