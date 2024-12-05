using AirbnbMinimal.DbOperations;
using AirbnbMinimal.DTOs;
using AirbnbMinimal.Enums;
using AirbnbMinimal.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AirbnbMinimal.Controllers;

[Authorize]
[Route("api/v1/[controller]s")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly WebApiContext _dbContext;
    private readonly JwtToken _jwtToken;

    public UserController(WebApiContext dbContext, JwtToken jwtToken)
    {
        _dbContext = dbContext;
        _jwtToken = jwtToken;
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("GetAllUsers")]
    public async Task<IResult> GetAllUsers()
    {
        var users = await _dbContext.Users
            .Include(u => u.Role) 
            .Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Surname = u.Surname,
                Email = u.Email,
                BirthDate = u.BirthDate,
                Phone = u.Phone,
                Biography = u.Biography,
                ExtraInfo = u.ExtraInfo,
                ProfileImageUrl = u.ProfileImageUrl,
                Gender = u.Gender,
                RoleName = u.Role.Name,
                IsBanned = u.IsBanned,
                IsDeleted = u.IsDeleted
            })
            .ToListAsync();

        return Results.Ok(users);
    }

    [Authorize(Roles = "Admin, Host, Guest")]
    [HttpGet("GetUserById")]
    public async Task<IResult> GetUserById([FromQuery] int id)
    {
        var currentUserId =  _jwtToken.GetUserId(User);
        var currentUserRole = _jwtToken.GetUserRole(User);
        
        if (currentUserId == -1)
            return Results.Forbid();
        

        var user = await _dbContext.Users
            .Include(u => u.Role)
            .Where(u => u.Id == id && !u.IsDeleted && !u.IsBanned)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Surname = u.Surname,
                Email = u.Email,
                BirthDate = u.BirthDate,
                Phone = u.Phone,
                Biography = u.Biography,
                ExtraInfo = u.ExtraInfo,
                ProfileImageUrl = u.ProfileImageUrl,
                Gender = u.Gender,
                RoleName = u.Role.Name,
                IsBanned = u.IsBanned,
                IsDeleted = u.IsDeleted
            })
            .FirstOrDefaultAsync();
       
       if (currentUserRole != "Admin" && currentUserId != id)
           return Results.Forbid();

        if (user == null)
            return Results.NotFound("User not found.");

        return Results.Ok(user);
    }
    
    [Authorize(Roles = "Admin, Host, Guest")]
    [HttpPut("UpdateUser")]
    public async Task<IResult> UpdateUser([FromQuery] int id, [FromBody] UpdateUserDto model)
    {
        var currentUserId =  _jwtToken.GetUserId(User);
        var currentUserRole = _jwtToken.GetUserRole(User);
        
        if (currentUserId == -1)
            return Results.Forbid();
        
        var existingUser = await _dbContext.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted && !u.IsBanned);
        
        if (existingUser == null)
            return Results.NotFound("User not found.");
        
        if (currentUserRole != "Admin" && currentUserId != existingUser.Id)
            return Results.Forbid();
            
        existingUser.Name = string.IsNullOrEmpty(model.Name) ? existingUser.Name : model.Name;
        existingUser.Surname = string.IsNullOrEmpty(model.Surname) ? existingUser.Surname : model.Surname;
        existingUser.Email = string.IsNullOrEmpty(model.Email) ? existingUser.Email : model.Email;
        existingUser.Phone = string.IsNullOrEmpty(model.Phone) ? existingUser.Phone : model.Phone;
        existingUser.Biography = string.IsNullOrEmpty(model.Biography) ? existingUser.Biography : model.Biography;
        existingUser.ExtraInfo = string.IsNullOrEmpty(model.ExtraInfo) ? existingUser.ExtraInfo : model.ExtraInfo;
        existingUser.ProfileImageUrl = string.IsNullOrEmpty(model.ProfileImageUrl) ? existingUser.ProfileImageUrl : model.ProfileImageUrl;
        
        await _dbContext.SaveChangesAsync();
        
        return Results.Ok("User updated successfully.");
    }
    
    [HttpDelete("DeleteUser")]
    [Authorize(Roles = "Admin, Host, Guest")]
    public async Task<IResult> DeleteUser([FromQuery] int id)
    {
        var currentUserId =  _jwtToken.GetUserId(User);
        var currentUserRole = _jwtToken.GetUserRole(User);
        
        if (currentUserId == -1)
            return Results.Forbid();
        
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        if (user == null)
            return Results.NotFound("User not found.");
        
        if (currentUserRole != "Admin" && currentUserId != user.Id)
            return Results.Forbid();
        
        user.IsDeleted = true;

        await _dbContext.SaveChangesAsync();

        return Results.Ok("The user was deleted successfully.");
    }
    
    [HttpPut("ChangePassword")]
    [Authorize(Roles = "Admin, Host, Guest")]
    public async Task<IResult> ChangePassword([FromQuery] int id, [FromBody] ChangePasswordDto model)
    { 
        var currentUserId =  _jwtToken.GetUserId(User);
            
        if (currentUserId == -1)
            return Results.Forbid();
        
        var currentUserRole = _jwtToken.GetUserRole(User);
        
        if (currentUserRole != "Admin" && currentUserId != id)
            return Results.Forbid();
        
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted && !u.IsBanned);

        if (user == null)
            return Results.NotFound("User not found.");

        if (user.Password != model.CurrentPassword)
            return Results.BadRequest("The current password is incorrect.");

        user.Password = model.NewPassword;

        await _dbContext.SaveChangesAsync();

        return Results.Ok("Password changed successfully.");
    }

    [HttpPut("BanUser")]
    [Authorize(Roles = "Admin")]
    public async Task<IResult> BanUser([FromQuery]int id)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);

        if (user == null)
            return Results.NotFound("User not found.");

        if (user.IsBanned)
            return Results.BadRequest("The user has already been banned.");

        user.IsBanned = true;

        await _dbContext.SaveChangesAsync();

        return Results.Ok("The user was successfully banned.");
    }

    [HttpPut("UnbanUser")]
    [Authorize(Roles = "Admin")]
    public async Task<IResult> UnbanUser([FromQuery] int id)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);

        if (user == null)
            return Results.NotFound("User not found.");

        if (!user.IsBanned)
            return Results.BadRequest("The user is not already banned");

        user.IsBanned = false;

        await _dbContext.SaveChangesAsync();

        return Results.Ok("The user was successfully unbanned.");
    }
    
    [HttpGet("GetUserStats")]
    [Authorize(Roles = "Admin")]
    public async Task<IResult> GetUserStats()
    {
        var stats = await _dbContext.Users
            .GroupBy(u => u.Role.Name)
            .Select(g => new 
            {
                Role = g.Key,
                TotalUsers = g.Count(),
                ActiveUsers = g.Count(u => !u.IsDeleted && !u.IsBanned),
                BannedUsers = g.Count(u => u.IsBanned),
                DeletedUsers = g.Count(u => u.IsDeleted)
            })
            .ToListAsync();

        return Results.Ok(stats);
    }
    
    [HttpGet("GetBookingStats")]
    [Authorize(Roles = "Admin")]
    public async Task<IResult> GetBookingStats()
    {
        var bookingStats = await _dbContext.Listings
            .Include(l => l.Bookings)
            .Select(l => new 
            {
                ListingId = l.Id,
                ListingTitle = l.Title,
                TotalBookings = l.Bookings.Count,
                ApprovedBookings = l.Bookings.Count(b => b.Status == BookingStatus.Onaylandi),
                PendingBookings = l.Bookings.Count(b => b.Status == BookingStatus.Bekleniyor)
            })
            .ToListAsync();

        return Results.Ok(bookingStats);
    }
    
    [HttpGet("TopRatedListings")]
    [Authorize(Roles = "Admin")]
    public async Task<IResult> GetTopRatedListings()
    {
        var topRatedListings = await _dbContext.Listings
            .Select(l => new
            {
                ListingId = l.Id,
                Title = l.Title,
                AverageRating = _dbContext.Comments
                    .Where(c => c.ListingId == l.Id)
                    .Average(c => (double?)c.Rating) ?? 0
            })
            .OrderByDescending(l => l.AverageRating)
            .Take(10)
            .ToListAsync();

        return Results.Ok(topRatedListings);
    }

}