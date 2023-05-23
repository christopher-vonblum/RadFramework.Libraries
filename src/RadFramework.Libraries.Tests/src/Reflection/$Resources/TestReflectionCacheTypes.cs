
using System;

#pragma warning disable 67

namespace RadFramework.Libraries.Reflection.Tests
{
    public static class TestReflectionCacheTypes
    {
        public static readonly Type TestType = typeof (ReflectionCacheTestType<string>);

        public static Type TestInterfaceType => typeof (ITestReflectionCacheInterface);

        [TestAnnotation]
        public class ReflectionCacheTestType<T1> : ITestReflectionCacheInterface
        {
            [TestAnnotation] public string Field;

            public string Field2;

            [TestAnnotation]
            public ReflectionCacheTestType()
            {
            }

            public ReflectionCacheTestType(string parameter)
            {
            }

            [TestAnnotation]
            public string Property { get; set; }

            public string Property2 { get; set; }

            [TestAnnotation]
            public event Action<string> Event;

            public event Action<string> Event2;

            [TestAnnotation]
            public void Method()
            {
            }

            public void Method(string parameter)
            {
            }
        }

        public interface ITestReflectionCacheInterface
        {
            [TestAnnotation]
            string Property { get; set; }

            string Property2 { get; set; }

            [TestAnnotation]
            event Action<string> Event;

            event Action<string> Event2;

            [TestAnnotation]
            void Method();

            void Method(string parameter);
        }

        public class TestAnnotationAttribute : Attribute
        {
        }
    }
}