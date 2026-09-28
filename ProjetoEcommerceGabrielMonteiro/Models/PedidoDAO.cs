using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ProjetoEcommerceGabrielMonteiro.Models
{
    public class PedidoDAO
    {
        private static string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // SALVAR PEDIDO NO BANCO (VERSÃO SIMPLIFICADA - SEM NumeroPedido)
        public static int SalvarPedido(Pedido pedido)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO Pedidos 
                        (ClienteId, EnderecoEntrega, MetodoPagamento, Total, Status)
                        OUTPUT INSERTED.Id
                        VALUES (@ClienteId, @EnderecoEntrega, @MetodoPagamento, @Total, @Status)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ClienteId", pedido.ClienteId);
                command.Parameters.AddWithValue("@EnderecoEntrega", pedido.EnderecoEntrega ?? "");
                command.Parameters.AddWithValue("@MetodoPagamento", pedido.MetodoPagamento ?? "");
                command.Parameters.AddWithValue("@Total", pedido.Total);
                command.Parameters.AddWithValue("@Status", pedido.Status ?? "Confirmado");

                connection.Open();
                int pedidoId = (int)command.ExecuteScalar();
                return pedidoId;
            }
        }

        // SALVAR ITENS DO PEDIDO
        public static bool SalvarItensPedido(int pedidoId, List<ItemCarrinho> itens)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (var item in itens)
                {
                    string query = @"INSERT INTO PedidoItens 
                                    (PedidoId, ProdutoId, Quantidade, PrecoUnitario)
                                    VALUES (@PedidoId, @ProdutoId, @Quantidade, @PrecoUnitario)";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@PedidoId", pedidoId);
                    command.Parameters.AddWithValue("@ProdutoId", item.ProdutoId);
                    command.Parameters.AddWithValue("@Quantidade", item.Quantidade);
                    command.Parameters.AddWithValue("@PrecoUnitario", item.Preco);

                    command.ExecuteNonQuery();

                    // Atualizar estoque
                    AtualizarEstoque(item.ProdutoId, item.Quantidade);
                }
                return true;
            }
        }

        // ATUALIZAR ESTOQUE
        private static bool AtualizarEstoque(int produtoId, int quantidade)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"UPDATE Produtos SET 
                                QuantidadeEstoque = QuantidadeEstoque - @Quantidade
                                WHERE Id = @Id AND QuantidadeEstoque >= @Quantidade";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", produtoId);
                command.Parameters.AddWithValue("@Quantidade", quantidade);

                connection.Open();
                int result = command.ExecuteNonQuery();
                return result > 0;
            }
        }

        // BUSCAR PEDIDOS POR CLIENTE
        public static List<Pedido> BuscarPedidosPorCliente(int clienteId)
        {
            var pedidos = new List<Pedido>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"SELECT * FROM Pedidos 
                               WHERE ClienteId = @ClienteId 
                               ORDER BY DataPedido DESC";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ClienteId", clienteId);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    pedidos.Add(new Pedido
                    {
                        Id = (int)reader["Id"],
                        ClienteId = (int)reader["ClienteId"],
                        DataPedido = (DateTime)reader["DataPedido"],
                        Status = reader["Status"]?.ToString() ?? "Confirmado",
                        EnderecoEntrega = reader["EnderecoEntrega"]?.ToString() ?? "",
                        MetodoPagamento = reader["MetodoPagamento"]?.ToString() ?? "",
                        Total = (decimal)reader["Total"]
                    });
                }
            }
            return pedidos;
        }

        // BUSCAR PEDIDO POR ID
        public static Pedido BuscarPedidoPorId(int pedidoId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"SELECT * FROM Pedidos WHERE Id = @Id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", pedidoId);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return new Pedido
                    {
                        Id = (int)reader["Id"],
                        ClienteId = (int)reader["ClienteId"],
                        DataPedido = (DateTime)reader["DataPedido"],
                        Status = reader["Status"]?.ToString() ?? "Confirmado",
                        EnderecoEntrega = reader["EnderecoEntrega"]?.ToString() ?? "",
                        MetodoPagamento = reader["MetodoPagamento"]?.ToString() ?? "",
                        Total = (decimal)reader["Total"]
                    };
                }
                return null;
            }
        }

        // BUSCAR ITENS DO PEDIDO
        public static List<PedidoItem> BuscarItensPedido(int pedidoId)
        {
            var itens = new List<PedidoItem>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"SELECT pi.*, p.Nome as NomeProduto 
                               FROM PedidoItens pi
                               INNER JOIN Produtos p ON pi.ProdutoId = p.Id
                               WHERE pi.PedidoId = @PedidoId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@PedidoId", pedidoId);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    itens.Add(new PedidoItem
                    {
                        Id = (int)reader["Id"],
                        PedidoId = (int)reader["PedidoId"],
                        ProdutoId = (int)reader["ProdutoId"],
                        Quantidade = (int)reader["Quantidade"],
                        PrecoUnitario = (decimal)reader["PrecoUnitario"],
                        NomeProduto = reader["NomeProduto"]?.ToString() ?? ""
                    });
                }
            }
            return itens;
        }

        // ATUALIZAR STATUS DO PEDIDO
        public static bool AtualizarStatusPedido(int pedidoId, string status)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"UPDATE Pedidos SET Status = @Status WHERE Id = @Id";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", pedidoId);
                command.Parameters.AddWithValue("@Status", status);

                connection.Open();
                int result = command.ExecuteNonQuery();
                return result > 0;
            }
        }
    }
}