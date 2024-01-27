using GerenciaVendas.Models;
using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System;

namespace GerenciaVendas.Services
{
    public class UsuarioService
    {
        private readonly string _connectionString;
        private readonly HashService _hashService;

        public UsuarioService(IConfiguration configuration, HashService hashService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException("String de conexão 'DefaultConnection' não encontrada.");
            _hashService = hashService ?? throw new ArgumentNullException(nameof(hashService), "HashService não pode ser nulo.");
        }

        public async Task<Usuario> GetUsuarioByIdAsync(int id)
        {
            const string query = "SELECT * FROM Usuarios_Dev WHERE Id = @Id";
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QuerySingleOrDefaultAsync<Usuario>(query, new { Id = id });
            }
        }



        public async Task<Usuario> GetUsuarioByNomeAsync(string nome)
        {
            const string query = "SELECT * FROM Usuarios_Dev WHERE Nome = @Nome";
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QuerySingleOrDefaultAsync<Usuario>(query, new { Nome = nome });
            }
        }







        public async Task AddUsuarioAsync(Usuario usuario, string senha)
        {
            if (string.IsNullOrEmpty(senha))
            {
                throw new ArgumentException("A senha não pode ser nula ou vazia.", nameof(senha));
            }

            usuario.DefinirSenha(senha, _hashService);

            const string query = "INSERT INTO Usuarios_Dev (Nome, Email, SenhaHash, DataCadastro, Estado) VALUES (@Nome, @Email, @SenhaHash, GETDATE(), @Estado)";
            await ExecuteQueryAsync(query, usuario);
        }

        public async Task<IEnumerable<Usuario>> GetAllUsuariosAsync()
        {
            const string query = "SELECT * FROM Usuarios_Dev";
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryAsync<Usuario>(query);
            }
        }

        public async Task UpdateUsuarioAsync(Usuario usuario)
        {
            const string query = "UPDATE Usuarios_Dev SET Nome = @Nome, Email = @Email, Estado = @Estado WHERE Id = @Id";
            await ExecuteQueryAsync(query, usuario);
        }

        public async Task UpdateUsuarioSenhaAsync(int id, string novaSenha)
        {
            var usuario = await GetUsuarioByIdAsync(id);
            if (usuario == null)
            {
                throw new InvalidOperationException("Usuário não encontrado.");
            }

            usuario.DefinirSenha(novaSenha, _hashService);
            const string query = "UPDATE Usuarios_Dev SET SenhaHash = @SenhaHash WHERE Id = @Id";
            await ExecuteQueryAsync(query, new { Id = usuario.Id, SenhaHash = usuario.SenhaHash });
        }

        public async Task<Usuario> AuthenticateAsync(string email, string senha)
        {
            var usuario = await GetUsuarioByEmailAsync(email);
            if (usuario != null && usuario.VerificarSenha(senha, _hashService))
            {
                return usuario;
            }
            return null;
        }

        public async Task<Usuario> GetUsuarioByEmailAsync(string email)
        {
            const string query = "SELECT * FROM Usuarios_Dev WHERE Email = @Email";
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QuerySingleOrDefaultAsync<Usuario>(query, new { Email = email });
            }
        }

        public async Task AtivarUsuarioAsync(int id)
        {
            const string query = "UPDATE Usuarios_Dev SET Estado = 1 WHERE Id = @Id";
            await ExecuteQueryAsync(query, new { Id = id });
        }

        public async Task InativarUsuarioAsync(int id)
        {
            const string query = "UPDATE Usuarios_Dev SET Estado = 0 WHERE Id = @Id";
            await ExecuteQueryAsync(query, new { Id = id });
        }

        private async Task ExecuteQueryAsync(string query, object parameters)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task<bool> AlterarSenhaAsync(int id, string senhaAtual, string novaSenha)
        {
            var usuario = await GetUsuarioByIdAsync(id);
            if (usuario == null)
            {
                return false;
            }

            if (!usuario.VerificarSenha(senhaAtual, _hashService))
            {
                return false;
            }

            await UpdateUsuarioSenhaAsync(id, novaSenha);
            return true;
        }
    }
}