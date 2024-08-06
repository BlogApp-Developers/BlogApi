using BlogApi.Models;
using BlogApi.Services.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class BlogController : ControllerBase
{
    private readonly IBlogService blogService;
    public BlogController(IBlogService blogService)
    {
        this.blogService = blogService;
    }


    [HttpPost("[action]")]
    public async Task<IActionResult> CreateBlog([FromForm] Blog blog, IFormFile image)
    {
        try
        {
            await blogService.CreateNewBlogAsync(blog, image);
            return Ok();
            
        }
        catch (Exception ex)
        {
            
            return StatusCode(400, ex.Message);
        }
    }


    [HttpGet("[action]")]
    public async Task<ActionResult<IEnumerable<Blog>>> GetBlogsByTopic(int topicId)
    {
        try
        {


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
}
