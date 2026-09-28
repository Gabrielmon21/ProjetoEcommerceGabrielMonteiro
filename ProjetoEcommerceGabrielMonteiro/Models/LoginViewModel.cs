using System.ComponentModel.DataAnnotations;

namespace ProjetoEcommerceGabrielMonteiro.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Senha é obrigatória")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        public string TipoUsuario { get; set; } // "cliente" ou "admin"
    }
}