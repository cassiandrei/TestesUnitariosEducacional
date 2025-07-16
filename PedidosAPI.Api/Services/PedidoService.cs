using PedidosAPI.Api.DTOs;
using PedidosAPI.Api.Models;
using PedidosAPI.Api.Repositories;

namespace PedidosAPI.Api.Services
{
    public interface IPedidoService
    {
        Task<IEnumerable<PedidoDto>> GetTodosAsync();
        Task<PedidoDto?> GetPorIdAsync(int id);
        Task<PedidoDto> CriarAsync(CriarPedidoDto criarPedidoDto);
    }

    public class PedidoService : IPedidoService
    {
        private readonly IPedidoRepository _pedidoRepository;

        public PedidoService(IPedidoRepository pedidoRepository)
        {
            _pedidoRepository = pedidoRepository;
        }

        public async Task<IEnumerable<PedidoDto>> GetTodosAsync()
        {
            var pedidos = await _pedidoRepository.GetTodosAsync();
            return pedidos.Select(MapearParaDto);
        }

        public async Task<PedidoDto?> GetPorIdAsync(int id)
        {
            var pedido = await _pedidoRepository.GetPorIdAsync(id);
            return pedido != null ? MapearParaDto(pedido) : null;
        }

        public async Task<PedidoDto> CriarAsync(CriarPedidoDto criarPedidoDto)
        {
            ValidarCriarPedido(criarPedidoDto);

            var pedido = new Pedido
            {
                NomeCliente = criarPedidoDto.NomeCliente,
                EmailCliente = criarPedidoDto.EmailCliente,
                Status = StatusPedido.Pendente,
                Itens = criarPedidoDto.Itens.Select(item => new ItemPedido
                {
                    NomeProduto = item.NomeProduto,
                    Quantidade = item.Quantidade,
                    PrecoUnitario = item.PrecoUnitario
                }).ToList()
            };

            pedido.ValorTotal = pedido.Itens.Sum(i => i.ValorTotal);

            var pedidoCriado = await _pedidoRepository.CriarAsync(pedido);
            return MapearParaDto(pedidoCriado);
        }

        private static void ValidarCriarPedido(CriarPedidoDto criarPedidoDto)
        {
            if (string.IsNullOrWhiteSpace(criarPedidoDto.NomeCliente))
                throw new ArgumentException("Nome do cliente é obrigatório");

            if (string.IsNullOrWhiteSpace(criarPedidoDto.EmailCliente))
                throw new ArgumentException("Email do cliente é obrigatório");

            if (!criarPedidoDto.Itens.Any())
                throw new ArgumentException("Pedido deve ter pelo menos um item");

            foreach (var item in criarPedidoDto.Itens)
            {
                if (string.IsNullOrWhiteSpace(item.NomeProduto))
                    throw new ArgumentException("Nome do produto é obrigatório");

                if (item.Quantidade <= 0)
                    throw new ArgumentException("Quantidade deve ser maior que zero");

                if (item.PrecoUnitario <= 0)
                    throw new ArgumentException("Preço unitário deve ser maior que zero");
            }
        }

        private static PedidoDto MapearParaDto(Pedido pedido)
        {
            return new PedidoDto
            {
                Id = pedido.Id,
                NomeCliente = pedido.NomeCliente,
                EmailCliente = pedido.EmailCliente,
                DataPedido = pedido.DataPedido,
                ValorTotal = pedido.ValorTotal,
                Status = pedido.Status.ToString(),
                Itens = pedido.Itens.Select(item => new ItemPedidoDto
                {
                    NomeProduto = item.NomeProduto,
                    Quantidade = item.Quantidade,
                    PrecoUnitario = item.PrecoUnitario
                }).ToList()
            };
        }
    }
}
