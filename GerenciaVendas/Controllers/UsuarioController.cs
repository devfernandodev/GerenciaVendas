using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Collections.Generic;
using System;
using GerenciaVendas.Models;
using GerenciaVendas.Services;
using Microsoft.AspNetCore.Mvc;
using GerenciaVendas.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;


namespace GerenciaVendas.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly UsuarioService _usuarioService;
        private readonly HashService _hashService;

        public UsuarioController(UsuarioService usuarioService, HashService hashService)
        {
            _usuarioService = usuarioService;
            _hashService = hashService;
        }

        public async Task<IActionResult> Index()
        {
            var loggedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.LoggedUserId = loggedUserId;

            var usuarios = await _usuarioService.GetAllUsuariosAsync();
            return View(usuarios);
        }



        public async Task<IActionResult> Details(int id)
        {
            var usuario = await _usuarioService.GetUsuarioByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var newUser = new Usuario
                    {
                        Nome = model.Nome,
                        Email = model.Email,
                        // Estado precisa ser definido, considere adicionar no ViewModel se necessário
                    };

                    await _usuarioService.AddUsuarioAsync(newUser, model.Senha);
                    TempData["SuccessMessage"] = "Usuário criado com sucesso.";
                    return RedirectToAction("Login");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Erro ao criar usuário: " + ex.Message);
                }
            }
            return View(model);
        }


        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var newUser = new Usuario
                    {
                        Nome = model.Nome,
                        Email = model.Email,
                        Estado = true
                    };

                    await _usuarioService.AddUsuarioAsync(newUser, model.Senha);
                    return RedirectToAction("Login");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Erro ao registrar usuário: " + ex.Message);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _usuarioService.AuthenticateAsync(model.Email, model.Senha);
                if (usuario != null)
                {
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                // Inclui a role do usuário com base na propriedade IsAdministrador
                new Claim(ClaimTypes.Role, usuario.IsAdministrador ? "Administrador" : "Usuario")
            };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                    // Redireciona para a página de administração se for um administrador
                    if (usuario.IsAdministrador)
                    {
                        return RedirectToAction("IndexAdmin", "Administrador"); // Certifique-se de que 'IndexAdmin' é o nome correto da sua View
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home"); // Redireciona para a página inicial de usuários comuns
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Nome de usuário ou senha inválidos.");
                }
            }

            return View(model);
        }





        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Page", "Home");
        }

        public async Task<IActionResult> Edit(int id)
        {
            // Obtém o ID do usuário logado como uma string.
            var loggedUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Verifica se o ID do usuário logado é um número e compara com o ID fornecido.
            if (!int.TryParse(loggedUserIdString, out int loggedUserId) || loggedUserId != id)
            {
                return Unauthorized();
            }

            var usuario = await _usuarioService.GetUsuarioByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            var model = new EditUserViewModel
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                // Outros campos, excluindo a senha
            };

            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditUserViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var usuarioExistente = await _usuarioService.GetUsuarioByIdAsync(id);
                    if (usuarioExistente == null)
                    {
                        return NotFound();
                    }

                    // Verifica se a senha atual está correta
                    if (!usuarioExistente.VerificarSenha(model.SenhaAtual, _hashService))
                    {
                        ModelState.AddModelError("SenhaAtual", "A senha atual está incorreta.");
                        return View(model);
                    }

                    // Atualiza Nome e Email
                    usuarioExistente.Nome = model.Nome;
                    usuarioExistente.Email = model.Email;
                    await _usuarioService.UpdateUsuarioAsync(usuarioExistente);

                    TempData["SuccessMessage"] = "Informações do usuário atualizadas com sucesso.";
                    return RedirectToAction(nameof(Edit));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Erro ao atualizar usuário: " + ex.Message);
                }
            }

            // Se chegarmos aqui, algo falhou, reexibir formulário
            return View(model);
        }










        [HttpGet]
        public IActionResult ChangePassword(int id)
        {
            var model = new ChangePasswordViewModel { UserId = id };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _usuarioService.GetUsuarioByIdAsync(model.UserId);
                if (usuario == null)
                {
                    return NotFound();
                }

                // Verifica se a senha atual está correta
                if (!usuario.VerificarSenha(model.SenhaAtual, _hashService))
                {
                    ModelState.AddModelError("SenhaAtual", "A senha atual está incorreta.");
                    return View(model);
                }

                // Atualiza para a nova senha
                await _usuarioService.UpdateUsuarioSenhaAsync(usuario.Id, model.NovaSenha);
                TempData["SuccessMessage"] = "Senha atualizada com sucesso.";
                return RedirectToAction(nameof(ChangePassword));
            }

            return View(model);
        }















        public async Task<IActionResult> Ativar(int id)
        {
            try
            {
                await _usuarioService.AtivarUsuarioAsync(id);
                TempData["SuccessMessage"] = "Usuário ativado com sucesso.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erro ao ativar usuário: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Inativar(int id)
        {
            try
            {
                await _usuarioService.InativarUsuarioAsync(id);
                TempData["SuccessMessage"] = "Usuário inativado com sucesso.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erro ao inativar usuário: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }











        // ... Adicione outros métodos conforme necessário ...
    }
}

