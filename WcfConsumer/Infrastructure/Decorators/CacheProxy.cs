using System.Linq.Expressions;
using System.Reflection;
using WcfConsumer.Application.Interfaces;

namespace WcfConsumer.Infrastructure.Decorators
{
    public class CacheProxy<TService> : DispatchProxy where TService : class
    {
        public TService? Decorated { get; set; }
        public ICacheService? CacheService { get; set; }
        public ILogger? Logger { get; set; }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if (targetMethod == null) throw new ArgumentNullException(nameof(targetMethod));
            if (Decorated == null) throw new InvalidOperationException("Decorated service is null");

            Console.WriteLine($"DispatchProxy Invoked: {targetMethod.Name}");

            var returnType = targetMethod.ReturnType;

            if (!returnType.IsGenericType || returnType.GetGenericTypeDefinition() != typeof(Task<>))
            {
                return targetMethod.Invoke(Decorated, args);
            }

            var cacheAttr = targetMethod.GetCustomAttribute<CacheableAttribute>();
            if (cacheAttr == null)
            {
                var implMethod = Decorated.GetType().GetMethod(
                    targetMethod.Name,
                    targetMethod.GetParameters().Select(p => p.ParameterType).ToArray()
                );

                cacheAttr = implMethod?.GetCustomAttribute<CacheableAttribute>();
            }

            if (cacheAttr == null)
            {
                return targetMethod.Invoke(Decorated, args);
            }

            var resultType = returnType.GetGenericArguments()[0];

            var key = $"{typeof(TService).Name}.{targetMethod.Name}:" +
                      (args != null ? string.Join("_", args.Select(a => a?.ToString() ?? "null")) : "");

            var method = typeof(CacheHelper).GetMethod(nameof(CacheHelper.GetOrSetAsync))!
                .MakeGenericMethod(resultType);

            var factory = CreateFactory(resultType, Decorated, targetMethod, args);

            return method.Invoke(null, new object?[]
            {
                CacheService!,
                key,
                Logger!,
                factory,
                TimeSpan.FromMinutes(cacheAttr.ExpireMinutes)
            });
        }

        private object CreateFactory(Type resultType, object decorated, MethodInfo targetMethod, object?[]? args)
        {
            var helperMethod = typeof(CacheProxy<TService>)
                .GetMethod(nameof(InvokeDecoratedMethodGeneric), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(resultType);

            var decoratedConst = Expression.Constant(decorated);
            var methodConst = Expression.Constant(targetMethod);
            var argsConst = Expression.Constant(args, typeof(object[]));

            var call = Expression.Call(helperMethod, decoratedConst, methodConst, argsConst);

            var funcType = typeof(Func<>).MakeGenericType(typeof(Task<>).MakeGenericType(resultType));
            var lambda = Expression.Lambda(funcType, call);

            var compiled = lambda.Compile(); // returns Delegate
            return compiled;
        }
        private static async Task<T> InvokeDecoratedMethodGeneric<T>(object decorated, MethodInfo method, object?[]? args)
        {
            var task = (Task<T>)method.Invoke(decorated, args)!;
            return await task.ConfigureAwait(false);
        }
    }
}
