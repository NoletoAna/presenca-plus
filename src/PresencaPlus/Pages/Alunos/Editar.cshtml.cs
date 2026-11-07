using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace PresencaPlus.Pages.Alunos;

public class EditarModel(AppDbContext db) : PageModel
{
    [BindProperty]
    public Aluno Aluno { get; set; } = new();

    public SelectList Turmas { get; private set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var aluno = await db.Alunos.FindAsync(id);
        if (aluno is null) return NotFound();
        Aluno = aluno;
        await CarregarTurmasAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var aluno = await db.Alunos.FindAsync(id);
        if (aluno is null) return NotFound();

        // RN03: matrícula do aluno é única.
        if (await db.Alunos.AnyAsync(a => a.Matricula == Aluno.Matricula && a.Id != id))
            ModelState.AddModelError("Aluno.Matricula", "Já existe um aluno com esta matrícula.");

        if (Aluno.TurmaId > 0 && !await db.Turmas.AnyAsync(t => t.Id == Aluno.TurmaId))
            ModelState.AddModelError("Aluno.TurmaId", "Selecione a turma.");

        if (!ModelState.IsValid)
        {
            await CarregarTurmasAsync();
            return Page();
        }

        aluno.Matricula = Aluno.Matricula;
        aluno.Nome = Aluno.Nome;
        aluno.TurmaId = Aluno.TurmaId;
        await db.SaveChangesAsync();
        TempData["Mensagem"] = "Aluno atualizado.";
        return RedirectToPage("Index");
    }

    private async Task CarregarTurmasAsync() =>
        Turmas = new SelectList(await db.Turmas.OrderBy(t => t.Nome).ToListAsync(), "Id", "Nome");
}
