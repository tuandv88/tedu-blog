namespace TeduBlog.Data.SeedWorks
{
	public interface IUnitOfWork
	{
		Task<int> CompleteAsync();
	}
}
