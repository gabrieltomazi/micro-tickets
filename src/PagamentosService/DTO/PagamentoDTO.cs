namespace PagamentosService.DTO
{
    // dto de entrada para requisitar um novo pagamento de ingresso
    public class PagamentoRequest
    {
        public Guid IngressoId { get; set; }
        public decimal Valor { get; set; }
        public string MetodoPagamento { get; set; } = "PIX";
    }

    // dto de resposta com dados detalhados da transacao
    public class PagamentoResponseDTO
    {
        public int Id { get; set; }
        public Guid IngressoId { get; set; }
        public decimal Valor { get; set; }
        public string MetodoPagamento { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime DataPagamento { get; set; }
    }
}
