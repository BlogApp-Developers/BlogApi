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

        var directory = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "BlogsImg");
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        obj.PictureUrl = Path.Combine(directory, $"{obj.Id}.{extension}");

        using var newFileStream = System.IO.File.Create(obj.PictureUrl);
        await image.CopyToAsync(newFileStream);

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
