using System.ComponentModel.DataAnnotations;

namespace ProjetoEcommerceGabrielMonteiro.Models
{
    public class RegistroViewModel
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [Display(Name = "Nome Completo")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Senha é obrigatória")]
        [MinLength(6, ErrorMessage = "Mínimo 6 caracteres")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [Required(ErrorMessage = "Confirme a senha")]
        [Compare("Senha", ErrorMessage = "Senhas não conferem")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Senha")]
        public string ConfirmarSenha { get; set; }

        [Display(Name = "Telefone")]
        public string NumContato { get; set; }

        [Display(Name = "Endereço")]
        public string Endereco { get; set; }
    }
}