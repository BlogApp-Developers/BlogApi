using Azure.Storage.Blobs;
using BlogApi.Data;
using BlogApi.Models;
using BlogApi.Options;
using BlogApi.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BlogApi.Repositories;
public class BlogEfRepository : IBlogRepository
{
    private readonly BlogDbContext _dbContext;
    //private readonly string connectionString;
    //private readonly BlobServiceClient _blobServiceClient;


    public BlogEfRepository(BlogDbContext dbContext)
    {
        _dbContext = dbContext;
        //this.connectionString = options.Value.ConnectionString;
        //_blobServiceClient = blobServiceClient;
    }


    // public async Task CreateNewBlogAsync(Blog obj, IFormFile image)
    // {

    //     obj.Id = Guid.NewGuid();

    //     var extension = new FileInfo(image.FileName).Extension[1..];

    //     var blobServiceClient = new BlobServiceClient(this.connectionString);
    //     var containerClient = blobServiceClient.GetBlobContainerClient("blogsimage");

    //     string blobName = $"{obj.Id}.{extension}";
    //     var blobClient = containerClient.GetBlobClient(blobName);

    //     using var stream = image.OpenReadStream();
    //     await blobClient.UploadAsync(stream, overwrite: true);

    //     obj.PictureUrl = blobClient.Uri.ToString();

    //     await _dbContext.Blogs.AddAsync(obj);
    //     await _dbContext.SaveChangesAsync();
    // }

    // public async Task CreateNewBlogAsync(Blog obj, IFormFile image)
    // {
    //     obj.Id = Guid.NewGuid();
    //     var extension = new FileInfo(image.FileName).Extension[1..];
    //     obj.PictureUrl = $"Assets/BlogsImg/{obj.Id}.{extension}";
    //     using var newFileStream = System.IO.File.Create(obj.PictureUrl);
    //     await image.CopyToAsync(newFileStream);

    //     await _dbContext.Blogs.AddAsync(obj);
    //     await _dbContext.SaveChangesAsync();
    // }

    public async Task CreateNewBlogAsync(Blog obj, IFormFile image)
    {

        obj.Id = Guid.NewGuid();

        var extension = Path.GetExtension(image.FileName);

        var blobName = $"{obj.Id}{extension}";
        var connectionString = "DefaultEndpointsProtocol=https;AccountName=blogteamstorage;AccountKey=Jj3XzYJg5sReLiUkj+1X6eap4T8DHLyY3uwbR9OwsqAx+q+HSHgdvKz0EPRWKgUOYChVJ6GCDFiQ+AStSO+mpg==;EndpointSuffix=core.windows.net";
        var blobServiceClient = new BlobServiceClient(connectionString);
        string containerName = "blogsimage";
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

        var blobClient = containerClient.GetBlobClient(blobName);

        using (var stream = image.OpenReadStream())
        {
            await blobClient.UploadAsync(stream, true);
        }

        obj.PictureUrl = blobClient.Uri.ToString();

        await _dbContext.Blogs.AddAsync(obj);
        await _dbContext.SaveChangesAsync();
        
    }




    public async Task<Blog?> GetBlogById(Guid id)
    {
        return await _dbContext.Blogs.FirstOrDefaultAsync((b) => b.Id == id);
    }

    public async Task<IEnumerable<BlogDto>?> GetBlogsByTopics(int topicId)
    {
        var blogs = await _dbContext.Blogs
            .Where(blog => blog.TopicId == topicId)
            .Include(blog => blog.User)
            .OrderByDescending(blog => blog.CreationDate) 
            .Select(blog => new BlogDto
            {
                Id = blog.Id,
                Title = blog.Title,
                Text = blog.Text,
                UserName = blog.User.UserName,
                UserId = blog.UserId,
                PictureUrl = blog.PictureUrl,
                CreationDate = blog.CreationDate
            })
            .ToListAsync();

        return blogs;
    }


    public async Task<IEnumerable<Blog>> SearchBlogsByTitle(string title)
    {
        return await _dbContext.Blogs
        .Where(b => b.Title.ToLower().Contains(title.ToLower()))
        .ToListAsync();
    }


    public async Task<IEnumerable<BlogDto>?> GetBlogByUserId(Guid userId)
    {
        var blogs = await _dbContext.Blogs
            .Where(blog => blog.UserId == userId)
            .Include(blog => blog.User)
            .Select(blog => new BlogDto
            {
                Id = blog.Id,
                Title = blog.Title,
                Text = blog.Text,
                UserName = blog.User.UserName,
                PictureUrl = blog.PictureUrl,
                CreationDate = blog.CreationDate
            })
            .ToListAsync();

        return blogs;
    }
}
