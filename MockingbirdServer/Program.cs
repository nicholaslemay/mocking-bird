using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace MockingbirdServer
{
    public class Program
    {
        public const string BaseUrl = "http://localhost:5050";

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)

                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls(BaseUrl);
                });


    }
}