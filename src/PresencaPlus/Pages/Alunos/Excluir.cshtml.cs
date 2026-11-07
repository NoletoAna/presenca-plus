using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace PresencaPlus.Pages.Alunos;

public class ExcluirModel(AppDbContext db) : PageModel
{
    public Aluno Aluno { get; private set; } = new();
    public string? Impedimento { get; private set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (!await CarregarAsync(id)) return NotFound();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!await CarregarAsync(id)) return NotFound();
        if (Impedimento is not null) return Page();

        db.Alunos.Remove(Aluno);
        await db.SaveChangesAsync();
        TempData["Mensagem"] = "Aluno excluído.";
        return RedirectToPage("Index");
    }

    private async Task<bool> CarregarAsync(int id)
    {
        var aluno = await db.Alunos.FindAsync(id);
        if (aluno is null) return false;
        Aluno = aluno;

        // RN08: não se exclui aluno com faltas.
        var faltas = await db.Faltas.CountAsync(f => f.AlunoId == id);
        if (faltas > 0)
            Impedimento = $"Este aluno não pode ser excluído porque tem {faltas} falta(s) registrada(s).";
        return true;
    }
}
