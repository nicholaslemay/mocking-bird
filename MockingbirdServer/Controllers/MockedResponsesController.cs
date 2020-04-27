using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MockingbirdServer.Controllers.Support;
using MockingbirdServer.Lib;

namespace MockingbirdServer.Controllers
{
    public class MockedResponsesController : MockingbirdController
    {
        public ActionResult GetMockedResponse()
        {
            var requeteRecue = RequeteRecue();
            ReceivedRequestRepository.ReceivedRequests.Add(requeteRecue);

            var matchingPreparedResponse = MatchingPreparedResponse();
            
            var reponse = matchingPreparedResponse == null ? DefaultResponseBasedOnVerb() : ResponseBuiltFrom(matchingPreparedResponse);
            Trace(requeteRecue, reponse);
            return reponse;
        }

        private static void Trace(Requete requete, ActionResult matchingPreparedResponse)
        {
            
            Console.WriteLine("*** Requête reçu ***");
            Console.WriteLine(requete);
            Console.WriteLine("");
            Console.WriteLine("*** Réponse retournée *** ");
            var reponse = matchingPreparedResponse as ObjectResult;
            Console.WriteLine(reponse.Value);
            Console.WriteLine("------------------------------------------");
        }

        private ActionResult DefaultResponseBasedOnVerb()
        {
            var responseBody = string.Format(CultureInfo.InvariantCulture, "Aucun mock associé à la requête: {0}", Request.HttpContext.Request.Path);
            if (Request.HttpContext.Request.Method == "POST")
                return StatusCode((int)HttpStatusCode.Created, JsonSerializer.Serialize(responseBody));
            if (new[] {"HEAD", "PUT", "DELETE", "OPTIONS"}.Contains(Request.HttpContext.Request.Method))
                return StatusCode((int)HttpStatusCode.OK, JsonSerializer.Serialize(responseBody));
            return StatusCode((int)HttpStatusCode.NotImplemented, responseBody);
        }

        private ReponsePreparee MatchingPreparedResponse()
        {
            if (!ResponsesPrepareesRepository.ResponsesPreparees.ContainsKey(CurrentScenarioId()))
                return null;

            var responsesPrepareesForCurrentScenario = ResponsesPrepareesRepository.ResponsesPreparees[CurrentScenarioId()];
            return responsesPrepareesForCurrentScenario.Where(Matching)
                                                       .OrderByDescending(x =>x.CreationDate)
                                                       .FirstOrDefault();
        }

        private bool Matching(ReponsePreparee r)
        {
            return string.Equals(r.Url, Request.HttpContext.Request.Path.ToString() + Request.HttpContext.Request.QueryString, StringComparison.InvariantCultureIgnoreCase)
                   && string.Equals(r.Verbe,Request.HttpContext.Request.Method, StringComparison.InvariantCultureIgnoreCase);
        }

        private Requete RequeteRecue()
        {
            return new Requete
                {
                    Verbe = Request.HttpContext.Request.Method,
                    Chemin = Request.HttpContext.Request.Path + Request.HttpContext.Request.QueryString,
                    Body = new StreamReader(Request.Body).ReadToEndAsync().Result,
                    ScenarioId = CurrentScenarioId()
                };
        }

        private ActionResult ResponseBuiltFrom(ReponsePreparee reponsePreparee)
        {
            var response = StatusCode(reponsePreparee.StatusCodeOrDefaultForVerbe(), reponsePreparee.Payload);
            foreach (var (key, value) in reponsePreparee.CustomHeaders)
            {
                Response.Headers.Add(key, value);                
            }
            return response;
        }
    }
}