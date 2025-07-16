namespace PedidosAPI.Api.DTOs
{
    public class CriarPedidoDto
    {
        public string NomeCliente { get; set; } = string.Empty;
        public string EmailCliente { get; set; } = string.Empty;
        public List<ItemPedidoDto> Itens { get; set; } = new();
    }

    public class ItemPedidoDto
    {
        public string NomeProduto { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
    }

    public class PedidoDto
    {
        public int Id { get; set; }
        public string NomeCliente { get; set; } = string.Empty;
        public string EmailCliente { get; set; } = string.Empty;
        public DateTime DataPedido { get; set; }
        public decimal ValorTotal { get; set; }
        public List<ItemPedidoDto> Itens { get; set; } = new();
        public string Status { get; set; } = string.Empty;
    }
}
