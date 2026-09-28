using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace ProjetoEcommerceGabrielMonteiro.Models
{
    public class ProdutoDAO
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // 📋 LISTAR TODOS OS PRODUTOS
        public static List<Produto> ListarProdutos()
        {
            var produtos = new List<Produto>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"SELECT * FROM Produtos 
                                   ORDER BY DataCadastro DESC";
                    SqlCommand command = new SqlCommand(query, connection);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        produtos.Add(new Produto
                        {
                            Id = reader["Id"] != DBNull.Value ? (int)reader["Id"] : 0,
                            Nome = reader["Nome"] != DBNull.Value ? reader["Nome"].ToString() : "",
                            DescricaoCurta = reader["DescricaoCurta"] != DBNull.Value ? reader["DescricaoCurta"].ToString() : "",
                            DescricaoLonga = reader["DescricaoLonga"] != DBNull.Value ? reader["DescricaoLonga"].ToString() : "",
                            Preco = reader["Preco"] != DBNull.Value ? (decimal)reader["Preco"] : 0,
                            QuantidadeEstoque = reader["QuantidadeEstoque"] != DBNull.Value ? (int)reader["QuantidadeEstoque"] : 0,
                            CategoriaId = reader["CategoriaId"] != DBNull.Value ? (int)reader["CategoriaId"] : 1,
                            Marca = reader["Marca"] != DBNull.Value ? reader["Marca"].ToString() : "",
                            Imagem = reader["Imagem"] != DBNull.Value ? reader["Imagem"].ToString() : "",
                            Ativo = reader["Ativo"] != DBNull.Value ? (bool)reader["Ativo"] : true,
                            DataCadastro = reader["DataCadastro"] != DBNull.Value ? (DateTime)reader["DataCadastro"] : DateTime.Now
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // Em produção, você logaria esse erro
                System.Diagnostics.Debug.WriteLine("Erro ao listar produtos: " + ex.Message);
            }

            return produtos;
        }

        // ➕ INSERIR NOVO PRODUTO
        public static bool InserirProduto(Produto produto)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO Produtos 
                                    (Nome, DescricaoCurta, DescricaoLonga, Preco, QuantidadeEstoque, 
                                     CategoriaId, Marca, Imagem, Ativo, DataCadastro) 
                                    VALUES (@Nome, @DescricaoCurta, @DescricaoLonga, @Preco, 
                                            @QuantidadeEstoque, @CategoriaId, @Marca, @Imagem, @Ativo, @DataCadastro)";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Nome", produto.Nome ?? "");
                    command.Parameters.AddWithValue("@DescricaoCurta", produto.DescricaoCurta ?? "");
                    command.Parameters.AddWithValue("@DescricaoLonga", produto.DescricaoLonga ?? "");
                    command.Parameters.AddWithValue("@Preco", produto.Preco);
                    command.Parameters.AddWithValue("@QuantidadeEstoque", produto.QuantidadeEstoque);
                    command.Parameters.AddWithValue("@CategoriaId", produto.CategoriaId);
                    command.Parameters.AddWithValue("@Marca", produto.Marca ?? "");
                    command.Parameters.AddWithValue("@Imagem", produto.Imagem ?? "");
                    command.Parameters.AddWithValue("@Ativo", produto.Ativo);
                    command.Parameters.AddWithValue("@DataCadastro", DateTime.Now);

                    connection.Open();
                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erro ao inserir produto: " + ex.Message);
                return false;
            }
        }

        // 🔍 BUSCAR PRODUTO POR ID
        public static Produto BuscarProdutoPorId(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Produtos WHERE Id = @Id";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Id", id);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        return new Produto
                        {
                            Id = (int)reader["Id"],
                            Nome = reader["Nome"].ToString(),
                            DescricaoCurta = reader["DescricaoCurta"] != DBNull.Value ? reader["DescricaoCurta"].ToString() : "",
                            DescricaoLonga = reader["DescricaoLonga"] != DBNull.Value ? reader["DescricaoLonga"].ToString() : "",
                            Preco = (decimal)reader["Preco"],
                            QuantidadeEstoque = (int)reader["QuantidadeEstoque"],
                            CategoriaId = reader["CategoriaId"] != DBNull.Value ? (int)reader["CategoriaId"] : 1,
                            Marca = reader["Marca"] != DBNull.Value ? reader["Marca"].ToString() : "",
                            Imagem = reader["Imagem"] != DBNull.Value ? reader["Imagem"].ToString() : "",
                            Ativo = (bool)reader["Ativo"],
                            DataCadastro = (DateTime)reader["DataCadastro"]
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erro ao buscar produto: " + ex.Message);
            }

            return null;
        }

        // ✏️ ATUALIZAR PRODUTO
        public static bool AtualizarProduto(Produto produto)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"UPDATE Produtos SET 
                                    Nome = @Nome, 
                                    DescricaoCurta = @DescricaoCurta, 
                                    DescricaoLonga = @DescricaoLonga, 
                                    Preco = @Preco, 
                                    QuantidadeEstoque = @QuantidadeEstoque, 
                                    CategoriaId = @CategoriaId,
                                    Marca = @Marca, 
                                    Imagem = @Imagem, 
                                    Ativo = @Ativo
                                    WHERE Id = @Id";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Id", produto.Id);
                    command.Parameters.AddWithValue("@Nome", produto.Nome ?? "");
                    command.Parameters.AddWithValue("@DescricaoCurta", produto.DescricaoCurta ?? "");
                    command.Parameters.AddWithValue("@DescricaoLonga", produto.DescricaoLonga ?? "");
                    command.Parameters.AddWithValue("@Preco", produto.Preco);
                    command.Parameters.AddWithValue("@QuantidadeEstoque", produto.QuantidadeEstoque);
                    command.Parameters.AddWithValue("@CategoriaId", produto.CategoriaId);
                    command.Parameters.AddWithValue("@Marca", produto.Marca ?? "");
                    command.Parameters.AddWithValue("@Imagem", produto.Imagem ?? "");
                    command.Parameters.AddWithValue("@Ativo", produto.Ativo);

                    connection.Open();
                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erro ao atualizar produto: " + ex.Message);
                return false;
            }
        }

        // 🗑️ EXCLUIR PRODUTO
        public static bool ExcluirProduto(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Produtos WHERE Id = @Id";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Id", id);

                    connection.Open();
                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erro ao excluir produto: " + ex.Message);
                return false;
            }
        }

        // 📊 LISTAR PRODUTOS ATIVOS (PARA LOJA)
        public static List<Produto> ListarProdutosAtivos()
        {
            var produtos = new List<Produto>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"SELECT * FROM Produtos 
                                   WHERE Ativo = 1 AND QuantidadeEstoque > 0
                                   ORDER BY Nome";
                    SqlCommand command = new SqlCommand(query, connection);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        produtos.Add(new Produto
                        {
                            Id = (int)reader["Id"],
                            Nome = reader["Nome"].ToString(),
                            DescricaoCurta = reader["DescricaoCurta"] != DBNull.Value ? reader["DescricaoCurta"].ToString() : "",
                            Preco = (decimal)reader["Preco"],
                            QuantidadeEstoque = (int)reader["QuantidadeEstoque"],
                            Marca = reader["Marca"] != DBNull.Value ? reader["Marca"].ToString() : "",
                            Imagem = reader["Imagem"] != DBNull.Value ? reader["Imagem"].ToString() : ""
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erro ao listar produtos ativos: " + ex.Message);
            }

            return produtos;
        }

        // 🔄 ATUALIZAR ESTOQUE
        public static bool AtualizarEstoque(int produtoId, int quantidade)
        {
            try
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erro ao atualizar estoque: " + ex.Message);
                return false;
            }
        }
    }
}