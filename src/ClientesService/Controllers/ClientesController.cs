using ClientesService.DTO;
using ClientesService.Servicos;
using Microsoft.AspNetCore.Mvc;

namespace ClientesService.Controllers
{
    // controller para gerenciamento cadastral de clientes
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly ServCliente _servCliente;

        public ClientesController()
        {
            _servCliente = new ServCliente();
        }

        // retorna a listagem completa de clientes cadastrados
        [HttpGet]
        public IActionResult Listar()
        {
            try
            {
                var clientes = _servCliente.Listar();

                // se a lista estiver vazia retorna 204 no content
                if (clientes == null || !clientes.Any())
                {
                    return NoContent();
                }

                return Ok(clientes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }

        // busca os dados cadastrais de um cliente pelo id
        [HttpGet("{id:int}")]
        public IActionResult ObterPorId(int id)
        {
            try
            {
                var cliente = _servCliente.ObterPorId(id);
                if (cliente == null)
                {
                    return NotFound(new { mensagem = "cliente não encontrado." });
                }

                return Ok(cliente);
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }

        // realiza o cadastro de um novo cliente
        [HttpPost]
        public IActionResult Criar([FromBody] CriarClienteDTO dto)
        {
            try
            {
                var clienteCriado = _servCliente.Criar(dto);
                return CreatedAtAction(nameof(ObterPorId), new { id = clienteCriado.Id }, clienteCriado);
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
