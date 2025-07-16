namespace PedidosAPI.Api.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public string NomeCliente { get; set; } = string.Empty;
        public string EmailCliente { get; set; } = string.Empty;
        public DateTime DataPedido { get; set; }
        public decimal ValorTotal { get; set; }
        public List<ItemPedido> Itens { get; set; } = new();
        public StatusPedido Status { get; set; }
    }

    public class ItemPedido
    {
        public int Id { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal ValorTotal => Quantidade * PrecoUnitario;
    }

    public enum StatusPedido
    {
        Pendente = 0,
        Confirmado = 1,
        EmPreparacao = 2,
        Enviado = 3,
        Entregue = 4,
        Cancelado = 5
    }
}
