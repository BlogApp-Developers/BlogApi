using BlogApi.Data;
using BlogApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LikesController : ControllerBase
    {
        private readonly BlogDbContext _context;

        public LikesController(BlogDbContext context)
        {
            _context = context;
        }

        [HttpPost("LikeBlog")]
        public async Task<IActionResult> LikeBlog([FromBody] Guid blogId, [FromBody] Guid userId)
        {
            // if (like == null)
            // {
            //     return BadRequest("Like object is null.");
            // }

            

            var existingLike = await _context.Likes
                .Where(l => l.BlogId == blogId && l.UserId == userId)
                .FirstOrDefaultAsync();

            if (existingLike != null)
            {
                return BadRequest("User has already liked this blog.");
            }

            var like = new Like();
            like.LikedAt = DateTime.UtcNow;
            like.BlogId = blogId;
            like.UserId = userId;

            _context.Likes.Add(like);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLikesForBlog), new { blogId = like.BlogId }, like);
        }



        [HttpGet("blog/{blogId}")]
        public async Task<IActionResult> GetLikesForBlog(Guid blogId)
        {
            var likes = await _context.Likes
                .Where(l => l.BlogId == blogId)
                .ToListAsync();

            return Ok(likes);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> UnlikeBlog(int id)
        {
            var like = await _context.Likes.FindAsync(id);
            if (like == null)
            {
                return NotFound();
            }

            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}