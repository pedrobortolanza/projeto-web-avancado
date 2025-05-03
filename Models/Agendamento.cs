namespace PetshopPeterson.Models 
{
    public class Agendamento 
    {
        public int Id { get; set; }

        public int TutorId { get; set; }

        public Tutor Tutor { get; set; } 

        public List<AgendamentoServico> AgendamentoServico {get; set;} = new();

        public DateTime Data { get; set; }
    }
}
