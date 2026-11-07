using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace PresencaPlus.Pages.Relatorio;

public class IndexModel(AppDbContext db, FrequenciaService frequencia) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public int? TurmaId { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateOnly? De { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateOnly? Ate { get; set; }

    public SelectList Turmas { get; private set; } = null!;
    public string NomeTurma { get; private set; } = "";
    public string? Erro { get; private set; }
    public List<LinhaFrequencia> Linhas { get; private set; } = [];

    public async Task OnGetAsync()
    {
        var hoje = DateOnly.FromDateTime(DateTime.Today);
        De ??= new DateOnly(hoje.Year, 1, 1);
        Ate ??= hoje;

        var turmas = await db.Turmas.OrderBy(t => t.Nome).ToListAsync();
        var turma = turmas.FirstOrDefault(t => t.Id == TurmaId) ?? turmas.FirstOrDefault();
        TurmaId = turma?.Id;
        NomeTurma = turma?.Nome ?? "";
        Turmas = new SelectList(turmas, "Id", "Nome", TurmaId);

        if (De > Ate)
        {
            Erro = "A data inicial não pode ser posterior à data final.";
            return;
        }

        if (turma is not null)
            Linhas = await frequencia.GerarRelatorioAsync(turma.Id, De.Value, Ate.Value);
    }
}
