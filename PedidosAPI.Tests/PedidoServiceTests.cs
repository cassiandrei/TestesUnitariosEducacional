using Moq;
using PedidosAPI.Api.DTOs;
using PedidosAPI.Api.Models;
using PedidosAPI.Api.Repositories;
using PedidosAPI.Api.Services;

namespace PedidosAPI.Tests
{
    public class PedidoServiceTests
    {
        private readonly Mock<IPedidoRepository> _repositoryMock;
        private readonly PedidoService _service;

        public PedidoServiceTests()
        {
            _repositoryMock = new Mock<IPedidoRepository>();
            _service = new PedidoService(_repositoryMock.Object);
        }

        [Fact]
        public async Task GetTodosAsync_DeveRetornarListaDePedidos()
        {
            // Arrange
            var pedidos = new List<Pedido>
            {
                new Pedido
                {
                    Id = 1,
                    NomeCliente = "João Silva",
                    EmailCliente = "joao@email.com",
                    DataPedido = DateTime.Now,
                    ValorTotal = 100.50m,
                    Status = StatusPedido.Pendente,
                    Itens = new List<ItemPedido>
                    {
                        new ItemPedido
                        {
                            Id = 1,
                            NomeProduto = "Produto A",
                            Quantidade = 2,
                            PrecoUnitario = 50.25m,
                        },
                    },
                },
            };

            _repositoryMock.Setup(r => r.GetTodosAsync()).ReturnsAsync(pedidos);

            // Act
            var resultado = await _service.GetTodosAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.Single(resultado);

            var pedidoDto = resultado.First();
            Assert.Equal(1, pedidoDto.Id);
            Assert.Equal("João Silva", pedidoDto.NomeCliente);
            Assert.Equal("joao@email.com", pedidoDto.EmailCliente);
            Assert.Equal(100.50m, pedidoDto.ValorTotal);
            Assert.Equal("Pendente", pedidoDto.Status);
        }

        [Fact]
        public async Task GetPorIdAsync_PedidoExistente_DeveRetornarPedido()
        {
            // Arrange
            var pedido = new Pedido
            {
                Id = 1,
                NomeCliente = "Maria Santos",
                EmailCliente = "maria@email.com",
                DataPedido = DateTime.Now,
                ValorTotal = 200.00m,
                Status = StatusPedido.Confirmado,
                Itens = new List<ItemPedido>(),
            };

            _repositoryMock.Setup(r => r.GetPorIdAsync(1)).ReturnsAsync(pedido);

            // Act
            var resultado = await _service.GetPorIdAsync(1);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
            Assert.Equal("Maria Santos", resultado.NomeCliente);
            Assert.Equal("maria@email.com", resultado.EmailCliente);
            Assert.Equal(200.00m, resultado.ValorTotal);
            Assert.Equal("Confirmado", resultado.Status);
        }

        [Fact]
        public async Task GetPorIdAsync_PedidoInexistente_DeveRetornarNull()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetPorIdAsync(999)).ReturnsAsync((Pedido?)null);

            // Act
            var resultado = await _service.GetPorIdAsync(999);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        public async Task CriarAsync_PedidoValido_DeveCriarPedidoComSucesso()
        {
            // Arrange
            var criarPedidoDto = new CriarPedidoDto
            {
                NomeCliente = "Carlos Oliveira",
                EmailCliente = "carlos@email.com",
                Itens = new List<ItemPedidoDto>
                {
                    new ItemPedidoDto
                    {
                        NomeProduto = "Produto B",
                        Quantidade = 3,
                        PrecoUnitario = 25.00m,
                    },
                },
            };

            var pedidoCriado = new Pedido
            {
                Id = 1,
                NomeCliente = criarPedidoDto.NomeCliente,
                EmailCliente = criarPedidoDto.EmailCliente,
                DataPedido = DateTime.Now,
                ValorTotal = 75.00m,
                Status = StatusPedido.Pendente,
                Itens = new List<ItemPedido>
                {
                    new ItemPedido
                    {
                        Id = 1,
                        NomeProduto = "Produto B",
                        Quantidade = 3,
                        PrecoUnitario = 25.00m,
                    },
                },
            };

            _repositoryMock.Setup(r => r.CriarAsync(It.IsAny<Pedido>())).ReturnsAsync(pedidoCriado);

            // Act
            var resultado = await _service.CriarAsync(criarPedidoDto);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
            Assert.Equal("Carlos Oliveira", resultado.NomeCliente);
            Assert.Equal("carlos@email.com", resultado.EmailCliente);
            Assert.Equal(75.00m, resultado.ValorTotal);
            Assert.Equal("Pendente", resultado.Status);
            Assert.Single(resultado.Itens);

            // Verifica se o repositório foi chamado
            _repositoryMock.Verify(r => r.CriarAsync(It.IsAny<Pedido>()), Times.Once);
        }

        [Theory]
        [InlineData("", "email@teste.com", "Nome do cliente é obrigatório")]
        [InlineData("   ", "email@teste.com", "Nome do cliente é obrigatório")]
        [InlineData("João", "", "Email do cliente é obrigatório")]
        [InlineData("João", "   ", "Email do cliente é obrigatório")]
        public async Task CriarAsync_DadosInvalidos_DeveLancarArgumentException(
            string nomeCliente,
            string emailCliente,
            string mensagemEsperada
        )
        {
            // Arrange
            var criarPedidoDto = new CriarPedidoDto
            {
                NomeCliente = nomeCliente,
                EmailCliente = emailCliente,
                Itens = new List<ItemPedidoDto>
                {
                    new ItemPedidoDto
                    {
                        NomeProduto = "Produto",
                        Quantidade = 1,
                        PrecoUnitario = 10.00m,
                    },
                },
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CriarAsync(criarPedidoDto)
            );

            Assert.Equal(mensagemEsperada, exception.Message);
        }

        [Fact]
        public async Task CriarAsync_SemItens_DeveLancarArgumentException()
        {
            // Arrange
            var criarPedidoDto = new CriarPedidoDto
            {
                NomeCliente = "João",
                EmailCliente = "joao@email.com",
                Itens = new List<ItemPedidoDto>(),
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CriarAsync(criarPedidoDto)
            );

            Assert.Equal("Pedido deve ter pelo menos um item", exception.Message);
        }

        [Theory]
        [InlineData("", 1, 10.00, "Nome do produto é obrigatório")]
        [InlineData("Produto", 0, 10.00, "Quantidade deve ser maior que zero")]
        [InlineData("Produto", -1, 10.00, "Quantidade deve ser maior que zero")]
        [InlineData("Produto", 1, 0, "Preço unitário deve ser maior que zero")]
        [InlineData("Produto", 1, -5.00, "Preço unitário deve ser maior que zero")]
        public async Task CriarAsync_ItemInvalido_DeveLancarArgumentException(
            string nomeProduto,
            int quantidade,
            decimal precoUnitario,
            string mensagemEsperada
        )
        {
            // Arrange
            var criarPedidoDto = new CriarPedidoDto
            {
                NomeCliente = "João",
                EmailCliente = "joao@email.com",
                Itens = new List<ItemPedidoDto>
                {
                    new ItemPedidoDto
                    {
                        NomeProduto = nomeProduto,
                        Quantidade = quantidade,
                        PrecoUnitario = precoUnitario,
                    },
                },
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CriarAsync(criarPedidoDto)
            );

            Assert.Equal(mensagemEsperada, exception.Message);
        }
    }
}
