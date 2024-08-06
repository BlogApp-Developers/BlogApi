namespace BlogApi.Models;
#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Blog
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string Text { get; set; }
 
    [Required]
    [ForeignKey("Topic")]
    public int TopicId { get; set; }
    public Topic Topic { get; set; }

    [Required]
    [ForeignKey("User")]
    public Guid UserId { get; set; }
    public User User { get; set; }

    public string PictureUrl { get; set; } 

    [Required]
    public DateTime CreationDate { get; set; }
}
