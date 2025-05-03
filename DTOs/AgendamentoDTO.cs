namespace PetshopPeterson.DTOs
{
    public class AgendamentoDTO
    {
        public int TutorId { get; set; }
        public List<ServicoDTO> AgendamentoServico { get; set; } = new();
    }

    public class ServicoDTO
    {
        public int ServicoId { get; set; }
        public int Quantidade { get; set; }
    }
}