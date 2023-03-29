using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHostContainer
{
    /// <summary>
    /// 字符串资源类
    /// </summary>
    [JsonConverter(typeof(StringResourceConverter))]
    public class StringResource
    {
        private readonly Dictionary<string, string> _textMap = new Dictionary<string, string>();
        private CultureInfo _culture = new CultureInfo("zh-CN");
        private bool _isDefault = false;

        /// <summary>
        /// 字符串索引器
        /// </summary>
        /// <param name="key">索引</param>
        /// <returns>字符串</returns>
        public string this[string key]
        {
            get
            {
                if (_textMap.ContainsKey(key))
                {
                    return _textMap[key];
                }
                else
                {
                    return key;
                }
            }
        }

        /// <summary>
        /// 格式化字符串索引器
        /// </summary>
        /// <param name="key">索引</param>
        /// <param name="args">参数列表</param>
        /// <returns>格式化后的字符串</returns>
        public string this[string key, params object[] args]
        {
            get
            {
                if (_textMap.ContainsKey(key))
                {
                    string format = _textMap[key];

                    return string.Format(format, args);
                }
                else
                {
                    return key;
                }
            }
        }

        /// <summary>
        /// 获取语言信息
        /// </summary>
        public CultureInfo Culture => _culture;

        /// <summary>
        /// 获取字符串资源
        /// </summary>
        public IReadOnlyDictionary<string, string> Texts => _textMap;

        /// <summary>
        /// 获取一个值来标记是否是默认字符串资源
        /// </summary>
        public bool IsDefault => _isDefault;

        /// <summary>
        /// 字符串资源合并
        /// </summary>
        /// <param name="resource">另一个实例</param>
        public void Merge(StringResource resource)
        {
            List<string> keys = new List<string>(_textMap.Keys);

            foreach (var key in resource._textMap.Keys)
            {
                if (!keys.Contains(key))
                {
                    keys.Add(key);
                }
            }

            foreach (var key in keys)
            {
                if (!_textMap.ContainsKey(key))
                {
                    _textMap.Add(key, resource._textMap[key]);
                }
                else if (!resource._textMap.ContainsKey(key))
                {
                    resource._textMap.Add(key, _textMap[key]);
                }
            }
        }

        /// <summary>
        /// 字符串资源帮助类
        /// </summary>
        internal static class Helper
        {
            /// <summary>
            /// 设置语言
            /// </summary>
            /// <param name="resource"><see cref="StringResource"/>实例</param>
            /// <param name="culture">语言信息实例</param>
            public static void SetCulture(StringResource resource, CultureInfo culture)
            {
                resource._culture = culture;
            }

            /// <summary>
            /// 添加字符串资源
            /// </summary>
            /// <param name="resource"><see cref="StringResource"/>实例</param>
            /// <param name="key">字符串键值</param>
            /// <param name="value">字符串</param>
            /// <exception cref="Exception"/>
            public static void AddText(StringResource resource, string key, string value)
            {
                if (!resource._textMap.ContainsKey(key))
                {
                    resource._textMap.Add(key, value);
                }
                else
                {
                    throw new Exception($"键值【{key}】已经存在！");
                }
            }

            /// <summary>
            /// 设置默认标记
            /// </summary>
            /// <param name="resouces"><see cref="StringResource"/>实例</param>
            /// <param name="isDefault">默认标记值</param>
            public static void SetIsDefault(StringResource resouces, bool isDefault)
            {
                resouces._isDefault = isDefault;
            }
        }
    }
}
