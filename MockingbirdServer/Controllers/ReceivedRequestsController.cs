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
    }
}