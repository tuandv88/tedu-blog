using TeduBlog.Core.Repositories;

namespace TeduBlog.Data.SeedWorks
{
	public interface IUnitOfWork
	{
		IPostRepository Posts { get; }
		Task<int> CompleteAsync();
	}
}
