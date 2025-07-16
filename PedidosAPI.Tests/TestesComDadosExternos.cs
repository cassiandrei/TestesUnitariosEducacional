using System.Text.Json;
using Xunit;
using PedidosAPI.Api.Services;
using PedidosAPI.Api.Repositories;
using PedidosAPI.Api.DTOs;
using PedidosAPI.Api.Models;
using Moq;

namespace PedidosAPI.Tests
{
    /// <summary>
    /// Exemplo de como usar Theory com dados externos (JSON)
    /// Demonstra padrões avançados de testes parametrizados
    /// </summary>
    public class TestesComDadosExternos
    {
        private readonly Mock<IPedidoRepository> _repositoryMock;
        private readonly PedidoService _service;

        public TestesComDadosExternos()
        {
            _repositoryMock = new Mock<IPedidoRepository>();
            _service = new PedidoService(_repositoryMock.Object);
        }

        /// <summary>
        /// Classe para deserializar os cenários do JSON
        /// </summary>
        public class CenarioValidacao
        {
            public string NomeCliente { get; set; } = string.Empty;
            public string EmailCliente { get; set; } = string.Empty;
            public int Quantidade { get; set; }
            public decimal PrecoUnitario { get; set; }
            public string MensagemEsperada { get; set; } = string.Empty;
        }

