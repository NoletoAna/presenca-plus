using Microsoft.EntityFrameworkCore;

namespace PresencaPlus;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Turma> Turmas => Set<Turma>();
    public DbSet<Aluno> Alunos => Set<Aluno>();
    public DbSet<Aula> Aulas => Set<Aula>();
    public DbSet<Falta> Faltas => Set<Falta>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Usuario>(e =>
        {
            e.Property(u => u.Nome).HasMaxLength(100);
            e.Property(u => u.Login).HasMaxLength(50);
            e.Property(u => u.SenhaHash).HasMaxLength(200);
            e.Property(u => u.Perfil).HasMaxLength(15);
            e.HasIndex(u => u.Login).IsUnique();
        });

        b.Entity<Turma>(e =>
        {
            e.HasIndex(t => t.Nome).IsUnique();
        });

        b.Entity<Aluno>(e =>
        {
            e.HasIndex(a => a.Matricula).IsUnique();
            e.HasOne(a => a.Turma).WithMany(t => t.Alunos)
                .HasForeignKey(a => a.TurmaId).OnDelete(DeleteBehavior.Restrict);
        });

        b.Entity<Aula>(e =>
        {
            e.HasIndex(a => new { a.TurmaId, a.Data, a.Horario }).IsUnique();
            e.HasOne(a => a.Turma).WithMany(t => t.Aulas)
                .HasForeignKey(a => a.TurmaId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(a => a.Professor).WithMany()
                .HasForeignKey(a => a.ProfessorId).OnDelete(DeleteBehavior.Restrict);
        });

        b.Entity<Falta>(e =>
        {
            e.HasIndex(f => new { f.AulaId, f.AlunoId }).IsUnique();
            e.HasOne(f => f.Aula).WithMany(a => a.Faltas)
                .HasForeignKey(f => f.AulaId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(f => f.Aluno).WithMany(a => a.Faltas)
                .HasForeignKey(f => f.AlunoId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
