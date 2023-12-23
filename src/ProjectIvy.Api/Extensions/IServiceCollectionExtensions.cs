using Microsoft.Extensions.DependencyInjection;
using ProjectIvy.Business.Handlers;
using ProjectIvy.Common.Interfaces;

namespace ProjectIvy.Api.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddHandler<THandlerInterface, THandler>(this IServiceCollection collection) where THandler : Handler<THandler>, THandlerInterface where THandlerInterface : class
        {
            collection.AddScoped<IHandlerContext<THandler>, HandlerContext<THandler>>();
            collection.AddScoped<THandlerInterface, THandler>();

            return collection;
        }

        public static IServiceCollection AddSingletonFactory<T, TFactory>(this IServiceCollection collection) where T : class where TFactory : class, IServiceFactory<T>
        {
            collection.AddTransient<TFactory>();
            return AddInternal<T, TFactory>(collection, p => p.GetRequiredService<TFactory>(), ServiceLifetime.Singleton);
        }

        public static IServiceCollection AddSingletonFactory<T, TFactory>(this IServiceCollection collection, TFactory factory) where T : class where TFactory : class, IServiceFactory<T>
        {
            return AddInternal<T, TFactory>(collection, p => factory, ServiceLifetime.Singleton);
        }

        public static IServiceCollection AddScopedFactory<T, TFactory>(this IServiceCollection collection) where T : class where TFactory : class, IServiceFactory<T>
        {
            collection.AddTransient<TFactory>();
            return AddInternal<T, TFactory>(collection, p => p.GetRequiredService<TFactory>(), ServiceLifetime.Scoped);
        }

        public static IServiceCollection AddScopedFactory<T, TFactory>(this IServiceCollection collection, TFactory factory) where T : class where TFactory : class, IServiceFactory<T>
        {
            return AddInternal<T, TFactory>(collection, p => factory, ServiceLifetime.Scoped);
        }

        private static IServiceCollection AddInternal<T, TFactory>(this IServiceCollection collection, Func<IServiceProvider, TFactory> factoryProvider, ServiceLifetime lifetime) where T : class where TFactory : class, IServiceFactory<T>
        {
            Func<IServiceProvider, object> factoryFunc = provider =>
            {
                var factory = factoryProvider(provider);
                return factory.Build();
            };
            var descriptor = new ServiceDescriptor(
                typeof(T), factoryFunc, lifetime);
            collection.Add(descriptor);
            return collection;
        }
    }
}
