namespace PresencaPlus;

// Dados fictícios de demonstração (seção 8 do contrato), sem aleatoriedade.
public static class Seed
{
    private const int AulasPorTurma = 20;

    // Faltas de cada um dos 10 alunos da turma, em 20 aulas:
    // 6 faltas = 70% (Abaixo de 75%); 4 faltas = 80% (Atenção); as demais ficam em "Regular".
    private static readonly int[] FaltasPorAluno = [6, 4, 2, 1, 0, 3, 0, 1, 0, 2];

    private static readonly string[][] Nomes =
    [
        ["Ana Clara Souza", "Bruno Lima", "Camila Rocha", "Daniel Alves", "Eduarda Martins",
         "Felipe Costa", "Gabriela Nunes", "Henrique Dias", "Isabela Freitas", "João Pedro Ramos"],
        ["Karina Teixeira", "Lucas Barbosa", "Mariana Pires", "Nicolas Moreira", "Olívia Cardoso",
         "Paulo Vieira", "Rafaela Gomes", "Samuel Duarte", "Tainá Ribeiro", "Vitor Mendes"],
        ["Amanda Farias", "Breno Tavares", "Carolina Melo", "Diego Fonseca", "Elisa Campos",
         "Fábio Xavier", "Giovana Lopes", "Hugo Batista", "Júlia Azevedo", "Leonardo Prado"]
    ];

    public static void Executar(AppDbContext db)
    {
        if (db.Usuarios.Any()) return;

        var hoje = DateOnly.FromDateTime(DateTime.Today);

        var beatriz = NovoUsuario("Beatriz Andrade", "beatriz", "Coordenacao");
        var marina = NovoUsuario("Marina Oliveira", "marina", "Professor");
        var carlos = NovoUsuario("Carlos Souza", "carlos", "Professor");
        db.Usuarios.AddRange(beatriz, marina, carlos);

        Turma[] turmas =
        [
            new() { Nome = "7º ano A", AnoLetivo = hoje.Year, Turno = "Matutino" },
            new() { Nome = "8º ano B", AnoLetivo = hoje.Year, Turno = "Matutino" },
            new() { Nome = "9º ano A", AnoLetivo = hoje.Year, Turno = "Vespertino" }
        ];
        TimeOnly[] horarios = [new(7, 30), new(9, 30), new(13, 30)];

        var diasUteis = DiasUteisAnteriores(hoje).Take(AulasPorTurma).Reverse().ToList();

        for (var t = 0; t < turmas.Length; t++)
        {
            var turma = turmas[t];
            var horario = horarios[t];

            var alunos = Nomes[t]
                .Select((nome, i) => new Aluno { Matricula = $"2026{t + 1}{i + 1:D2}", Nome = nome, Turma = turma })
                .ToList();
            db.Alunos.AddRange(alunos);

            var aulas = diasUteis.Select((dia, i) => new Aula
            {
                Turma = turma,
                Professor = i % 2 == 0 ? marina : carlos,
                Disciplina = i % 2 == 0 ? "Língua Portuguesa" : "Matemática",
                Data = dia,
                Horario = horario,
                ChamadaRegistradaEm = dia.ToDateTime(horario).AddHours(1)
            }).ToList();
            db.Aulas.AddRange(aulas);

            for (var a = 0; a < alunos.Count; a++)
                for (var f = 0; f < FaltasPorAluno[a]; f++)
                    db.Faltas.Add(new Falta { Aluno = alunos[a], Aula = aulas[(f * 3 + a) % AulasPorTurma] });
        }

        // Aula de hoje com chamada pendente e aula futura, ambas de marina.
        db.Aulas.AddRange(
            new Aula { Turma = turmas[0], Professor = marina, Disciplina = "Língua Portuguesa", Data = hoje, Horario = new(10, 0) },
            new Aula { Turma = turmas[0], Professor = marina, Disciplina = "Língua Portuguesa", Data = hoje.AddDays(7), Horario = new(10, 0) });

        db.SaveChanges();
    }

    private static Usuario NovoUsuario(string nome, string login, string perfil)
    {
        var usuario = new Usuario { Nome = nome, Login = login, Perfil = perfil };
        usuario.DefinirSenha("demo1234");
        return usuario;
    }

    private static IEnumerable<DateOnly> DiasUteisAnteriores(DateOnly hoje)
    {
        for (var dia = hoje.AddDays(-1); ; dia = dia.AddDays(-1))
            if (dia.DayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday))
                yield return dia;
    }
}
