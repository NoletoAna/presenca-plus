using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace PresencaPlus.Pages.Turmas;

public class EditarModel(AppDbContext db) : PageModel
{
    [BindProperty]
    public Turma Turma { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var turma = await db.Turmas.FindAsync(id);
        if (turma is null) return NotFound();
        Turma = turma;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var turma = await db.Turmas.FindAsync(id);
        if (turma is null) return NotFound();

        // RN03: nome da turma é único.
        if (await db.Turmas.AnyAsync(t => t.Nome == Turma.Nome && t.Id != id))
            ModelState.AddModelError("Turma.Nome", "Já existe uma turma com este nome.");

        if (!ModelState.IsValid) return Page();

        turma.Nome = Turma.Nome;
        turma.AnoLetivo = Turma.AnoLetivo;
        turma.Turno = Turma.Turno;
        await db.SaveChangesAsync();
        TempData["Mensagem"] = "Turma atualizada.";
        return RedirectToPage("Index");
    }
}
