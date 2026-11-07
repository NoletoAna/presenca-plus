using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace PresencaPlus.Pages.Turmas;

public class CriarModel(AppDbContext db) : PageModel
{
    [BindProperty]
    public Turma Turma { get; set; } = new() { AnoLetivo = DateTime.Today.Year };

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        // RN03: nome da turma é único.
        if (await db.Turmas.AnyAsync(t => t.Nome == Turma.Nome))
            ModelState.AddModelError("Turma.Nome", "Já existe uma turma com este nome.");

        if (!ModelState.IsValid) return Page();

        db.Turmas.Add(Turma);
        await db.SaveChangesAsync();
        TempData["Mensagem"] = "Turma cadastrada.";
        return RedirectToPage("Index");
    }
}
