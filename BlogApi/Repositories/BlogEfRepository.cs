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

    public async Task<IEnumerable<Blog>?> GetBlogsByTopics(int topicId)
    {
        return await _dbContext.Blogs
            .Where(blog => blog.TopicId == topicId)
            .ToListAsync();
    }

}
