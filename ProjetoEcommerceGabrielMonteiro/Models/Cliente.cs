using System;
using System.ComponentModel.DataAnnotations;

namespace ProjetoEcommerceGabrielMonteiro.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória")]
        [MinLength(6, ErrorMessage = "Mínimo 6 caracteres")]
        public string Senha { get; set; }

        public string NumContato { get; set; }
        public string Endereco { get; set; }

        // ✅ ADICIONE ESTA PROPRIEDADE
        public DateTime DataCadastro { get; set; } = DateTime.Now;
    }
}