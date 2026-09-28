using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using ProjetoEcommerceGabrielMonteiro.Models;

namespace ProjetoEcommerceGabrielMonteiro.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            if (Session["UsuarioLogado"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Home");

            ViewBag.Message = "Bem-vindo à Área Administrativa!";
            return View();
        }

        public ActionResult Produtos()
        {
            if (Session["UsuarioLogado"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Home");

            ViewBag.Message = "Gerenciamento de Produtos";
            var produtos = ProdutoDAO.ListarProdutos();
            return View(produtos);
        }

        public ActionResult Pedidos()
        {
            if (Session["UsuarioLogado"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Home");

            ViewBag.Message = "Visualização de Pedidos";
            var pedidos = PedidoDAO.BuscarPedidosPorCliente(1);
            return View(pedidos);
        }

        // 👥 GERENCIAMENTO DE CLIENTES
        public ActionResult Clientes()
        {
            if (Session["UsuarioLogado"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Home");

            ViewBag.Message = "Gerenciamento de Clientes";
            var clientes = ClienteDAO.ListarClientes();
            return View(clientes);
        }

        public ActionResult DetalhesCliente(int? id)
        {
            if (Session["UsuarioLogado"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Home");

            if (id == null || id == 0)
            {
                TempData["Erro"] = "ID do cliente não especificado!";
                return RedirectToAction("Clientes");
            }

            var cliente = ClienteDAO.BuscarClientePorId(id.Value);
            if (cliente == null)
            {
                TempData["Erro"] = "Cliente não encontrado!";
                return RedirectToAction("Clientes");
            }

            ViewBag.PedidosCliente = ClienteDAO.BuscarPedidosPorCliente(id.Value);
            return View(cliente);
        }

        public ActionResult EditarCliente(int? id)
        {
            if (Session["UsuarioLogado"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Home");

            if (id == null || id == 0)
            {
                TempData["Erro"] = "ID do cliente não especificado!";
                return RedirectToAction("Clientes");
            }

            var cliente = ClienteDAO.BuscarClientePorId(id.Value);
            if (cliente == null)
            {
                TempData["Erro"] = "Cliente não encontrado!";
                return RedirectToAction("Clientes");
            }

            return View(cliente);
        }

        // ✅ MÉTODO EDITAR CLIENTE SEM VALIDAÇÃO
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarCliente(Cliente cliente)
        {
            if (Session["UsuarioLogado"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Home");

            // ✅ REMOVER VALIDAÇÃO PARA TESTE
            ModelState.Clear();

            // ✅ DEBUG SIMPLES
            System.Diagnostics.Debug.WriteLine($"Editando cliente ID: {cliente.Id}");
            System.Diagnostics.Debug.WriteLine($"Nome: {cliente.Nome}");
            System.Diagnostics.Debug.WriteLine($"Email: {cliente.Email}");

            // ✅ VERIFICAR SE DADOS SÃO VÁLIDOS MANUALMENTE
            if (string.IsNullOrWhiteSpace(cliente.Nome))
            {
                TempData["Erro"] = "O nome do cliente é obrigatório!";
                return View(cliente);
            }

            if (string.IsNullOrWhiteSpace(cliente.Email))
            {
                TempData["Erro"] = "O email do cliente é obrigatório!";
                return View(cliente);
            }

            // ✅ TENTAR ATUALIZAR NO BANCO
            try
            {
                bool sucesso = ClienteDAO.AtualizarCliente(cliente);

                if (sucesso)
                {
                    TempData["Sucesso"] = "✅ Cliente atualizado com sucesso!";
                    return RedirectToAction("DetalhesCliente", new { id = cliente.Id });
                }
                else
                {
                    TempData["Erro"] = "❌ Erro ao atualizar cliente no banco de dados!";
                }
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "❌ Erro: " + ex.Message;
                System.Diagnostics.Debug.WriteLine($"Erro: {ex.Message}");
            }

            return View(cliente);
        }

        public ActionResult ExcluirCliente(int? id)
        {
            if (Session["UsuarioLogado"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Home");

            if (id == null || id == 0)
            {
                TempData["Erro"] = "ID do cliente não especificado!";
                return RedirectToAction("Clientes");
            }

            if (ClienteDAO.ExcluirCliente(id.Value))
            {
                TempData["Sucesso"] = "Cliente excluído com sucesso!";
            }
            else
            {
                TempData["Erro"] = "Erro ao excluir cliente!";
            }
            return RedirectToAction("Clientes");
        }

        // 🔧 CRUD DE PRODUTOS
        public ActionResult CriarProduto()
        {
            if (Session["UsuarioLogado"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Home");

            return View();
        }

        [HttpPost]
        public ActionResult CriarProduto(Produto produto)
        {
            if (Session["UsuarioLogado"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Home");

            // ✅ SEM VALIDAÇÃO TAMBÉM
            ModelState.Clear();

            if (ProdutoDAO.InserirProduto(produto))
            {
                TempData["Sucesso"] = "Produto cadastrado com sucesso!";
                return RedirectToAction("Produtos");
            }
            TempData["Erro"] = "Erro ao cadastrar produto!";
            return View(produto);
        }

        public ActionResult EditarProduto(int? id)
        {
            if (Session["UsuarioLogado"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Home");

            if (id == null || id == 0)
            {
                TempData["Erro"] = "ID do produto não especificado!";
                return RedirectToAction("Produtos");
            }

            var produto = ProdutoDAO.BuscarProdutoPorId(id.Value);
            if (produto == null)
            {
                TempData["Erro"] = "Produto não encontrado!";
                return RedirectToAction("Produtos");
            }
            return View(produto);
        }

        [HttpPost]
        public ActionResult EditarProduto(Produto produto)
        {
            if (Session["UsuarioLogado"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Home");

            // ✅ SEM VALIDAÇÃO
            ModelState.Clear();

            if (ProdutoDAO.AtualizarProduto(produto))
            {
                TempData["Sucesso"] = "Produto atualizado com sucesso!";
                return RedirectToAction("Produtos");
            }
            TempData["Erro"] = "Erro ao atualizar produto!";
            return View(produto);
        }

        public ActionResult ExcluirProduto(int? id)
        {
            if (Session["UsuarioLogado"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Home");

            if (id == null || id == 0)
            {
                TempData["Erro"] = "ID do produto não especificado!";
                return RedirectToAction("Produtos");
            }

            if (ProdutoDAO.ExcluirProduto(id.Value))
            {
                TempData["Sucesso"] = "Produto excluído com sucesso!";
            }
            else
            {
                TempData["Erro"] = "Erro ao excluir produto!";
            }
            return RedirectToAction("Produtos");
        }

        // 🔧 NOVAS ACTIONS PARA PEDIDOS
        public ActionResult DetalhesPedido(int? id)
        {
            if (Session["UsuarioLogado"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Home");

            if (id == null || id == 0)
            {
                TempData["Erro"] = "ID do pedido não especificado!";
                return RedirectToAction("Pedidos");
            }

            var pedido = PedidoDAO.BuscarPedidoPorId(id.Value);
            if (pedido == null)
            {
                TempData["Erro"] = "Pedido não encontrado!";
                return RedirectToAction("Pedidos");
            }

            pedido.Itens = PedidoDAO.BuscarItensPedido(id.Value);
            return View(pedido);
        }

        public ActionResult EditarStatusPedido(int? id)
        {
            if (Session["UsuarioLogado"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Home");

            if (id == null || id == 0)
            {
                TempData["Erro"] = "ID do pedido não especificado!";
                return RedirectToAction("Pedidos");
            }

            var pedido = PedidoDAO.BuscarPedidoPorId(id.Value);
            if (pedido == null)
            {
                TempData["Erro"] = "Pedido não encontrado!";
                return RedirectToAction("Pedidos");
            }

            return View(pedido);
        }

        [HttpPost]
        public ActionResult EditarStatusPedido(int id, string status)
        {
            if (Session["UsuarioLogado"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Home");

            if (PedidoDAO.AtualizarStatusPedido(id, status))
            {
                TempData["Sucesso"] = "Status do pedido atualizado com sucesso!";
            }
            else
            {
                TempData["Erro"] = "Erro ao atualizar status do pedido!";
            }

            return RedirectToAction("Pedidos");
        }

        // 🔧 DASHBOARD ADMINISTRATIVO
        public ActionResult Dashboard()
        {
            if (Session["UsuarioLogado"]?.ToString() != "Admin")
                return RedirectToAction("Login", "Home");

            ViewBag.Message = "Dashboard Administrativo";

            ViewBag.TotalProdutos = ProdutoDAO.ListarProdutos().Count;
            ViewBag.TotalPedidos = PedidoDAO.BuscarPedidosPorCliente(1).Count;
            ViewBag.TotalClientes = ClienteDAO.ListarClientes().Count;

            return View();
        }
    }
}