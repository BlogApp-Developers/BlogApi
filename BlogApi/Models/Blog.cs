#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BlogApi.Models
{
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
        [JsonIgnore]
        public Topic Topic { get; set; }

        [Required]
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; }

        public string PictureUrl { get; set; } 

        [Required]
        public DateTime CreationDate { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();
    }
}
