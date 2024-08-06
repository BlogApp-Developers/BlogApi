using BlogApi.Models;

namespace BlogApi.Repositories.Base;
public interface IBlogRepository : IGetableAsync<Blog>, ICreatableAsync<Blog>
{

}
