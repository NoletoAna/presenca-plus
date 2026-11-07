using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace PresencaPlus;

public class Aula
{
    public int Id { get; set; }

    [Display(Name = "Turma")]
    [Range(1, int.MaxValue, ErrorMessage = "Selecione a turma.")]
    public int TurmaId { get; set; }

    [Display(Name = "Professor")]
    [Range(1, int.MaxValue, ErrorMessage = "Selecione o professor.")]
    public int ProfessorId { get; set; }

    [Required(ErrorMessage = "Informe a disciplina.")]
    [StringLength(50, ErrorMessage = "A disciplina deve ter até 50 caracteres.")]
    public string Disciplina { get; set; } = "";

    [Required(ErrorMessage = "Informe a data.")]
    public DateOnly Data { get; set; }

    [Display(Name = "Horário")]
    [Required(ErrorMessage = "Informe o horário.")]
    public TimeOnly Horario { get; set; }

    public DateTime? ChamadaRegistradaEm { get; set; }

    [ValidateNever]
    public Turma Turma { get; set; } = null!;

    [ValidateNever]
    public Usuario Professor { get; set; } = null!;

    public List<Falta> Faltas { get; set; } = [];

    // RN05: chamada só em aula com data igual ou anterior a hoje.
    public bool PodeRegistrarChamada(DateOnly hoje) => Data <= hoje;

    // RN06: salvar de novo substitui as faltas anteriores da aula.
    public void RegistrarChamada(IEnumerable<int> alunosFaltantes, DateTime agora)
    {
        if (!PodeRegistrarChamada(DateOnly.FromDateTime(agora)))
            throw new InvalidOperationException("A chamada só pode ser registrada em aula de hoje ou de data anterior.");

        Faltas.Clear();
        foreach (var alunoId in alunosFaltantes.Distinct())
            Faltas.Add(new Falta { AulaId = Id, AlunoId = alunoId });

        ChamadaRegistradaEm = agora;
    }
}
