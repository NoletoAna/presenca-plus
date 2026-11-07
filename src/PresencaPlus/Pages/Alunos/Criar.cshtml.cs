using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace PresencaPlus.Pages.Alunos;

public class CriarModel(AppDbContext db) : PageModel
{
    [BindProperty]
    public Aluno Aluno { get; set; } = new();

    public SelectList Turmas { get; private set; } = null!;

    public async Task OnGetAsync() => await CarregarTurmasAsync();

    public async Task<IActionResult> OnPostAsync()
    {
        // RN03: matrícula do aluno é única.
        if (await db.Alunos.AnyAsync(a => a.Matricula == Aluno.Matricula))
            ModelState.AddModelError("Aluno.Matricula", "Já existe um aluno com esta matrícula.");

        if (Aluno.TurmaId > 0 && !await db.Turmas.AnyAsync(t => t.Id == Aluno.TurmaId))
            ModelState.AddModelError("Aluno.TurmaId", "Selecione a turma.");

        if (!ModelState.IsValid)
        {
            await CarregarTurmasAsync();
            return Page();
        }

        db.Alunos.Add(Aluno);
        await db.SaveChangesAsync();
        TempData["Mensagem"] = "Aluno cadastrado.";
        return RedirectToPage("Index");
    }

    private async Task CarregarTurmasAsync() =>
        Turmas = new SelectList(await db.Turmas.OrderBy(t => t.Nome).ToListAsync(), "Id", "Nome");
}
