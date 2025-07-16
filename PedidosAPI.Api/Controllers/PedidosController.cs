using Microsoft.AspNetCore.Mvc;
using PedidosAPI.Api.DTOs;
using PedidosAPI.Api.Services;

namespace PedidosAPI.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;

        public PedidosController(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        /// <summary>
        /// Lista todos os pedidos
        /// </summary>
        /// <returns>Lista de pedidos</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PedidoDto>), 200)]
        public async Task<ActionResult<IEnumerable<PedidoDto>>> GetTodos()
        {
            try
            {
                var pedidos = await _pedidoService.GetTodosAsync();
                return Ok(pedidos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Busca um pedido por ID
        /// </summary>
        /// <param name="id">ID do pedido</param>
        /// <returns>Dados do pedido</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PedidoDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<PedidoDto>> GetPorId(int id)
        {
            try
            {
                var pedido = await _pedidoService.GetPorIdAsync(id);

                if (pedido == null)
                    return NotFound($"Pedido com ID {id} não encontrado");

                return Ok(pedido);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Cria um novo pedido
        /// </summary>
        /// <param name="criarPedidoDto">Dados do pedido a ser criado</param>
        /// <returns>Pedido criado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(PedidoDto), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<PedidoDto>> Criar([FromBody] CriarPedidoDto criarPedidoDto)
        {
            try
            {
                var pedido = await _pedidoService.CriarAsync(criarPedidoDto);
                return CreatedAtAction(nameof(GetPorId), new { id = pedido.Id }, pedido);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }
    }
}
