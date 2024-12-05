using AirbnbMinimal.DbOperations;
using AirbnbMinimal.DTOs;
using AirbnbMinimal.Models;
using AirbnbMinimal.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AirbnbMinimal.Controllers;

[ApiController]
[Route("api/v1/[controller]s")]
public class AuthenticationController : ControllerBase
{
    private readonly WebApiContext _dbContext;
    private readonly JwtToken _jwtToken;
    
    public AuthenticationController(WebApiContext dbContext, JwtToken jwtToken)
    {
        _dbContext = dbContext;
        _jwtToken = jwtToken;
    }

    [HttpPost("register")]
    public async Task<IResult> Register([FromBody] RegisterDto model)
    {
        if (!ModelState.IsValid)  
        {
            return Results.BadRequest(ModelState);
        }
        
        var existingUser = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Username == model.Username || u.Email == model.Email);

        if (existingUser != null)
            return Results.Conflict("username or email is already in use.");
        
        if (existingUser?.IsDeleted == true)
            return Results.BadRequest("User deleted.");
        
        if (existingUser?.IsBanned == true)
            return Results.BadRequest("This user has been blocked.");
        
        var role = await _dbContext.Roles
            .FirstOrDefaultAsync(r => r.Id == model.RoleId);

        if (role == null || role.IsDeleted || role.Name == "Admin")
            return Results.BadRequest("invalid role.");
        
        var user = new User
        {
            Name = model.Name,
            Surname = model.Surname,
            Email = model.Email,
            Password = model.Password,
            Phone = model.Phone,
            Gender = model.Gender,
            BirthDate = model.BirthDate,
            Username = model.Username,
            CreatedDate = DateTime.UtcNow,
            CreatedVersion = "1.0",
            RoleId = role.Id,
            Role = role
        };
        
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        
        var token = _jwtToken.GenerateToken(user);
        
        return Results.Json(token, statusCode: 201);
    }
    
    [HttpPost("Login")]
    public async Task<IResult> Login(LoginDto model)
    {
        if (!ModelState.IsValid) 
        {
            return Results.BadRequest(ModelState); 
        }   
        
        var existingUser = await _dbContext.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => (u.Username == model.UsernameOrMail || u.Email == model.UsernameOrMail) 
                                      && u.IsDeleted == false);
        
        if (existingUser is null || model.Password != existingUser.Password)
            return Results.Json("username or email is wrong.", statusCode: 401);
        
        if (existingUser.IsBanned) 
            return Results.Json("our account has been suspended.", statusCode: 403);
        
        var token = _jwtToken.GenerateToken(existingUser);
        return Results.Ok(token);
    }
  
}
