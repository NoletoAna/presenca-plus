# Apresentação do tema

## Sistema escolhido

O sistema escolhido é o Presença+, um sistema web para uma escola controlar as faltas dos alunos. A coordenação cadastra turmas, alunos e aulas. O professor faz a chamada de cada aula. O relatório mostra a frequência de cada aluno e destaca quem está abaixo do mínimo exigido.

## Motivo da escolha

O controle de frequência é uma obrigação da escola. A Lei de Diretrizes e Bases da Educação Nacional estabelece que esse controle fica a cargo da escola e exige frequência mínima de 75% do total de horas letivas para aprovação (BRASIL, 1996, art. 24, VI). Um sistema que calcula esse percentual permite identificar a tempo os alunos em risco de reprovação por falta.

A ideia do sistema foi retomada de um plano de negócios elaborado anteriormente pela autora. Esse plano partiu de um problema: muitas escolas ainda fazem a chamada em diário de papel ou em planilhas soltas, o que gera retrabalho e erros, e a coordenação só percebe o excesso de faltas de um aluno quando já é tarde. Para este projeto, a ideia foi reduzida às funções essenciais.

## Levantamento de informações

O levantamento foi feito por meio de uma conversa com um assistente de coordenação de uma escola de ensino fundamental, que atende os anos iniciais e finais. A escola não usava nenhum sistema para o controle de frequência. O trabalho era feito em planilhas do Excel e por envio de e-mail. Com base nisso, definiu-se que o sistema deve reunir em um só lugar os cadastros, a chamada e o cálculo da frequência.

## Validação do tema

O tema atende aos três critérios de aprovação, conforme a Tabela X.

Tabela X — Critérios de validação do tema

| Critério | Como o sistema atende |
|---|---|
| Pelo menos 3 funcionalidades distintas | Cadastrar alunos, registrar a chamada e emitir o relatório de frequência |
| Pelo menos 1 consulta a banco de dados | O relatório consulta aulas e faltas gravadas no banco para calcular a frequência |
| Viável de prototipar no período letivo | Escopo enxuto: uma escola, dois perfis e cinco tabelas, sem pagamentos nem integração com outros sistemas |

Fonte: elaborado pelo autor (2026).

Ficam fora do escopo: atendimento a várias escolas, cobrança, cadastro de usuários pela tela, log de auditoria, importação de planilha, integração com outros sistemas e aviso aos responsáveis.
