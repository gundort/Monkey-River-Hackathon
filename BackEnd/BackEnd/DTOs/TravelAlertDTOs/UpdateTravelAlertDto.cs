using System;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs.TravelAlertDTOs
{
    public class UpdateTravelAlertDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; } = string.Empty;
    }
    
}

