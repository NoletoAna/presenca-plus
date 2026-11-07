using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace PresencaPlus.Pages.Chamada;

public class RegistrarModel(AppDbContext db) : PageModel
{
    public Aula Aula { get; private set; } = new();
    public List<Aluno> Alunos { get; private set; } = [];
    public string? Impedimento { get; private set; }

    [BindProperty]
    public List<int> Faltantes { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var resultado = await CarregarAsync(id);
        if (resultado is not null) return resultado;

        Faltantes = Aula.Faltas.Select(f => f.AlunoId).ToList();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var resultado = await CarregarAsync(id);
        if (resultado is not null) return resultado;
        if (Impedimento is not null) return Page();

        // Só valem alunos da turma da aula, mesmo que o formulário seja alterado.
        var idsDaTurma = Alunos.Select(a => a.Id).ToHashSet();

        // Com DeleteBehavior.Restrict, as faltas antigas precisam ser removidas de forma explícita.
        db.Faltas.RemoveRange(Aula.Faltas);
        Aula.RegistrarChamada(Faltantes.Where(idsDaTurma.Contains), DateTime.Now);
        await db.SaveChangesAsync();

        TempData["Mensagem"] = "Chamada registrada.";
        return RedirectToPage("Index");
    }

    private async Task<IActionResult?> CarregarAsync(int id)
    {
        var aula = await db.Aulas
            .Include(a => a.Turma)
            .Include(a => a.Faltas)
            .SingleOrDefaultAsync(a => a.Id == id);
        if (aula is null) return NotFound();

        // RN07: o professor só registra as próprias aulas, inclusive por acesso direto à URL.
        var professorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (aula.ProfessorId != professorId) return Forbid();

        Aula = aula;
        Alunos = await db.Alunos.Where(a => a.TurmaId == aula.TurmaId).OrderBy(a => a.Nome).ToListAsync();

        // RN05: chamada só em aula com data igual ou anterior a hoje.
        if (!aula.PodeRegistrarChamada(DateOnly.FromDateTime(DateTime.Today)))
            Impedimento = "A chamada desta aula ainda não pode ser registrada, pois a data é futura.";

        return null;
    }
}
