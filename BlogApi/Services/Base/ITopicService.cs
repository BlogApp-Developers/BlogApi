using BlogApi.Models;

namespace BlogApi.Services.Base;
public interface ITopicService
{
    public Task<IEnumerable<Topic>> GetAllTopicsAsync();
}
