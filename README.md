# API de Pedidos - Demonstração de Testes Unitários

Esta é uma API simples para gerenciamento de pedidos, desenvolvida em .NET 8, demonstrando os fundamentos de testes unitários com XUnit e Moq.

## 📋 Funcionalidades

- **Criar Pedido**: Permite criar um novo pedido com itens
- **Listar Pedidos**: Lista todos os pedidos cadastrados
- **Buscar Pedido**: Busca um pedido específico pelo ID

## 🏗️ Arquitetura

O projeto segue os princípios de arquitetura limpa e está organizado em camadas:

- **Controllers**: Responsáveis por receber as requisições HTTP
- **Services**: Contém a lógica de negócio
- **Repositories**: Gerencia o acesso aos dados (in-memory neste exemplo)
- **DTOs**: Objetos de transferência de dados
- **Models**: Entidades de domínio

## 🧪 Testes Unitários

O projeto inclui testes unitários abrangentes para demonstrar conceitos fundamentais:

### Conceitos Demonstrados:

1. **Arrange, Act, Assert (AAA)**: Padrão de organização dos testes
2. **Mocking com Moq**: Simulação de dependências
3. **Fact e Theory**: Diferentes tipos de testes no XUnit
4. **InlineData**: Testes parametrizados
5. **Verify**: Verificação de chamadas de métodos
6. **Exception Testing**: Testes de comportamento de exceções

### Cobertura de Testes:

- ✅ **PedidoService**: Testa toda a lógica de negócio
- ✅ **PedidosController**: Testa o comportamento da API
- ✅ **PedidoRepository**: Testa operações de dados
- ✅ **Validações**: Testa cenários de erro e validação

## 🚀 Como Executar

### Pré-requisitos
- .NET 8 SDK

### Executar a API
```bash
cd PedidosAPI.Api
dotnet run
```

A API estará disponível em: `https://localhost:7000` ou `http://localhost:5000`

### Executar os Testes
```bash
# Executar todos os testes
dotnet test

# Executar com relatório de cobertura
dotnet test --collect:"XPlat Code Coverage"

# Executar testes de uma classe específica
dotnet test --filter "PedidoServiceTests"
```

## 📡 Endpoints da API

### GET /api/pedidos
Lista todos os pedidos

**Resposta:**
```json
[
  {
    "id": 1,
    "nomeCliente": "João Silva",
    "emailCliente": "joao@email.com",
    "dataPedido": "2024-01-15T10:30:00",
    "valorTotal": 150.50,
    "status": "Pendente",
    "itens": [
      {
        "nomeProduto": "Produto A",
        "quantidade": 2,
        "precoUnitario": 75.25
      }
    ]
  }
]
```

### GET /api/pedidos/{id}
Busca um pedido específico

**Resposta:**
```json
{
  "id": 1,
  "nomeCliente": "João Silva",
  "emailCliente": "joao@email.com",
  "dataPedido": "2024-01-15T10:30:00",
  "valorTotal": 150.50,
  "status": "Pendente",
  "itens": [
    {
      "nomeProduto": "Produto A",
      "quantidade": 2,
      "precoUnitario": 75.25
    }
  ]
}
```

### POST /api/pedidos
Cria um novo pedido

**Payload:**
```json
{
  "nomeCliente": "Maria Santos",
  "emailCliente": "maria@email.com",
  "itens": [
    {
      "nomeProduto": "Produto B",
      "quantidade": 1,
      "precoUnitario": 99.90
    },
    {
      "nomeProduto": "Produto C",
      "quantidade": 3,
      "precoUnitario": 25.00
    }
  ]
}
```

## 🔍 Exemplos de Testes

### 1. Teste Simples com AAA Pattern
```csharp
[Fact]
public async Task GetTodosAsync_DeveRetornarListaDePedidos()
{
    // Arrange
    var pedidos = new List<Pedido> { /* dados de teste */ };
    _repositoryMock.Setup(r => r.GetTodosAsync()).ReturnsAsync(pedidos);

    // Act
    var resultado = await _service.GetTodosAsync();

    // Assert
    Assert.NotNull(resultado);
    Assert.Single(resultado);
}
```

### 2. Teste com Mock e Verify
```csharp
[Fact]
public async Task CriarAsync_DeveChamarRepositorio()
{
    // Arrange
    var criarPedidoDto = new CriarPedidoDto { /* dados */ };
    
    // Act
    await _service.CriarAsync(criarPedidoDto);
    
    // Assert
    _repositoryMock.Verify(r => r.CriarAsync(It.IsAny<Pedido>()), Times.Once);
}
```

### 3. Teste Parametrizado com Theory
```csharp
[Theory]
[InlineData("", "email@teste.com", "Nome do cliente é obrigatório")]
[InlineData("João", "", "Email do cliente é obrigatório")]
public async Task CriarAsync_DadosInvalidos_DeveLancarArgumentException(
    string nomeCliente, string emailCliente, string mensagemEsperada)
{
    // Arrange & Act & Assert
    var exception = await Assert.ThrowsAsync<ArgumentException>(/* ... */);
    Assert.Equal(mensagemEsperada, exception.Message);
}
```

## 🎯 Objetivos Educacionais

Este projeto foi criado para demonstrar:

1. **Estrutura de Testes**: Como organizar e nomear testes
2. **Isolamento**: Como usar mocks para isolar unidades de teste
3. **Cobertura**: Como garantir que todos os cenários sejam testados
4. **Manutenibilidade**: Como escrever testes que são fáceis de entender e manter
5. **TDD/BDD**: Princípios de desenvolvimento orientado a testes

## 📚 Recursos Adicionais

- [Documentação do XUnit](https://xunit.net/)
- [Documentação do Moq](https://github.com/Moq/moq4)
- [Boas Práticas de Testes Unitários](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)

---

**Nota**: Esta é uma implementação didática usando armazenamento em memória. Em um ambiente de produção, você deve usar um banco de dados real e considerar aspectos como persistência, segurança e performance.
