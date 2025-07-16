using PedidosAPI.Api.Models;

namespace PedidosAPI.Api.Repositories
{
    public interface IPedidoRepository
    {
        Task<IEnumerable<Pedido>> GetTodosAsync();
        Task<Pedido?> GetPorIdAsync(int id);
        Task<Pedido> CriarAsync(Pedido pedido);
        Task<Pedido> AtualizarAsync(Pedido pedido);
        Task DeletarAsync(int id);
    }

    public class PedidoRepository : IPedidoRepository
    {
        private readonly List<Pedido> _pedidos = new();
        private int _proximoId = 1;

        public Task<IEnumerable<Pedido>> GetTodosAsync()
        {
            return Task.FromResult<IEnumerable<Pedido>>(_pedidos.ToList());
        }

        public Task<Pedido?> GetPorIdAsync(int id)
        {
            var pedido = _pedidos.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(pedido);
        }

        public Task<Pedido> CriarAsync(Pedido pedido)
        {
            pedido.Id = _proximoId++;
            pedido.DataPedido = DateTime.Now;
            _pedidos.Add(pedido);
            return Task.FromResult(pedido);
        }

        public Task<Pedido> AtualizarAsync(Pedido pedido)
        {
            var pedidoExistente = _pedidos.FirstOrDefault(p => p.Id == pedido.Id);
            if (pedidoExistente != null)
            {
                var index = _pedidos.IndexOf(pedidoExistente);
                _pedidos[index] = pedido;
                return Task.FromResult(pedido);
            }
            throw new KeyNotFoundException($"Pedido com ID {pedido.Id} não encontrado");
        }

        public Task DeletarAsync(int id)
        {
            var pedido = _pedidos.FirstOrDefault(p => p.Id == id);
            if (pedido != null)
            {
                _pedidos.Remove(pedido);
            }
            return Task.CompletedTask;
        }
    }
}
