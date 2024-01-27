using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations;
namespace GerenciaVendas.ViewModels
{
    public class EditUserViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string SenhaAtual { get; set; }
    }
}