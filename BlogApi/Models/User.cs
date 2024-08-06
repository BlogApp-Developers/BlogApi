namespace BlogApi.Models;
#pragma warning disable CS8618
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

public class User : IdentityUser<Guid>
{
    public string? AvatarUrl { get; set; }

    public ICollection<UserTopic> Topics { get; set; } = new List<UserTopic>();

    public string? AboutMe { get; set; }

    public ICollection<Blog> Blogs { get; set; } = new List<Blog>();
}
