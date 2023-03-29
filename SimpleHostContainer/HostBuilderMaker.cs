using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using static SimpleHostContainer.StringResourceLoader;

namespace SimpleHostContainer
{
    /// <summary>
    /// 主机建造者生成类
    /// </summary>
    public static class HostBuilderMaker
    {
        private static readonly Regex DLL_REGEX = new Regex(@"^[\w\S\-\\\:]*\\([\w\S\-]*\.dll)$", RegexOptions.IgnoreCase);

        /// <summary>
        /// 创建主机建造者
        /// </summary>
        /// <param name="args">参数列表</param>
        /// <returns><see cref="StartDelegate"/>实例</returns>
        public static StartDelegate CreateHostBuilder(params string[] args)
        {
            StartDelegate startDelegate = new StartDelegate(options =>
            {
                IHostBuilder builder = Host.CreateDefaultBuilder(args);

                if (options == null) throw new ArgumentNullException(nameof(options));

                //// 程序集-安装信息类型映射表，按照设计每个程序集只能有一个安装信息类
                Dictionary<Assembly, Type> dictionary = new Dictionary<Assembly, Type>();

                LoadDefaultAssemblies(dictionary);

                UseUseAssembliesDelegateHandler uuad = new UseUseAssembliesDelegateHandler(uad =>
                {
                    if (uad != null)
                    {
                        Assembly[] assemblies = uad();

                        assemblies.ForEach(A =>
                        {
                            ExtractAssembly(dictionary, A);
                        });
                    }

                    if (options.IsEnablePlugins)
                    {
                        if (!string.IsNullOrWhiteSpace(options.PluginsRootPath))
                        {
                            options.PluginsRootPath = "Plugins";
                        }

                        DirectoryInfo plugins_root = new DirectoryInfo(options.PluginsRootPath);

                        if (!Directory.Exists(plugins_root.FullName))
                        {
                            Directory.CreateDirectory(plugins_root.FullName);
                        }
                        else
                        {
                            List<Assembly> assemblies = new List<Assembly>();

                            foreach (DirectoryInfo plugin_dir in plugins_root.GetDirectories())
                            {
                                var latest = plugin_dir.GetDirectories().Select(D => new
                                {
                                    A = D,
                                    B = Version.Parse(D.Name.Replace("v", string.Empty).Replace("V", string.Empty))
                                })
                                    .OrderByDescending(M => M.B)
                                    .FirstOrDefault()?.A;

                                if (latest != null)
                                {
                                    GoThroughDirectory(latest, assemblies);
                                }
                            }

                            AppDomain.CurrentDomain.AssemblyResolve += (a, b) =>
                            {
                                var referenced = assemblies.Where(A => A.FullName.Equals(b.Name)).FirstOrDefault();

                                if (referenced == null)
                                {
                                    referenced = AppDomain.CurrentDomain.GetAssemblies().Where(A => A.FullName.Equals(b.Name)).FirstOrDefault();
                                }

                                return referenced;
                            };

                            assemblies.ForEach(A =>
                            {
                                ExtractAssembly(dictionary, A);
                            });
                        }
                    }

                    UseConfigureServicesDelegateHandler ucsdh = new UseConfigureServicesDelegateHandler(ucsd =>
                    {
                        Stack<Type> stack = new Stack<Type>();
                        dictionary.Values.ForEach(T =>
                        {
                            if (T != null)
                            {
                                SortSetupInfoTypeDependenc(T, stack);
                            }
                        });
                        var stack_1 = new Stack<Type>(stack);

                        stack_1.Pop(T =>
                        {
                            if (!dictionary.ContainsKey(T.Assembly))
                            {
                                dictionary.Add(T.Assembly, T);
                            }
                        });

                        dictionary.Keys.ForEach(A =>
                        {
                            A.GetTypes().Enroll(builder);
                        });

                        if (ucsd != null)
                        {
                            builder = builder.ConfigureServices(services =>
                            {
                                ucsd(services);
                            });
                        }

                        builder.ConfigureServices(services =>
                        {
                            stack = new Stack<Type>(stack);

                            stack.Pop(T =>
                            {
                                ISetupInfo setupInfo = Activator.CreateInstance(T) as ISetupInfo;

                                if (setupInfo != null)
                                {
                                    setupInfo.ConfigureServices(services);
                                }
                            });
                        });

                        UseConfigureLoggingDelegateHandler ucldh = new UseConfigureLoggingDelegateHandler(ucld =>
                        {
                            if (ucld != null)
                            {
                                builder = builder.ConfigureLogging(b =>
                                {
                                    ucld(b);
                                });
                            }

                            return builder;
                        });

                        return ucldh;
                    });

                    return ucsdh;
                });

                return uuad;
            });

            return startDelegate;
        }

