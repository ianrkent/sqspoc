using AutofacContrib.NSubstitute;
using NUnit.Framework;

namespace PoC.Sqs.Tests.Inner
{
    [TestFixture]
    public abstract class AutoSubstituteTestBase<TSut>
    {
        protected TSut Sut;
        private readonly AutoSubstitute _autoSubstitute = new AutoSubstitute();

        [SetUp]
        public virtual void Setup()
        {
            Sut = _autoSubstitute.Resolve<TSut>();
        }

        protected TDependency TheDependency<TDependency>()
        {
            return _autoSubstitute.Resolve<TDependency>();
        }
    }
}