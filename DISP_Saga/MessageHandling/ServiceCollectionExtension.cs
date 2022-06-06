using MessageHandling.Abstractions;
using MessageHandling.Internal;
using MessageHandling.Internal.Wrappers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MessageHandling
{
    /// <summary>
    /// ServiceCollection extension methods
    /// </summary>
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// Add support for RabbitMQ message handlers to an ASP.NET application
        /// </summary>
        /// <param name="serviceCollection">Web application service collection</param>
        /// <returns>Web application service collection</returns>
        public static IServiceCollection AddMessageHandling(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddOptions<MessageHandlingConfiguration>()
                .Configure<IConfiguration>((options, configuration) => configuration.GetSection(MessageHandlingConfiguration.Key).Bind(options))
                .Services
                .AddSingleton<RabbitConnection>()
                .AddSingleton<IMessageListener, MessageListener>()
                .AddSingleton<IMessageProducer, MessageProducer>()
                .AddHostedService<MessageProvider>();
        }

        /// <summary>
        /// Register a message handler to the RabbitMQ message handling.
        /// </summary>
        /// <param name="serviceCollection">Web application service collection</param>
        /// <typeparam name="T"><see cref="Abstractions.IMessageHandler"/> to add as consumer.</typeparam>
        /// <returns>Web application service collection</returns>
        /// <seealso cref="MessageHandling.Abstractions.CommandHandler"/>
        /// <seealso cref="MessageHandling.Abstractions.QueryHandler"/>
        /// <seealso cref="MessageHandling.Abstractions.ResponseHandler"/>
        public static IServiceCollection AddMessageHandler<T>(this IServiceCollection serviceCollection)
            where T : class, IMessageHandler
        {
            return serviceCollection
                .AddSingleton<IMessageHandler, T>();
        }
    }
}