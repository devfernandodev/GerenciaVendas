using System;
using System.ComponentModel.DataAnnotations;
using GerenciaVendas.Services;

namespace GerenciaVendas.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter menos de 100 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido")]
        public string Email { get; set; }
        public bool IsAdmin { get; set; } = false;
        public string SenhaHash { get; private set; } // Tornar setter privado para controle interno
        public bool IsAdministrador { get; set; }



        [Required]
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

        [Required]
        public bool Estado { get; set; } // Ativo/Inativo

        // Método privado para definir a senha
        public void DefinirSenha(string senha, HashService hashService)
        {
            if (string.IsNullOrWhiteSpace(senha))
            {
                throw new ArgumentException("Senha não pode ser nula ou em branco.", nameof(senha));
            }

            SenhaHash = hashService.HashPassword(senha);
        }

        // Método para verificar a senha
        public bool VerificarSenha(string senha, HashService hashService)
        {
            if (string.IsNullOrEmpty(SenhaHash))
            {
                throw new InvalidOperationException("Hash da senha não está definido.");
            }

            return hashService.VerifyPasswordHash(senha, SenhaHash);
        }
    }
}