namespace ClientesService.Models
{
    // entidade que representa um comprador ou participante de eventos no sistema
    public class Cliente
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
    }
}
