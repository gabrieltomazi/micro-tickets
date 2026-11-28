using Microsoft.EntityFrameworkCore;
using PagamentosService.DTO;
using PagamentosService.Infra;
using PagamentosService.Models;

namespace PagamentosService.Servicos
{
    // servico responsavel pelo processamento e confirmacao de pagamentos
    public class ServPagamento
    {
        public static readonly string[] MetodosValidos = { "PIX", "CartaoCredito", "Boleto" };

        private readonly DataContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public ServPagamento()
        {
            // recupera as dependencias configuradas no provedor estatico
            _context = GeradorDeServicos.CarregarContexto();
            _httpClientFactory = GeradorDeServicos.CarregarHttpClientFactory();
            _configuration = GeradorDeServicos.CarregarConfiguracao();
        }

        // cria o registro de pagamento e aciona a confirmacao do ingresso no IngressosService
        public async Task<(Pagamento? pagamento, string? erro)> CriarAsync(PagamentoRequest req)
        {
            if (req.Valor <= 0)
            {
                return (null, "o valor do pagamento deve ser maior que zero.");
            }

            var metodo = MetodosValidos.FirstOrDefault(m =>
                m.Equals(req.MetodoPagamento, StringComparison.OrdinalIgnoreCase));
            if (metodo is null)
            {
                return (null, "método de pagamento inválido. Use: PIX, CartaoCredito ou Boleto.");
            }

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

            // disparar a requisicao de confirmacao para o IngressosService
            var client = _httpClientFactory.CreateClient();
            var ingressosBaseUrl = _configuration["Services:IngressosUrl"] ?? "http://localhost:5088";

            try
            {
                var url = $"{ingressosBaseUrl.TrimEnd('/')}/api/ingressos/{pagamento.IngressoId}/confirmar";
                var resp = await client.PatchAsync(url, null);

                if (!resp.IsSuccessStatusCode)
                {
                    await DesfazerAsync(pagamento);
                    return (null, $"IngressosService recusou a confirmação (HTTP {(int)resp.StatusCode}).");
                }
            }
            catch (HttpRequestException)
            {
                await DesfazerAsync(pagamento);
                return (null, "não foi possível se comunicar com o IngressosService.");
            }

            return (pagamento, null);
        }

        // consulta um pagamento especifico por id
        public Task<Pagamento?> ObterPorIdAsync(int id) =>
            _context.Pagamentos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

        // lista todos os pagamentos vinculados a determinado ingresso (Guid)
        public Task<List<Pagamento>> ListarPorIngressoAsync(Guid ingressoId) =>
            _context.Pagamentos.AsNoTracking()
                .Where(p => p.IngressoId == ingressoId)
                .OrderBy(p => p.DataPagamento)
                .ToListAsync();

        // cancela a persistencia caso o servico de ingressos nao confirme
        private async Task DesfazerAsync(Pagamento pagamento)
        {
            _context.Pagamentos.Remove(pagamento);
            await _context.SaveChangesAsync();
        }
    }
}
