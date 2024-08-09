namespace BlogApi.Repositories.Base;
public interface IGetableAsync<TEntity>
{
    public Task<IEnumerable<TEntity>?> GetBlogsByTopics(int topicId);
    public Task<IEnumerable<TEntity>> SearchBlogsByTitle(string title);
}
