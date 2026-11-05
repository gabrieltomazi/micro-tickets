namespace EventosService.Servicos
{
    // serviço de domínio responsável pelas regras de negócio de eventos
    public class ServEvento
    {
        private readonly DataContext _dataContext;

        public ServEvento(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void InserirEvento(Evento evento)
        {
            ValidarEvento(evento);

            evento.Ativo = true;

            _dataContext.Eventos.Add(evento);
            _dataContext.SaveChanges();
        }

        public void ValidarEvento(Evento evento)
        {
            if (string.IsNullOrWhiteSpace(evento.Nome))
            {
                throw new Exception("o nome do evento é obrigatório.");
            }

            if (string.IsNullOrWhiteSpace(evento.Local))
            {
                throw new Exception("o local do evento é obrigatório.");
            }

            if (evento.CapacidadeMaxima <= 0)
            {
                throw new Exception("a capacidade máxima do evento deve ser maior que zero.");
            }

            if (evento.Data == default)
            {
                throw new Exception("a data do evento é inválida.");
            }
        }

        public List<Evento> ListarEventos()
        {
            return _dataContext.Eventos.ToList();
        }

        public Evento? BuscarPorId(int id)
        {
            return _dataContext.Eventos.FirstOrDefault(e => e.Id == id);
        }
    }
}
