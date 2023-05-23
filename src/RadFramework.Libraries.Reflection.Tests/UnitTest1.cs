using System.Reflection;
using NUnit.Framework;
using RadFramework.Libraries.Reflection.Caching;
using RadFramework.Libraries.Reflection.Caching.Queries;

namespace RadFramework.Libraries.Reflection.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }
        
        [Test]
        public void QueryType()
        {
            var publicProperties =
                ((CachedType) typeof(TestType<,>)).Query(ClassQueries.GetPublicImplementedProperties);
            ;
        }
        
        [Test]
        public void QueryType_Constructors()
        {
            var publicProperties =
                ((CachedType) typeof(TestType<,>)).Query(ClassQueries.GetPublicConstructors);
            ;
        }

        [Test]
        public void QueryInterface()
        {
            var publicProperties=
                ((CachedType) typeof(TestInterface)).Query(InterfaceQueries.GetProperties);
        }
        
        [Test]
        public void QueryAssembly()
        {
            var publicProperties =
                ((CachedAssembly) Assembly.GetExecutingAssembly()).Query(AssemblyQueries.GetTypes);
            ;
        }

        [Test]
        public void Assumption_Method()
        {
            var x = typeof(TestType<,>);

            var xConstructed = typeof(TestType<string, string>);

            CachedMethodInfo testMethod2 = x.GetMethod("TestMethod2");

            var parameters = testMethod2.Query(MethodBaseQueries.GetParameters);
        }
        
        [Test]
        public void Assumption_MethodOfGenericTypeHasDifferentToken()
        {
            var x = typeof(TestType<,>);

            var xConstructed = typeof(TestType<string, string>);

            var testMethod2 = x.GetMethod("TestMethod2");

            var testMethod2c = xConstructed.GetMethod("TestMethod2");

            Assert.AreNotEqual(ReflectionCache.CurrentCache.BuildMethodKey(testMethod2),
                ReflectionCache.CurrentCache.BuildMethodKey(testMethod2c));
            ;
        }
    }

    public interface BaseInterface
    {
        string Prop1 { get; set; }
    }
    public interface TestInterface : BaseInterface
    {
        string Prop2 { get; set; }
    }
    
    public class TestType<T, T2>
    {
        public T Prop { get; set; }
        public T TestMethod()
        {
            return default(T);
        }

        public T3 TestMethod2<T3>(T t)
        {
            return default(T3);
        }

    }
}