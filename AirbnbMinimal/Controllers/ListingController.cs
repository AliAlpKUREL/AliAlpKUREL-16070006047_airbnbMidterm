using AirbnbMinimal.DbOperations;
using AirbnbMinimal.Models;
using AirbnbMinimal.DTOs;
using AirbnbMinimal.Enums;
using AirbnbMinimal.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AirbnbMinimal.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/[controller]s")]
public class ListingController : ControllerBase
{
    private readonly WebApiContext _dbContext;
    private readonly JwtToken _jwtToken;

    public ListingController(WebApiContext dbContext, JwtToken jwtToken)
    {
        _dbContext = dbContext;
        _jwtToken = jwtToken;
    }

    [HttpPost("AddListing")]
    [Authorize(Roles = "Host")]
    public async Task<IResult> AddListing([FromBody] AddListingDto model)
    {
        
        var currentUserId =  _jwtToken.GetUserId(User);
        if (currentUserId == -1)
            return Results.Forbid();
        
        var location = await _dbContext.Locations.FindAsync(model.LocationId);

        if (location == null)
            return Results.BadRequest("Invalid location ID.");

        var listing = new Listing
        {
            Title = model.Title,
            Description = model.Description,
            PricePerNight = model.PricePerNight,
            MaxGuests = model.MaxGuests,
            RoomCount = model.RoomCount,
            BedCount = model.BedCount,
            BathroomCount = model.BathroomCount,
            FullAddress = model.FullAddress,
            LocationMap = model.LocationMap,
            LocationId = model.LocationId,
            StartedDate = model.StartedDate,
            EndDate = model.EndDate,
            CreatedDate = DateTime.UtcNow,
            UserId = currentUserId,
            CreatedVersion = "1.0"
        };

        _dbContext.Listings.Add(listing);
        await _dbContext.SaveChangesAsync();

        return Results.Ok("Ad added successfully.");
    }

    [HttpGet("GetAllListings")]
    [AllowAnonymous]
    public async Task<IResult> GetAllListings([FromQuery] ListingQueryDto query, [FromQuery] PaginationParamsDto pagination)
    {
        var listingsQuery = _dbContext.Listings
            .Include(l => l.Location)
            .ThenInclude(loc => loc.ParentLocation)
            .ThenInclude(parent => parent!.ParentLocation) 
            .Where(l =>
                (query.NoOfPeople == null || l.MaxGuests >= query.NoOfPeople) &&
                (!query.MinPrice.HasValue || l.PricePerNight >= query.MinPrice) &&
                (!query.MaxPrice.HasValue || l.PricePerNight <= query.MaxPrice) &&
                (!query.StartDate.HasValue || l.StartedDate <= query.StartDate.Value) &&
                (!query.EndDate.HasValue || l.EndDate >= query.EndDate.Value))
            .AsQueryable();
        
        if (!string.IsNullOrEmpty(query.Country))
        {
            listingsQuery = listingsQuery.Where(l => l.Location.ParentLocation!.ParentLocation!.ParentLocation!.Name == query.Country);
        }
        if (!string.IsNullOrEmpty(query.City))
        {
            listingsQuery = listingsQuery.Where(l => l.Location.ParentLocation!.ParentLocation!.Name == query.City);
        }
        if (!string.IsNullOrEmpty(query.District))
        {
            listingsQuery = listingsQuery.Where(l => l.Location.ParentLocation!.Name == query.District);
        }
        if (!string.IsNullOrEmpty(query.Neighborhood))
        {
            listingsQuery = listingsQuery.Where(l => l.Location.Name == query.Neighborhood);
        }

        listingsQuery = listingsQuery.Where(l =>
            !_dbContext.Bookings.Any(b =>
                b.ListingId == l.Id &&
                !b.IsDeleted &&
                (query.StartDate.HasValue && query.EndDate.HasValue) &&
                (query.StartDate.Value < b.EndDate && query.EndDate.Value > b.StartDate)
            ));
        
        var totalRecords = await listingsQuery.CountAsync();
    
        
        var listings = await listingsQuery
            .Where(l => !l.IsDeleted)
            .OrderBy(l => l.PricePerNight)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(l => new ListingDto
            {
                Id = l.Id,
                Title = l.Title,
                Description = l.Description,
                PricePerNight = l.PricePerNight,
                MaxGuests = l.MaxGuests,
                RoomCount = l.RoomCount,
                BedCount = l.BedCount,
                BathroomCount = l.BathroomCount,
                FullAddress = l.FullAddress,
                LocationMap = l.LocationMap,
                Neighborhood = l.Location.Name,
                DateList = CalculateAvailableDates(_dbContext.Bookings.ToList(), l.StartedDate, l.EndDate, l.Id).ToList(),
                District = l.Location.ParentLocation!.Name,
                City = l.Location.ParentLocation!.ParentLocation!.Name,
                Country = l.Location.ParentLocation!.ParentLocation!.ParentLocation!.Name
            })
            .ToListAsync();

        var response = new PagedResponseDto<ListingDto>
        {
            Data = listings,
            TotalRecords = totalRecords,
            CurrentPage = pagination.Page,
            PageSize = pagination.PageSize,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pagination.PageSize)
        };

