using BlogApi.Models;
using BlogApi.Services.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace BlogApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class BlogController : ControllerBase
{
    private readonly IBlogService blogService;
    private readonly BlogApi.TokenValidation.TokenValidation tokenValidation;
    public BlogController(IBlogService blogService, BlogApi.TokenValidation.TokenValidation tokenValidation)
    {
        this.blogService = blogService;
        this.tokenValidation = tokenValidation;
    }


    [HttpPost("[action]")]
    public async Task<IActionResult> CreateBlog([FromForm] string title, [FromForm] string text, [FromForm] int topicId, [FromForm] Guid userId, IFormFile image)
    {
        try
        {
            try
            {
                base.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues headerValues);

                var tokenNew = headerValues.FirstOrDefault().Substring(7);
                this.tokenValidation.ValidateToken(tokenNew);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
            var blog = new Blog
            {
                Id = Guid.NewGuid(),
                Title = title,
                Text = text,
                TopicId = topicId,
                UserId = userId,
                CreationDate = DateTime.UtcNow
            };

            await blogService.CreateNewBlogAsync(blog, image);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(400, ex.Message);
        }
    }


    [HttpGet("GetBlogsByTopic/{topicId}")]
    public async Task<ActionResult<IEnumerable<Blog>>> GetBlogsByTopic(int topicId)
    {
        try
        {

            try
            {
                base.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues headerValues);

                var tokenNew = headerValues.FirstOrDefault().Substring(7);
                this.tokenValidation.ValidateToken(tokenNew);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
            var blogs = await blogService.GetBlogsByTopicAsync(topicId);

            if (blogs == null || !blogs.Any())
            {
                return NotFound("Blogs not found");
            }

            return Ok(blogs);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }


    [HttpGet("SearchBlogsByTitle/{title}")]
    public async Task<ActionResult<IEnumerable<Blog>>> SearchBlogsByTitle(string title)
    {
        try
        {
            try
            {
                base.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues headerValues);

                var tokenNew = headerValues.FirstOrDefault().Substring(7);
                this.tokenValidation.ValidateToken(tokenNew);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
            if (string.IsNullOrWhiteSpace(title))
            {
                return BadRequest("Title parameter is required");
            }

            var blogs = await blogService.SearchBlogsByTitleAsync(title);

            if (blogs == null || !blogs.Any())
            {
                return NotFound("No blogs found with the given title");
            }

            return Ok(blogs);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }


    [HttpGet("GetBlogById/{id}")]
    public async Task<ActionResult<Blog>> GetBlogById(Guid id)
    {
        try
        {
            base.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues headerValues);

            var tokenNew = headerValues.FirstOrDefault().Substring(7);
            this.tokenValidation.ValidateToken(tokenNew);
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }
        return await blogService.GetBlogById(id);
    }


    [HttpGet("[action]/{id}")]
    public async Task<IActionResult> Image(Guid id)
    {
        try
        {
            base.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues headerValues);

            var tokenNew = headerValues.FirstOrDefault().Substring(7);
            this.tokenValidation.ValidateToken(tokenNew);
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }
        var blog = await blogService.GetBlogById(id);
        if (blog == null || string.IsNullOrEmpty(blog.PictureUrl))
        {
            return NotFound("Blogs or image not found.");
        }
        var fileStream = System.IO.File.Open(blog.PictureUrl!, FileMode.Open);
        return File(fileStream, "image/jpeg");
    }

}
