using Microsoft.EntityFrameworkCore;

namespace PresencaPlus;

public class FrequenciaService(AppDbContext db)
{
    public async Task<List<LinhaFrequencia>> GerarRelatorioAsync(int turmaId, DateOnly de, DateOnly ate)
    {
        var aulas = await db.Aulas.CountAsync(a => a.TurmaId == turmaId
            && a.ChamadaRegistradaEm != null && a.Data >= de && a.Data <= ate);

        var alunos = await db.Alunos
            .Where(a => a.TurmaId == turmaId)
            .OrderBy(a => a.Nome)
            .Select(a => new
            {
                Aluno = a,
                Faltas = a.Faltas.Count(f => f.Aula.TurmaId == turmaId
                    && f.Aula.ChamadaRegistradaEm != null
                    && f.Aula.Data >= de && f.Aula.Data <= ate)
            })
            .ToListAsync();

        return alunos.Select(x => new LinhaFrequencia(x.Aluno, aulas, x.Faltas)).ToList();
    }
}
