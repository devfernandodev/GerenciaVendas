using System.ComponentModel.DataAnnotations;

namespace GerenciaVendas.ViewModels
{
    public class EditUsuarioViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter menos de 100 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido")]
        public string Email { get; set; }

        // Novas propriedades para representar se o usuário é um administrador e se está ativo
        public bool IsAdministrador { get; set; }
        public bool Estado { get; set; }

        // Outros campos conforme necessário
    }
}
