using NUnit.Framework;
using RadFramework.Libraries.DataTypes;
using RadFramework.Libraries.Threading.Internals.ThreadAffinity;

namespace RadFramework.Libraries.Threading.Tests
{
    [TestFixture]
    public class CpuAffinityTests
    {
        [Test]
        public void Test()
        {
            BitMask mask = new BitMask();
            
            mask.Set(1);
            mask.Set(2);
            mask.Set(5);
            mask.Set(6);

            Assert.IsTrue(mask.IsSet(1));
            Assert.IsTrue(mask.IsSet(2));
            Assert.IsTrue(!mask.IsSet(3));
            Assert.IsTrue(!mask.IsSet(4));
            Assert.IsTrue(mask.IsSet(5));
            Assert.IsTrue(mask.IsSet(6));
            Assert.IsTrue(!mask.IsSet(7));
            Assert.IsTrue(!mask.IsSet(8));
            ;
        }
    }
}