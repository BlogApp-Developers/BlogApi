namespace BlogApi.Models;
public class LikeRequest
{
    public Guid BlogId { get; set; }
    public Guid UserId { get; set; }
}
