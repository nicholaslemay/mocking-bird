using Microsoft.AspNetCore.Mvc;

namespace MockingbirdServer.Controllers.Support
{
    public abstract class MockingbirdController : ControllerBase
    {
        public string CurrentScenarioId()
        {
            var requestedScenarioId = Request.Headers["scenario-id"];

            return string.IsNullOrEmpty(requestedScenarioId) ? "Commun" : requestedScenarioId.ToString();
        }
    }
}