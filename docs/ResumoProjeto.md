# 🎯 API de Pedidos - Projeto Educacional Completo

Este projeto foi criado para demonstrar os **fundamentos de testes unitários** usando **.NET 8**, **XUnit** e **Moq**. É um exemplo prático e completo que você pode usar para ensinar conceitos essenciais de testes para sua turma.

## 📋 O que foi implementado

### 🏗️ Arquitetura da API
- **Controllers**: `PedidosController` - Endpoints REST
- **Services**: `PedidoService` - Lógica de negócio com validações
- **Repositories**: `PedidoRepository` - Camada de dados (in-memory)
- **Models**: `Pedido`, `ItemPedido`, `StatusPedido` - Entidades de domínio
- **DTOs**: `CriarPedidoDto`, `PedidoDto` - Transferência de dados

### 🧪 Cobertura de Testes (44 testes)
- ✅ **PedidoServiceTests** (12 testes) - Lógica de negócio
- ✅ **PedidosControllerTests** (11 testes) - Comportamento da API
- ✅ **PedidoRepositoryTests** (10 testes) - Operações de dados
- ✅ **TestesComDadosExternos** (11 testes) - Testes com dados JSON

### 🎓 Conceitos Demonstrados

#### 1. **Padrão AAA (Arrange, Act, Assert)**
```csharp
[Fact]
public async Task CriarPedido_DadosValidos_DeveRetornarPedidoCriado()
{
    // Arrange - Preparar
    var dados = new CriarPedidoDto { /* ... */ };
    
    // Act - Executar
    var resultado = await _service.CriarAsync(dados);
    
    // Assert - Verificar
    Assert.NotNull(resultado);
    Assert.Equal(1, resultado.Id);
}
```

#### 2. **Mocking com Moq**
```csharp
// Setup de comportamento
_repositoryMock.Setup(r => r.GetTodosAsync())
               .ReturnsAsync(listaPedidos);

// Verificação de chamadas
_repositoryMock.Verify(r => r.CriarAsync(It.IsAny<Pedido>()), Times.Once);
```

#### 3. **Testes Parametrizados (Theory)**
```csharp
[Theory]
[InlineData("", "email@teste.com", "Nome do cliente é obrigatório")]
[InlineData("João", "", "Email do cliente é obrigatório")]
public async Task Validar_DadosInvalidos_DeveLancarExcecao(
    string nome, string email, string mensagemEsperada)
```

#### 4. **Teste de Exceções**
```csharp
var exception = await Assert.ThrowsAsync<ArgumentException>(
    () => _service.CriarAsync(dadosInvalidos));
    
Assert.Equal("Mensagem esperada", exception.Message);
```

#### 5. **Isolamento de Dependências**
- Cada teste usa mocks para isolar a unidade testada
- Não há dependências externas (banco, APIs, etc.)
- Testes rápidos e determinísticos

## 🚀 Como usar na sua aula

### 1. **Clone e Execute**
```bash
git clone [seu-repositorio]
cd API
dotnet restore
dotnet build
dotnet test
```

### 2. **Execute a API**
```bash
cd PedidosAPI.Api
dotnet run
```
- Acesse: `http://localhost:5286/swagger`
- Teste os endpoints interativamente

### 3. **Demonstre os Conceitos**

#### **Comece com um teste simples:**
```csharp
[Fact]
public void ItemPedido_ValorTotal_DeveSerCalculadoCorretamente()
{
    // Arrange
    var item = new ItemPedido
    {
        Quantidade = 3,
        PrecoUnitario = 25.50m
    };
    
    // Act
    var valorTotal = item.ValorTotal;
    
    // Assert
    Assert.Equal(76.50m, valorTotal);
}
```

#### **Evolua para mocks:**
```csharp
[Fact]
public async Task CriarPedido_DeveUsarRepositorio()
{
    // Arrange
    var mock = new Mock<IPedidoRepository>();
    var service = new PedidoService(mock.Object);
    
    // Act
    await service.CriarAsync(dadosValidos);
    
    // Assert
    mock.Verify(r => r.CriarAsync(It.IsAny<Pedido>()), Times.Once);
}
```

### 4. **Exercícios para os Alunos**

#### **Nível Básico:**
1. Criar teste para validação de email
2. Testar cálculo de valor total do pedido
3. Verificar status inicial do pedido

#### **Nível Intermediário:**
1. Adicionar novos cenários de validação
2. Testar comportamentos de erro
3. Criar mocks para novas dependências

#### **Nível Avançado:**
1. Implementar novos endpoints (PUT, DELETE)
2. Adicionar autenticação e testar
3. Criar testes de performance

## 📊 Estatísticas do Projeto

- **Classes testadas**: 3 (Service, Controller, Repository)
- **Métodos testados**: 15+
- **Cenários cobertos**: 44
- **Cobertura estimada**: ~90%
- **Tempo de execução**: <100ms

## 🔧 Comandos Úteis

```bash
# Executar todos os testes
dotnet test

# Executar com cobertura
dotnet test --collect:"XPlat Code Coverage"

# Executar testes específicos
dotnet test --filter "PedidoServiceTests"

# Build e run
dotnet build
dotnet run --project PedidosAPI.Api

# Assistir mudanças (watch mode)
dotnet watch run --project PedidosAPI.Api
```

## 📚 Recursos Educacionais Incluídos

- **README.md**: Visão geral e instruções
- **FundamentosTestes.md**: Guia completo de conceitos
- **ExemplosRequisicoes.md**: Como testar a API manualmente
- **Comentários detalhados**: Código autodocumentado
- **Swagger UI**: Interface para testes interativos

## 🎯 Objetivos de Aprendizagem Atingidos

Após este projeto, os alunos saberão:

✅ **O que são testes unitários**
✅ **Como estruturar testes (AAA)**
✅ **Usar mocks para isolamento**
✅ **Verificar comportamentos e chamadas**
✅ **Testar cenários de erro**
✅ **Parametrizar testes**
✅ **Interpretar resultados de testes**
✅ **Aplicar boas práticas**

## 💡 Próximos Passos Sugeridos

1. **Adicionar Entity Framework** para persistência real
2. **Implementar autenticação JWT**
3. **Criar testes de integração** completos
4. **Adicionar logging** e testar comportamentos
5. **Implementar cache** e validar com testes
6. **Deploy na nuvem** e testes de smoke

## 🏆 Conclusão

Este projeto oferece uma base sólida para ensinar testes unitários, combinando:
- **Teoria**: Conceitos e boas práticas
- **Prática**: Código real e funcional
- **Exercícios**: Desafios progressivos
- **Ferramentas**: Stack moderna (.NET 8, XUnit, Moq)

Perfeito para demonstrar que **testes não são apenas "verificações"**, mas sim **especificações vivas** que documentam e garantem o comportamento do software! 🚀
