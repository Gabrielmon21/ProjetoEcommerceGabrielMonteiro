using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ProjetoEcommerceGabrielMonteiro.Models
{
    public class ClienteDAO
    {
        private static string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // LISTAR TODOS OS CLIENTES
        public static List<Cliente> ListarClientes()
        {
            var clientes = new List<Cliente>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"SELECT * FROM Clientes ORDER BY DataCadastro DESC";
                    SqlCommand command = new SqlCommand(query, connection);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        clientes.Add(new Cliente
                        {
                            Id = (int)reader["Id"],
                            Nome = reader["Nome"].ToString(),
                            Email = reader["Email"].ToString(),
                            Senha = reader["Senha"].ToString(), // ✅ ADICIONADO
                            NumContato = reader["NumContato"] != DBNull.Value ? reader["NumContato"].ToString() : "",
                            Endereco = reader["Endereco"] != DBNull.Value ? reader["Endereco"].ToString() : "",
                            DataCadastro = reader["DataCadastro"] != DBNull.Value ? (DateTime)reader["DataCadastro"] : DateTime.Now
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erro ao listar clientes: " + ex.Message);
            }

            return clientes;
        }

        // BUSCAR CLIENTE POR ID
        public static Cliente BuscarClientePorId(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Clientes WHERE Id = @Id";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Id", id);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        return new Cliente
                        {
                            Id = (int)reader["Id"],
                            Nome = reader["Nome"].ToString(),
                            Email = reader["Email"].ToString(),
                            Senha = reader["Senha"].ToString(), // ✅ ADICIONADO
                            NumContato = reader["NumContato"] != DBNull.Value ? reader["NumContato"].ToString() : "",
                            Endereco = reader["Endereco"] != DBNull.Value ? reader["Endereco"].ToString() : "",
                            DataCadastro = (DateTime)reader["DataCadastro"]
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erro ao buscar cliente: " + ex.Message);
            }

            return null;
        }

        // ✅ INSERIR NOVO CLIENTE (ATUALIZADO COM SENHA)
        public static bool InserirCliente(Cliente cliente)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== TENTANDO INSERIR CLIENTE NO BANCO ===");
                System.Diagnostics.Debug.WriteLine($"Nome: {cliente.Nome}, Email: {cliente.Email}, Senha: {cliente.Senha}");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO Clientes 
                                    (Nome, Email, Senha, NumContato, Endereco, DataCadastro) 
                                    VALUES 
                                    (@Nome, @Email, @Senha, @NumContato, @Endereco, @DataCadastro)";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Nome", cliente.Nome ?? "");
                    command.Parameters.AddWithValue("@Email", cliente.Email ?? "");
                    command.Parameters.AddWithValue("@Senha", cliente.Senha ?? ""); // ✅ ADICIONADO

                    // ✅ TRATAMENTO PARA CAMPOS OPCIONAIS
                    if (string.IsNullOrEmpty(cliente.NumContato))
                        command.Parameters.AddWithValue("@NumContato", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@NumContato", cliente.NumContato);

                    if (string.IsNullOrEmpty(cliente.Endereco))
                        command.Parameters.AddWithValue("@Endereco", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@Endereco", cliente.Endereco);

                    command.Parameters.AddWithValue("@DataCadastro", DateTime.Now);

                    connection.Open();
                    int result = command.ExecuteNonQuery();

                    // ✅ DEBUG
                    System.Diagnostics.Debug.WriteLine($"Linhas afetadas no INSERT: {result}");

                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ERRO ao inserir cliente: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                return false;
            }
        }

        // ATUALIZAR CLIENTE (ATUALIZADO COM SENHA)
        public static bool AtualizarCliente(Cliente cliente)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== INICIANDO ATUALIZAÇÃO DO CLIENTE ===");
                System.Diagnostics.Debug.WriteLine($"ID: {cliente.Id}");
                System.Diagnostics.Debug.WriteLine($"Nome: {cliente.Nome}");
                System.Diagnostics.Debug.WriteLine($"Email: {cliente.Email}");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"UPDATE Clientes SET 
                                    Nome = @Nome, 
                                    Email = @Email, 
                                    Senha = @Senha, 
                                    NumContato = @NumContato, 
                                    Endereco = @Endereco
                                    WHERE Id = @Id";

                    SqlCommand command = new SqlCommand(query, connection);

                    // Parâmetros obrigatórios
                    command.Parameters.AddWithValue("@Id", cliente.Id);
                    command.Parameters.AddWithValue("@Nome", cliente.Nome ?? "");
                    command.Parameters.AddWithValue("@Email", cliente.Email ?? "");
                    command.Parameters.AddWithValue("@Senha", cliente.Senha ?? ""); // ✅ ADICIONADO

                    // Parâmetros opcionais com tratamento correto para NULL
                    if (string.IsNullOrEmpty(cliente.NumContato))
                        command.Parameters.AddWithValue("@NumContato", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@NumContato", cliente.NumContato);

                    if (string.IsNullOrEmpty(cliente.Endereco))
                        command.Parameters.AddWithValue("@Endereco", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@Endereco", cliente.Endereco);

                    connection.Open();
                    int result = command.ExecuteNonQuery();

                    System.Diagnostics.Debug.WriteLine($"=== RESULTADO DA ATUALIZAÇÃO ===");
                    System.Diagnostics.Debug.WriteLine($"Linhas afetadas no banco: {result}");
                    System.Diagnostics.Debug.WriteLine($"Atualização bem-sucedida: {result > 0}");

                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"=== ERRO NA ATUALIZAÇÃO ===");
                System.Diagnostics.Debug.WriteLine($"Mensagem: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }

                return false;
            }
        }

        // EXCLUIR CLIENTE
        public static bool ExcluirCliente(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Clientes WHERE Id = @Id";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Id", id);

                    connection.Open();
                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erro ao excluir cliente: " + ex.Message);
                return false;
            }
        }

        // BUSCAR PEDIDOS DO CLIENTE
        public static List<Pedido> BuscarPedidosPorCliente(int clienteId)
        {
            var pedidos = new List<Pedido>();

            try
            {
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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erro ao buscar pedidos do cliente: " + ex.Message);
            }

            return pedidos;
        }

        // ✅ VALIDAR LOGIN DO CLIENTE
        public static Cliente ValidarLogin(string email, string senha)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Clientes WHERE Email = @Email AND Senha = @Senha";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Senha", senha);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        return new Cliente
                        {
                            Id = (int)reader["Id"],
                            Nome = reader["Nome"].ToString(),
                            Email = reader["Email"].ToString(),
                            Senha = reader["Senha"].ToString(),
                            NumContato = reader["NumContato"] != DBNull.Value ? reader["NumContato"].ToString() : "",
                            Endereco = reader["Endereco"] != DBNull.Value ? reader["Endereco"].ToString() : "",
                            DataCadastro = (DateTime)reader["DataCadastro"]
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erro ao validar login: " + ex.Message);
            }

            return null;
        }
    }
}