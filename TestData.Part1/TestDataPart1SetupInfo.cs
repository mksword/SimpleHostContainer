using Microsoft.Extensions.DependencyInjection;
using SimpleHostContainer;
using TestData.Part1.Share;

namespace TestData.Part1
{
    public class TestDataPart1SetupInfo : SetupInfo
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient(typeof(Class_1));

            services.Configure<Part1Options>(option =>
            {
                option.Value = 5;
            });
        }
    }
}
