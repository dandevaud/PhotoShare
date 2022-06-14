using PhotoShare.Server.BusinessLogic;
using PhotoShare.Server.Contracts;
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

            return serviceCollection;
        }
    }
}
