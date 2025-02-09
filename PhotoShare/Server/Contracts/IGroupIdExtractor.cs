namespace PhotoShare.Server.Contracts
{
	public interface IGroupIdExtractor
	{
		public Task<Guid?> GetGroupIdFromHttpContext(HttpContext? context);
	}
}
