using EventosService.DTO;
using EventosService.Servicos;
using Microsoft.AspNetCore.Mvc;

namespace EventosService.Controllers
{
    // controller para gerenciamento de eventos
    [ApiController]
    [Route("api/[controller]")]
    public class EventosController : ControllerBase
    {
        private readonly ServEvento _servEvento;

        public EventosController()
        {
            _servEvento = new ServEvento();
        }

        // retorna a listagem completa de eventos
        [HttpGet]
        public IActionResult Listar()
        {
            try
            {
                var eventos = _servEvento.Listar();

                // verifica se a lista esta vazia ou nula para retornar 204 no content
                if (eventos == null || !eventos.Any())
                {
                    return NoContent();
                }

                return Ok(eventos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // busca um evento pelo id
        [HttpGet("{id}")]
        public IActionResult ObterPorId(int id)
        {
            try
            {
                var evento = _servEvento.ObterPorId(id);
                if (evento == null)
                {
                    return NotFound(new { mensagem = "evento não encontrado." });
                }

                return Ok(evento);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // cadastra um novo evento
        [HttpPost]
        public IActionResult Criar([FromBody] CriarEventoDTO dto)
        {
            try
            {
                var eventoCriado = _servEvento.Criar(dto);
                return CreatedAtAction(nameof(ObterPorId), new { id = eventoCriado.Id }, eventoCriado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }
    }
}
