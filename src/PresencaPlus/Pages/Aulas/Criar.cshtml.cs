using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace PresencaPlus.Pages.Aulas;

public class CriarModel(AppDbContext db) : PageModel
{
    [BindProperty]
    public Aula Aula { get; set; } = new()
    {
        Data = DateOnly.FromDateTime(DateTime.Today),
        Horario = new TimeOnly(7, 30)
    };

    public SelectList Turmas { get; private set; } = null!;
    public SelectList Professores { get; private set; } = null!;

    public async Task OnGetAsync() => await CarregarListasAsync();

    public async Task<IActionResult> OnPostAsync()
    {
        // RN04: não pode haver duas aulas na mesma turma, data e horário.
        if (await db.Aulas.AnyAsync(a => a.TurmaId == Aula.TurmaId && a.Data == Aula.Data && a.Horario == Aula.Horario))
            ModelState.AddModelError("", "Já existe uma aula nesta turma, data e horário.");

        if (Aula.TurmaId > 0 && !await db.Turmas.AnyAsync(t => t.Id == Aula.TurmaId))
            ModelState.AddModelError("Aula.TurmaId", "Selecione a turma.");

        if (Aula.ProfessorId > 0 && !await db.Usuarios.AnyAsync(u => u.Id == Aula.ProfessorId && u.Perfil == "Professor"))
            ModelState.AddModelError("Aula.ProfessorId", "Selecione o professor.");

        if (!ModelState.IsValid)
        {
            await CarregarListasAsync();
            return Page();
        }

        db.Aulas.Add(Aula);
        await db.SaveChangesAsync();
        TempData["Mensagem"] = "Aula cadastrada.";
        return RedirectToPage("Index");
    }

    private async Task CarregarListasAsync()
    {
        Turmas = new SelectList(await db.Turmas.OrderBy(t => t.Nome).ToListAsync(), "Id", "Nome");
        Professores = new SelectList(
            await db.Usuarios.Where(u => u.Perfil == "Professor").OrderBy(u => u.Nome).ToListAsync(), "Id", "Nome");
    }
}
