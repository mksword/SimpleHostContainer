using Microsoft.Extensions.DependencyInjection;

namespace SimpleHostContainer
{
    /// <summary>
    /// 安装信息接口
    /// </summary>
    public interface ISetupInfo
    {
        /// <summary>
        /// 设置服务
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/>实例</param>
        void ConfigureServices(IServiceCollection services);
    }
}
