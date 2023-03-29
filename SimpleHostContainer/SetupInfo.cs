using Microsoft.Extensions.DependencyInjection;

namespace SimpleHostContainer
{
    /// <summary>
    /// 安装信息抽像类
    /// </summary>
    public abstract class SetupInfo : ISetupInfo
    {
        /// <inheritdoc/>
        public virtual void ConfigureServices(IServiceCollection services)
        {
        }
    }
}
