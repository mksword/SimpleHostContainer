using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace SimpleHostContainer
{
    /// <summary>
    /// 先决条件标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PrerequisiteAttribute : Attribute
    {
        /// <summary>
        /// 先决条件标签构造方法
        /// </summary>
        /// <param name="prerequisiteTypes">先决条件类型列表</param>
        public PrerequisiteAttribute(params Type[] prerequisiteTypes)
        {
            var types = (from i in prerequisiteTypes where !typeof(ISetupInfo).IsAssignableFrom(i) select i).ToArray();

            if (types.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                bool flag = false;

                types.ForEach(T =>
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

                var rs = StringResourceLoader.Load(typeof(PrerequisiteAttribute).Assembly).GetResource(Thread.CurrentThread.CurrentCulture);

                if (rs != null)
                {
                    throw new Exception(rs["InvalidSetupInfoType", sb.ToString()]);
                }
                else
                {
                    throw new Exception($"Invalid SetupInfo Type [{sb.ToString()}]");
                }
            }

            PrerequisiteTypes = prerequisiteTypes;
        }

        /// <summary>
        /// 获取先决条件类列表
        /// </summary>
        public Type[] PrerequisiteTypes { get; private set; }
    }
}
