using AirbnbMinimal.DbOperations;
using AirbnbMinimal.DTOs;
using AirbnbMinimal.Enums;
using AirbnbMinimal.Models;
using AirbnbMinimal.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AirbnbMinimal.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/[controller]s")]
public class BookingController : ControllerBase
{
    private readonly WebApiContext _dbContext;
    private readonly JwtToken _jwtToken;

    public BookingController(WebApiContext dbContext, JwtToken jwtToken)
    {
        _dbContext = dbContext;
        _jwtToken = jwtToken;
    }

    [HttpPost("AddBooking")]
    [Authorize(Roles = "Guest")]
    public async Task<IResult> AddBooking([FromBody] AddBookingDto model)
    {
        var currentUserId = _jwtToken.GetUserId(User);
        if (currentUserId == -1)
            return Results.Forbid();

        var listing = await _dbContext.Listings
            .Include(l => l.Bookings)
            .FirstOrDefaultAsync(x => x.Id == model.ListingId);
        if (listing == null || listing.IsDeleted)
            return Results.NotFound("No ad found.");

        if (listing.MaxGuests < model.NumberOfGuests)
            return Results.BadRequest("The specified number of guests exceeds the maximum capacity.");


        if (model.StartDate < listing.StartedDate || model.EndDate > listing.EndDate)
            return Results.BadRequest("The specified date range is outside the rental period of the advertisement.");

        var hasConflictingBooking = listing.Bookings
            .Any(b => b.Status == BookingStatus.Onaylandi && 
                      !(model.EndDate <= b.StartDate || model.StartDate >= b.EndDate));

        if (hasConflictingBooking)
            return Results.BadRequest("There is another reservation available for the specified date range.");

        var booking = new Booking
        {
            UserId = currentUserId,
            ListingId = model.ListingId,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            NumberOfGuests = model.NumberOfGuests,
            Status = BookingStatus.Bekleniyor,
            CreatedDate = DateTime.UtcNow,
            CreatedVersion = "1.0"
        };

        _dbContext.Bookings.Add(booking);
        await _dbContext.SaveChangesAsync();

        return Results.Ok("Reservation created successfully, awaiting confirmation.");
    }

    [HttpPut("ApproveBooking")]
    [Authorize(Roles = "Host")]
    public async Task<IResult> ApproveBooking(int bookingId)
    {
        var currentUserId = _jwtToken.GetUserId(User);
        if (currentUserId == -1)
            return Results.Forbid();

        var booking = await _dbContext.Bookings
            .Include(b => b.Listing)
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking == null || booking.Status != BookingStatus.Bekleniyor)
            return Results.BadRequest("No available reservations were found to be confirmed.");
        
        if (booking.Listing.UserId != currentUserId)
            return Results.Forbid();
        
        if (booking.Listing.MaxGuests < booking.NumberOfGuests)
            return Results.BadRequest("The number of people booked exceeds the available capacity.");

        booking.Listing.MaxGuests -= booking.NumberOfGuests;
        booking.Status = BookingStatus.Onaylandi;
        
        await _dbContext.SaveChangesAsync();

        return Results.Ok("The reservation has been confirmed successfully.");
    }

    [HttpGet("GetAllBookings")]
    [Authorize(Roles = "Admin")]
    public async Task<IResult> GetAllBookings()
    {
        var bookings = await _dbContext.Bookings
            .Include(b => b.User)
            .Include(b => b.Listing)
            .Select(b => new BookingDto
            {
                Id = b.Id,
                ListingTitle = b.Listing.Title,
                GuestName = $"{b.User.Name} {b.User.Surname}",
                StartDate = b.StartDate,
                EndDate = b.EndDate,
                NumberOfGuests = b.NumberOfGuests,
                Status = b.Status.ToString()
            })
            .ToListAsync();

        return Results.Ok(bookings);
    }

    [HttpGet("GetGuestBookings")]
    [Authorize(Roles = "Guest")]
    public async Task<IResult> GetGuestBookings()
    {
        var currentUserId = _jwtToken.GetUserId(User);
        if (currentUserId == -1)
            return Results.Forbid();

        var bookings = await _dbContext.Bookings
            .Where(b => b.UserId == currentUserId)
            .Include(b => b.Listing)
            .Select(b => new BookingDto
            {
                Id = b.Id,
                ListingTitle = b.Listing.Title,
                StartDate = b.StartDate,
                EndDate = b.EndDate,
                NumberOfGuests = b.NumberOfGuests,
                Status = b.Status.ToString()
            })
            .ToListAsync();

        return Results.Ok(bookings);
    }
}
