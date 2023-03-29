namespace SimpleHostContainer
{
    /// <summary>
    /// 启动选项
    /// </summary>
    public class StartOptions
    {
        /// <summary>
        /// 获取或设置一个布尔值来表示是否激活插件功能
        /// </summary>
        public bool IsEnablePlugins { get; set; }
        /// <summary>
        /// 获取或设置插件根目录
        /// </summary>
        public string PluginsRootPath { get; set; } = "Plugins";
    }
}
