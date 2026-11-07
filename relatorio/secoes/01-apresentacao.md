# Apresentação do tema

## Sistema escolhido

O sistema escolhido é o Presença+, um sistema web para uma escola controlar as faltas dos alunos. A coordenação cadastra turmas, alunos e aulas. O professor faz a chamada de cada aula. O relatório mostra a frequência de cada aluno e destaca quem está abaixo do mínimo exigido.

## Motivo da escolha

O controle de frequência é uma obrigação da escola. A Lei de Diretrizes e Bases da Educação Nacional estabelece que esse controle fica a cargo da escola e exige frequência mínima de 75% do total de horas letivas para aprovação (BRASIL, 1996, art. 24, VI). Um sistema que calcula esse percentual permite identificar a tempo os alunos em risco de reprovação por falta.

[PENDENTE: motivo pessoal da escolha do tema, a ser informado pela autora.]

## Levantamento de informações

[PENDENTE: técnica usada, quando, com quem ou em quais sistemas, como o processo é feito hoje e o que foi descoberto. O arquivo `relatorio/insumos/levantamento.md` está em branco, e este conteúdo depende do levantamento real exigido no Passo 0.]

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
