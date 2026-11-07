using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace PresencaPlus.Pages.Aulas;

public class EditarModel(AppDbContext db) : PageModel
{
    [BindProperty]
    public Aula Aula { get; set; } = new();

    public SelectList Turmas { get; private set; } = null!;
    public SelectList Professores { get; private set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var aula = await db.Aulas.FindAsync(id);
        if (aula is null) return NotFound();
        Aula = aula;
        await CarregarListasAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var aula = await db.Aulas.FindAsync(id);
        if (aula is null) return NotFound();

        // RN04: não pode haver duas aulas na mesma turma, data e horário.
        if (await db.Aulas.AnyAsync(a => a.Id != id && a.TurmaId == Aula.TurmaId && a.Data == Aula.Data && a.Horario == Aula.Horario))
            ModelState.AddModelError("", "Já existe uma aula nesta turma, data e horário.");

        // Aula com chamada registrada mantém a turma e não pode virar aula futura,
        // pois as faltas gravadas pertencem aos alunos daquela turma (RN05).
        if (aula.ChamadaRegistradaEm is not null && Aula.TurmaId != aula.TurmaId)
            ModelState.AddModelError("Aula.TurmaId", "Não é possível mudar a turma de uma aula com chamada registrada.");

        if (aula.ChamadaRegistradaEm is not null && !Aula.PodeRegistrarChamada(DateOnly.FromDateTime(DateTime.Today)))
            ModelState.AddModelError("Aula.Data", "Aula com chamada registrada não pode ter data futura.");

        if (Aula.TurmaId > 0 && !await db.Turmas.AnyAsync(t => t.Id == Aula.TurmaId))
            ModelState.AddModelError("Aula.TurmaId", "Selecione a turma.");

        if (Aula.ProfessorId > 0 && !await db.Usuarios.AnyAsync(u => u.Id == Aula.ProfessorId && u.Perfil == "Professor"))
            ModelState.AddModelError("Aula.ProfessorId", "Selecione o professor.");

        if (!ModelState.IsValid)
        {
            await CarregarListasAsync();
            return Page();
        }

        aula.TurmaId = Aula.TurmaId;
        aula.ProfessorId = Aula.ProfessorId;
        aula.Disciplina = Aula.Disciplina;
        aula.Data = Aula.Data;
        aula.Horario = Aula.Horario;
        await db.SaveChangesAsync();
        TempData["Mensagem"] = "Aula atualizada.";
        return RedirectToPage("Index");
    }

    private async Task CarregarListasAsync()
    {
        Turmas = new SelectList(await db.Turmas.OrderBy(t => t.Nome).ToListAsync(), "Id", "Nome");
        Professores = new SelectList(
            await db.Usuarios.Where(u => u.Perfil == "Professor").OrderBy(u => u.Nome).ToListAsync(), "Id", "Nome");
    }
}
