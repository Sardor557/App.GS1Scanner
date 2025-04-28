using App.Repository.Services;
using Microsoft.Extensions.DependencyInjection;

namespace App.Repository
{
    public static class DependencyInjection
    {
        public static void AddMyServices(this IServiceCollection services)
        {
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<ICodeSenderService, CodeSenderService>();
        }
    }
}
