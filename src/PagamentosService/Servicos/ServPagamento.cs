using Microsoft.EntityFrameworkCore;
using PagamentosService.Data;
using PagamentosService.Models;
using PagamentosService.DTO;
 
namespace PagamentosService.Servicos;
 
public class ServPagamento
{
    public static readonly string[] MetodosValidos = { "PIX", "CartaoCredito", "Boleto" };
 
    private readonly DataContext _context;
    private readonly HttpClient _http;
 
    public ServPagamento(DataContext context, HttpClient http)
    {
        _context = context;
        _http = http;
    }
 
    public async Task<(Pagamento? pagamento, string? erro)> CriarAsync(PagamentoRequest req)
    {
        if (req.Valor <= 0)
            return (null, "O valor do pagamento deve ser maior que zero.");
 
        var metodo = MetodosValidos.FirstOrDefault(m =>
            m.Equals(req.MetodoPagamento, StringComparison.OrdinalIgnoreCase));
        if (metodo is null)
            return (null, "Método de pagamento inválido. Use: PIX, CartaoCredito ou Boleto.");
 
        var pagamento = new Pagamento
        {
            IngressoId = req.IngressoId,
            Valor = req.Valor,
            MetodoPagamento = metodo,
            Status = "Aprovado",
            DataPagamento = DateTime.UtcNow
        };
 
        _context.Pagamentos.Add(pagamento);
        await _context.SaveChangesAsync();
 
        try
        {
            var resp = await _http.PatchAsync($"/api/ingressos/{pagamento.IngressoId}/confirmar", null);
            if (!resp.IsSuccessStatusCode)
            {
                await DesfazerAsync(pagamento);
                return (null, $"IngressosService recusou a confirmação (HTTP {(int)resp.StatusCode}).");
            }
        }
        catch (HttpRequestException)
        {
            await DesfazerAsync(pagamento);
            return (null, "Não foi possível se comunicar com o IngressosService.");
        }
 
        return (pagamento, null);
    }
 
    public Task<Pagamento?> ObterPorIdAsync(int id) =>
        _context.Pagamentos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
 
    public Task<List<Pagamento>> ListarPorIngressoAsync(int ingressoId) =>
        _context.Pagamentos.AsNoTracking()
            .Where(p => p.IngressoId == ingressoId)
            .OrderBy(p => p.DataPagamento)
            .ToListAsync();
 
    private async Task DesfazerAsync(Pagamento pagamento)
    {
        _context.Pagamentos.Remove(pagamento);
        await _context.SaveChangesAsync();
    }
}