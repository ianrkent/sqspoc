using System;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using PoC.Sqs.Core;
using PoC.Sqs.Core.Adapters.Configuration;
using PoC.Sqs.Core.Adapters.DataStore;
using PoC.Sqs.Core.Adapters.Messaging;

namespace PoC.Sqs.Tests.Outer
{
    public class OuterTestBase  
    {
        private readonly Dictionary<Type, object> _adapterDependencies = new Dictionary<Type, object>();

        [OneTimeSetUp]
        public void OneTimeSetup() 
        {
            IoCConfig.BuildContainer();
            InjectAdapterSubstitutes();
        }

        private void InjectAdapterSubstitutes()
        {
            InjectAdapterSubstitute<IServiceConfiguration>();
            InjectAdapterSubstitute<IStockQuantityQuery>();
            InjectAdapterSubstitute<IStockQuantityCommand>();
            InjectAdapterSubstitute<IAzureTopicSubscriber>();
        }

        private void InjectAdapterSubstitute<TService>() where TService : class 
        {
            var substitute = Substitute.For<TService>();
            IoCConfig.Inject(substitute);
            _adapterDependencies[typeof(TService)] = substitute;
        }

        protected TService AdapterSubstitute<TService>() where TService : class
        {
            return _adapterDependencies[typeof (TService)] as TService;
        }
    }
}