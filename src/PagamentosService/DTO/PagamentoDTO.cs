namespace PagamentosService.DTO;

public class PagamentoRequest
{
    public int IngressoId { get; set; }
    public decimal Valor { get; set; }
    public string MetodoPagamento { get; set; } = "PIX";
}