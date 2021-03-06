using System;

namespace MockingbirdServer.Lib
{
    public class Requete
    {        
        public string Chemin { get; set; }
        public string Verbe { get; set;}
        public string Body { get; set;}

        public string ScenarioId { get; set; }
        public override string ToString()
        {
            return @$"Verbe : {Verbe}{Environment.NewLine}Chemin : {Chemin}{Environment.NewLine}Body : {Body}{Environment.NewLine}ScenarioId : {ScenarioId}";
        }
    }
}