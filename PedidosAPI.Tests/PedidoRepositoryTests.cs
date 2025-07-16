using Xunit;
using PedidosAPI.Api.Repositories;
using PedidosAPI.Api.Models;

namespace PedidosAPI.Tests
{
    public class PedidoRepositoryTests
    {
        [Fact]
        public async Task CriarAsync_DeveCriarPedidoComIdIncremental()
        {
            // Arrange
            var repository = new PedidoRepository();
            var pedido1 = new Pedido
            {
                NomeCliente = "João Silva",
                EmailCliente = "joao@email.com",
                Status = StatusPedido.Pendente,
                Itens = new List<ItemPedido>()
            };

            var pedido2 = new Pedido
            {
                NomeCliente = "Maria Santos",
                EmailCliente = "maria@email.com",
                Status = StatusPedido.Confirmado,
                Itens = new List<ItemPedido>()
            };

            // Act
            var resultado1 = await repository.CriarAsync(pedido1);
            var resultado2 = await repository.CriarAsync(pedido2);

            // Assert
            Assert.Equal(1, resultado1.Id);
            Assert.Equal(2, resultado2.Id);
            Assert.True(resultado1.DataPedido > DateTime.MinValue);
            Assert.True(resultado2.DataPedido > DateTime.MinValue);
        }

        [Fact]
        public async Task GetTodosAsync_DeveRetornarTodosPedidosCriados()
        {
            // Arrange
            var repository = new PedidoRepository();
            var pedido1 = new Pedido
            {
                NomeCliente = "João Silva",
                EmailCliente = "joao@email.com",
                Status = StatusPedido.Pendente,
                Itens = new List<ItemPedido>()
            };

            var pedido2 = new Pedido
            {
                NomeCliente = "Maria Santos",
                EmailCliente = "maria@email.com",
                Status = StatusPedido.Confirmado,
                Itens = new List<ItemPedido>()
            };

            await repository.CriarAsync(pedido1);
            await repository.CriarAsync(pedido2);

            // Act
            var resultado = await repository.GetTodosAsync();

            // Assert
            Assert.Equal(2, resultado.Count());
            Assert.Contains(resultado, p => p.NomeCliente == "João Silva");
            Assert.Contains(resultado, p => p.NomeCliente == "Maria Santos");
        }

