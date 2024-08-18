using BlogApi.Models;

namespace BlogApi.Services.Base;
public interface IBlogService
{
    public Task CreateNewBlogAsync(Blog newFilm, IFormFile image);
    public Task<IEnumerable<BlogDto>> GetBlogsByTopicAsync(int topicId);
    public Task<Blog> GetBlogById(Guid id);

    public Task<IEnumerable<Blog>> SearchBlogsByTitleAsync(string title);
    public Task<IEnumerable<BlogDto>> GetBlogByUserId(Guid userId);
}
