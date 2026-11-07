using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace PresencaPlus.Pages.Chamada;

public class IndexModel(AppDbContext db) : PageModel
{
    public List<Aula> Aulas { get; private set; } = [];

    public async Task OnGetAsync()
    {
        // RN07: o professor só vê as próprias aulas.
        var professorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        Aulas = await db.Aulas
            .Include(a => a.Turma)
            .Where(a => a.ProfessorId == professorId)
            .OrderByDescending(a => a.Data).ThenBy(a => a.Horario)
            .ToListAsync();
    }

    public string Situacao(Aula aula)
    {
        if (aula.ChamadaRegistradaEm is not null) return "Registrada";
        return aula.PodeRegistrarChamada(DateOnly.FromDateTime(DateTime.Today)) ? "Pendente" : "Futura";
    }
}
