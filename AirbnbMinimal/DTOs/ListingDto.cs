namespace AirbnbMinimal.DTOs;

public class ListingDto
{
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public int MaxGuests { get; set; }
        public int RoomCount { get; set; }
        public int BedCount { get; set; }
        public int BathroomCount { get; set; }
        public string FullAddress { get; set; } = string.Empty;
        public string LocationMap { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Neighborhood  { get; set; } = string.Empty; 
        
        public List<DateRangeDto> DateList { get; set; } = [];
        // public DateTime StartedDate { get; set; }
        // public DateTime EndDate { get; set; }
}

public class DateRangeDto
{
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
}