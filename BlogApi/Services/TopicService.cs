using BlogApi.Models;
using BlogApi.Repositories.Base;
using BlogApi.Services.Base;

namespace BlogApi.Services;
public class TopicService : ITopicService
{

    private readonly ITopicRepository _topicRepository;

    public TopicService(ITopicRepository topicRepository)
    {
        _topicRepository = topicRepository;
    }
    public Task<IEnumerable<Topic>> GetAllTopicsAsync()
    {
        return _topicRepository.GetAllTopicsAsync();
    }

}
