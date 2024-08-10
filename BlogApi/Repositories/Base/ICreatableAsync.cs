namespace BlogApi.Repositories.Base;
public interface ICreatableAsync<TEntity>
{
    public Task CreateNewBlogAsync(TEntity obj, IFormFile image);
}
