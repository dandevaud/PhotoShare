using PhotoShare.Server.Contracts;
using PhotoShare.Server.Files;

namespace PhotoShare.Server.IoC
{
    public static class IoC
    {
        public static IServiceCollection BindServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IFileHandler, FileHandler>();

            return serviceCollection;
        }
    }
}
