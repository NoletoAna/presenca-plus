namespace PresencaPlus;

public class Falta
{
    public int Id { get; set; }
    public int AulaId { get; set; }
    public int AlunoId { get; set; }
    public Aula Aula { get; set; } = null!;
    public Aluno Aluno { get; set; } = null!;
}
