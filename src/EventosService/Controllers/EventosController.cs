using EventosService.DTO;
using EventosService.Servicos;
using Microsoft.AspNetCore.Mvc;

namespace EventosService.Controllers
{
    // controller responsável pelos endpoints de eventos
    [ApiController]
    [Route("api/[controller]")]
    public class EventosController : ControllerBase
    {
        private readonly ServEvento _servEvento;

        public EventosController(ServEvento servEvento)
        {
            _servEvento = servEvento;
        }

        [HttpGet]
        public IActionResult Listar()
        {
            try
            {
                var eventos = _servEvento.ListarEventos();
                return Ok(eventos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult BuscarPorId(int id)
        {
            try
            {
                var evento = _servEvento.BuscarPorId(id);

                if (evento == null)
                {
                    return NotFound("evento não encontrado.");
                }

                return Ok(evento);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Inserir([FromBody] InserirEventoDTO dto)
        {
            try
            {
                var evento = new Evento
                {
                    Nome = dto.Nome,
                    Local = dto.Local,
                    Data = dto.Data,
                    CapacidadeMaxima = dto.CapacidadeMaxima
                };

                _servEvento.InserirEvento(evento);

                return CreatedAtAction(nameof(BuscarPorId), new { id = evento.Id }, evento);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
