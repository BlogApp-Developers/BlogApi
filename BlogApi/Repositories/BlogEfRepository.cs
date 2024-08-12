using Azure.Storage.Blobs;
using BlogApi.Data;
using BlogApi.Models;
using BlogApi.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Repositories;
public class BlogEfRepository : IBlogRepository
{
    private readonly BlogDbContext _dbContext;
    

    public BlogEfRepository(BlogDbContext dbContext)
    {
        _dbContext = dbContext;
        
    }


    public async Task CreateNewBlogAsync(Blog obj, IFormFile image)
    {
        obj.Id = Guid.NewGuid();
        var extension = new FileInfo(image.FileName).Extension[1..];
        obj.PictureUrl = $"Assets/BlogsImg/{obj.Id}.{extension}";

        using var newFileStream = System.IO.File.Create(obj.PictureUrl);
        await image.CopyToAsync(newFileStream);

        await _dbContext.Blogs.AddAsync(obj);
        await _dbContext.SaveChangesAsync();
        
    }


    public async Task<Blog?> GetBlogById(Guid id)
    {
        return await _dbContext.Blogs.FirstOrDefaultAsync((b) => b.Id == id);
    }

    public async Task<IEnumerable<Blog>?> GetBlogsByTopics(int topicId)
    {
        var blogs = await _dbContext.Blogs
        .Where(blog => blog.TopicId == topicId)
        .Include(blog => blog.User)
        .Include(blog => blog.Topic)
        .ToListAsync();

        return blogs.AsEnumerable();
    }

    public async Task<IEnumerable<Blog>> SearchBlogsByTitle(string title)
    {
        return await _dbContext.Blogs
            .Where(b => EF.Functions.Like(b.Title, $"%{title}%"))
            .ToListAsync();
    }
}
