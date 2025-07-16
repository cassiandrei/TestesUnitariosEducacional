# Fundamentos de Testes Unitários - Guia Educacional

## 🎯 O que são Testes Unitários?

Testes unitários são testes automatizados que verificam o comportamento de uma **unidade** de código (método, classe) de forma **isolada**. Eles são a base da pirâmide de testes e devem ser:

- ⚡ **Rápidos**: Executam em milissegundos
- 🔒 **Isolados**: Não dependem de recursos externos
- 🔄 **Repetíveis**: Mesmo resultado sempre
- 📝 **Autodocumentados**: Servem como documentação viva

## 🏗️ Padrão AAA (Arrange, Act, Assert)

Todos os testes seguem este padrão:

```csharp
[Fact]
public async Task MetodoTeste_Cenario_ComportamentoEsperado()
{
    // Arrange - Configurar o teste
    var dados = new DadosTeste();
    var mock = new Mock<IDependencia>();
    
    // Act - Executar a ação
    var resultado = await metodoSobTeste(dados);
    
    // Assert - Verificar o resultado
    Assert.Equal(valorEsperado, resultado);
}
```

## 🎭 Mocking com Moq

### Por que usar Mocks?
- **Isolamento**: Testa apenas a unidade específica
- **Controle**: Define exatamente o comportamento das dependências
- **Performance**: Evita chamadas reais a banco de dados, APIs, etc.

### Exemplos práticos:

#### 1. Setup básico
```csharp
var mock = new Mock<IRepository>();
mock.Setup(r => r.GetById(1))
    .ReturnsAsync(new Pedido { Id = 1 });
```

#### 2. Verificação de chamadas
```csharp
// Verifica se o método foi chamado exatamente uma vez
mock.Verify(r => r.Save(It.IsAny<Pedido>()), Times.Once);

// Verifica se nunca foi chamado
mock.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
```

#### 3. Simulação de exceções
```csharp
mock.Setup(r => r.GetById(999))
    .ThrowsAsync(new NotFoundException());
```

## 🧪 Tipos de Testes no XUnit

### Fact - Testes simples
```csharp
[Fact]
public void Calculadora_Somar_DeveRetornarSomaCorreta()
{
    // Teste de um cenário específico
    var resultado = Calculadora.Somar(2, 3);
    Assert.Equal(5, resultado);
}
```

### Theory - Testes parametrizados
```csharp
[Theory]
[InlineData(2, 3, 5)]
[InlineData(0, 0, 0)]
[InlineData(-1, 1, 0)]
public void Calculadora_Somar_MultiplosValores(int a, int b, int esperado)
{
    var resultado = Calculadora.Somar(a, b);
    Assert.Equal(esperado, resultado);
}
```

### Theory com dados externos (JSON)
Para cenários mais complexos, você pode carregar dados de arquivos externos:

```csharp
// Arquivo: TestData/cenarios-validacao.json
[
  {
    "nomeCliente": "",
    "emailCliente": "teste@email.com",
    "quantidade": 1,
    "precoUnitario": 10.0,
    "mensagemEsperada": "Nome do cliente é obrigatório"
  },
  {
    "nomeCliente": "João Silva",
    "emailCliente": "",
    "quantidade": 1,
    "precoUnitario": 10.0,
    "mensagemEsperada": "Email do cliente é obrigatório"
  }
]

// Classe para deserializar
public class CenarioValidacao
{
    public string NomeCliente { get; set; } = string.Empty;
    public string EmailCliente { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public string MensagemEsperada { get; set; } = string.Empty;
}

// Método que lê os dados do JSON
public static IEnumerable<object[]> ObterCenariosDeValidacao()
{
    var caminhoArquivo = Path.Combine(
        Directory.GetCurrentDirectory(), 
        "TestData", 
        "cenarios-validacao.json"
    );

    var jsonContent = File.ReadAllText(caminhoArquivo);
    var cenarios = JsonSerializer.Deserialize<List<CenarioValidacao>>(jsonContent);

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

// Teste usando MemberData
[Theory]
[MemberData(nameof(ObterCenariosDeValidacao))]
public async Task ValidarPedido_CenariosDoJson_DeveRetornarErroCorreto(
    string nomeCliente,
    string emailCliente,
    int quantidade,
    decimal precoUnitario,
    string mensagemEsperada)
{
    // Arrange
    var dados = new CriarPedidoDto
    {
        NomeCliente = nomeCliente,
        EmailCliente = emailCliente,
        Itens = new[] { new ItemPedidoDto 
        { 
            Quantidade = quantidade, 
            PrecoUnitario = precoUnitario 
        }}
    };

    // Act & Assert
    var exception = await Assert.ThrowsAsync<ArgumentException>(
        () => _service.CriarAsync(dados));
    
    Assert.Contains(mensagemEsperada, exception.Message);
}
```

