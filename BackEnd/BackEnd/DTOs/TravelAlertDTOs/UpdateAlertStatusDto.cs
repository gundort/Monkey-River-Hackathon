using System;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs.TravelAlertDTOs
{
    
    public class UpdateAlertStatusDto
    {
        [Required(ErrorMessage = "Status is required")]
        [RegularExpression("^(Active|Resolved|Warning)$", ErrorMessage = "Status must be Active, Resolved, or Warning")]
        public string Status { get; set; } = string.Empty;
    }
        
}
