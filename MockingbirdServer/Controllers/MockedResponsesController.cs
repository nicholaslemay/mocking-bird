using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MockingbirdServer.Controllers.Support;
using MockingbirdServer.Lib;

namespace MockingbirdServer.Controllers
{
    public class MockedResponsesController : MockingbirdController
    {
        public ActionResult GetMockedResponse()
        {
            ReceivedRequestRepository.ReceivedRequests.Add(RequeteRecue());

            var matchingPreparedResponse = MatchingPreparedResponse();

            return matchingPreparedResponse == null ? NotFound() : ResponseBuiltFrom(matchingPreparedResponse);
        }

        private ReponsePreparee MatchingPreparedResponse()
        {
            if (!ResponsesPrepareesRepository.ResponsesPreparees.ContainsKey(CurrentScenarioId()))
                return null;

            var responsesPrepareesForCurrentScenario = ResponsesPrepareesRepository.ResponsesPreparees[CurrentScenarioId()];
            return responsesPrepareesForCurrentScenario.FirstOrDefault(r=> r.Url == Request.HttpContext.Request.Path.ToString() 
                                                                                      && r.Verbe == Request.HttpContext.Request.Method);
        }

        private Requete RequeteRecue()
        {
            return new Requete
                {
                    Verbe = Request.HttpContext.Request.Method,
                    Chemin = Request.HttpContext.Request.Path,
                    Body = new StreamReader(Request.Body).ReadToEndAsync().Result,
                    ScenarioId = CurrentScenarioId()
                };
        }

        private ActionResult ResponseBuiltFrom(ReponsePreparee reponsePreparee)
        {
            return StatusCode(reponsePreparee.StatusCodeOrDefaultForVerbe(), reponsePreparee.Payload);
        }
    }
}