using Autofac;
using PoC.Sqs.Core.Adapters;
using PoC.Sqs.Core.Adapters.Configuration;
using PoC.Sqs.Core.Adapters.DataStore;
using PoC.Sqs.Core.Adapters.DataStore.DocumentDb;
using PoC.Sqs.Core.Adapters.Messaging;
using PoC.Sqs.Core.Messaging;
using PoC.Sqs.Core.Messaging.Handlers;

namespace PoC.Sqs.Core
{
    public static class IoCConfig
    {
        public static IContainer Container { get; private set; }

        public static void BuildContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<SqsMessagingWorker>();
            builder.RegisterType<StockChangeHandler>().As<IMessageHandler<StockChangeEventV1>>();

            RegisterAdapterServices(builder);

            Container = builder.Build();
        }

        private static void RegisterAdapterServices(ContainerBuilder builder)
        {
            builder.RegisterType<AppSettingsConfiguration>().As<IServiceConfiguration>();
            builder.RegisterType<StockQuantityQuery>().As<IStockQuantityQuery>();
            builder.RegisterType<StockQuantityCommand>().As<IStockQuantityCommand>();
            builder.RegisterType<AzureTopicSubscriber>().As<IAzureTopicSubscriber>();

            builder.RegisterType<DocumentDbHealthChecker>().As<IDependencyHealthCheck>();  // currently only using the one type of health checker.
        }

        public static TService ResolveService<TService>()
        {
            return Container.Resolve<TService>();
        }

        public static void Inject<TService>(TService instance) where TService : class
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(instance)
                .As<TService>()
                .ExternallyOwned();
            builder.Update(Container);
        }
    }
}