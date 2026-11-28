namespace ClientesService.DTO
{
    // dto de entrada para realizacao do cadastro de um novo cliente
    public class CriarClienteDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
    }

    // dto de saida com os dados cadastrais completos do cliente
    public class ClienteDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
    }
}
