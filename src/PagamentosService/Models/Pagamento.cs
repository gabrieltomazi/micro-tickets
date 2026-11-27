namespace PagamentosService.Entidades
{
    public class Pagamento
    {
        public int Id { get; set; }

        public int IngressoId { get; set; }

        public decimal Valor { get; set; }

        public string MetodoPagamento { get; set; } = "";

        public string Status { get; set; } = "Aprovado";

        public DateTime DataPagamento { get; set; }
    }
}