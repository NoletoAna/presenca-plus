using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace PresencaPlus.Pages.Alunos;

public class IndexModel(AppDbContext db) : PageModel
{
    public List<Aluno> Alunos { get; private set; } = [];
    public SelectList Turmas { get; private set; } = null!;

    public async Task OnGetAsync(int? turmaId)
    {
        var consulta = db.Alunos.Include(a => a.Turma).AsQueryable();
        if (turmaId is not null)
            consulta = consulta.Where(a => a.TurmaId == turmaId);

        Alunos = await consulta.OrderBy(a => a.Turma.Nome).ThenBy(a => a.Nome).ToListAsync();
        Turmas = new SelectList(await db.Turmas.OrderBy(t => t.Nome).ToListAsync(), "Id", "Nome", turmaId);
    }
}
