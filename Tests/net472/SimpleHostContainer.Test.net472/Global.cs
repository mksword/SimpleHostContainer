using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.Linq;
using System.Reflection;
using TestData.Part1;
using TestData.Part2;

namespace SimpleHostContainer.Test.net472
{
    public class Global
    {
        private static bool _initilized;
        private static IServiceProvider _serviceProvider;
        private static StringResource _stringResource;

        public static StringResource _ => _stringResource;

        public static void Init()
        {
            if (!_initilized)
            {
                string subName = DateTime.Now.ToString("yyyyMMdd");
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .Enrich.FromLogContext()
                    .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] 【{SourceContext}】: {Message:lj}{NewLine}{Exception}")
                    .WriteTo.File(
                        $"Logs/log{subName}.log",
                        rollingInterval: RollingInterval.Day,
                        rollOnFileSizeLimit: true,
                        fileSizeLimitBytes: 10485670,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] 【{SourceContext}】: {Message:lj}{NewLine}{Exception}")
#if DEBUG
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
#else
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
#endif
                    .Enrich.FromLogContext()
                    .CreateLogger();

                var host = HostBuilderMaker.CreateHostBuilder()
                    (new StartOptions()
                    {
                        IsEnablePlugins = false,
                    })
                    (() =>
                    {
                        return new Assembly[]
                        {
                            typeof(Global).Assembly,
                            typeof(TestDataPart1SetupInfo).Assembly,
                            typeof(TestDataPart2SetupInfo).Assembly,
                        };
                    })
                    (null)
                    (builder =>
                    {
                        builder.ClearProviders();
#if DEBUG
                        builder.SetMinimumLevel(LogLevel.Debug);
#else
                        builder.SetMinimumLevel(LogLevel.Information);
#endif
                        builder.AddSerilog();
                    }).Build();

                _serviceProvider = host.Services.GetService<IServiceProvider>();

                _stringResource = StringResourceLoader.Load(typeof(Global).Assembly).FirstOrDefault();

                _initilized = true;
            }
        }

        public static T Get<T>()
            where T : class
        {
            return _serviceProvider?.GetService<T>();
        }
    }
}
