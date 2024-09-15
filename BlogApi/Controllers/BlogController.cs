using Azure.Storage.Blobs;
using BlogApi.Models;
using BlogApi.Options;
using BlogApi.Services.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace BlogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlogController : ControllerBase
{
    private readonly IBlogService blogService;
    //private readonly BlobServiceClient _blobServiceClient;
    //private readonly string connectionString;
    private readonly BlogApi.TokenValidation.TokenValidation tokenValidation;
    public BlogController(IBlogService blogService, BlogApi.TokenValidation.TokenValidation tokenValidation)
    {
        this.blogService = blogService;
        this.tokenValidation = tokenValidation;
        //this.connectionString = options.Value.ConnectionString;
        //_blobServiceClient = blobServiceClient;
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

                var tokenNew = headerValues.FirstOrDefault()?.Substring(7);
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

            var reversedBlogs = blogs.Reverse();

            return Ok(reversedBlogs);
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



    [HttpGet("[action]/{userId}")]
    public async Task<ActionResult<IEnumerable<Blog>>> GetBlogByUserId(Guid userId)
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

        var blogs = await blogService.GetBlogByUserId(userId);

        if (blogs == null || !blogs.Any())
        {
            return NotFound("No blogs found for this user.");
        }

        return Ok(blogs);
    }






    // [HttpGet("[action]/{id}")]
    // public async Task<IActionResult> Image(Guid id)
    // {

    //     var blog = await blogService.GetBlogById(id);
    //     if (blog == null || string.IsNullOrEmpty(blog.PictureUrl))
    //     {
    //         return NotFound("Blogs or image not found.");
    //     }
    //     var fileStream = System.IO.File.Open(blog.PictureUrl!, FileMode.Open);
    //     return File(fileStream, "image/jpeg");
    // }


    // [HttpGet("[action]/{id}")]
    // public async Task<IActionResult> Image(Guid id)
    // {

    //     var connectionString = "";
    //     var blobServiceClient = new BlobServiceClient(connectionString);
    //     string containerName = "blogsimage"; 

    //     string blobName = $"{id}.jpg"; 

    //     var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

    //     var blobClient = containerClient.GetBlobClient(blobName);

    //     if (await blobClient.ExistsAsync())
    //     {
    //         var downloadInfo = await blobClient.DownloadAsync();
    //         return File(downloadInfo.Value.Content, downloadInfo.Value.ContentType);
    //     }
    //     else
    //     {
    //         return NotFound("Image not found");
    //     }
    // }

    [HttpGet("[action]/{id}")]
    public async Task<IActionResult> Image(Guid id)
    {
        var connectionString = "DefaultEndpointsProtocol=https;AccountName=blogteamstorage;AccountKey=Jj3XzYJg5sReLiUkj+1X6eap4T8DHLyY3uwbR9OwsqAx+q+HSHgdvKz0EPRWKgUOYChVJ6GCDFiQ+AStSO+mpg==;EndpointSuffix=core.windows.net";
        var blobServiceClient = new BlobServiceClient(connectionString);
        string containerName = "blogsimage";

        string[] possibleExtensions = { ".jpg", ".png", ".jpeg", ".gif" };
        string foundBlobName = null;

        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

        foreach (var extension in possibleExtensions)
        {
            string blobName = $"{id}{extension}";
            var blobClient = containerClient.GetBlobClient(blobName);

            if (await blobClient.ExistsAsync())
            {
                foundBlobName = blobName;
                break;
            }
        }

        if (foundBlobName == null)
        {
            return NotFound("Image not found");
        }
        Console.WriteLine("ok");
        var finalBlobClient = containerClient.GetBlobClient(foundBlobName);
        var downloadInfo = await finalBlobClient.DownloadAsync();
        return File(downloadInfo.Value.Content, downloadInfo.Value.ContentType);
    }



}
