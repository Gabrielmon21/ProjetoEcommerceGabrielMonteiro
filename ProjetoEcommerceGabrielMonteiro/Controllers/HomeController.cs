using System;
using System.Web.Mvc;
using ProjetoEcommerceGabrielMonteiro.Models;

namespace ProjetoEcommerceGabrielMonteiro.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        // ✅ NOVAS ACTIONS DE AUTENTICAÇÃO
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel login)
        {
            // ✅ REMOVER VALIDAÇÃO TEMPORARIAMENTE
            ModelState.Clear();

            if (string.IsNullOrWhiteSpace(login.Email) || string.IsNullOrWhiteSpace(login.Senha))
            {
                TempData["Erro"] = "Email e senha são obrigatórios!";
                return View(login);
            }

            // ✅ VALIDAR ADMIN
            if (login.Email == "admin@gabrielmonteiro.com" && login.Senha == "123456")
            {
                Session["UsuarioLogado"] = "Admin";
                Session["UsuarioEmail"] = login.Email;
                return RedirectToAction("Index", "Admin");
            }

            // ✅ VALIDAR CLIENTE NO BANCO
            var cliente = ClienteDAO.ValidarLogin(login.Email, login.Senha);
            if (cliente != null)
            {
                Session["UsuarioLogado"] = "Cliente";
                Session["UsuarioEmail"] = login.Email;
                Session["ClienteId"] = cliente.Id;
                Session["ClienteNome"] = cliente.Nome;
                return RedirectToAction("Index", "Loja");
            }

            TempData["Erro"] = "Email ou senha inválidos!";
            return View(login);
        }

        public ActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registrar(RegistroViewModel registro)
        {
            // ✅ REMOVER VALIDAÇÃO TEMPORARIAMENTE
            ModelState.Clear();

            // ✅ DEBUG
            System.Diagnostics.Debug.WriteLine("=== TENTANDO CADASTRAR CLIENTE ===");
            System.Diagnostics.Debug.WriteLine($"Nome: {registro.Nome}");
            System.Diagnostics.Debug.WriteLine($"Email: {registro.Email}");
            System.Diagnostics.Debug.WriteLine($"Senha: {registro.Senha}");

            // ✅ VERIFICAÇÃO MANUAL DOS CAMPOS OBRIGATÓRIOS
            if (string.IsNullOrWhiteSpace(registro.Nome))
            {
                TempData["Erro"] = "O nome é obrigatório!";
                return View(registro);
            }

            if (string.IsNullOrWhiteSpace(registro.Email))
            {
                TempData["Erro"] = "O email é obrigatório!";
                return View(registro);
            }

            if (string.IsNullOrWhiteSpace(registro.Senha))
            {
                TempData["Erro"] = "A senha é obrigatória!";
                return View(registro);
            }

            // ✅ CONVERTER RegistroViewModel para Cliente (COM SENHA)
            Cliente novoCliente = new Cliente
            {
                Nome = registro.Nome,
                Email = registro.Email,
                Senha = registro.Senha, // ✅ AGORA COM SENHA
                NumContato = registro.NumContato,
                Endereco = registro.Endereco,
                DataCadastro = DateTime.Now
            };

            // ✅ SALVAR NO BANCO DE DADOS
            try
            {
                bool sucesso = ClienteDAO.InserirCliente(novoCliente);
                System.Diagnostics.Debug.WriteLine($"Resultado do cadastro: {sucesso}");

                if (sucesso)
                {
                    System.Diagnostics.Debug.WriteLine($"✅ Cliente {registro.Nome} cadastrado com sucesso!");

                    Session["UsuarioLogado"] = "Cliente";
                    Session["UsuarioEmail"] = registro.Email;
                    Session["ClienteNome"] = registro.Nome;
                    TempData["Sucesso"] = "Cadastro realizado com sucesso!";
                    return RedirectToAction("Index", "Loja");
                }
                else
                {
                    TempData["Erro"] = "Erro ao cadastrar no banco de dados!";
                    System.Diagnostics.Debug.WriteLine("❌ Erro ao salvar cliente no banco!");
                }
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro interno: " + ex.Message;
                System.Diagnostics.Debug.WriteLine($"❌ EXCEÇÃO: {ex.Message}");
            }

            return View(registro);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}