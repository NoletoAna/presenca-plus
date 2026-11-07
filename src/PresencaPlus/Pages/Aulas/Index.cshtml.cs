using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace PresencaPlus.Pages.Aulas;

public class IndexModel(AppDbContext db) : PageModel
{
    public List<Aula> Aulas { get; private set; } = [];

    public async Task OnGetAsync()
    {
        Aulas = await db.Aulas
            .Include(a => a.Turma)
            .Include(a => a.Professor)
            .OrderByDescending(a => a.Data).ThenBy(a => a.Horario)
            .ToListAsync();
    }
}
