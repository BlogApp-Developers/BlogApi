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
        public async Task<IActionResult> LikeBlog([FromBody] LikeRequest request)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            var blog = await _context.Blogs.FindAsync(request.BlogId);

            if (user == null || blog == null)
            {
                return NotFound();
            }

            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.BlogId == request.BlogId && l.UserId == request.UserId);

            if (existingLike != null)
            {
                return BadRequest("User already liked this blog.");
            }

            var like = new Like
            {
                BlogId = request.BlogId,
                UserId = request.UserId,
                LikedAt = DateTime.UtcNow
            };

            _context.Likes.Add(like);
            await _context.SaveChangesAsync();

            return Ok();
        }


        [HttpGet("IsLiked")]
        public async Task<IActionResult> IsLiked([FromQuery] Guid blogId, [FromQuery] Guid userId)
        {
            var isLiked = await _context.Likes.AnyAsync(l => l.BlogId == blogId && l.UserId == userId);
            return Ok(isLiked);
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