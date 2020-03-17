using System;
using Microsoft.Extensions.Hosting;
using MockingbirdServer;
using Xunit;

namespace MockingbirdTests.Support
{
    [CollectionDefinition("ApiTestCollection")]
    public class ApiTestCollection : ICollectionFixture<ApiTestClassFixture>
    {
    }
    
    public class ApiTestClassFixture : IDisposable
    {
        private IHost _host;
        public ApiTestClassFixture()
        {
            if (_host != null) return;
            
            var hostBuilder = Program.CreateHostBuilder(new string[]{});
            _host = hostBuilder.Build();
            _host.StartAsync();
        }

        public void Dispose()
        {
            try
            {
                _host.StopAsync().GetAwaiter().GetResult();
                _host.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}