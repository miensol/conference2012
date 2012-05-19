using NUnit.Framework;
using Nito.AsyncEx;

namespace DeepDive
{
    [TestFixture]
    public abstract class BaseAsyncTest
    {
        [SetUp]
        public void SetUp()
        {
            AsyncContext.Run(Because);
        }

        protected abstract void Because();

        [Test]
        public void Finshed()
        {            
        }
    }
}