**Vantagens dos dados externos:**
- ✅ **Flexibilidade**: Fácil de modificar sem recompilar
- ✅ **Reutilização**: Mesmos dados para diferentes testes
- ✅ **Colaboração**: Analistas podem criar cenários
- ✅ **Manutenção**: Centraliza cenários complexos

### Outras fontes de dados para Theory

#### 1. ClassData - Para lógica mais complexa
```csharp
public class CenariosComplexos : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        // Pode incluir lógica complexa, consultas a DB, etc.
        yield return new object[] { "cenario1", "dados1" };
        yield return new object[] { "cenario2", "dados2" };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

[Theory]
[ClassData(typeof(CenariosComplexos))]
public void Teste_ComClassData(string cenario, string dados) { }
```

#### 2. Dados de banco de dados (avançado)
```csharp
public static IEnumerable<object[]> CenariosDeProducao()
{
    using var context = new TestDbContext();
    var cenarios = context.CenariosValidos.Take(10);
    
    foreach (var cenario in cenarios)
    {
        yield return new object[] { cenario.Input, cenario.ExpectedOutput };
    }
}

[Theory]
[MemberData(nameof(CenariosDeProducao))]
public void Teste_ComDadosReais(string input, string expected) { }
```

### Quando usar cada abordagem

| Abordagem | Quando usar | Vantagens | Desvantagens |
|-----------|-------------|-----------|--------------|
| **InlineData** | Poucos cenários simples | Rápido, direto | Código pode ficar verboso |
| **MemberData** | Lógica de geração de dados | Flexível, programático | Mais complexo |
| **ClassData** | Dados complexos reutilizáveis | Organizado, reutilizável | Mais arquivos |
| **JSON/Externos** | Muitos cenários, colaboração | Flexível, não técnico | Dependência externa |

### Exemplo prático completo
```csharp
// Arquivo: TestData/cenarios-pedidos.json
[
  {
    "nome": "Cenário: Cliente sem nome",
    "dadosPedido": {
      "nomeCliente": "",
      "emailCliente": "teste@email.com",
      "itens": [{"nomeProduto": "Produto", "quantidade": 1, "precoUnitario": 10.0}]
    },
    "erroEsperado": "Nome do cliente é obrigatório"
  }
]

// Teste que usa JSON complexo
[Theory]
[MemberData(nameof(CarregarCenariosPedidos))]
public async Task ValidarPedido_CenariosCompletos(
    string nomeCenario, 
    CriarPedidoDto dados, 
    string erroEsperado)
{
    // Arrange é feito pelo MemberData
    
    // Act & Assert
    var exception = await Assert.ThrowsAsync<ArgumentException>(
        () => _service.CriarAsync(dados));
    
    Assert.Contains(erroEsperado, exception.Message);
}
```

## ✅ O que Testar

### ✅ Cenários para testar:
1. **Caminho feliz** - Funcionamento normal
2. **Validações** - Entrada inválida
3. **Casos limites** - Valores extremos
4. **Exceções** - Comportamento em erro
5. **Dependências** - Integração com mocks

### ❌ O que NÃO testar:
- Propriedades simples (getters/setters)
- Código de terceiros
- Configurações do framework
- Constantes

## 🔍 Asserções Importantes

```csharp
// Igualdade
Assert.Equal(esperado, atual);
Assert.NotEqual(naoEsperado, atual);

// Nulos
Assert.Null(objeto);
Assert.NotNull(objeto);

// Booleanos
Assert.True(condicao);
Assert.False(condicao);

// Coleções
Assert.Empty(lista);
Assert.NotEmpty(lista);
Assert.Single(lista);
Assert.Contains(item, lista);

// Exceções
Assert.Throws<ArgumentException>(() => metodo());
await Assert.ThrowsAsync<InvalidOperationException>(() => metodoAsync());

// Strings
Assert.Contains("texto", stringCompleta);
Assert.StartsWith("inicio", string);
Assert.EndsWith("fim", string);

// Tipos
Assert.IsType<TipoEsperado>(objeto);
Assert.IsAssignableFrom<TipoBase>(objeto);
```

## 📊 Cobertura de Código

### Como medir:
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Interpretação:
- **80-90%**: Meta razoável para a maioria dos projetos
- **100%**: Nem sempre necessário ou prático
- **Foque na qualidade**, não apenas na quantidade

### O que a cobertura NÃO garante:
- Que todos os cenários foram testados
- Que os testes são úteis
- Que não há bugs

## 🏗️ Estrutura dos Testes

