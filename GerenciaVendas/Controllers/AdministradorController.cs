using GerenciaVendas.Models;
using GerenciaVendas.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using GerenciaVendas.ViewModels;

namespace GerenciaVendas.Controllers
{
    [Authorize(Roles = "Administrador")] // Apenas administradores podem acessar este controller
    public class AdministradorController : Controller
    {
        private readonly AdministradorService _adminService;

        public AdministradorController(AdministradorService adminService)
        {
            _adminService = adminService;
        }

        public async Task<IActionResult> IndexAdmin()
        {
            var usuarios = await _adminService.ListarTodosUsuariosAsync();
            return View("IndexAdmin", usuarios);
        }


        public async Task<IActionResult> DetailsAdmin(int id)
        {
            var usuario = await _adminService.ObterUsuarioPorIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View("DetailsAdmin", usuario);
        }
        public async Task<IActionResult> EditAdmin(int id)
        {
            var usuario = await _adminService.ObterUsuarioPorIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            var model = new EditUsuarioViewModel
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                // Mapeie outros campos necessários
            };

            return View("EditAdmin", model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAdmin(EditUsuarioViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var usuario = new Usuario
                    {
                        Id = model.Id,
                        Nome = model.Nome,
                        Email = model.Email,
                        // Mapeie outros campos necessários
                    };

                    await _adminService.AtualizarUsuarioAsync(usuario);
                    TempData["SuccessMessage"] = "Usuário atualizado com sucesso.";
                    return RedirectToAction(nameof(IndexAdmin));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Erro ao atualizar usuário: " + ex.Message);
                }
            }

            // Retornar a View com o ViewModel, não com o Modelo
            return View("EditAdmin", model);
        }










        public async Task<IActionResult> Ativar(int id)
        {
            try
            {
                await _adminService.AtivarUsuarioAsync(id);
                TempData["SuccessMessage"] = "Usuário ativado com sucesso.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erro ao ativar usuário: " + ex.Message;
            }
            return RedirectToAction(nameof(IndexAdmin));
        }

        public async Task<IActionResult> Inativar(int id)
        {
            try
            {
                await _adminService.InativarUsuarioAsync(id);
                TempData["SuccessMessage"] = "Usuário inativado com sucesso.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erro ao inativar usuário: " + ex.Message;
            }
            return RedirectToAction(nameof(IndexAdmin));
        }





        public async Task<IActionResult> TornarAdministrador(int id)
        {
            try
            {
                await _adminService.TornarAdministradorAsync(id);
                TempData["SuccessMessage"] = "Usuário designado como administrador com sucesso.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erro ao designar usuário como administrador: " + ex.Message;
            }
            return RedirectToAction(nameof(IndexAdmin));
        }

        public async Task<IActionResult> RemoverAdministrador(int id)
        {
            try
            {
                await _adminService.RemoverAdministradorAsync(id);
                TempData["SuccessMessage"] = "Status de administrador removido do usuário com sucesso.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erro ao remover status de administrador do usuário: " + ex.Message;
            }
            return RedirectToAction(nameof(IndexAdmin));
        }


        // Outros métodos administrativos conforme necessário
    }
}
