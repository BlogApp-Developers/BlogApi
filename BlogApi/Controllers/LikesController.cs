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
        public async Task<IActionResult> LikeBlog([FromBody] LikeRequest likeRequest)
        {
            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.BlogId == likeRequest.BlogId && l.UserId == likeRequest.UserId);

            if (existingLike != null)
            {
                return BadRequest("Blog already liked by this user.");
            }

            var like = new Like
            {
                BlogId = likeRequest.BlogId,
                UserId = likeRequest.UserId,
                LikedAt = DateTime.UtcNow
            };
            _context.Likes.Add(like);
            await _context.SaveChangesAsync();

            var likeCount = await _context.Likes.CountAsync(l => l.BlogId == likeRequest.BlogId);
            return Ok(likeCount);
        }

        [HttpPost("UnlikeBlog")]
        public async Task<IActionResult> UnlikeBlog([FromBody] LikeRequest likeRequest)
        {
            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.BlogId == likeRequest.BlogId && l.UserId == likeRequest.UserId);

            if (existingLike == null)
            {
                return BadRequest("Blog not liked by this user.");
            }

            _context.Likes.Remove(existingLike);
            await _context.SaveChangesAsync();

            var likeCount = await _context.Likes.CountAsync(l => l.BlogId == likeRequest.BlogId);
            return Ok(likeCount);
        }

        [HttpGet("IsLiked")]
        public async Task<IActionResult> IsLiked([FromQuery] Guid blogId, [FromQuery] Guid userId)
        {
            var isLiked = await _context.Likes.AnyAsync(l => l.BlogId == blogId && l.UserId == userId);
            return Ok(isLiked);
        }

        [HttpGet("GetLikesForBlog/{blogId}")]
        public async Task<IActionResult> GetLikesForBlog(Guid blogId)
        {
            var likeCount = await _context.Likes.CountAsync(l => l.BlogId == blogId);
            return Ok(likeCount);
        }


    }
}