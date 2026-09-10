using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs
{
    public class DiagnosticTestDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Result { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }
}
