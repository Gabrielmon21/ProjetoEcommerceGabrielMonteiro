using System;
using System.ComponentModel.DataAnnotations;

namespace ProjetoEcommerceGabrielMonteiro.Models
{
    public class Produto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
        public string Nome { get; set; }

        [StringLength(200, ErrorMessage = "Máximo 200 caracteres")]
        [Display(Name = "Descrição Curta")]
        public string DescricaoCurta { get; set; }

        [Display(Name = "Descrição Longa")]
        public string DescricaoLonga { get; set; }

        [Required(ErrorMessage = "Preço é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço deve ser maior que zero")]
        [Display(Name = "Preço")]
        public decimal Preco { get; set; }

        [Required(ErrorMessage = "Estoque é obrigatório")]
        [Range(0, int.MaxValue, ErrorMessage = "Estoque não pode ser negativo")]
        [Display(Name = "Quantidade em Estoque")]
        public int QuantidadeEstoque { get; set; }

        [Display(Name = "Categoria")]
        public int CategoriaId { get; set; }

        [StringLength(50)]
        public string Marca { get; set; }

        [Display(Name = "Imagem")]
        public string Imagem { get; set; }

        public bool Ativo { get; set; } = true;

        [Display(Name = "Data de Cadastro")]
        public DateTime DataCadastro { get; set; } = DateTime.Now;
    }
}