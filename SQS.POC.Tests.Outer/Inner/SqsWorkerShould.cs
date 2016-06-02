using NSubstitute;
using NUnit.Framework;
using SQS.POC.Core;
using SQS.POC.Core.Adapters.Configuration;

namespace SQS.POC.Tests.Inner
{
    [TestFixture]
    public class SqsWorkerShould : AutoSubstituteFixture<SqsWorker>
    {
        [Test]
        public void IdentifyItsWarehousesFromConfiguration()
        {
            Target.Start();

            GetDependencyAsSubstitute<IServiceConfiguration>().Received().GetConfigSetting(ConfigSettingKeys.WarehouseList);
        }
    }
}
