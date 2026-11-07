# Mudanças em relação ao contrato

Nenhuma muda o escopo.

| Mudança | Motivo |
|---|---|
| Rota `/Sair`, que o contrato não lista | O plano pede "sair", e o logout precisa de uma rota |
| Rota `/` redireciona conforme o perfil | Evita 404 na raiz do site |
| Edição de aula com chamada registrada não muda a turma nem aceita data futura | Evita faltas de alunos de outra turma e chamada em aula futura (RN05) |
| `Aula.RegistrarChamada` limpa a coleção, e a página remove as faltas antigas com `RemoveRange` | Exigência do `DeleteBehavior.Restrict` |
| Solução em `.slnx` | Formato padrão do .NET 10 |
