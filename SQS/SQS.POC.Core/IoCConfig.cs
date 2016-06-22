using Autofac;
using SQS.POC.Core.Adapters.Configuration;
using SQS.POC.Core.Adapters.DataStore;
using SQS.POC.Core.Adapters.DataStore.DocumentDb;
using SQS.POC.Core.Adapters.Messaging;
using SQS.POC.Core.Messages;

namespace SQS.POC.Core
{
    public static class IoCConfig
    {
        private static IContainer Container { get; set; }

        public static void BuildContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<SqsWorker>();
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