using Microsoft.AspNetCore.Identity;
using PhotoShare.Server.BusinessLogic;
using PhotoShare.Server.Contracts;
using PhotoShare.Server.Contracts.Authentication;
using PhotoShare.Server.Files;

namespace PhotoShare.Server.IoC
{
	public static class IoC
	{
		public static IServiceCollection BindServices(this IServiceCollection serviceCollection)
		{
			serviceCollection.AddTransient<IFileHandler, FileHandler>();
			serviceCollection.AddTransient<IGroupCrudExecutor, GroupCrudExecutor>();
			serviceCollection.AddTransient<IEncryptionHandler, EncryptionHandler>();
			serviceCollection.AddTransient<IPictureCrudExecutor, PictureCrudExecutor>();
			serviceCollection.AddTransient<IGroupKeyRetriever, GroupKeyRetriever>();
			serviceCollection.AddTransient<IPictureLoader, PictureLoader>();
			serviceCollection.AddTransient<IIdentityService, IdentityService>();
			serviceCollection.AddSingleton<PasswordHasher<GroupPassword>>();
			serviceCollection.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
			serviceCollection.AddTransient<IGroupIdExtractor, GroupIdExtractor>();

			return serviceCollection;
		}
	}
}
