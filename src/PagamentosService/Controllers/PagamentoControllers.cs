using Microsoft.AspNetCore.Mvc;
using PagamentosService.DTO;
using PagamentosService.Servicos;

namespace PagamentosService.Controllers
{
    // controller para registro e consulta de pagamentos
    [ApiController]
    [Route("api/[controller]")]
    public class PagamentosController : ControllerBase
    {
        private readonly ServPagamento _serv;

        public PagamentosController()
        {
            _serv = new ServPagamento();
        }

        // registra um novo pagamento e aciona a confirmacao do ingresso
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PagamentoRequest req)
        {
            var (pagamento, erro) = await _serv.CriarAsync(req);
            if (pagamento is null)
            {
                return erro!.StartsWith("não foi possível") || erro.StartsWith("IngressosService")
                    ? StatusCode(StatusCodes.Status502BadGateway, new { erro })
                    : BadRequest(new { erro });
            }

            return CreatedAtAction(nameof(GetById), new { id = pagamento.Id }, pagamento);
        }

        // busca os dados de um pagamento pelo id
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var pagamento = await _serv.ObterPorIdAsync(id);
            return pagamento is null ? NotFound(new { mensagem = "pagamento não encontrado." }) : Ok(pagamento);
        }

        // lista os pagamentos vinculados a determinado ingresso
        [HttpGet("ingresso/{ingressoId:guid}")]
        public async Task<IActionResult> GetPorIngresso(Guid ingressoId)
        {
            var pagamentos = await _serv.ListarPorIngressoAsync(ingressoId);

            // retorna 204 no content caso nenhum pagamento esteja vinculado ao ingresso
            if (pagamentos == null || !pagamentos.Any())
            {
                return NoContent();
            }

            return Ok(pagamentos);
        }
    }
}
