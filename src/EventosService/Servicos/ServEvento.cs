using EventosService.DTO;
using EventosService.Infra;
using EventosService.Models;

namespace EventosService.Servicos
{
    // serviço responsável pelas regras de negócio de eventos
    public class ServEvento
    {
        private readonly DataContext _dataContext;

        public ServEvento()
        {
            // recupera o contexto do banco de dados
            _dataContext = GeradorDeServicos.CarregarContexto();
        }

        // lista todos os eventos cadastrados
        public List<EventoDTO> Listar()
        {
            return _dataContext.Eventos
                .Select(e => new EventoDTO
                {
                    Id = e.Id,
                    Nome = e.Nome,
                    Local = e.Local,
                    Data = e.Data,
                    CapacidadeMaxima = e.CapacidadeMaxima,
                    Ativo = e.Ativo
                })
                .ToList();
        }

        // busca um evento por id
        public EventoDTO? ObterPorId(int id)
        {
            var evento = _dataContext.Eventos.Find(id);
            if (evento == null)
            {
                return null;
            }

            return new EventoDTO
            {
                Id = evento.Id,
                Nome = evento.Nome,
                Local = evento.Local,
                Data = evento.Data,
                CapacidadeMaxima = evento.CapacidadeMaxima,
                Ativo = evento.Ativo
            };
        }

        // cria e salva um novo evento
        public EventoDTO Criar(CriarEventoDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome))
            {
                throw new ArgumentException("o nome do evento é obrigatório.");
            }

            if (dto.CapacidadeMaxima <= 0)
            {
                throw new ArgumentException("a capacidade máxima deve ser maior que zero.");
            }

            var evento = new Evento
            {
                Nome = dto.Nome,
                Local = dto.Local,
                Data = dto.Data,
                CapacidadeMaxima = dto.CapacidadeMaxima,
                Ativo = dto.Ativo
            };

            _dataContext.Eventos.Add(evento);
            _dataContext.SaveChanges();

            return new EventoDTO
            {
                Id = evento.Id,
                Nome = evento.Nome,
                Local = evento.Local,
                Data = evento.Data,
                CapacidadeMaxima = evento.CapacidadeMaxima,
                Ativo = evento.Ativo
            };
        }
    }
}
