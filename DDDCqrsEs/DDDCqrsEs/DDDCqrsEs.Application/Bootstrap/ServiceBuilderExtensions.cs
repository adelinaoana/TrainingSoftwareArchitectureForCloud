using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using DDDCqrsEs.Common;

namespace DDDCqrsEs.Application.Bootstrap
{
    public static class ServiceBuilderExtensions
    {
        public static void RegisterApplicationServices(this IServiceCollection services)
        {
            services.Scan(scan => scan
                .FromAssemblies(Assembly.GetExecutingAssembly())
                .AddClasses(t => t.WithAttribute<MapServiceDependency>())
                .AsImplementedInterfaces()
                .WithScopedLifetime());
        }
    }
}
