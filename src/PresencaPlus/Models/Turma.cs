using System.ComponentModel.DataAnnotations;

namespace PresencaPlus;

public class Turma
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o nome da turma.")]
    [StringLength(30, ErrorMessage = "O nome deve ter até 30 caracteres.")]
    public string Nome { get; set; } = "";

    [Display(Name = "Ano letivo")]
    [Range(2000, 2100, ErrorMessage = "Informe um ano letivo entre 2000 e 2100.")]
    public int AnoLetivo { get; set; }

    [Required(ErrorMessage = "Selecione o turno.")]
    [AllowedValues("Matutino", "Vespertino", "Noturno", ErrorMessage = "Selecione um turno válido.")]
    [StringLength(10)]
    public string Turno { get; set; } = "";

    public List<Aluno> Alunos { get; set; } = [];
    public List<Aula> Aulas { get; set; } = [];
}
