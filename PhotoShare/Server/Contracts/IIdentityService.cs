namespace PhotoShare.Server.Contracts
{
	public interface IIdentityService
	{
		public Task<bool> HasAccess(Guid group, string password);
		public Task<bool> SetPasswordForGroup(Guid group, string password);

		public Task<bool> ChangePasswordForGroup(Guid group, string oldPassword, string newPassword);
	}
}
