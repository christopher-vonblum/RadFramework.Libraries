using System.Linq;
using NUnit.Framework;
using RadFramework.Libraries.Reflection.Caching;
using RadFramework.Libraries.Reflection.Caching.Queries;

namespace RadFramework.Libraries.Reflection.Tests
{
    public class ReflectionCacheTest
    {
        public enum TestEnumType
        {
            A1,
            B2,
            C3
        }

        [Test]
        public void GetCachedType_MatchesTestType()
        {
            CachedType cachedType = ReflectionCache.CurrentCache.GetCachedMetaData(TestReflectionCacheTypes.TestType);

            Assert.IsTrue(cachedType.Query(AttributeLocationQueries.GetAttributes).Length == 1);

            Assert.IsTrue(cachedType.Query(ClassQueries.GetPublicFields).Count() == 2);

            Assert.IsTrue(cachedType.Query(ClassQueries.GetPublicImplementedProperties).Count() == 2);

            Assert.IsTrue(cachedType.Query(ClassQueries.GetPublicImplementedMethods).Count() == 2);

            Assert.IsTrue(cachedType.Query(ClassQueries.GetPublicImplementedEvents).Count() == 2);

            Assert.IsTrue(cachedType.Query(ClassQueries.GetPublicImplementedMethods).Count() == 2);

            Assert.IsTrue(cachedType.Query(ClassQueries.GetGenericArguments).Length == 1);
        }
    }
}