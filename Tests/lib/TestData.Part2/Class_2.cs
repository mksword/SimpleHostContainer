using Microsoft.Extensions.Logging;
using SimpleHostContainer;
using TestData.Part2.Share;

namespace TestData.Part2
{
    [IoC(typeof(IInterface_1))]
    public class Class_2 : IInterface_1
    {
        private readonly ILogger<Class_2> _logger;

        public Class_2(ILogger<Class_2> logger)
        {
            _logger = logger;

            _logger.LogInformation($"初始化【{typeof(Class_2)}】！");
        }
    }
}
