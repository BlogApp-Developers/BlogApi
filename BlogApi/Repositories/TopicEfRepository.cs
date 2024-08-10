using BlogApi.Data;
using BlogApi.Models;
using BlogApi.Repositories.Base;

namespace BlogApi.Repositories;
public class TopicEfRepository : ITopicRepository
{
    private readonly BlogDbContext _dbContext;

    public TopicEfRepository(BlogDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<IEnumerable<Topic>?> GetAllTopicsAsync()
    {
        return _dbContext.Topics.ToList();
    }
}
