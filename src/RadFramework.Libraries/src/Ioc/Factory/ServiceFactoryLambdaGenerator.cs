using System.Linq.Expressions;
using RadFramework.Libraries.Reflection.Caching;
using RadFramework.Libraries.Reflection.Caching.Queries;

namespace RadFramework.Libraries.Ioc.Factory
{
    public class ServiceFactoryLambdaGenerator
    {
        private DependencyInjectionLambdaGenerator lambdaGenerator = new DependencyInjectionLambdaGenerator();

        public Func<Container, object> CreateInstanceFactory(CachedType type, InjectionOptions containerInjectionOptions, InjectionOptions injectionOptions)
        {
            CachedConstructorInfo constructor =
                (injectionOptions.ChooseInjectionConstructor ?? containerInjectionOptions.ChooseInjectionConstructor)(
                    type
                        .Query(ClassQueries.GetPublicConstructors)
                        .Select(c => (CachedConstructorInfo) c));

            var constructLamda = constructor.Query(info => lambdaGenerator.CreateConstructorInjectionLambda(info));
            
            List<Action<Container, object>> injectionLambdas = new List<Action<Container, object>>();

            var chooseInjectionMethods = injectionOptions.ChooseInjectionMethods ??
                                          containerInjectionOptions.ChooseInjectionMethods;
            
            if (chooseInjectionMethods != null)
            {
                var injectionMethods = chooseInjectionMethods(type
                    .Query(ClassQueries.GetPublicImplementedMethods).Select(m => (CachedMethodInfo) m));

                foreach (var cachedMethodInfo in injectionMethods)
                {
                    var methodInjectionLambda = cachedMethodInfo.Query(m =>
                        lambdaGenerator.CreateMethodInjectionLambda(type, cachedMethodInfo));
                    
                    injectionLambdas.Add(methodInjectionLambda);
                }
            }

            var chooseInjectionProperties = injectionOptions.ChooseInjectionProperties ??
                                            containerInjectionOptions.ChooseInjectionProperties;
            
            if (chooseInjectionProperties != null)
            {
                var injectionProperties = chooseInjectionProperties(type.Query(ClassQueries.GetPublicImplementedProperties).Select(p =>(CachedPropertyInfo)p)).ToArray();

                if (injectionProperties.Length > 0)
                {
                    injectionLambdas.Add(lambdaGenerator.CreatePropertyInjectionLambda(type, injectionProperties));
                }
            }

            return CombineConstructorInjectionAndMemberInjectionLambdas(constructLamda, injectionLambdas.ToArray());
        }
        public Func<Container, object> CombineConstructorInjectionAndMemberInjectionLambdas(Func<Container, object> constructorInjectionLambda, Action<Container, object>[] memberInjectionLambdas)
        {
            if (!memberInjectionLambdas.Any())
            {
                return constructorInjectionLambda;
            }

            Type returnType = typeof(object);

            ParameterExpression containerInstance = Expression.Variable(typeof(Container), "containerArg");
            ParameterExpression constructionResult = Expression.Variable(returnType, "constructionResult");

            var returnLabel = Expression.Label(returnType, "returnLabel");

            List<Expression> methodBody = new List<Expression>
            {
                Expression.Assign(constructionResult, Expression.Invoke(Expression.Constant(constructorInjectionLambda), containerInstance))
            };

            foreach (Action<Container, object> injectionLambda in memberInjectionLambdas)
            {
                methodBody.Add(Expression.Invoke(Expression.Constant(injectionLambda), containerInstance, constructionResult));
            }

            methodBody.Add(Expression.Return(returnLabel, constructionResult, returnType));
            methodBody.Add(Expression.Label(returnLabel, constructionResult));

            return Expression
                .Lambda<Func<Container, object>>(Expression.Block(new List<ParameterExpression> { constructionResult }, methodBody), containerInstance)
                .Compile();
        }
    }
}