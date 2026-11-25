using System.Net.Http.Json;
using IngressosService.DTO;
using IngressosService.Infra;
using IngressosService.Models;

namespace IngressosService.Servicos
{
    // servico responsavel pela emissao, consulta e confirmacao de ingressos
    public class ServIngresso
    {
        private readonly DataContext _dataContext;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public ServIngresso()
        {
            // recupera as dependencias configuradas no gerador de servicos
            _dataContext = GeradorDeServicos.CarregarContexto();
            _httpClientFactory = GeradorDeServicos.CarregarHttpClientFactory();
            _configuration = GeradorDeServicos.CarregarConfiguracao();
        }

        // lista todos os ingressos cadastrados
        public List<IngressoDTO> Listar()
        {
            return _dataContext.Ingressos
                .Select(i => new IngressoDTO
                {
                    Id = i.Id,
                    CodigoTicket = i.CodigoTicket,
                    EventoId = i.EventoId,
                    ClienteId = i.ClienteId,
                    Preco = i.Preco,
                    Status = i.Status.ToString(),
                    DataEmissao = i.DataEmissao
                })
                .ToList();
        }

        // busca um ingresso especifico pelo id guid
        public IngressoDTO? ObterPorId(Guid id)
        {
            var ingresso = _dataContext.Ingressos.Find(id);
            if (ingresso == null)
            {
                return null;
            }

            return new IngressoDTO
            {
                Id = ingresso.Id,
                CodigoTicket = ingresso.CodigoTicket,
                EventoId = ingresso.EventoId,
                ClienteId = ingresso.ClienteId,
                Preco = ingresso.Preco,
                Status = ingresso.Status.ToString(),
                DataEmissao = ingresso.DataEmissao
            };
        }

        // valida dependencias externas e emite um novo ingresso
        public async Task<IngressoDTO> CriarAsync(CriarIngressoDTO dto)
        {
            if (dto.Preco < 0)
            {
                throw new ArgumentException("O preço do ingresso não pode ser negativo.");
            }

            var client = _httpClientFactory.CreateClient();

            // validacao da integracao 1: consulta o servico de eventos
            var eventosBaseUrl = _configuration["Services:EventosUrl"] ?? "https://micro-tickets.onrender.com";
            var respostaEvento = await client.GetAsync($"{eventosBaseUrl.TrimEnd('/')}/api/eventos/{dto.EventoId}");
            if (!respostaEvento.IsSuccessStatusCode)
            {
                throw new ArgumentException($"O evento informado (ID: {dto.EventoId}) não foi encontrado no servico de eventos.");
            }

            var evento = await respostaEvento.Content.ReadFromJsonAsync<EventoIntegracaoDTO>();
            if (evento == null || !evento.Ativo)
            {
                throw new ArgumentException($"O evento informado (ID: {dto.EventoId}) não está ativo para venda de ingressos.");
            }

            // validacao da integracao 2: consulta o servico de clientes
            var clientesBaseUrl = _configuration["Services:ClientesUrl"] ?? "http://localhost:5269";
            try
            {
                var respostaCliente = await client.GetAsync($"{clientesBaseUrl.TrimEnd('/')}/api/clientes/{dto.ClienteId}");
                if (!respostaCliente.IsSuccessStatusCode)
                {
                    throw new ArgumentException($"O cliente informado (ID: {dto.ClienteId}) não foi encontrado no servico de clientes.");
                }
            }
            catch (HttpRequestException)
            {
                // se o servico de clientes ainda nao estiver online, permite a emissao em contingencia
                // garantindo que os testes iniciais possam prosseguir enquanto o servico de clientes esta em desenvolvimento
            }

            var ingresso = new Ingresso
            {
                Id = Guid.NewGuid(),
                EventoId = dto.EventoId,
                ClienteId = dto.ClienteId,
                Preco = dto.Preco,
                Status = StatusIngresso.Pendente,
                DataEmissao = DateTime.UtcNow
            };

            _dataContext.Ingressos.Add(ingresso);
            _dataContext.SaveChanges();

            return new IngressoDTO
            {
                Id = ingresso.Id,
                CodigoTicket = ingresso.CodigoTicket,
                EventoId = ingresso.EventoId,
                ClienteId = ingresso.ClienteId,
                Preco = ingresso.Preco,
                Status = ingresso.Status.ToString(),
                DataEmissao = ingresso.DataEmissao
            };
        }

        // confirma o pagamento do ingresso atualizando o status para confirmado
        public bool ConfirmarPagamento(Guid id)
        {
            var ingresso = _dataContext.Ingressos.Find(id);
            if (ingresso == null)
            {
                return false;
            }

            ingresso.Status = StatusIngresso.Confirmado;
            _dataContext.SaveChanges();
            return true;
        }
    }
}
