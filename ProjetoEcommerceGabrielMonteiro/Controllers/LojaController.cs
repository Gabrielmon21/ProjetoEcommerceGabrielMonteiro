using System.Web.Mvc;
using ProjetoEcommerceGabrielMonteiro.Models;

namespace ProjetoEcommerceGabrielMonteiro.Controllers
{
    public class LojaController : Controller
    {
        // GET: Loja
        public ActionResult Index()
        {
            var produtos = ProdutoDAO.ListarProdutosAtivos();
            ViewBag.Message = "Bem-vindo à Nossa Loja!";
            return View(produtos);
        }

        public ActionResult DetalhesProduto(int id)
        {
            var produto = ProdutoDAO.BuscarProdutoPorId(id);
            if (produto == null)
            {
                TempData["Erro"] = "Produto não encontrado!";
                return RedirectToAction("Index");
            }

            if (!produto.Ativo)
            {
                TempData["Erro"] = "Este produto não está disponível!";
                return RedirectToAction("Index");
            }

            return View(produto);
        }
    }
}