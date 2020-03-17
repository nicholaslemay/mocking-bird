namespace MockingbirdServer.Lib
{
    public class ReponsePreparee
    {
        public static readonly string DefaultScenarioId = "Commun";
        public string Verbe { get; set; }
        public string Url { get; set;}
        public string Payload { get; set;}
        public string ScenarioId { get; set; } = DefaultScenarioId ;
        public int? StatusCode { get; set; }
        public int StatusCodeOrDefaultForVerbe()
        {
            if (StatusCode != null)
                return StatusCode.Value;

            switch (Verbe)
            {
                case "POST":
                    return 201;
                case "PUT":
                    return 204;
                default:
                    return 200;
                
            }
            
        }
        
        
    }
}