using System;
using System.Collections.Generic;

namespace ProjetoEcommerceGabrielMonteiro.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public DateTime DataPedido { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Confirmado";
        public string EnderecoEntrega { get; set; }
        public string MetodoPagamento { get; set; }
        public decimal Total { get; set; }
        public string NumeroPedido { get; set; }
        public List<PedidoItem> Itens { get; set; }

        public Pedido()
        {
            Itens = new List<PedidoItem>();
            // Gerar número do pedido baseado no timestamp
            NumeroPedido = "PED" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }
    }
}