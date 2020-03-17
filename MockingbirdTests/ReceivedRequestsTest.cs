using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using FluentAssertions;
using MockingbirdServer.Lib;
using MockingbirdTests.Support;
using Newtonsoft.Json;
using Xunit;

namespace MockingbirdTests
{
    [Collection("ApiTestCollection")]
    public class ReceivedRequestsTest : ApiTest
    {
        [Fact]
        public async void EnregistreProprietes_AppelsRecus()
        {
            var json =  JsonConvert.SerializeObject(new {Prenom = "Johnny", Nom = "Greenwood"});

            await new HttpClient().PostAsync("http://localhost:5000/musiciens", AsStringContent(json));

            var requete = ReceivedRequests().First( r=> r.Chemin.Contains("/musiciens"));
            requete.Chemin.Should().Contain("/musiciens");
            requete.Verbe.Should().Be("POST");
            requete.Body.Should().Be(json);
        }
        
        [Fact]
        public async void EnregistreParScenarioId()
        {
            var json =  JsonConvert.SerializeObject(new {Prenom = "Johnny", Nom = "Greenwood"});

            var httpClientScenario1 = new HttpClient();
            httpClientScenario1.DefaultRequestHeaders.Add("scenario-id", "12345");
            await httpClientScenario1.PostAsync("http://localhost:5000/musiciens", AsStringContent(json));
            await httpClientScenario1.PostAsync("http://localhost:5000/musiciens", AsStringContent(json));
            await httpClientScenario1.PostAsync("http://localhost:5000/musiciens", AsStringContent(json));
            
            var httpClientScenario2 = new HttpClient();
            httpClientScenario2.DefaultRequestHeaders.Add("scenario-id", "56789");
            await httpClientScenario2.PostAsync("http://localhost:5000/musiciens", AsStringContent(json));
            await httpClientScenario2.PostAsync("http://localhost:5000/musiciens", AsStringContent(json));

            ReceivedRequests("12345").Should().HaveCount(3);
            ReceivedRequests("56789").Should().HaveCount(2);
        }

        private static List<Requete> ReceivedRequests()
        {
            var stringResponse = new HttpClient().GetAsync("http://localhost:5000/received-requests").Result.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<List<Requete>>(stringResponse) ;
        }
        
        private static List<Requete> ReceivedRequests(string scenarioId)
        {
            var stringResponse = new HttpClient().GetAsync($"http://localhost:5000/received-requests?scenarioId={scenarioId}").Result.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<List<Requete>>(stringResponse) ;
        }
    }
}