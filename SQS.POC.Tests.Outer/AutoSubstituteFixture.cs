using AutofacContrib.NSubstitute;
using NUnit.Framework;

namespace SQS.POC.Tests
{
    [TestFixture]
    public abstract class AutoSubstituteFixture<T>
    {
        protected T Target;
        private readonly AutoSubstitute _autoSubstitute = new AutoSubstitute();

        [SetUp]
        public void Setup()
        {
            Target = _autoSubstitute.Resolve<T>();
        }

        protected T GetDependencyAsSubstitute<T>()
        {
            return _autoSubstitute.Resolve<T>();
        }
    }
}