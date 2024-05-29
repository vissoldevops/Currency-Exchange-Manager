using System.ComponentModel.DataAnnotations;

namespace CurrencyExchangeManager.Api.Models
{
    public class BaseModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
