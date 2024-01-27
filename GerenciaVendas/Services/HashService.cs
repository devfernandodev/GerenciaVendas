using BCrypt.Net;
using System;

namespace GerenciaVendas.Services
{
    public class HashService
    {
        private const int WorkFactor = 10; // Ajustável conforme necessário

        public string HashPassword(string senha)
        {
            if (string.IsNullOrWhiteSpace(senha))
            {
                throw new ArgumentException("A senha não pode ser nula ou em branco.", nameof(senha));
            }

            // Implementação do hash da senha
            return BCrypt.Net.BCrypt.HashPassword(senha, WorkFactor);
        }

        public bool VerifyPasswordHash(string senha, string hashSalvo)
        {
            if (string.IsNullOrWhiteSpace(hashSalvo))
            {
                throw new ArgumentException("Hash salvo não pode ser nulo ou em branco.", nameof(hashSalvo));
            }

            // Verifica se a senha corresponde ao hash
            return BCrypt.Net.BCrypt.Verify(senha, hashSalvo);
        }
    }
}
