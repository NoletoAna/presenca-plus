using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace PresencaPlus.Pages.Turmas;

public class ExcluirModel(AppDbContext db) : PageModel
{
    public Turma Turma { get; private set; } = new();
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

        db.Turmas.Remove(Turma);
        await db.SaveChangesAsync();
        TempData["Mensagem"] = "Turma excluída.";
        return RedirectToPage("Index");
    }

    private async Task<bool> CarregarAsync(int id)
    {
        var turma = await db.Turmas.FindAsync(id);
        if (turma is null) return false;
        Turma = turma;

        // RN08: não se exclui turma com alunos ou aulas.
        var alunos = await db.Alunos.CountAsync(a => a.TurmaId == id);
        var aulas = await db.Aulas.CountAsync(a => a.TurmaId == id);
        if (alunos > 0 || aulas > 0)
            Impedimento = $"Esta turma não pode ser excluída porque tem {alunos} aluno(s) e {aulas} aula(s) vinculados.";
        return true;
    }
}
