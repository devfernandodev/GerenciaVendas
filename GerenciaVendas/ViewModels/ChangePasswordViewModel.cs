using System.ComponentModel.DataAnnotations;

namespace GerenciaVendas.ViewModels
{
    public class ChangePasswordViewModel
    {
        public int UserId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string SenhaAtual { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NovaSenha { get; set; }

        [DataType(DataType.Password)]
        [Compare("NovaSenha", ErrorMessage = "A nova senha e a confirmação não correspondem.")]
        public string ConfirmarNovaSenha { get; set; }
    }

}