        [Fact]
        public async Task GetPorIdAsync_PedidoExistente_DeveRetornarPedido()
        {
            // Arrange
            var repository = new PedidoRepository();
            var pedido = new Pedido
            {
                NomeCliente = "Carlos Oliveira",
                EmailCliente = "carlos@email.com",
                Status = StatusPedido.EmPreparacao,
                ValorTotal = 150.75m,
                Itens = new List<ItemPedido>()
            };

            var pedidoCriado = await repository.CriarAsync(pedido);

            // Act
            var resultado = await repository.GetPorIdAsync(pedidoCriado.Id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(pedidoCriado.Id, resultado.Id);
            Assert.Equal("Carlos Oliveira", resultado.NomeCliente);
            Assert.Equal("carlos@email.com", resultado.EmailCliente);
            Assert.Equal(StatusPedido.EmPreparacao, resultado.Status);
            Assert.Equal(150.75m, resultado.ValorTotal);
        }

        [Fact]
        public async Task GetPorIdAsync_PedidoInexistente_DeveRetornarNull()
        {
            // Arrange
            var repository = new PedidoRepository();

            // Act
            var resultado = await repository.GetPorIdAsync(999);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        public async Task AtualizarAsync_PedidoExistente_DeveAtualizarPedido()
        {
            // Arrange
            var repository = new PedidoRepository();
            var pedido = new Pedido
            {
                NomeCliente = "Ana Costa",
                EmailCliente = "ana@email.com",
                Status = StatusPedido.Pendente,
                ValorTotal = 100.00m,
                Itens = new List<ItemPedido>()
            };

            var pedidoCriado = await repository.CriarAsync(pedido);

            // Modificar dados para atualização
            pedidoCriado.Status = StatusPedido.Confirmado;
            pedidoCriado.ValorTotal = 120.00m;

            // Act
            var resultado = await repository.AtualizarAsync(pedidoCriado);

            // Assert
            Assert.Equal(StatusPedido.Confirmado, resultado.Status);
            Assert.Equal(120.00m, resultado.ValorTotal);

            // Verificar se foi realmente atualizado no repositório
            var pedidoAtualizado = await repository.GetPorIdAsync(pedidoCriado.Id);
            Assert.NotNull(pedidoAtualizado);
            Assert.Equal(StatusPedido.Confirmado, pedidoAtualizado.Status);
            Assert.Equal(120.00m, pedidoAtualizado.ValorTotal);
        }

        [Fact]
        public async Task AtualizarAsync_PedidoInexistente_DeveLancarKeyNotFoundException()
        {
            // Arrange
            var repository = new PedidoRepository();
            var pedido = new Pedido
            {
                Id = 999,
                NomeCliente = "Teste",
                EmailCliente = "teste@email.com",
                Status = StatusPedido.Pendente,
                Itens = new List<ItemPedido>()
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => repository.AtualizarAsync(pedido));

            Assert.Contains("Pedido com ID 999 não encontrado", exception.Message);
        }

        [Fact]
        public async Task DeletarAsync_PedidoExistente_DeveRemoverPedido()
        {
            // Arrange
            var repository = new PedidoRepository();
            var pedido = new Pedido
            {
                NomeCliente = "Pedro Lima",
                EmailCliente = "pedro@email.com",
                Status = StatusPedido.Pendente,
                Itens = new List<ItemPedido>()
            };

            var pedidoCriado = await repository.CriarAsync(pedido);

            // Act
            await repository.DeletarAsync(pedidoCriado.Id);

            // Assert
            var pedidoDeletado = await repository.GetPorIdAsync(pedidoCriado.Id);
            Assert.Null(pedidoDeletado);

            var todosPedidos = await repository.GetTodosAsync();
            Assert.Empty(todosPedidos);
        }

        [Fact]
        public async Task DeletarAsync_PedidoInexistente_NaoDeveLancarExcecao()
        {
            // Arrange
            var repository = new PedidoRepository();

            // Act & Assert (não deve lançar exceção)
            await repository.DeletarAsync(999);
        }

        [Fact]
        public void ItemPedido_ValorTotal_DeveSerCalculadoCorretamente()
        {
            // Arrange
            var item = new ItemPedido
            {
                NomeProduto = "Produto Teste",
                Quantidade = 3,
                PrecoUnitario = 25.50m
            };

            // Act
            var valorTotal = item.ValorTotal;

            // Assert
            Assert.Equal(76.50m, valorTotal);
        }

        [Fact]
        public void StatusPedido_DeveConterTodosOsValoresEsperados()
        {
            // Arrange & Act & Assert
            Assert.True(Enum.IsDefined(typeof(StatusPedido), StatusPedido.Pendente));
            Assert.True(Enum.IsDefined(typeof(StatusPedido), StatusPedido.Confirmado));
            Assert.True(Enum.IsDefined(typeof(StatusPedido), StatusPedido.EmPreparacao));
            Assert.True(Enum.IsDefined(typeof(StatusPedido), StatusPedido.Enviado));
            Assert.True(Enum.IsDefined(typeof(StatusPedido), StatusPedido.Entregue));
            Assert.True(Enum.IsDefined(typeof(StatusPedido), StatusPedido.Cancelado));

            Assert.Equal(0, (int)StatusPedido.Pendente);
            Assert.Equal(1, (int)StatusPedido.Confirmado);
            Assert.Equal(2, (int)StatusPedido.EmPreparacao);
            Assert.Equal(3, (int)StatusPedido.Enviado);
            Assert.Equal(4, (int)StatusPedido.Entregue);
            Assert.Equal(5, (int)StatusPedido.Cancelado);
        }
    }
}
