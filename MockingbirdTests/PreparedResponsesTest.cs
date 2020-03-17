using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using MockingbirdServer.Lib;
using MockingbirdTests.Support;
using Newtonsoft.Json;
using Xunit;

namespace MockingbirdTests
{
    [Collection("ApiTestCollection")]
    public class PreparedResponsesTest : ApiTest
    {
        [Fact]
        public async void PrepareReponseARetourner()
        {
            var reponsePreparee = new ReponsePreparee
            {
                StatusCode = 203,
                Url = "/potatoes",
                Verbe = "GET",
                Payload = "[{x:1}]"
            };
            
            await WhenPreparerReponse(reponsePreparee);

            var response = new HttpClient().GetAsync("http://localhost:5000/potatoes").Result;
            response.StatusCode.Should().Be(203);
            response.Content.ReadAsStringAsync().Result.Should().Be("[{x:1}]");
            
            var unknownUrlResponse = new HttpClient().GetAsync("http://localhost:5000/someOtherUrl").Result;
            unknownUrlResponse.StatusCode.Should().Be(404);
        }
        
        [Fact]
        public async void PrepareReponseARetourner_ParScenarioId()
        {
            var reponsePrepareeScenario1 = new ReponsePreparee
            {
                StatusCode = 200,
                Url = "/institutionsFinancieres",
                Verbe = "GET",
                Payload = "[{id:815}]",
                ScenarioId = "1"
            };
                
            var reponsePrepareeScenario2 = new ReponsePreparee
            {
                StatusCode = 200,
                Url = "/institutionsFinancieres",
                Verbe = "GET",
                Payload = "[{id:6}]",
                ScenarioId = "2"
            };
            
            await WhenPreparerReponse(reponsePrepareeScenario1);
            await WhenPreparerReponse(reponsePrepareeScenario2);

            var httpClientScenario1 = new HttpClient();
            httpClientScenario1.DefaultRequestHeaders.Add("scenario-id", "1");
            var response1 = httpClientScenario1.GetAsync("http://localhost:5000/institutionsFinancieres").Result;
            response1.Content.ReadAsStringAsync().Result.Should().Be("[{id:815}]");
            
            var httpClientScenario2 = new HttpClient();
            httpClientScenario2.DefaultRequestHeaders.Add("scenario-id", "2");
            var response2 = httpClientScenario2.GetAsync("http://localhost:5000/institutionsFinancieres").Result;
            response2.Content.ReadAsStringAsync().Result.Should().Be("[{id:6}]");
        }

        private static async Task WhenPreparerReponse(ReponsePreparee reponsePreparee)
        {
            var stringPayload = JsonConvert.SerializeObject(reponsePreparee);

            var httpContent = AsStringContent(stringPayload);

            await new HttpClient().PostAsync("http://localhost:5000/prepared-response", httpContent);
        }
    }
}