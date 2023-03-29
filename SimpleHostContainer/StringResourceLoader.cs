using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

namespace SimpleHostContainer
{
    /// <summary>
    /// 字符串资源加载器
    /// </summary>
    public static class StringResourceLoader
    {
        private static readonly Regex REGEX = new Regex(@"^Resources\.(.*).json$", RegexOptions.IgnoreCase);
        /// <summary>
        /// 加载字符串资源
        /// </summary>
        /// <param name="assembly">程序集实例</param>
        /// <returns><see cref="StringResource"/>数组</returns>
        public static StringResource[] Load(Assembly assembly)
        {
            IFileProvider fileProvider = new EmbeddedFileProvider(assembly);

            var contents = fileProvider.GetDirectoryContents("/");

            List<StringResource> list = new List<StringResource>();

            foreach (var content in contents)
            {
                if (REGEX.IsMatch(content.Name))
                {
                    string name = REGEX.Match(content.Name).Groups[1].Value;

                    using (Stream stream = content.CreateReadStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string payload = reader.ReadToEnd();

                            StringResource resource = JsonConvert.DeserializeObject<StringResource>(payload);

                            if (name.Equals("Resources", StringComparison.OrdinalIgnoreCase))
                            {
                                StringResource.Helper.SetIsDefault(resource, true);
                            }

                            foreach (var item in list)
                            {
                                resource.Merge(item);
                            }

                            list.Add(resource);
                        }
                    }
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// 获取本地字符串资源
        /// </summary>
        /// <param name="culture">语言信息</param>
        /// <returns><see cref="StringResource"/>实例</returns>
        internal static StringResource GetLocal(CultureInfo culture = null)
        {
            var rs = Load(typeof(StringResourceLoader).Assembly);

            if (culture == null) culture = Thread.CurrentThread.CurrentCulture;

            var r = rs.GetResource(culture);

            return r;
        }
    }
}
