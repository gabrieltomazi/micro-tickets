using ClientesService.DTO;
using ClientesService.Infra;
using ClientesService.Models;

namespace ClientesService.Servicos
{
    // servico responsavel pelas regras de negocio e persistencia de clientes
    public class ServCliente
    {
        private readonly DataContext _dataContext;

        public ServCliente()
        {
            // recupera o contexto do banco de dados configurado no provedor estatico
            _dataContext = GeradorDeServicos.CarregarContexto();
        }

        // retorna a listagem de todos os clientes cadastrados
        public List<ClienteDTO> Listar()
        {
            return _dataContext.Clientes
                .Select(c => new ClienteDTO
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    Email = c.Email,
                    Documento = c.Documento
                })
                .ToList();
        }

        // busca os dados de um cliente pelo id
        public ClienteDTO? ObterPorId(int id)
        {
            var cliente = _dataContext.Clientes.Find(id);
            if (cliente == null)
            {
                return null;
            }

            return new ClienteDTO
            {
                Id = cliente.Id,
                Nome = cliente.Nome,
                Email = cliente.Email,
                Documento = cliente.Documento
            };
        }

        // valida e persiste um novo cliente no banco de dados
        public ClienteDTO Criar(CriarClienteDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome))
            {
                throw new ArgumentException("o nome do cliente é obrigatório.");
            }

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                throw new ArgumentException("o e-mail do cliente é obrigatório.");
            }

            if (string.IsNullOrWhiteSpace(dto.Documento))
            {
                throw new ArgumentException("o documento do cliente é obrigatório.");
            }

            var cliente = new Cliente
            {
                Nome = dto.Nome.Trim(),
                Email = dto.Email.Trim(),
                Documento = dto.Documento.Trim()
            };

            _dataContext.Clientes.Add(cliente);
            _dataContext.SaveChanges();

            return new ClienteDTO
            {
                Id = cliente.Id,
                Nome = cliente.Nome,
                Email = cliente.Email,
                Documento = cliente.Documento
            };
        }
    }
}
