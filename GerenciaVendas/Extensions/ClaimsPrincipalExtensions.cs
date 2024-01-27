using System;
using System.Security.Claims;

namespace GerenciaVendas.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }

            // A Claim de tipo NameIdentifier é geralmente usada para armazenar o ID do usuário.
            return principal.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public static string GetUserName(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }

            // A Claim de tipo Name é usada para armazenar o nome do usuário.
            return principal.FindFirstValue(ClaimTypes.Name);
        }

        public static string GetUserEmail(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }

            // A Claim de tipo Email é usada para armazenar o e-mail do usuário.
            return principal.FindFirstValue(ClaimTypes.Email);
        }

        // Você pode adicionar mais métodos para outras Claims conforme necessário.
    }
}
