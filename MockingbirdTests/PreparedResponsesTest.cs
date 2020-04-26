using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using MockingbirdServer;
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
            await WhenPreparerReponse(new ReponsePreparee
                                      {
                                          StatusCode = 203,
                                          Url = "/potatoes",
                                          Verbe = "GET",
                                          Payload = "[{x:1}]"
                                      });

            var response = new HttpClient().GetAsync($"{Program.BaseUrl}/potatoes").Result;
            response.StatusCode.Should().Be(203);
            response.Content.ReadAsStringAsync().Result.Should().Be("[{x:1}]");
            
            var unknownUrlResponse = new HttpClient().GetAsync($"{Program.BaseUrl}/someOtherUrl").Result;
            unknownUrlResponse.StatusCode.Should().Be(501);
        }
        
        [Fact]
        public async void PrepareReponseARetourner_InsensibleALaCase()
        {
            await WhenPreparerReponse(new ReponsePreparee
                                      {
                                          StatusCode = 203,
                                          Url = "/potatoes",
                                          Verbe = "GET",
                                          Payload = "[{x:1}]"
                                      });

            var response = new HttpClient().GetAsync($"{Program.BaseUrl}/potatOes").Result;
            response.StatusCode.Should().Be(203);
            response.Content.ReadAsStringAsync().Result.Should().Be("[{x:1}]");
        }
        
        [Fact]
        public async void PrepareReponseARetourner_SupportLesRequetesAvecQueryString()
        {
            await WhenPreparerReponse(new ReponsePreparee
                                      {
                                          StatusCode = 203,
                                          Url = "/beets?taste=good",
                                          Verbe = "GET",
                                          Payload = "[{x:1}]"
                                      });

            var response = new HttpClient().GetAsync($"{Program.BaseUrl}/beets").Result;
            response.StatusCode.Should().Be(501);
            
            response = new HttpClient().GetAsync($"{Program.BaseUrl}/beets?taste=good").Result;
            response.StatusCode.Should().Be(203);
            response.Content.ReadAsStringAsync().Result.Should().Be("[{x:1}]");
        }
        
        [Fact]
        public async void SupporteLeRetourDeHeadersSpecifies()
        {
            var reponsePreparee = new ReponsePreparee
            {
                StatusCode = 203,
                Url = "/carrots",
                Verbe = "GET",
                Payload = "[{x:1}]", 
                CustomHeaders = new Dictionary<string, string>
                                {
                                    {"coucou", "allo"}
                                } 
            };
            
            await WhenPreparerReponse(reponsePreparee);
            
            var response = new HttpClient().GetAsync($"{Program.BaseUrl}/carrots").Result;
            response.StatusCode.Should().Be(203);
            response.Content.ReadAsStringAsync().Result.Should().Be("[{x:1}]");
            response.Headers.GetValues("coucou").Should().Contain("allo");
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
            var response1 = httpClientScenario1.GetAsync($"{Program.BaseUrl}/institutionsFinancieres").Result;
            response1.Content.ReadAsStringAsync().Result.Should().Be("[{id:815}]");
            
            var httpClientScenario2 = new HttpClient();
            httpClientScenario2.DefaultRequestHeaders.Add("scenario-id", "2");
            var response2 = httpClientScenario2.GetAsync($"{Program.BaseUrl}/institutionsFinancieres").Result;
            response2.Content.ReadAsStringAsync().Result.Should().Be("[{id:6}]");
        }
        
        [Fact]
        public async void SupprimeLesReponsesPreparees()
        {
            await WhenPreparerReponse(new ReponsePreparee
                                      {
                                          StatusCode = 203,
                                          Url = "/carrots",
                                          Verbe = "GET",
                                          Payload = "[{x:1}]"
                                      });
            
            await new HttpClient().DeleteAsync($"{Program.BaseUrl}/prepared-response");
            
            var response = new HttpClient().GetAsync($"{Program.BaseUrl}/carrots").Result;
            response.StatusCode.Should().Be(501);
        }
        
        [Fact]
        public async void SupprimeLesReponsesPreparees_ParScenarioId()
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

            await new HttpClient().DeleteAsync($"{Program.BaseUrl}/prepared-response?scenarioId=1");
            
            var httpClientScenario1 = new HttpClient();
            httpClientScenario1.DefaultRequestHeaders.Add("scenario-id", "1");
            var response1 = httpClientScenario1.GetAsync($"{Program.BaseUrl}/institutionsFinancieres").Result;
            response1.StatusCode.Should().Be(501);
            
            var httpClientScenario2 = new HttpClient();
            httpClientScenario2.DefaultRequestHeaders.Add("scenario-id", "2");
            var response2 = httpClientScenario2.GetAsync($"{Program.BaseUrl}/institutionsFinancieres").Result;
            response2.StatusCode.Should().Be(200);
        }

        private static async Task WhenPreparerReponse(ReponsePreparee reponsePreparee)
        {
            var stringPayload = JsonConvert.SerializeObject(reponsePreparee);

            var httpContent = AsStringContent(stringPayload);

            await new HttpClient().PostAsync($"{Program.BaseUrl}/prepared-response", httpContent);
        }
    }
}