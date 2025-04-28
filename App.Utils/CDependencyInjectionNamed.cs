using Microsoft.Extensions.DependencyInjection;

namespace App.Utils
{
    public interface INamedService
    {
        string Name { get; }
    }

    public static class NamedServiceExtensions
    {
        public static T GetServiceByName<T>(this IServiceProvider provider, string serviceName) where T : INamedService
        {
            var candidates = provider.GetServices<T>();
            return candidates.FirstOrDefault(s => s.Name == serviceName);
        }
    }
}
