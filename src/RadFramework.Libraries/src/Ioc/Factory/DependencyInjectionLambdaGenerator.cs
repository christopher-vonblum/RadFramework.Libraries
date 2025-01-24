using System.Linq.Expressions;
using System.Reflection;
using RadFramework.Libraries.Reflection.Caching;
using RadFramework.Libraries.Reflection.Caching.Queries;

namespace RadFramework.Libraries.Ioc.Factory
{
    public class DependencyInjectionLambdaGenerator
    {
        private static CachedMethodInfo dependencyMethod;
        
        static DependencyInjectionLambdaGenerator()
        {
            CachedType argType = typeof (IocContainer);

            dependencyMethod = argType
                .Query(t => t.GetMethod(nameof(IocContainer.Resolve), new Type[] {typeof(Type)}));
        }

        public Func<IocContainer, object> CreateConstructorInjectionLambda(CachedConstructorInfo injectionConstructor)
        {
            Type returnType = typeof (object);

            ParameterExpression containerArg = Expression.Parameter(typeof(IocContainer), "container");
            ParameterExpression constructionResult = Expression.Variable(returnType, "constructionResult");

            var returnLabel = Expression.Label(returnType, "returnLabel");

            List<Expression> methodBody = new List<Expression>
                                          {
                                              Expression.Assign(constructionResult,
                                                  Expression.New(injectionConstructor,
                                                      BuildInjectionLambdaArguments(
                                                          containerArg,
                                                          injectionConstructor.Query(MethodBaseQueries.GetParameters).Select(p => (CachedParameterInfo)p).ToArray()))),

                                              Expression.Return(returnLabel, constructionResult, returnType),
                                              Expression.Label(returnLabel, constructionResult)
                                          };

            return Expression
                .Lambda<Func<IocContainer, object>>(Expression.Block(new List<ParameterExpression> {constructionResult}, methodBody), containerArg)
                .Compile();
        }

        public Action<IocContainer, object> CreateMethodInjectionLambda(Type targetType, CachedMethodInfo injectionMethod)
        {
            ParameterExpression containerArg = Expression.Parameter(typeof(IocContainer), "container");
            ParameterExpression injectionTarget = Expression.Parameter(typeof(object), "injectionTarget");
            ParameterExpression typedInjectionTarget = Expression.Variable(targetType, "typedInjectionTarget");


            List<Expression> methodBody = new List<Expression>
                                          {
                                                Expression.Assign(typedInjectionTarget, Expression.Convert(injectionTarget, targetType)),
                                                Expression.Call(typedInjectionTarget, 
                                                    injectionMethod,
                                                    BuildInjectionLambdaArguments(
                                                        containerArg,
                                                        injectionMethod.Query(MethodBaseQueries.GetParameters).Select(p => (CachedParameterInfo)p).ToArray()))
                                          };


            return Expression
                .Lambda<Action<IocContainer, object>>(Expression.Block(new [] { typedInjectionTarget }, methodBody), containerArg, injectionTarget)
                .Compile();
        }

        public Action<IocContainer, object> CreatePropertyInjectionLambda(Type targetType, CachedPropertyInfo[] injectionProperties)
        {
            ParameterExpression containerArg = Expression.Parameter(typeof(IocContainer), "container");
            ParameterExpression injectionTarget = Expression.Parameter(typeof(object), "injectionTarget");

            ParameterExpression typedInjectionTarget = Expression.Parameter(targetType, "typedInjectionTarget");

            List<Expression> injectionExpressions = new List<Expression>
                                                    {
                                                        Expression.Assign(typedInjectionTarget, Expression.Convert(injectionTarget, targetType))
                                                    };

            foreach (PropertyInfo propertyInfo in injectionProperties)
            {
                MemberExpression propertyExpression = Expression.Property(typedInjectionTarget, propertyInfo);

                Expression argInjectionPlaceholder = ResolveDependency(containerArg, propertyInfo.PropertyType);

                injectionExpressions.Add(Expression.Assign(propertyExpression, argInjectionPlaceholder));
            }

            return Expression
                    .Lambda<Action<IocContainer, object>>(Expression.Block(new[] { typedInjectionTarget }, injectionExpressions), containerArg, injectionTarget)
                    .Compile();
        }
        
        private static Expression[] BuildInjectionLambdaArguments(Expression containerInstance, CachedParameterInfo[] parameterInfos)
        {
            List<Expression> arguments = new List<Expression>();

            foreach (CachedParameterInfo parmeterInfo in parameterInfos)
            {
                arguments.Add(ResolveDependency(containerInstance, parmeterInfo.InnerMetaData.ParameterType));
            }

            return arguments.ToArray();
        }

        private static Expression ResolveDependency(Expression instance, Type placeholderType)
        {
            return Expression.Convert(Expression.Call(instance, dependencyMethod, Expression.Constant(placeholderType)), placeholderType);
        }
    }
}