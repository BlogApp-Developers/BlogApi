namespace BlogApi.Repositories.Base;
public interface IGetableAsync<TEntity>
{
    public Task<IEnumerable<TEntity>?> GetBlogsByTopics(int topicId);
    public Task<TEntity> GetBlogById(Guid id);
    public Task<IEnumerable<TEntity>> SearchBlogsByTitle(string title);
}
