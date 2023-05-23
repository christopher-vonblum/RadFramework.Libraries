using System;
using System.Collections;
using System.Collections.Generic;
using RadFramework.Libraries.Reflection.Caching;

namespace RadFramework.Libraries.Ioc.Factory
{
    public class InjectionOptions
    {
        public Func<IEnumerable<CachedConstructorInfo>, CachedConstructorInfo> ChooseInjectionConstructor { get; set; }
        public Func<IEnumerable<CachedPropertyInfo>, IEnumerable<CachedPropertyInfo>> ChooseInjectionProperties { get; set; }
        public Func<IEnumerable<CachedMethodInfo>, IEnumerable<CachedMethodInfo>> ChooseInjectionMethods { get; set; }
        public InjectionOptions Clone()
        {
            return new InjectionOptions()
            {
                ChooseInjectionConstructor = ChooseInjectionConstructor,
                ChooseInjectionMethods = ChooseInjectionMethods,
                ChooseInjectionProperties = ChooseInjectionProperties
            };
        }
    }
}