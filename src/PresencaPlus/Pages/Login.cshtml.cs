using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace PresencaPlus.Pages;

[AllowAnonymous]
public class LoginModel(AppDbContext db) : PageModel
{
    [BindProperty]
    [Display(Name = "Login")]
    [Required(ErrorMessage = "Informe o login.")]
    public string UsuarioLogin { get; set; } = "";

    [BindProperty]
    [Display(Name = "Senha")]
    [Required(ErrorMessage = "Informe a senha.")]
    [DataType(DataType.Password)]
    public string Senha { get; set; } = "";

    public string? Erro { get; private set; }

    public IActionResult OnGet() =>
        User.Identity?.IsAuthenticated == true ? Redirect("/") : Page();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var usuario = await db.Usuarios.SingleOrDefaultAsync(u => u.Login == UsuarioLogin);
        if (usuario is null || !usuario.VerificarSenha(Senha))
        {
            Erro = "Login ou senha inválidos.";
            return Page();
        }

        var identidade = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Role, usuario.Perfil)
        ], CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(new ClaimsPrincipal(identidade));
        return Redirect(usuario.Perfil == "Professor" ? "/Chamada" : "/Relatorio");
    }
}
