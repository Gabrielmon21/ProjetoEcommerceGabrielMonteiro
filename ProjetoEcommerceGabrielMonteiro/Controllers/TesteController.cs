using System.Web.Mvc;
using System.Data.SqlClient;
using ProjetoEcommerceGabrielMonteiro.Models;

namespace ProjetoEcommerceGabrielMonteiro.Controllers
{
    public class TesteController : Controller
    {
        public ActionResult TestarConexao()
        {
            try
            {
                using (SqlConnection connection = ConexaoBD.GetConnection())
                {
                    connection.Open();
                    ViewBag.Mensagem = "✅ Conexão com banco OK!";
                    return View();
                }
            }
            catch (System.Exception ex)
            {
                ViewBag.Mensagem = "❌ Erro na conexão: " + ex.Message;
                return View();
            }
        }
    }
}