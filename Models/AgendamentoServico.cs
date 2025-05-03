namespace PetshopPeterson.Models 
{
    public class AgendamentoServico
    {
        public int AgendamentoId { get; set; }
        public Agendamento Agendamento { get; set; }

        public int ServicoId { get; set; }
        public Servico Servico { get; set; }

        public int Quantidade {get; set;}
    }    
}