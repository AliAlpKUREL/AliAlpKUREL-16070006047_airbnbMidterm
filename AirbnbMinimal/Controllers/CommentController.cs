using AirbnbMinimal.DbOperations;
using AirbnbMinimal.Models;
using AirbnbMinimal.DTOs;
using AirbnbMinimal.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AirbnbMinimal.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/[controller]s")]
public class CommentController : ControllerBase
{
    private readonly WebApiContext _dbContext;
    private readonly JwtToken _jwtToken;

    public CommentController(WebApiContext dbContext, JwtToken jwtToken)
    {
        _dbContext = dbContext;
        _jwtToken = jwtToken;
    }

    [HttpPost("AddComment")]
    public async Task<IResult> AddComment([FromBody] AddCommentDto model)
    {
        var currentUserId = _jwtToken.GetUserId(User);
        if (currentUserId == -1)
            return Results.Forbid();

        if (model.Rating < 1 || model.Rating > 5)
            return Results.BadRequest("The score must be between 1 and 5.");
 
        if (model.ListingId > 0)
        {
            var listing = await _dbContext.Listings.FirstOrDefaultAsync(l => l.Id == model.ListingId && !l.IsDeleted);
            if (listing == null)
                return Results.NotFound("No ad found.");

            var isComment = _dbContext.Comments.Any(l => l.UserId == currentUserId && l.ListingId == model.ListingId);
            if (isComment)
                return Results.NotFound("You have added a comment to this ad before.");

            var comment = new Comment
            {
                UserId = currentUserId,
                ListingId = model.ListingId,
                CommentText = model.CommentText,
                Rating = model.Rating,
                CreatedDate = DateTime.UtcNow,
                CreatedVersion = "1.0"
            };

            _dbContext.Comments.Add(comment);
            await _dbContext.SaveChangesAsync();

            return Results.Ok("Comment added to ad successfully.");
        }


        if (model.TargetUserId > 0)
        {
            var targetUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == model.TargetUserId && !u.IsDeleted);
            if (targetUser == null)
                return Results.NotFound("Target user not found.");
            
            var isComment = _dbContext.Comments.Any(l => l.UserId == currentUserId && l.ListingId == model.ListingId);
            if (isComment)
                return Results.NotFound("You have already added a comment to this user.");

            var comment = new Comment
            {
                UserId = currentUserId,
                TargetUserId = model.TargetUserId,
                CommentText = model.CommentText,
                Rating = model.Rating,
                CreatedDate = DateTime.UtcNow,
                CreatedVersion = "1.0"
            };

            _dbContext.Comments.Add(comment);
            await _dbContext.SaveChangesAsync();

            return Results.Ok("Comment added successfully for user.");
        }

        return Results.BadRequest("Invalid parameters.");
    }

    [HttpGet("GetComments")]
    [AllowAnonymous]
    public async Task<IResult> GetComments([FromQuery] int? listingId, [FromQuery] int? targetUserId)
    {
        var commentsQuery = _dbContext.Comments.AsQueryable();

        if (listingId.HasValue)
            commentsQuery = commentsQuery.Where(c => c.ListingId == listingId.Value);

        if (targetUserId.HasValue)
            commentsQuery = commentsQuery.Where(c => c.TargetUserId == targetUserId.Value);

        var comments = await commentsQuery
            .Select(c => new
            {
                c.Id,
                c.UserId,
                c.ListingId,
                c.TargetUserId,
                c.CommentText,
                c.Rating,
                c.CreatedDate
            })
            .ToListAsync();

        return Results.Ok(comments);
    }

    [HttpDelete("DeleteComment")]
    public async Task<IResult> DeleteComment([FromQuery] int commentId)
    {
        var currentUserId = _jwtToken.GetUserId(User);
        if (currentUserId == -1)
            return Results.Forbid();

        var comment = await _dbContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId && !c.IsDeleted);

        if (comment == null)
            return Results.NotFound("No comments found");

        if (comment.UserId != currentUserId)
            return Results.Forbid();

        comment.IsDeleted = true;
        await _dbContext.SaveChangesAsync();

        return Results.Ok("Comment deleted successfully.");
    }
    
    [HttpPut("UpdateComment")]
    public async Task<IResult> UpdateComment([FromQuery] int commentId, [FromBody] UpdateCommentDto model)
    {
        var currentUserId = _jwtToken.GetUserId(User);
        if (currentUserId == -1)
            return Results.Forbid();

        var comment = await _dbContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId && !c.IsDeleted);

        if (comment == null)
            return Results.NotFound("No comments found.");

        if (comment.UserId != currentUserId)
            return Results.Forbid();

        if (model.Rating.HasValue)
        {
            if (model.Rating < 1 || model.Rating > 5)
                return Results.BadRequest("The score must be between 1 and 5.");
            
            comment.Rating = model.Rating.Value;
        }

        if (!string.IsNullOrEmpty(model.CommentText))
            comment.CommentText = model.CommentText;

        comment.ModifiedDate = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return Results.Ok("Comment updated successfully.");
    }
}
