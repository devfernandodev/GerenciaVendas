using System.ComponentModel.DataAnnotations;

namespace GerenciaVendas.ViewModels
{
    public class UsuarioEditModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido")]
        public string Email { get; set; }

        // Senha atual é opcional, pois administradores podem editar sem precisar dela
        [DataType(DataType.Password)]
        public string SenhaAtual { get; set; }

        // Nova senha é opcional, usada somente se for necessário mudar a senha
        [DataType(DataType.Password)]
        public string NovaSenha { get; set; }

        // Campo para indicar se o usuário está ativo ou inativo
        public bool Estado { get; set; }
    }
}
