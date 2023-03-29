using Microsoft.Extensions.Logging;

namespace TestData.Part1
{
    public class Class_1
    {
        private readonly ILogger<Class_1> _logger;

        public Class_1(ILogger<Class_1> logger)
        {
            _logger = logger;

            _logger.LogInformation($"初始化【{typeof(Class_1)}】！");
        }
    }
}
