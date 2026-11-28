namespace PagamentosService.Models
{
    // entidade que representa um pagamento de ingresso no sistema
    public class Pagamento
    {
        public int Id { get; set; }
        public Guid IngressoId { get; set; }
        public decimal Valor { get; set; }
        public string MetodoPagamento { get; set; } = "PIX"; // pix, cartaocredito, boleto
        public string Status { get; set; } = "Aprovado";
        public DateTime DataPagamento { get; set; } = DateTime.UtcNow;
    }
}
