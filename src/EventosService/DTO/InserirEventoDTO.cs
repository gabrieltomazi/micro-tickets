namespace EventosService.DTO
{
    // dto para receber os dados de cadastro de um novo evento
    public class InserirEventoDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string Local { get; set; } = string.Empty;
        public DateTime Data { get; set; }
        public int CapacidadeMaxima { get; set; }
    }
}
