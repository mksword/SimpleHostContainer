using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Threading.Tasks;

namespace SimpleHostContainer
{
    /// <summary>
    /// 设置日志委托
    /// </summary>
    /// <param name="builder"><see cref="ILoggingBuilder"/>实例</param>
    public delegate void ConfigureLoggingDelegate(ILoggingBuilder builder);

    /// <summary>
    /// 使用<see cref="ConfigureLoggingDelegate"/>委托
    /// </summary>
    /// <param name="delegate"><see cref="ConfigureLoggingDelegate"/>实例</param>
    /// <returns><see cref="IHostBuilder"/>实例</returns>
    public delegate IHostBuilder UseConfigureLoggingDelegateHandler(ConfigureLoggingDelegate @delegate);

    /// <summary>
    /// 设置服务委托
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/>实例</param>
    public delegate void ConfigureServicesDelegate(IServiceCollection services);

    /// <summary>
    /// 使用<see cref="ConfigureServicesDelegate"/>委托
    /// </summary>
    /// <param name="delegate"><see cref="ConfigureServicesDelegate"/>实例</param>
    /// <returns><see cref="UseConfigureLoggingDelegateHandler"/>实例</returns>
    public delegate UseConfigureLoggingDelegateHandler UseConfigureServicesDelegateHandler(ConfigureServicesDelegate @delegate);

    /// <summary>
    /// 使用程序集列表委托
    /// </summary>
    /// <returns>程序集列表</returns>
    public delegate Assembly[] UseAssembliesDelegate();

    /// <summary>
    /// 使用<see cref="UseAssembliesDelegate"/>委托
    /// </summary>
    /// <param name="delegate"><see cref="UseAssembliesDelegate"/>实例</param>
    /// <returns><see cref="UseConfigureServicesDelegateHandler"/>实例</returns>
    public delegate UseConfigureServicesDelegateHandler UseUseAssembliesDelegateHandler(UseAssembliesDelegate @delegate);

    /// <summary>
    /// 启动委托
    /// </summary>
    /// <param name="options">启动选项</param>
    /// <returns><see cref="UseUseAssembliesDelegateHandler"/>实例</returns>
    public delegate UseUseAssembliesDelegateHandler StartDelegate(StartOptions options);

    /// <summary>
    /// 枚举委托
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <param name="item">元素实例</param>
    public delegate void EnumerateDelegate<T>(T item);

    /// <summary>
    /// 枚举异步委托
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    /// <param name="item">元素实例</param>
    /// <returns>异步实例</returns>
    public delegate Task EnumerateAsyncDelegate<T>(T item);
}
