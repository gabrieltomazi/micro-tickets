namespace EventosService.Servicos
{
    // entidade que representa um evento no sistema
    public class Evento
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Local { get; set; } = string.Empty;
        public DateTime Data { get; set; }
        public int CapacidadeMaxima { get; set; }
        public bool Ativo { get; set; } = true;
    }
}
