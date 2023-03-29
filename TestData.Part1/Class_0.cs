using Microsoft.Extensions.Logging;
using SimpleHostContainer;
using TestData.Part1.Share;

namespace TestData.Part1
{

    [IoC(typeof(IInterface_0), typeof(Class_0))]
    public class Class_0 : IInterface_0
    {
        private readonly ILogger<Class_0> _logger;

        public Class_0(ILogger<Class_0> logger)
        {
            _logger = logger;

            _logger.LogInformation($"初始化类型【{typeof(Class_0)}】！");
        }
    }
}
