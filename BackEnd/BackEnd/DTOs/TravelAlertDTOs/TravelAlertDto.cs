using System;

namespace BackEnd.DTOs.TravelAlertDTOs
{
    public class TravelAlertDto
    {
            public string Id { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public DateTime Timestamp { get; set; }
    }

}

