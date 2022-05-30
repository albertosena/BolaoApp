namespace BolaoApp.Models
{
    public class Jogo
    {
        public int Id {get; set;}
        public string User {get; set;}
        public int Rodada {get; set;}
        public int IdMandante {get; set;}
        public int GolsMandante {get; set;}
        public int IdVisitante {get; set;}
        public int GolsVisitante{get;set;}
    }
}