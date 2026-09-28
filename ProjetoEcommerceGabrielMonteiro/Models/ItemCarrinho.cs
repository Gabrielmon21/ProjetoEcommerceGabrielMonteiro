namespace ProjetoEcommerceGabrielMonteiro.Models
{
    public class ItemCarrinho
    {
        public int ProdutoId { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }
        public int Quantidade { get; set; }
        public string Imagem { get; set; }

        public decimal Subtotal
        {
            get { return Preco * Quantidade; }
        }
    }
}