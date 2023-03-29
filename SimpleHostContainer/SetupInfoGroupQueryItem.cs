using System;
using System.Reflection;

namespace SimpleHostContainer
{
    /// <summary>
    /// 安装信息分组查询垫片类
    /// </summary>
    internal class SetupInfoGroupQueryItem
    {
        /// <summary>
        /// 安装信息分组查询垫片类构造方法
        /// </summary>
        /// <param name="assembly">程序集实例</param>
        /// <param name="types">安装类型列表</param>
        public SetupInfoGroupQueryItem(Assembly assembly, params Type[] types)
        {
            Assembly = assembly;
            Types = types;
        }

        /// <summary>
        /// 获取程序集实例
        /// </summary>
        public Assembly Assembly { get; private set; }

        /// <summary>
        /// 获取安装信息类型实例
        /// </summary>
        public Type[] Types { get; private set; }
    }
}
