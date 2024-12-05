using AirbnbMinimal.DbOperations;
using AirbnbMinimal.DTOs;
using AirbnbMinimal.Enums;
using AirbnbMinimal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AirbnbMinimal.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/[controller]s")]
public class LocationController : ControllerBase
{
    private readonly WebApiContext _dbContext;

    public LocationController(WebApiContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost("AddLocation")]
    [Authorize(Roles = "Admin")]
    public async Task<IResult> AddLocation([FromBody] AddLocationDto model)
    {
        if (model.ParentLocationId.HasValue)
        {
            var parentLocation = await _dbContext.Locations.FindAsync(model.ParentLocationId.Value);
            if (parentLocation == null)
                return Results.BadRequest("Invalid location ID.");
        }

        var location = new Location
        {
            Name = model.Name,
            Type = model.Type,
            ParentLocationId = model.ParentLocationId,
            CreatedDate = DateTime.Now,
            CreatedVersion = "1.0"
        };

        _dbContext.Locations.Add(location);
        await _dbContext.SaveChangesAsync();

        return Results.Ok("Location added successfully.");
    }

    [HttpGet("GetAllLocations")]
    [AllowAnonymous]
    public async Task<IResult> GetAllLocations()
    {
        var locations = await _dbContext.Locations
            .Include(l => l.ParentLocation)
            .Select(l => new LocationDto
            {
                Id = l.Id,
                Name = l.Name,
                Type = l.Type,
                ParentLocationId = l.ParentLocationId,
                ParentLocationName = l.ParentLocation != null ? l.ParentLocation.Name : null
            })
            .ToListAsync();

        return Results.Ok(locations);
    }

    [HttpGet("GetLocationById")]
    [AllowAnonymous]
    public async Task<IResult> GetLocationById([FromQuery] int id)
    {
        var location = await _dbContext.Locations
            .Include(l => l.ParentLocation)
            .Where(l => l.Id == id)
            .Select(l => new LocationDto
            {
                Id = l.Id,
                Name = l.Name,
                Type = l.Type,
                ParentLocationId = l.ParentLocationId,
                ParentLocationName = l.ParentLocation != null ? l.ParentLocation.Name : null
            })
            .FirstOrDefaultAsync();

        if (location == null)
            return Results.NotFound("Location not found.");

        return Results.Ok(location);
    }

    [HttpPut("UpdateLocation")]
    [Authorize(Roles = "Admin")]
    public async Task<IResult> UpdateLocation([FromQuery] int id, [FromBody] UpdateLocationDto model)
    {
        var location = await _dbContext.Locations.FindAsync(id);

        if (location == null)
            return Results.NotFound("Location not found.");

        if (model.ParentLocationId.HasValue && model.ParentLocationId != location.ParentLocationId)
        {
            var parentLocation = await _dbContext.Locations.FindAsync(model.ParentLocationId.Value);
            if (parentLocation == null)
                return Results.BadRequest("Invalid top level location ID.");
        }

        location.Name = string.IsNullOrEmpty(model.Name) ? location.Name : model.Name;
        location.Type = model.Type ?? location.Type;
        location.ParentLocationId = model.ParentLocationId ?? location.ParentLocationId;

        await _dbContext.SaveChangesAsync();

        return Results.Ok("Location updated successfully.");
    }

    [HttpDelete("DeleteLocation")]
    [Authorize(Roles = "Admin")]
    public async Task<IResult> DeleteLocation([FromQuery] int id)
    {
        var location = await _dbContext.Locations.FindAsync(id);

        if (location == null)
            return Results.NotFound("Location not found.");

        var childLocations = await _dbContext.Locations.AnyAsync(l => l.ParentLocationId == id);
        if (childLocations)
            return Results.BadRequest("This location cannot be deleted because it has sub-locations that are dependent on it.");

        _dbContext.Locations.Remove(location);
        await _dbContext.SaveChangesAsync();

        return Results.Ok("The location was deleted successfully.");
    }
}