        private static void GoThroughDirectory(DirectoryInfo directory, List<Assembly> assemblies)
        {
            FileInfo[] files = directory.GetFiles().Where(F => DLL_REGEX.IsMatch(F.FullName)).ToArray();

            foreach (FileInfo file in files)
            {
                Match match = DLL_REGEX.Match(file.FullName);
                string dll_name = match.Groups[1].Value;

                if (!assemblies.Any(A =>
                {
                    return A.FullName.EndsWith(dll_name);
                }))
                {
                    Assembly assembly = Assembly.LoadFile(file.FullName);

                    var refrenced = assembly.GetReferencedAssemblies();

                    assemblies.Add(assembly);
                }
            }

            var subdirs = directory.GetDirectories();

            foreach (DirectoryInfo sub in subdirs)
            {
                GoThroughDirectory(sub, assemblies);
            }
        }

        private static void SortSetupInfoTypeDependenc(Type type, Stack<Type> stack)
        {
            PrerequisiteAttribute attribute = type.GetCustomAttribute<PrerequisiteAttribute>();

            if (attribute != null)
            {
                if (attribute.PrerequisiteTypes.Contains(type))
                {
                    throw new Exception($"{type} can not depend on itself!");
                }

                attribute.PrerequisiteTypes.ForEach(PT =>
                {
                    if (!stack.Any(T => T == PT))
                    {
                        SortSetupInfoTypeDependenc(PT, stack);
                    }
                });
            }

            if (!stack.Any(T => T == type))
            {
                stack.Push(type);
            }
        }

        private static void ExtractAssembly(Dictionary<Assembly, Type> map, Assembly assembly)
        {
            var query = assembly.GetTypes().GetSetupInfoTypes();

            StringResource rs = GetLocal();

            if (query.Any(Q => Q.Types.Length > 1))
            {
                if (rs == null)
                {
                    rs = GetLocal();
                }

                throw new Exception(rs["OneSetupInfoForEveryAssembly"]);
            }

            if (query.Count() > 0)
            {
                query.ForEach(Q =>
                {
                    Type type = Q.Types.Length == 1 ? Q.Types[0] : null;

                    if (type != null)
                    {
                        PrerequisiteAttribute attribute = type.GetCustomAttribute<PrerequisiteAttribute>();

                        if (attribute != null)
                        {
                            attribute.PrerequisiteTypes.Select(T => T.Assembly).ForEach(A =>
                            {
                                ExtractAssembly(map, A);
                            });
                        }
                    }

                    if (map.ContainsKey(Q.Assembly))
                    {
                        Type setupInfoType = map[Q.Assembly];

                        if (setupInfoType == null)
                        {
                            if (Q.Types.Length == 1)
                            {
                                map[Q.Assembly] = Q.Types[0];
                            }
                        }
                        else
                        {
                            if (setupInfoType != Q.Types[0])
                            {
                                if (rs == null)
                                {
                                    rs = GetLocal();
                                }

                                throw new Exception(rs["OneSetupInfoForEveryAssembly"]);
                            }
                        }
                    }
                    else
                    {
                        if (Q.Types.Length == 1)
                        {
                            map.Add(Q.Assembly, Q.Types[0]);
                        }
                        else
                        {
                            map.Add(Q.Assembly, null);
                        }
                    }
                });
            }
            else
            {
                if (!map.ContainsKey(assembly))
                {
                    map.Add(assembly, null);
                }
            }
        }

        private static void LoadDefaultAssemblies(Dictionary<Assembly, Type> map)
        {

            Assembly local = typeof(HostBuilderMaker).Assembly;
            Assembly entry = Assembly.GetEntryAssembly();

            if (local == entry)
            {
                var query = local.GetTypes().GetSetupInfoTypes();

                RegistryAssemblySetupInfoTypeMap(map, query);
            }
            else
            {
                var query = (from i in local.GetTypes().GetSetupInfoTypes() select i).ToArray();

                if (entry != null)
                {
                    var other = from i in entry.GetTypes()?.GetSetupInfoTypes() select i;

                    if (other != null)
                    {
                        query = query.Union(other).Distinct().ToArray();
                    }
                }

                RegistryAssemblySetupInfoTypeMap(map, query);
            }
        }

        private static void RegistryAssemblySetupInfoTypeMap(Dictionary<Assembly, Type> map, SetupInfoGroupQueryItem[] query)
        {
            if (query.Any(Q => Q.Types.Length > 1))
            {
                var rs = GetLocal();

                throw new Exception("OneSetupInfoForEveryAssembly");
            }

            query.ForEach(Q =>
            {
                if (!map.ContainsKey(Q.Assembly))
                {
                    map.Add(Q.Assembly, Q.Types.Length == 1 ? Q.Types[0] : null);
                }
            });
        }
    }
}
