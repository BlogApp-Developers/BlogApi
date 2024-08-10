using Azure.Storage.Blobs;
using BlogApi.Data;
using BlogApi.Models;
using BlogApi.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Repositories;
public class BlogEfRepository : IBlogRepository
{
    private readonly BlogDbContext _dbContext;
    private readonly BlobServiceClient _blobServiceClient;

    public BlogEfRepository(BlogDbContext dbContext, BlobServiceClient blobServiceClient)
    {
        _dbContext = dbContext;
        _blobServiceClient = blobServiceClient;
    }


    public async Task CreateNewBlogAsync(Blog obj, IFormFile image)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient("blogimages");

        obj.Id = Guid.NewGuid();
        var extension = new FileInfo(image.FileName).Extension[1..];
        var blobName = $"{obj.Id}.{extension}";
        obj.PictureUrl = $"https://{_blobServiceClient.AccountName}.blob.core.windows.net/blogimages/{blobName}";

        var blobClient = containerClient.GetBlobClient(blobName);

        using (var imageStream = image.OpenReadStream())
        {
            await blobClient.UploadAsync(imageStream, true);
        }

        await _dbContext.Blogs.AddAsync(obj);
        await _dbContext.SaveChangesAsync();
    }


    public async Task<Blog?> GetBlogById(Guid id)
    {
        return await _dbContext.Blogs.FirstOrDefaultAsync((b) => b.Id == id);
    }

    public async Task<IEnumerable<Blog>?> GetBlogsByTopics(int topicId)
    {
        return await _dbContext.Blogs
            .Where(blog => blog.TopicId == topicId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Blog>> SearchBlogsByTitle(string title)
    {
        return await _dbContext.Blogs
            .Where(b => EF.Functions.Like(b.Title, $"%{title}%"))
            .ToListAsync();
    }
}