        /// <summary>
        /// Método que lê os cenários do arquivo JSON
        /// </summary>
        public static IEnumerable<object[]> ObterCenariosDeValidacao()
        {
            var caminhoArquivo = Path.Combine(
                Directory.GetCurrentDirectory(),
                "TestData",
                "cenarios-validacao.json"
            );

            if (!File.Exists(caminhoArquivo))
            {
                // Retorna cenários padrão se o arquivo não existir
                yield return new object[]
                {
                    "", "email@teste.com", 1, 10.0m, "Nome do cliente é obrigatório"
                };
                yield break;
            }

            var jsonContent = File.ReadAllText(caminhoArquivo);
            var cenarios = JsonSerializer.Deserialize<List<CenarioValidacao>>(jsonContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (cenarios != null)
            {
                foreach (var cenario in cenarios)
                {
                    yield return new object[]
                    {
                        cenario.NomeCliente,
                        cenario.EmailCliente,
                        cenario.Quantidade,
                        cenario.PrecoUnitario,
                        cenario.MensagemEsperada
                    };
                }
            }
        }

        /// <summary>
        /// Teste que usa Theory com dados vindos de arquivo JSON
        /// Demonstra como centralizar cenários de teste em arquivos externos
        /// </summary>
        [Theory]
        [MemberData(nameof(ObterCenariosDeValidacao))]
        public async Task CriarPedido_CenariosDoJson_DeveValidarCorretamente(
            string nomeCliente,
            string emailCliente,
            int quantidade,
            decimal precoUnitario,
            string mensagemEsperada)
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
                        NomeProduto = "Produto Teste",
                        Quantidade = quantidade,
                        PrecoUnitario = precoUnitario
                    }
                }
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CriarAsync(criarPedidoDto));

            Assert.Contains(mensagemEsperada, exception.Message);
        }

        /// <summary>
        /// Exemplo alternativo usando ClassData para organização ainda melhor
        /// </summary>
        public class CenariosValidacaoClassData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                var caminhoArquivo = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "TestData",
                    "cenarios-validacao.json"
                );

                if (File.Exists(caminhoArquivo))
                {
                    var jsonContent = File.ReadAllText(caminhoArquivo);
                    var cenarios = JsonSerializer.Deserialize<List<CenarioValidacao>>(jsonContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (cenarios != null)
                    {
                        foreach (var cenario in cenarios)
                        {
                            yield return new object[]
                            {
                                cenario.NomeCliente,
                                cenario.EmailCliente,
                                cenario.Quantidade,
                                cenario.PrecoUnitario,
                                cenario.MensagemEsperada
                            };
                        }
                    }
                }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
                => GetEnumerator();
        }

        /// <summary>
        /// Teste usando ClassData - abordagem mais limpa para dados externos
        /// </summary>
        [Theory]
        [ClassData(typeof(CenariosValidacaoClassData))]
        public async Task CriarPedido_UsandoClassData_DeveValidarCorretamente(
            string nomeCliente,
            string emailCliente,
            int quantidade,
            decimal precoUnitario,
            string mensagemEsperada)
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
                        NomeProduto = "Produto Teste",
                        Quantidade = quantidade,
                        PrecoUnitario = precoUnitario
                    }
                }
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CriarAsync(criarPedidoDto));

            Assert.Contains(mensagemEsperada, exception.Message);
        }

        /// <summary>
        /// Classe para cenários mais complexos com diferentes tipos de dados
        /// </summary>
        public class CenarioCompleto
        {
            public string Nome { get; set; } = string.Empty;
            public CriarPedidoDto DadosPedido { get; set; } = new();
            public bool DeveSerValido { get; set; }
            public decimal ValorTotalEsperado { get; set; }
            public string ErroEsperado { get; set; } = string.Empty;
        }

        /// <summary>
        /// Exemplo avançado: lendo cenários complexos de JSON
        /// Demonstra como trabalhar com estruturas de dados mais elaboradas
        /// </summary>
        public static IEnumerable<object[]> ObterCenariosCompletos()
        {
            var caminhoArquivo = Path.Combine(
                Directory.GetCurrentDirectory(),
                "TestData",
                "cenarios-completos.json"
            );

            if (!File.Exists(caminhoArquivo))
            {
                // Fallback para demonstração
                yield return new object[]
                {
                    "Cenário Padrão",
                    new CriarPedidoDto
                    {
                        NomeCliente = "Teste",
                        EmailCliente = "teste@email.com",
                        Itens = new List<ItemPedidoDto>
                        {
                            new ItemPedidoDto { NomeProduto = "Produto", Quantidade = 1, PrecoUnitario = 10.0m }
                        }
                    },
                    true,
                    10.0m,
                    ""
                };
                yield break;
            }

            var jsonContent = File.ReadAllText(caminhoArquivo);
            var cenarios = JsonSerializer.Deserialize<List<CenarioCompleto>>(jsonContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (cenarios != null)
            {
                foreach (var cenario in cenarios)
                {
                    yield return new object[]
                    {
                        cenario.Nome,
                        cenario.DadosPedido,
                        cenario.DeveSerValido,
                        cenario.ValorTotalEsperado,
                        cenario.ErroEsperado
                    };
                }
            }
        }

        /// <summary>
        /// Teste que demonstra como usar dados JSON complexos
        /// Valida tanto cenários de sucesso quanto de erro
        /// </summary>
        [Theory]
        [MemberData(nameof(ObterCenariosCompletos))]
        public async Task ValidarPedido_CenariosCompletos_DeveComportarCorretamente(
            string nomeCenario,
            CriarPedidoDto dadosPedido,
            bool deveSerValido,
            decimal valorTotalEsperado,
            string erroEsperado)
        {
            if (deveSerValido)
            {
                // Arrange - Mock para cenário de sucesso
                _repositoryMock.Setup(r => r.CriarAsync(It.IsAny<Pedido>()))
                              .ReturnsAsync((Pedido p) =>
                              {
                                  p.Id = 1;
                                  p.DataPedido = DateTime.Now;
                                  return p;
                              });

                // Act
                var resultado = await _service.CriarAsync(dadosPedido);

                // Assert
                Assert.NotNull(resultado);
                Assert.Equal(1, resultado.Id);
                Assert.Equal(valorTotalEsperado, resultado.ValorTotal);
            }
            else
            {
                // Act & Assert para cenários de erro
                var exception = await Assert.ThrowsAsync<ArgumentException>(
                    () => _service.CriarAsync(dadosPedido));

                Assert.Contains(erroEsperado, exception.Message);
            }
        }
    }
}
