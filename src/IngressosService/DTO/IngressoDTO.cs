using IngressosService.Models;

namespace IngressosService.DTO
{
    // dto utilizado para solicitar a emissao de um novo ingresso
    public class CriarIngressoDTO
    {
        public int EventoId { get; set; }
        public int ClienteId { get; set; }
        public decimal Preco { get; set; }
    }

    // dto de resposta com dados completos do ingresso emitido
    public class IngressoDTO
    {
        public Guid Id { get; set; }
        public string CodigoTicket { get; set; } = string.Empty;
        public int EventoId { get; set; }
        public int ClienteId { get; set; }
        public decimal Preco { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime DataEmissao { get; set; }
    }

    // dto auxiliar para leitura de dados retornados pelo EventosService
    public class EventoIntegracaoDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public bool Ativo { get; set; }
    }

    // dto auxiliar para leitura de dados retornados pelo ClientesService
    public class ClienteIntegracaoDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
    }
}
