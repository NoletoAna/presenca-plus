using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace PresencaPlus.Pages.Aulas;

public class ExcluirModel(AppDbContext db) : PageModel
{
    public Aula Aula { get; private set; } = new();
    public string? Impedimento { get; private set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (!await CarregarAsync(id)) return NotFound();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!await CarregarAsync(id)) return NotFound();
        if (Impedimento is not null) return Page();

        db.Aulas.Remove(Aula);
        await db.SaveChangesAsync();
        TempData["Mensagem"] = "Aula excluída.";
        return RedirectToPage("Index");
    }

    private async Task<bool> CarregarAsync(int id)
    {
        var aula = await db.Aulas.Include(a => a.Turma).SingleOrDefaultAsync(a => a.Id == id);
        if (aula is null) return false;
        Aula = aula;

        // RN08: não se exclui aula com chamada registrada.
        if (aula.ChamadaRegistradaEm is not null)
            Impedimento = "Esta aula não pode ser excluída porque já tem chamada registrada.";
        return true;
    }
}
