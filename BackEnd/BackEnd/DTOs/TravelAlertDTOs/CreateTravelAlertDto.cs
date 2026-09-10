using System;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs.TravelAlertDTOs
{
    public class CreateTravelAlertDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status is required")]
        [RegularExpression("^(Active|Resolved|Warning)$", ErrorMessage = "Status must be Active, Resolved, or Warning")]
        public string Status { get; set; } = "Active";
    }    
}


