using IngressosService.DTO;
using IngressosService.Servicos;
using Microsoft.AspNetCore.Mvc;

namespace IngressosService.Controllers
{
    // controller para emissao e gestao de ingressos
    [ApiController]
    [Route("api/[controller]")]
    public class IngressosController : ControllerBase
    {
        private readonly ServIngresso _servIngresso;

        public IngressosController()
        {
            _servIngresso = new ServIngresso();
        }

        // retorna a listagem completa de ingressos
        [HttpGet]
        public IActionResult Listar()
        {
            try
            {
                var ingressos = _servIngresso.Listar();

                // se nao houver ingressos cadastrados retorna 204 no content
                if (ingressos == null || !ingressos.Any())
                {
                    return NoContent();
                }

                return Ok(ingressos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }

        // busca os dados de um ingresso por id guid
        [HttpGet("{id:guid}")]
        public IActionResult ObterPorId(Guid id)
        {
            try
            {
                var ingresso = _servIngresso.ObterPorId(id);
                if (ingresso == null)
                {
                    return NotFound(new { mensagem = "Ingresso não encontrado." });
                }

                return Ok(ingresso);
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }

        // emite um novo ingresso com validacao síncrona de evento e cliente
        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarIngressoDTO dto)
        {
            try
            {
                var ingressoCriado = await _servIngresso.CriarAsync(dto);
                return CreatedAtAction(nameof(ObterPorId), new { id = ingressoCriado.Id }, ingressoCriado);
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

        // atualiza o status do ingresso para confirmado apos o pagamento
        [HttpPatch("{id:guid}/confirmar")]
        public IActionResult Confirmar(Guid id)
        {
            try
            {
                var sucesso = _servIngresso.ConfirmarPagamento(id);
                if (!sucesso)
                {
                    return NotFound(new { mensagem = "Ingresso não encontrado para confirmação." });
                }

                return Ok(new { mensagem = "Ingresso confirmado com sucesso.", status = "Confirmado" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }
    }
}
