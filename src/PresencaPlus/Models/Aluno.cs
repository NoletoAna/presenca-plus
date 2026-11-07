using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace PresencaPlus;

public class Aluno
{
    public int Id { get; set; }

    [Display(Name = "Matrícula")]
    [Required(ErrorMessage = "Informe a matrícula.")]
    [StringLength(10, ErrorMessage = "A matrícula deve ter até 10 caracteres.")]
    public string Matricula { get; set; } = "";

    [Required(ErrorMessage = "Informe o nome do aluno.")]
    [StringLength(100, ErrorMessage = "O nome deve ter até 100 caracteres.")]
    public string Nome { get; set; } = "";

    [Display(Name = "Turma")]
    [Range(1, int.MaxValue, ErrorMessage = "Selecione a turma.")]
    public int TurmaId { get; set; }

    [ValidateNever]
    public Turma Turma { get; set; } = null!;

    public List<Falta> Faltas { get; set; } = [];
}
