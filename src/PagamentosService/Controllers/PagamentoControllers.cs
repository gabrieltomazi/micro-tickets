using Microsoft.AspNetCore.Mvc;
using PagamentosService.Models;
using PagamentosService.Servicos;
using PagamentosService.DTO;
 
namespace PagamentosService.Controllers;
 
[ApiController]
[Route("api/pagamentos")]
public class PagamentosController : ControllerBase
{
    private readonly ServPagamento _serv;
 
    public PagamentosController(ServPagamento serv) => _serv = serv;
 
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PagamentoRequest req)
    {
        var (pagamento, erro) = await _serv.CriarAsync(req);
        if (pagamento is null)
            return erro!.StartsWith("Não foi possível") || erro.StartsWith("IngressosService")
                ? StatusCode(StatusCodes.Status502BadGateway, new { erro })
                : BadRequest(new { erro });
 
        return CreatedAtAction(nameof(GetById), new { id = pagamento.Id }, pagamento);
    }
 
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var pagamento = await _serv.ObterPorIdAsync(id);
        return pagamento is null ? NotFound() : Ok(pagamento);
    }
 
    [HttpGet("ingresso/{ingressoId:int}")]
    public async Task<IActionResult> GetPorIngresso(int ingressoId) =>
        Ok(await _serv.ListarPorIngressoAsync(ingressoId));
}