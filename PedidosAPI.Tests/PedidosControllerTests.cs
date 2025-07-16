using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using PedidosAPI.Api.Controllers;
using PedidosAPI.Api.Services;
using PedidosAPI.Api.DTOs;

namespace PedidosAPI.Tests
{
    public class PedidosControllerTests
    {
        private readonly Mock<IPedidoService> _serviceMock;
        private readonly PedidosController _controller;

        public PedidosControllerTests()
        {
            _serviceMock = new Mock<IPedidoService>();
            _controller = new PedidosController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetTodos_DeveRetornarOkComListaDePedidos()
        {
            // Arrange
            var pedidos = new List<PedidoDto>
            {
                new PedidoDto
                {
                    Id = 1,
                    NomeCliente = "João Silva",
                    EmailCliente = "joao@email.com",
                    DataPedido = DateTime.Now,
                    ValorTotal = 100.50m,
                    Status = "Pendente",
                    Itens = new List<ItemPedidoDto>()
                }
            };

            _serviceMock.Setup(s => s.GetTodosAsync())
                       .ReturnsAsync(pedidos);

            // Act
            var resultado = await _controller.GetTodos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var pedidosRetornados = Assert.IsType<List<PedidoDto>>(okResult.Value);
            Assert.Single(pedidosRetornados);
            Assert.Equal(1, pedidosRetornados.First().Id);
        }

        [Fact]
        public async Task GetTodos_QuandoServiceLancaExcecao_DeveRetornarStatusCode500()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetTodosAsync())
                       .ThrowsAsync(new Exception("Erro de teste"));

            // Act
            var resultado = await _controller.GetTodos();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(resultado.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Erro interno do servidor", statusCodeResult.Value?.ToString());
        }

        [Fact]
        public async Task GetPorId_PedidoExistente_DeveRetornarOkComPedido()
        {
            // Arrange
            var pedido = new PedidoDto
            {
                Id = 1,
                NomeCliente = "Maria Santos",
                EmailCliente = "maria@email.com",
                DataPedido = DateTime.Now,
                ValorTotal = 200.00m,
                Status = "Confirmado",
                Itens = new List<ItemPedidoDto>()
            };

            _serviceMock.Setup(s => s.GetPorIdAsync(1))
                       .ReturnsAsync(pedido);

            // Act
            var resultado = await _controller.GetPorId(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var pedidoRetornado = Assert.IsType<PedidoDto>(okResult.Value);
            Assert.Equal(1, pedidoRetornado.Id);
            Assert.Equal("Maria Santos", pedidoRetornado.NomeCliente);
        }

        [Fact]
        public async Task GetPorId_PedidoInexistente_DeveRetornarNotFound()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetPorIdAsync(999))
                       .ReturnsAsync((PedidoDto?)null);

            // Act
            var resultado = await _controller.GetPorId(999);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(resultado.Result);
            Assert.Contains("Pedido com ID 999 não encontrado", notFoundResult.Value?.ToString());
        }

        [Fact]
        public async Task GetPorId_QuandoServiceLancaExcecao_DeveRetornarStatusCode500()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetPorIdAsync(It.IsAny<int>()))
                       .ThrowsAsync(new Exception("Erro de teste"));

            // Act
            var resultado = await _controller.GetPorId(1);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(resultado.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Erro interno do servidor", statusCodeResult.Value?.ToString());
        }

        [Fact]
        public async Task Criar_PedidoValido_DeveRetornarCreatedAtAction()
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
                        PrecoUnitario = 25.00m
                    }
                }
            };

            var pedidoCriado = new PedidoDto
            {
                Id = 1,
                NomeCliente = "Carlos Oliveira",
                EmailCliente = "carlos@email.com",
                DataPedido = DateTime.Now,
                ValorTotal = 75.00m,
                Status = "Pendente",
                Itens = new List<ItemPedidoDto>
                {
                    new ItemPedidoDto
                    {
                        NomeProduto = "Produto B",
                        Quantidade = 3,
                        PrecoUnitario = 25.00m
                    }
                }
            };

            _serviceMock.Setup(s => s.CriarAsync(criarPedidoDto))
                       .ReturnsAsync(pedidoCriado);

            // Act
            var resultado = await _controller.Criar(criarPedidoDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(resultado.Result);
            Assert.Equal(nameof(PedidosController.GetPorId), createdResult.ActionName);
            Assert.Equal(1, createdResult.RouteValues?["id"]);

            var pedidoRetornado = Assert.IsType<PedidoDto>(createdResult.Value);
            Assert.Equal(1, pedidoRetornado.Id);
            Assert.Equal("Carlos Oliveira", pedidoRetornado.NomeCliente);
        }

        [Fact]
        public async Task Criar_PedidoInvalido_DeveRetornarBadRequest()
        {
            // Arrange
            var criarPedidoDto = new CriarPedidoDto
            {
                NomeCliente = "",
                EmailCliente = "carlos@email.com",
                Itens = new List<ItemPedidoDto>()
            };

            _serviceMock.Setup(s => s.CriarAsync(criarPedidoDto))
                       .ThrowsAsync(new ArgumentException("Nome do cliente é obrigatório"));

            // Act
            var resultado = await _controller.Criar(criarPedidoDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado.Result);
            Assert.Equal("Nome do cliente é obrigatório", badRequestResult.Value);
        }

        [Fact]
        public async Task Criar_QuandoServiceLancaExcecaoGeral_DeveRetornarStatusCode500()
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
                        NomeProduto = "Produto",
                        Quantidade = 1,
                        PrecoUnitario = 10.00m
                    }
                }
            };

            _serviceMock.Setup(s => s.CriarAsync(criarPedidoDto))
                       .ThrowsAsync(new Exception("Erro de teste"));

            // Act
            var resultado = await _controller.Criar(criarPedidoDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(resultado.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Erro interno do servidor", statusCodeResult.Value?.ToString());
        }

        [Fact]
        public async Task Criar_DeveValidarChamadaDoService()
        {
            // Arrange
            var criarPedidoDto = new CriarPedidoDto
            {
                NomeCliente = "Teste",
                EmailCliente = "teste@email.com",
                Itens = new List<ItemPedidoDto>
                {
                    new ItemPedidoDto
                    {
                        NomeProduto = "Produto Teste",
                        Quantidade = 1,
                        PrecoUnitario = 50.00m
                    }
                }
            };

            var pedidoCriado = new PedidoDto
            {
                Id = 1,
                NomeCliente = "Teste",
                EmailCliente = "teste@email.com",
                DataPedido = DateTime.Now,
                ValorTotal = 50.00m,
                Status = "Pendente",
                Itens = new List<ItemPedidoDto>()
            };

            _serviceMock.Setup(s => s.CriarAsync(It.IsAny<CriarPedidoDto>()))
                       .ReturnsAsync(pedidoCriado);

            // Act
            await _controller.Criar(criarPedidoDto);

            // Assert
            _serviceMock.Verify(s => s.CriarAsync(criarPedidoDto), Times.Once);
        }
    }
}
