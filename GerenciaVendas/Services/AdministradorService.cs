using GerenciaVendas.Models;
using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace GerenciaVendas.Services
{
    public class AdministradorService
    {
        private readonly string _connectionString;

        public AdministradorService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException("String de conexão 'DefaultConnection' não encontrada.");
        }

        public async Task<IEnumerable<Usuario>> ListarTodosUsuariosAsync()
        {
            const string query = "SELECT * FROM Usuarios_Dev";
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryAsync<Usuario>(query);
            }
        }

        public async Task<Usuario> ObterUsuarioPorIdAsync(int id)
        {
            const string query = "SELECT * FROM Usuarios_Dev WHERE Id = @Id";
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QuerySingleOrDefaultAsync<Usuario>(query, new { Id = id });
            }
        }

        public async Task AtualizarUsuarioAsync(Usuario usuario)
        {
            // Adicionar verificação de permissão de administrador se necessário
            const string query = "UPDATE Usuarios_Dev SET Nome = @Nome, Email = @Email, Estado = @Estado WHERE Id = @Id";
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync(query, usuario);
            }
        }


        private async Task ExecuteQueryAsync(string query, object parameters)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync(query, parameters);
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


        public async Task TornarAdministradorAsync(int id)
        {
            const string query = "UPDATE Usuarios_Dev SET IsAdministrador = 1 WHERE Id = @Id";
            await ExecuteQueryAsync(query, new { Id = id });
        }

        public async Task RemoverAdministradorAsync(int id)
        {
            const string query = "UPDATE Usuarios_Dev SET IsAdministrador = 0 WHERE Id = @Id";
            await ExecuteQueryAsync(query, new { Id = id });
        }


    }
}

