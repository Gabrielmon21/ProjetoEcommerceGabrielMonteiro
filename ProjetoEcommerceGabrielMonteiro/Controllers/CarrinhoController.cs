using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ProjetoEcommerceGabrielMonteiro.Models;

namespace ProjetoEcommerceGabrielMonteiro.Controllers
{
    public class CarrinhoController : Controller
    {
        // GET: Carrinho
        public ActionResult Index()
        {
            if (Session["UsuarioLogado"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var carrinho = Session["Carrinho"] as List<ItemCarrinho> ?? new List<ItemCarrinho>();
            ViewBag.Total = CalcularTotal(carrinho);
            return View(carrinho);
        }

        // ADICIONAR ITEM AO CARRINHO
        public ActionResult Adicionar(int id, int quantidade = 1)
        {
            if (Session["UsuarioLogado"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var produto = ProdutoDAO.BuscarProdutoPorId(id);
            if (produto == null)
            {
                TempData["Erro"] = "Produto não encontrado!";
                return RedirectToAction("Index", "Loja");
            }

            // Verificar estoque
            if (produto.QuantidadeEstoque < quantidade)
            {
                TempData["Erro"] = $"Estoque insuficiente! Disponível: {produto.QuantidadeEstoque}";
                return RedirectToAction("Index", "Loja");
            }

            var carrinho = Session["Carrinho"] as List<ItemCarrinho> ?? new List<ItemCarrinho>();

            // Verificar se o produto já está no carrinho
            var itemExistente = carrinho.Find(x => x.ProdutoId == id);
            if (itemExistente != null)
            {
                // Verificar se a nova quantidade não excede o estoque
                if (itemExistente.Quantidade + quantidade > produto.QuantidadeEstoque)
                {
                    TempData["Erro"] = $"Estoque insuficiente! Disponível: {produto.QuantidadeEstoque}";
                    return RedirectToAction("Index", "Loja");
                }
                itemExistente.Quantidade += quantidade;
            }
            else
            {
                carrinho.Add(new ItemCarrinho
                {
                    ProdutoId = produto.Id,
                    Nome = produto.Nome,
                    Preco = produto.Preco,
                    Quantidade = quantidade,
                    Imagem = produto.Imagem
                });
            }

            Session["Carrinho"] = carrinho;
            TempData["Sucesso"] = $"{produto.Nome} adicionado ao carrinho!";
            return RedirectToAction("Index", "Loja");
        }

        // REMOVER ITEM DO CARRINHO
        public ActionResult Remover(int id)
        {
            var carrinho = Session["Carrinho"] as List<ItemCarrinho>;
            if (carrinho != null)
            {
                var item = carrinho.Find(x => x.ProdutoId == id);
                if (item != null)
                {
                    carrinho.Remove(item);
                    Session["Carrinho"] = carrinho;
                    TempData["Sucesso"] = "Item removido do carrinho!";
                }
            }
            return RedirectToAction("Index");
        }

        // ATUALIZAR QUANTIDADE
        [HttpPost]
        public ActionResult AtualizarQuantidade(int produtoId, int quantidade)
        {
            var carrinho = Session["Carrinho"] as List<ItemCarrinho>;
            if (carrinho != null)
            {
                var item = carrinho.Find(x => x.ProdutoId == produtoId);
                if (item != null)
                {
                    // Verificar estoque antes de atualizar
                    var produto = ProdutoDAO.BuscarProdutoPorId(produtoId);
                    if (produto != null && quantidade > produto.QuantidadeEstoque)
                    {
                        TempData["Erro"] = $"Estoque insuficiente! Disponível: {produto.QuantidadeEstoque}";
                        return RedirectToAction("Index");
                    }

                    if (quantidade <= 0)
                    {
                        carrinho.Remove(item);
                        TempData["Sucesso"] = "Item removido do carrinho!";
                    }
                    else
                    {
                        item.Quantidade = quantidade;
                        TempData["Sucesso"] = "Quantidade atualizada!";
                    }
                    Session["Carrinho"] = carrinho;
                }
            }
            return RedirectToAction("Index");
        }

        // FINALIZAR PEDIDO
        public ActionResult Finalizar()
        {
            if (Session["UsuarioLogado"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var carrinho = Session["Carrinho"] as List<ItemCarrinho> ?? new List<ItemCarrinho>();
            if (carrinho.Count == 0)
            {
                TempData["Erro"] = "Seu carrinho está vazio!";
                return RedirectToAction("Index");
            }

            // Verificar estoque antes de finalizar
            foreach (var item in carrinho)
            {
                var produto = ProdutoDAO.BuscarProdutoPorId(item.ProdutoId);
                if (produto == null || produto.QuantidadeEstoque < item.Quantidade)
                {
                    TempData["Erro"] = $"Estoque insuficiente para: {item.Nome}";
                    return RedirectToAction("Index");
                }
            }

            ViewBag.Total = CalcularTotal(carrinho);
            return View();
        }

        [HttpPost]
        public ActionResult FinalizarPedido(string enderecoEntrega, string metodoPagamento)
        {
            if (Session["UsuarioLogado"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var carrinho = Session["Carrinho"] as List<ItemCarrinho>;
            if (carrinho == null || carrinho.Count == 0)
            {
                TempData["Erro"] = "Seu carrinho está vazio!";
                return RedirectToAction("Index");
            }

            try
            {
                // Calcular total
                decimal total = CalcularTotal(carrinho);

                // Criar pedido (SEM NumeroPedido por enquanto)
                var pedido = new Pedido
                {
                    ClienteId = 1, // ID fixo para teste - em produção pegaria do usuário logado
                    EnderecoEntrega = enderecoEntrega,
                    MetodoPagamento = metodoPagamento,
                    Total = total,
                    Status = "Confirmado"
                };

                // Salvar no banco
                int pedidoId = PedidoDAO.SalvarPedido(pedido);
                bool itensSalvos = PedidoDAO.SalvarItensPedido(pedidoId, carrinho);

                if (itensSalvos)
                {
                    // Enviar e-mail de confirmação para 3523@fai.com.br
                    bool emailEnviado = EmailService.EnviarEmailConfirmacao("3523@fai.com.br", pedidoId, carrinho, total);
                    if (!emailEnviado)
                    {
                        // Ainda considera pedido como realizado, mas informa falha no envio do e-mail
                        TempData["Aviso"] = "Pedido realizado, mas falha ao enviar e-mail de confirmação.";
                    }
                    else
                    {
                        TempData["Sucesso"] = $"🎉 Pedido #{pedidoId} realizado com sucesso! E-mail de confirmação enviado.";
                    }

                    // Limpar carrinho
                    Session["Carrinho"] = new List<ItemCarrinho>();

                    // Se já havia uma mensagem de sucesso padrão, manter ou sobrescrever dependendo do envio do e-mail
                    if (TempData["Sucesso"] == null)
                    {
                        TempData["Sucesso"] = $"🎉 Pedido #{pedidoId} realizado com sucesso! Total: R$ {total:N2}. Status: Em processamento.";
                    }

                    return RedirectToAction("Index", "Loja");
                }
                else
                {
                    TempData["Erro"] = "Erro ao salvar itens do pedido!";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Erro"] = $"Erro ao finalizar pedido: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // LIMPAR CARRINHO COMPLETO
        public ActionResult Limpar()
        {
            Session["Carrinho"] = new List<ItemCarrinho>();
            TempData["Sucesso"] = "Carrinho limpo com sucesso!";
            return RedirectToAction("Index");
        }

        // MÉTODO AUXILIAR PARA CALCULAR TOTAL
        private decimal CalcularTotal(List<ItemCarrinho> carrinho)
        {
            decimal total = 0;
            foreach (var item in carrinho)
            {
                total += item.Preco * item.Quantidade;
            }
            return total;
        }
    }
}