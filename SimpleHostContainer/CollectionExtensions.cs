using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHostContainer
{
    /// <summary>
    /// 集合扩展类
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// ForEach方法
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="collection">元素集合</param>
        /// <param name="delegate">处理委托</param>
        /// <exception cref="ArgumentNullException"/>
        public static void ForEach<T>(this IEnumerable<T> collection, EnumerateDelegate<T> @delegate)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (@delegate == null) throw new ArgumentNullException(nameof(@delegate));

            foreach (var item in collection)
            {
                @delegate(item);
            }
        }

        /// <summary>
        /// ForEach异步方法
        /// </summary>
        /// <typeparam name="T">集合元素类型</typeparam>
        /// <param name="collection">元素集合</param>
        /// <param name="delegate">异步委托</param>
        /// <returns>异步实例</returns>
        /// <exception cref="ArgumentNullException"/>
        public static async Task ForEachAsync<T>(this IEnumerable<T> collection, EnumerateAsyncDelegate<T> @delegate)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (@delegate == null) throw new ArgumentNullException(nameof(@delegate));

            foreach (T item in collection)
            {
                await @delegate(item);
            }
        }

        /// <summary>
        /// 获取字符串资源
        /// </summary>
        /// <param name="resources">字符串资源集合</param>
        /// <param name="culture">语言信息</param>
        /// <returns>字符串资源</returns>
        public static StringResource GetResource(this IEnumerable<StringResource> resources, CultureInfo culture)
        {
            var @default = (from i in resources where i.Culture == culture select i).FirstOrDefault();

            if (@default == null)
            {
                @default = (from i in resources where i.IsDefault select i).FirstOrDefault();
            }

            if (@default == null)
            {
                @default = (from i in resources select i).FirstOrDefault();
            }

            return @default;
        }

        /// <summary>
        /// For循环方法
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="collection">元素集合</param>
        /// <param name="begin">起始索引</param>
        /// <param name="end">终止索引</param>
        /// <param name="delegate">处理委托</param>
        /// <param name="step">步长（默认为“1”）</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public static void For<T>(this IEnumerable<T> collection, long begin, long end, EnumerateDelegate<T> @delegate, long step = 1)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (@delegate == null) throw new ArgumentNullException(nameof(@delegate));
            if (begin < 0 || end < 0) throw new ArgumentOutOfRangeException();

            T[] array = collection.ToArray();

            if (begin <= end)
            {
                if (end > array.LongLength) throw new ArgumentOutOfRangeException(nameof(end));

                for (; begin < end; begin += step)
                {
                    T item = array[begin];
                    @delegate(item);
                }
            }
            else
            {
                if (begin >= array.LongLength) throw new ArgumentOutOfRangeException(nameof(begin));

                for (; begin > end; begin -= step)
                {
                    T item = array[begin];
                    @delegate(item);
                }
            }
        }

        /// <summary>
        /// For循环异步方法
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="collection">元素集合</param>
        /// <param name="begin">起始索引</param>
        /// <param name="end">终止索引</param>
        /// <param name="delegate">处理委托</param>
        /// <param name="step">步长（默认为“1”）</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public static async Task ForAsync<T>(this IEnumerable<T> collection, long begin, long end, EnumerateAsyncDelegate<T> @delegate, long step = 1)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (@delegate == null) throw new ArgumentNullException(nameof(@delegate));
            if (begin < 0 || end < 0) throw new ArgumentOutOfRangeException();

            T[] array = collection.ToArray();

            if (begin <= end)
            {
                if (end > array.LongLength) throw new ArgumentOutOfRangeException(nameof(end));

                for (; begin < end; begin += step)
                {
                    T item = array[begin];
                    await @delegate(item);
                }
            }
            else
            {
                if (begin >= array.LongLength) throw new ArgumentOutOfRangeException(nameof(begin));

                for (; begin > end; begin -= step)
                {
                    T item = array[begin];
                    await @delegate(item);
                }
            }
        }

        /// <summary>
        /// While循环方法
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="collection">元素集合</param>
        /// <param name="delegate">处理委托</param>
        /// <exception cref="ArgumentNullException"/>
        public static void While<T>(this IEnumerable<T> collection, EnumerateDelegate<T> @delegate)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (@delegate == null) throw new ArgumentNullException(nameof(@delegate));

            IEnumerator<T> enumerator = collection.GetEnumerator();

            while (enumerator.MoveNext())
            {
                T item = enumerator.Current;

                @delegate(item);
            }
        }

        /// <summary>
        /// While循环异步方法
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="collection">元素集合</param>
        /// <param name="delegate">处理委托</param>
        /// <returns>异步实例</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static async Task WhileAsync<T>(this IEnumerable<T> collection, EnumerateAsyncDelegate<T> @delegate)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (@delegate == null) throw new ArgumentNullException(nameof(@delegate));

            IEnumerator<T> enumerator = collection.GetEnumerator();

            while (enumerator.MoveNext())
            {
                T item = enumerator.Current;

                await @delegate(item);
            }
        }

        /// <summary>
        /// 判断是否存在描述方法
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/>实例</param>
        /// <param name="type">服务类型</param>
        /// <param name="lifetime">生命周期</param>
        /// <returns>存在则返回True</returns>
        public static bool HasDescriptor(this IServiceCollection services, Type type, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            return services.Any(D => D.ImplementationType == type && D.ServiceType == type && D.Lifetime == lifetime);
        }

        /// <summary>
        /// 判断是否存在描述方法
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/>实例</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="implementType">实现类型</param>
        /// <param name="lifetime">生命周期</param>
        /// <returns>存在则返回True</returns>
        public static bool HasDescriptor(this IServiceCollection services, Type serviceType, Type implementType, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            return services.Any(D => D.ServiceType == serviceType && D.ImplementationType == implementType && D.Lifetime == lifetime);
        }

        /// <summary>
        /// 注册类型
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/>实例</param>
        /// <param name="type">服务类型</param>
        /// <param name="delegate">注册委托</param>
        /// <returns><see cref="IServiceCollection"/>实例</returns>
        public static IServiceCollection EnrollType(this IServiceCollection services, Type type, Func<Type, IServiceCollection> @delegate)
        {
            ServiceLifetime lifetime = ServiceLifetime.Transient;
            string name = @delegate.Method.Name;

            switch (name)
            {
                case "AddSingleton":
                    break;
                case "AddScoped":
                    break;
                case "AddTransient":
                    break;
                default:
                    break;
            }

            if (!services.HasDescriptor(type, lifetime))
            {
                return @delegate(type);
            }
            else
            {
                return services;
            }
        }

        /// <summary>
        /// 注册类型
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/>实例</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="implementType">实现类型</param>
        /// <param name="delegate">注册委托</param>
        /// <returns><see cref="IServiceCollection"/>实例</returns>
        public static IServiceCollection EnrollType(this IServiceCollection services, Type serviceType, Type implementType, Func<Type, Type, IServiceCollection> @delegate)
        {
            ServiceLifetime lifetime = ServiceLifetime.Transient;
            string name = @delegate.Method.Name;

            switch (name)
            {
                case "AddSingleton":
                    break;
                case "AddScoped":
                    break;
                case "AddTransient":
                    break;
                default:
                    break;
            }

            if (!services.HasDescriptor(serviceType, implementType, lifetime))
            {
                return @delegate(serviceType, implementType);
            }
            else
            {
                return services;
            }
        }

        /// <summary>
        /// 注册类型
        /// </summary>
        /// <param name="types">类型集合</param>
        /// <param name="builder"><see cref="IHostBuilder"/>实例</param>
        public static void Enroll(this IEnumerable<Type> types, IHostBuilder builder)
        {
            var singletons = from i in types where typeof(ISingleton).IsAssignableFrom(i) && i.IsClass && !i.IsInterface && !i.IsGenericType && !i.IsAbstract select i;
            var scopeds = from i in types where typeof(IScoped).IsAssignableFrom(i) && i.IsClass && !i.IsInterface && !i.IsGenericType && !i.IsAbstract select i;
            var transients = from i in types where typeof(ITransient).IsAssignableFrom(i) && i.IsClass && !i.IsInterface && !i.IsGenericType && !i.IsAbstract select i;

            builder.ConfigureServices(services =>
            {
                singletons.ForEach(T =>
                {
                    IoCAttribute attribute = T.GetCustomAttribute<IoCAttribute>();

                    if (attribute == null)
                    {
                        services.EnrollType(T, services.AddSingleton);
                    }
                    else
                    {
                        attribute.ServieTypes.ForEach(ST =>
                        {
                            services.EnrollType(ST, T, services.AddSingleton);
                        });
                    }
                });

                scopeds.ForEach(T =>
                {
                    IoCAttribute attribute = T.GetCustomAttribute<IoCAttribute>();

                    if (attribute == null)
                    {
                        services.EnrollType(T, services.AddScoped);
                    }
                    else
                    {
                        attribute.ServieTypes.ForEach(ST =>
                        {
                            services.EnrollType(ST, T, services.AddScoped);
                        });
                    }
                });

                transients.ForEach(T =>
                {
                    IoCAttribute attribute = T.GetCustomAttribute<IoCAttribute>();

                    if (attribute == null)
                    {
                        services.EnrollType(T, services.AddTransient);
                    }
                    else
                    {
                        attribute.ServieTypes.ForEach(ST =>
                        {
                            services.EnrollType(ST, T, services.AddTransient);
                        });
                    }
                });
            });
        }

        /// <summary>
        /// 弹出栈方法
        /// </summary>
        /// <typeparam name="T">栈元素类型</typeparam>
        /// <param name="stack">栈</param>
        /// <param name="delegate">委托</param>
        public static void Pop<T>(this Stack<T> stack, EnumerateDelegate<T> @delegate)
        {
            while (stack.Count > 0)
            {
                T item = stack.Pop();

                @delegate(item);
            }
        }

        /// <summary>
        /// 弹出栈的异步方法
        /// </summary>
        /// <typeparam name="T">栈元素类型</typeparam>
        /// <param name="stack">栈</param>
        /// <param name="delegate">委托</param>
        /// <returns>异步实例</returns>
        public static async Task PopAsync<T>(this Stack<T> stack, EnumerateAsyncDelegate<T> @delegate)
        {
            while (stack.Count > 0)
            {
                T item = stack.Pop();

                await @delegate(item);
            }
        }

        /// <summary>
        /// 获取<see cref="ISetupInfo"/>实例集合
        /// </summary>
        /// <param name="types">类型集合</param>
        /// <returns><see cref="SetupInfoGroupQueryItem"/>数组</returns>
        internal static SetupInfoGroupQueryItem[] GetSetupInfoTypes(this IEnumerable<Type> types)
        {
            var list = from i in types
                       where typeof(ISetupInfo).IsAssignableFrom(i) && !i.IsInterface && !i.IsAbstract && i.IsClass
                       group i by i.Assembly into g
                       select new SetupInfoGroupQueryItem(g.Key, g.ToArray());

            return list.ToArray();
        }
    }
}
