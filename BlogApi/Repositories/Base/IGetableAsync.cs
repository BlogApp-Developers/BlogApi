using BlogApi.Models;

namespace BlogApi.Repositories.Base;
public interface IGetableAsync<TEntity>
{
    public Task<IEnumerable<BlogDto>?> GetBlogsByTopics(int topicId);
    public Task<TEntity> GetBlogById(Guid id);
    public Task<IEnumerable<TEntity>> SearchBlogsByTitle(string title);
    public Task<IEnumerable<BlogDto>?> GetBlogByUserId(Guid userId);
}
