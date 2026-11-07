using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace PresencaPlus.Pages.Turmas;

public class IndexModel(AppDbContext db) : PageModel
{
    public record Linha(Turma Turma, int Alunos, int Aulas);

    public List<Linha> Linhas { get; private set; } = [];

    public async Task OnGetAsync()
    {
        Linhas = await db.Turmas
            .OrderBy(t => t.Nome)
            .Select(t => new Linha(t, t.Alunos.Count, t.Aulas.Count))
            .ToListAsync();
    }
}
