using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MockingbirdServer.Lib;

namespace MockingbirdServer.Controllers
{
    [ApiController]
    [Route("/received-requests")]
    public class ReceivedRequestsController : ControllerBase
    {
        [HttpGet]
        public List<Requete> Get(string scenarioId = null)
        {
            if (string.IsNullOrEmpty(scenarioId))
                return ReceivedRequestRepository.ReceivedRequests;
            
            return ReceivedRequestRepository.ReceivedRequests.Where(r=> scenarioId.Equals(r.ScenarioId, StringComparison.InvariantCultureIgnoreCase)).ToList() ;
        }
        
        [HttpDelete]
        public IActionResult Delete([FromQuery]string scenarioId = null)
        {
            if (scenarioId == null)
                ReceivedRequestRepository.ReceivedRequests.Clear();
            else
                ReceivedRequestRepository.ReceivedRequests.RemoveAll(x => x.ScenarioId == scenarioId);
            return StatusCode(200);
        }
    }
}