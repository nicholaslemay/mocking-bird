using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MockingbirdServer.Controllers.Support;
using MockingbirdServer.Lib;

namespace MockingbirdServer.Controllers
{
    [ApiController]
    [Route("/prepared-response")]
    public class PreparedResponsesController : MockingbirdController
    {
        [HttpPost]
        public IActionResult Post([FromBody]ReponsePreparee reponsePreparee)
        {
            Trace(reponsePreparee);
            if(!ResponsesPrepareesRepository.ResponsesPreparees.ContainsKey(reponsePreparee.ScenarioId))
                ResponsesPrepareesRepository.ResponsesPreparees[reponsePreparee.ScenarioId] = new List<ReponsePreparee>();
            
            ResponsesPrepareesRepository.ResponsesPreparees[reponsePreparee.ScenarioId].Add(reponsePreparee);
            
            return StatusCode(201);
        }

        private static void Trace(ReponsePreparee reponsePreparee)
        {
            Console.WriteLine("+++ Ajout d'une réponse préparée +++ ");
            Console.WriteLine("");
            Console.WriteLine(reponsePreparee);
            Console.WriteLine("------------------------------------------");
        }

        //todo Delete
    }

    public class ResponsesPrepareesRepository
    {
        public static Dictionary<string, List<ReponsePreparee>> ResponsesPreparees { get; } = new Dictionary<string, List<ReponsePreparee>>
        {
            {ReponsePreparee.DefaultScenarioId, new List<ReponsePreparee>()}
        };
    }
    
}