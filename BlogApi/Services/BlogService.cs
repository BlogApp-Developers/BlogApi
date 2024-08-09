using BlogApi.Models;
using BlogApi.Repositories.Base;
using BlogApi.Services.Base;
#pragma warning disable CS8603

namespace BlogApi.Services;
public class BlogService : IBlogService
{
    private readonly IBlogRepository _blogRepository;

    public BlogService(IBlogRepository blogRepository)
    {
        _blogRepository = blogRepository;
    }


    public async Task CreateNewBlogAsync(Blog newFilm, IFormFile image)
    {
        await _blogRepository.CreateNewBlogAsync(newFilm, image);
    }

    public async Task<IEnumerable<Blog>> GetBlogsByTopicAsync(int topicId)
    {
        return await _blogRepository.GetBlogsByTopics(topicId);
    }

    public Task<IEnumerable<Blog>> SearchBlogsByTitleAsync(string title)
    {
        return _blogRepository.SearchBlogsByTitle(title);
    }
}
