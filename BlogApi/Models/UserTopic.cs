namespace BlogApi.Models;
#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class UserTopic
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Topic")]
    public int TopicId { get; set; }
    public Topic Topic { get; set; }

    [ForeignKey("User")]
    public Guid UserId { get; set; }
    public User User { get; set; }
}
