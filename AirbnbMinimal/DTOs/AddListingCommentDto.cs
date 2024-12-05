namespace AirbnbMinimal.DTOs;

public class AddCommentDto
{
    public int ListingId { get; set; } 
    public int TargetUserId { get; set; }  
    public string CommentText { get; set; } = string.Empty;
    public int Rating { get; set; }  
}
