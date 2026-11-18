namespace EventosService.DTO
{
    // dto utilizado para cadastrar um novo evento
    public class CriarEventoDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string Local { get; set; } = string.Empty;
        public DateTime Data { get; set; }
        public int CapacidadeMaxima { get; set; }
        public bool Ativo { get; set; } = true;
    }

    // dto de resposta com os dados completos do evento
    public class EventoDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Local { get; set; } = string.Empty;
        public DateTime Data { get; set; }
        public int CapacidadeMaxima { get; set; }
        public bool Ativo { get; set; }
    }
}
