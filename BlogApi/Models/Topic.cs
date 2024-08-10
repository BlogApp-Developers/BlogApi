namespace BlogApi.Models;
#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Topic
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    public ICollection<Blog> Blogs { get; set; } = new List<Blog>();

    public ICollection<UserTopic> Users { get; set; } = new List<UserTopic>();
}
