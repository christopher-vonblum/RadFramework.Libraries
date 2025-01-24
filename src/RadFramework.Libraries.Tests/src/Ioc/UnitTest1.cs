using System.Linq;
using NUnit.Framework;
using RadFramework.Libraries.Ioc.Factory;
using RadFramework.Libraries.Reflection.Caching.Queries;

namespace RadFramework.Libraries.Ioc.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Container container = new Container(new InjectionOptions
            {
                ChooseInjectionConstructor = ctors => ctors
                    .OrderByDescending(c => c.Query(MethodBaseQueries.GetParameters).Length)
                    .First(),
                ChooseInjectionMethods = infos => infos,
                ChooseInjectionProperties = infos => infos
            });
            
            container.RegisterTransient(typeof(Test));
            container.RegisterSingleton(typeof(TestDep));

            var res = container.Resolve<Test>();
            Assert.Pass();
        }
    }

    public class Test
    {
        public TestDep Dep { get; set; }
        private readonly TestDep dep;

        public Test(TestDep dep)
        {
            this.dep = dep;
        }

        private TestDep fromMethod;

        public void InjectHere(TestDep dep)
        {
            fromMethod = dep;
        }
    }

    public class TestDep
    {
        public TestDep()
        {
        }
    }
}