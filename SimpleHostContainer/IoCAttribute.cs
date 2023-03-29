using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace SimpleHostContainer
{
    /// <summary>
    /// 控制反转定义标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class IoCAttribute : Attribute
    {
        /// <summary>
        /// 控制反转定义标签构造方法
        /// </summary>
        /// <param name="serviceTypes">服务类型列表</param>
        public IoCAttribute(params Type[] serviceTypes)
        {
            var invalids = (from i in serviceTypes where !typeof(IIoC).IsAssignableFrom(i) select i).ToArray();

            if (invalids.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                bool flag = false;

                invalids.ForEach(T =>
                {
                    if (!flag)
                    {
                        flag = true;
                    }
                    else
                    {
                        sb.Append(", ");
                    }

                    sb.Append($"{T.Namespace}.{T.Name}");
                });

                var rs = StringResourceLoader.Load(typeof(IoCAttribute).Assembly).GetResource(Thread.CurrentThread.CurrentCulture);

                if (rs != null)
                {
                    throw new Exception(rs["InvalidServiceType", sb.ToString()]);
                }
                else
                {
                    throw new Exception($"Invalid Service Type : {sb.ToString()}!");
                }
            }

            ServieTypes = serviceTypes;
        }

        /// <summary>
        /// 获取服务类型数组
        /// </summary>
        public Type[] ServieTypes { get; private set; }
    }
}
