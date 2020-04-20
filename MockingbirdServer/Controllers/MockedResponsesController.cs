using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
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

            return matchingPreparedResponse == null ? DefaultResponseBasedOnVerb() : ResponseBuiltFrom(matchingPreparedResponse);
        }

        public ActionResult DefaultResponseBasedOnVerb()
        {
            var responseBody = string.Format(CultureInfo.InvariantCulture, "Aucun mock associé à la requête: {0}", Request.HttpContext.Request.Path);
            if (Request.HttpContext.Request.Method == "POST")
                return StatusCode((int)HttpStatusCode.Created, responseBody);
            if (new[] {"HEAD", "PUT", "DELETE", "OPTIONS"}.Contains(Request.HttpContext.Request.Method))
                return StatusCode((int)HttpStatusCode.OK, responseBody);
            return StatusCode((int)HttpStatusCode.NotImplemented, responseBody);
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