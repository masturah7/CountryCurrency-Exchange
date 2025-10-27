using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CountryCurrency_Exchange.API.Model.CountryModel
{
    public class Country
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Capital { get; set; } = string.Empty;

        public string Region { get; set; } = string.Empty;

        [Required]
        public long Population { get; set; }

        [Required]
        [Column("currency_code")]
        public string CurrencyCode { get; set; } = string.Empty;

        [Required]
        [Column("exchange_rate", TypeName = "decimal(18,2)")]
        public decimal ExchangeRate { get; set; }

        [Column("estimated_gdp", TypeName = "decimal(18,2)")]
        public decimal EstimatedGdp { get; set; }

        public string FlagUrl { get; set; } = string.Empty;

        public DateTime LastRefreshedAt { get; set; } = DateTime.UtcNow;


    }
}
