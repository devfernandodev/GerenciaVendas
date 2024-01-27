using System.ComponentModel.DataAnnotations;

namespace GerenciaVendas.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        // Campo adicional para a opção Lembrar-me
        public bool LembrarMe { get; set; }
    }
}
