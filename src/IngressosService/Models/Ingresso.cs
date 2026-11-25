namespace IngressosService.Models
{
    // enum dos status de um ingresso
    public enum StatusIngresso
    {
        Pendente = 1,
        Confirmado = 2,
        Cancelado = 3
    }

    // entidade que representa um ingresso no sistema utilizando identificador guid e enum de status
    public class Ingresso
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string CodigoTicket { get; set; } = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        public int EventoId { get; set; }
        public int ClienteId { get; set; }
        public decimal Preco { get; set; }
        public StatusIngresso Status { get; set; } = StatusIngresso.Pendente;
        public DateTime DataEmissao { get; set; } = DateTime.UtcNow;
    }
}
