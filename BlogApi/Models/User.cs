namespace BlogApi.Models
{
    #pragma warning disable CS8618
    using Microsoft.AspNetCore.Identity;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text.Json.Serialization;

    public class User : IdentityUser<Guid>
    {
        public string? AvatarUrl { get; set; }

        public ICollection<UserTopic> Topics { get; set; } = new List<UserTopic>();

        public string? AboutMe { get; set; }
        [JsonIgnore]
        public ICollection<Blog> Blogs { get; set; } = new List<Blog>();

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public ICollection<Like> Likes { get; set; } = new List<Like>();
    }
}
