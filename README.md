# Presença+

Sistema web para uma escola controlar as faltas dos alunos. Projeto Integrado do curso de ADS.

A coordenação cadastra turmas, alunos e aulas. O professor faz a chamada de cada aula. O relatório mostra a frequência de cada aluno e destaca quem está abaixo do mínimo de 75%.

## Tecnologias

C#, .NET 10, ASP.NET Core Razor Pages, Entity Framework Core e SQLite.

## Como executar

Requisito: SDK do .NET 10.

```powershell
dotnet run --project src/PresencaPlus --urls http://localhost:5100
```

Depois, abra `http://localhost:5100`. Na primeira execução, o banco `presenca.db` é criado em `src/PresencaPlus` com dados de demonstração fictícios.

## Usuários de demonstração

A senha de todos é `demo1234`.

| Login | Perfil |
|---|---|
| `beatriz` | Coordenação |
| `marina` | Professor |
| `carlos` | Professor |
