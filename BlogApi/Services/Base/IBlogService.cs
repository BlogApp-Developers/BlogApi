using BlogApi.Models;

namespace BlogApi.Services.Base;
public interface IBlogService
{
    public Task CreateNewBlogAsync(Blog newFilm, IFormFile image);
    public Task<IEnumerable<Blog>> GetBlogsByTopicAsync(int topicId);
}
