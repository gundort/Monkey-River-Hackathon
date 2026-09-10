using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs
{
    public class MonitoredDestinationDto
    {
        [Required]
        public required string Location { get; set; }

        [Required]
        public required string RiskLevel { get; set; }

        [Required]
        public DateTime LastChecked { get; set; }
    }
}