### Organização de arquivos:
```
Tests/
├── Services/
│   ├── PedidoServiceTests.cs
│   └── EmailServiceTests.cs
├── Controllers/
│   └── PedidosControllerTests.cs
├── Repositories/
│   └── PedidoRepositoryTests.cs
└── Helpers/
    └── TestDataBuilder.cs
```

### Convenções de nomenclatura:
```csharp
// Padrão: [MethodName]_[Scenario]_[ExpectedBehavior]
public void CriarPedido_ClienteValido_DeveRetornarPedidoCriado()

// Padrão alternativo: [Scenario]_[ExpectedBehavior]
public void DadosValidos_DeveCriarPedidoComSucesso()
```

## 🚫 Antipadrões (O que EVITAR)

### 1. Testes frágeis
```csharp
// ❌ Ruim - depende de data atual
Assert.Equal(DateTime.Now.Day, pedido.DataPedido.Day);

// ✅ Bom - usa data fixa
Assert.Equal(new DateTime(2024, 1, 15), pedido.DataPedido.Date);
```

### 2. Testes acoplados
```csharp
// ❌ Ruim - testa múltiplas unidades
public void TesteCompleto() 
{
    var pedido = service.CriarPedido(dto);
    var email = emailService.EnviarConfirmacao(pedido);
    var estoque = estoqueService.ReservarItens(pedido.Itens);
    // ... muitas verificações
}
```

### 3. Magic numbers/strings
```csharp
// ❌ Ruim
Assert.Equal(42, resultado.Count);

// ✅ Bom
const int QUANTIDADE_ESPERADA = 42;
Assert.Equal(QUANTIDADE_ESPERADA, resultado.Count);
```

## 🎯 Boas Práticas

### 1. Um conceito por teste
```csharp
// ✅ Cada teste verifica apenas uma coisa
[Fact] public void ValidarEmail_EmailValido_DeveRetornarTrue() { }
[Fact] public void ValidarEmail_EmailInvalido_DeveRetornarFalse() { }
[Fact] public void ValidarEmail_EmailNulo_DeveLancarException() { }
```

### 2. Dados de teste expressivos
```csharp
// ✅ Nomes que explicam o propósito
var clienteComEmailInvalido = new Cliente { Email = "email-invalido" };
var pedidoComValorNegativo = new Pedido { ValorTotal = -100 };
```

### 3. Usar builders para testes complexos
```csharp
public class PedidoBuilder
{
    public static Pedido UmPedidoValido() => new Pedido
    {
        NomeCliente = "João Silva",
        EmailCliente = "joao@email.com",
        // ... outros campos padrão
    };
    
    public static Pedido UmPedidoSemItens() => UmPedidoValido()
        .With(p => p.Itens = new List<ItemPedido>());
}
```

## 🔧 Configuração do Ambiente

### packages necessários:
```xml
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
<PackageReference Include="xunit" Version="2.4.2" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.4.5" />
<PackageReference Include="Moq" Version="4.20.69" />
<PackageReference Include="FluentAssertions" Version="6.12.0" /> <!-- Opcional -->
```

### Executar testes:
```bash
# Todos os testes
dotnet test

# Testes específicos
dotnet test --filter "PedidoServiceTests"

# Com cobertura
dotnet test --collect:"XPlat Code Coverage"

# Com relatório detalhado
dotnet test --logger:trx --results-directory ./TestResults
```

## 📚 Pirâmide de Testes

```
        /\
       /UI\          ← Poucos, lentos, caros
      /____\
     /      \
    /  API   \       ← Alguns, médios
   /__________\
  /            \
 /   UNITÁRIOS  \    ← Muitos, rápidos, baratos
/________________\
```

### Distribuição recomendada:
- **70%** Testes Unitários
- **20%** Testes de Integração  
- **10%** Testes E2E/UI

## 🎓 Conceitos Avançados

### Test Doubles (tipos de mocks):
- **Dummy**: Objetos passados mas nunca usados
- **Stub**: Retorna respostas predefinidas
- **Spy**: Registra informações sobre como foi usado
- **Mock**: Verifica se métodos foram chamados corretamente
- **Fake**: Implementação funcional simplificada

### TDD (Test-Driven Development):
1. **Red**: Escreva um teste que falha
2. **Green**: Escreva o mínimo de código para passar
3. **Refactor**: Melhore o código mantendo os testes passando

## 🚀 Próximos Passos

Para aprofundar seus conhecimentos:

1. **Estude BDD** (Behavior-Driven Development)
2. **Aprenda sobre Property-Based Testing**
3. **Explore Mutation Testing**
4. **Pratique TDD** em projetos reais
5. **Estude padrões de testes** (Test Data Builders, Object Mother, etc.)

---

**Lembre-se**: Testes são código de produção! Mantenha-os limpos, legíveis e bem organizados. 🧪✨
