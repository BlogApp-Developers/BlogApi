using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogApi.Models
{
    public class Comment
    {
         [Key]
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [ForeignKey("Blog")]
        public Guid BlogId { get; set; }
    

        [ForeignKey("User")]
        public Guid UserId { get; set; }
     
    }
}