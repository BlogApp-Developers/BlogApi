namespace BlogApi.Repositories.Base;
public interface IGetableAsync<TEntity>
{
    public Task<IEnumerable<TEntity>?> GetBlogsByTopics(int topicId);
}
