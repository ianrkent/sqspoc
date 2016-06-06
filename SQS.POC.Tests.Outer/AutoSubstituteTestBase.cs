using AutofacContrib.NSubstitute;
using NUnit.Framework;

namespace SQS.POC.Tests
{
    [TestFixture]
    public abstract class AutoSubstituteTestBase<TSut>
    {
        protected TSut Sut;
        private readonly AutoSubstitute _autoSubstitute = new AutoSubstitute();

        [SetUp]
        public void Setup()
        {
            Sut = _autoSubstitute.Resolve<TSut>();
        }

        protected TDependency TheDependency<TDependency>()
        {
            return _autoSubstitute.Resolve<TDependency>();
        }
    }
}