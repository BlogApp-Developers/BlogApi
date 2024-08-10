using BlogApi.Models;

namespace BlogApi.Repositories.Base;
public interface ITopicRepository
{
    public Task<IEnumerable<Topic>?> GetAllTopicsAsync();
}
