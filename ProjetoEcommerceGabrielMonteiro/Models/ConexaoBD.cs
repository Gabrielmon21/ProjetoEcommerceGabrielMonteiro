using System;
using System.Configuration;
using System.Data.SqlClient;

namespace ProjetoEcommerceGabrielMonteiro.Models
{
    public class ConexaoBD
    {
        public static SqlConnection GetConnection()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            return new SqlConnection(connectionString);
        }

        // Método para validar login no banco
        public static bool ValidarLogin(string email, string senha, string tipoUsuario)
        {
            using (SqlConnection connection = GetConnection())
            {
                string tabela = (tipoUsuario == "admin") ? "Administradores" : "Clientes";
                string query = $"SELECT COUNT(1) FROM {tabela} WHERE Email = @Email AND Senha = @Senha";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Senha", senha);

                connection.Open();
                int count = Convert.ToInt32(command.ExecuteScalar());
                return count == 1;
            }
        }
    }
}