        return Results.Ok(response);
    }
    
    private static List<DateRangeDto> CalculateAvailableDates(List<Booking> bookingList, DateTime listingStart, DateTime listingEnd, int listingId)
    {
        var bookings = bookingList
            .Where(b => b.ListingId == listingId && b.Status == BookingStatus.Onaylandi && !b.IsDeleted)
            .OrderBy(b => b.StartDate)
            .ToList();

        var availableDates = new List<DateRangeDto>();
        var currentStart = listingStart;

        foreach (var booking in bookings)
        {
            if (currentStart < booking.StartDate)
                availableDates.Add(new DateRangeDto
                {
                    StartDate = currentStart,
                    EndDate = booking.StartDate
                });

            currentStart = booking.EndDate > currentStart ? booking.EndDate : currentStart;
        }

        if (currentStart < listingEnd)
        {
            availableDates.Add(new DateRangeDto
            {
                StartDate = currentStart,
                EndDate = listingEnd
            });
        }

        return availableDates;
    }



    [HttpGet("GetListingById")]
    [AllowAnonymous]
    public async Task<IResult> GetListingById([FromQuery] int id)
    {
        var listing = await _dbContext.Listings
            .Include(l => l.Location)
            .Where(l => l.Id == id && !l.IsDeleted)
            .Select(l => new ListingDto
            {
                Id = l.Id,
                Title = l.Title,
                Description = l.Description,
                PricePerNight = l.PricePerNight,
                MaxGuests = l.MaxGuests,
                RoomCount = l.RoomCount,
                BedCount = l.BedCount,
                BathroomCount = l.BathroomCount,
                FullAddress = l.FullAddress,
                LocationMap = l.LocationMap,
                Neighborhood = l.Location.Name,
                DateList = CalculateAvailableDates(_dbContext.Bookings.ToList(), l.StartedDate, l.EndDate, l.Id),
                District = l.Location.ParentLocation!.Name,
                City = l.Location.ParentLocation!.ParentLocation!.Name,
                Country = l.Location.ParentLocation!.ParentLocation!.ParentLocation!.Name
            })
            .FirstOrDefaultAsync();

        if (listing == null)
            return Results.NotFound("No ad found");

        return Results.Ok(listing);
    }

    [Authorize(Roles = "Host")]
    [HttpPut("UpdateListing")]
    public async Task<IResult> UpdateListing([FromQuery] int id, [FromBody] UpdateListingDto model)
    {
        var currentUserId =  _jwtToken.GetUserId(User);
        if (currentUserId == -1)
            return Results.Forbid();
        
        var listing = await _dbContext.Listings.FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted);
     
        if (listing == null)
            return Results.NotFound("No ad Found");
        
        if (listing.UserId != currentUserId)
        {
            return Results.Forbid();
        }
        
        listing.Title = string.IsNullOrEmpty(model.Title) ? listing.Title : model.Title;
        listing.Description = string.IsNullOrEmpty(model.Description) ? listing.Description : model.Description;
        listing.PricePerNight = model.PricePerNight ?? listing.PricePerNight;
        listing.MaxGuests = model.MaxGuests ?? listing.MaxGuests;
        listing.RoomCount = model.RoomCount ?? listing.RoomCount;
        listing.BedCount = model.BedCount ?? listing.BedCount;
        listing.BathroomCount = model.BathroomCount ?? listing.BathroomCount;
        listing.StartedDate = model.StartedDate ?? listing.StartedDate;
        listing.EndDate = model.EndDate ?? listing.EndDate;
        listing.FullAddress = string.IsNullOrEmpty(model.FullAddress) ? listing.FullAddress : model.FullAddress;
        listing.LocationMap = string.IsNullOrEmpty(model.LocationMap) ? listing.LocationMap : model.LocationMap;

        await _dbContext.SaveChangesAsync();

        return Results.Ok("The ad has been updated successfully.");
    }

    [Authorize(Roles = "Host")]
    [HttpDelete("DeleteListing")]
    public async Task<IResult> DeleteListing(int id)
    {
        var currentUserId =  _jwtToken.GetUserId(User);
        if (currentUserId == -1)
            return Results.Forbid();
        
        var listing = await _dbContext.Listings.FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted);

        if (listing == null)
            return Results.NotFound("No ad found");
        
        if (listing.UserId != currentUserId)
        {
            return Results.Forbid();
        }
        listing.IsDeleted = true;
        await _dbContext.SaveChangesAsync();

        return Results.Ok("the ad has been deleted successfully");
    }
}
