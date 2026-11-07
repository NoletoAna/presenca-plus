# Plano de execução — Projeto Integrado (ADS)

**Tema:** Presença+, sistema web de controle de faltas, reduzido a um CRUD simples.
**Stack:** C# / .NET 10, Razor Pages, EF Core e SQLite.
**Entrega:** um único arquivo Word ou PDF de até 10 MB, postado no AVA.

O que o sistema faz, com requisitos, modelo, rotas e casos de teste, está em [docs/CONTRATO.md](docs/CONTRATO.md). Este arquivo diz **quem faz o quê e em que ordem**.

## Ordem de execução

```text
Etapa 0  Ana: preparação
            │
Etapa 1  J1: fundação do código
            │
Etapa 2  J2 ─ J3 ─ J4 ─ J5 ─ J6   ← 5 janelas ao mesmo tempo
            │
Etapa 3  J7: integração, testes e evidências
            │
Etapa 4  J8: montagem do relatório
            │
Etapa 5  Ana: revisão e postagem
```

Só a Etapa 2 é paralela. J1 vem antes porque cria o código e o Git de que as outras dependem. J7 e J8 vêm depois porque precisam do que as outras entregaram.

| Janela | Trabalho | Precisa de | Escreve somente em |
|---|---|---|---|
| J1 | Fundação do código | Etapa 0 | raiz, `src/PresencaPlus/` (exceto as pastas de J5 e J6) |
| J2 | Textos: tema, requisitos, modelagem, referências | J1 | `relatorio/secoes/01`, `02`, `03`, `08` |
| J3 | Diagramas UML e MER | J1 | `relatorio/diagramas/`, `relatorio/img/{uc,classes,mer,sequencia}.png` |
| J4 | Protótipos das telas | J1 | `relatorio/prototipos/`, `relatorio/img/tela-*.png`, `relatorio/secoes/04` |
| J5 | Cadastros: turmas, alunos, aulas | J1 | `Pages/Turmas`, `Pages/Alunos`, `Pages/Aulas` |
| J6 | Chamada e relatório | J1 | `Pages/Chamada`, `Pages/Relatorio` |
| J7 | Integração, testes, Fases 4 e 5 | J2 a J6 | `relatorio/secoes/05`, `06`, `08`, `relatorio/img/exec-*`, `relatorio/img/ct*`, correções em `src/` |
| J8 | Relatório final | J7 e tarefas da Ana | `relatorio/entrega/`, `relatorio/secoes/08` |

## Como abrir cada janela

Abra um terminal novo nesta pasta (exceto J5 e J6, que usam worktree, como explicado abaixo), inicie o Claude Code e cole, trocando `Jn`:

```text
Leia PLANO.md e docs/CONTRATO.md. Execute a janela Jn. Antes de começar, confira em status/ se os pré-requisitos estão concluídos. Escreva apenas nos caminhos dessa janela. Ao terminar, crie status/Jn.md.
```

## Regras para todas as janelas

1. **Contrato congelado.** Use os nomes de `docs/CONTRATO.md`. Se algo precisar mudar, registre em `status/mudancas-contrato.md`.
2. **Respeite os caminhos da sua janela.** É isso que permite trabalhar em paralelo sem conflito.
3. **Status.** Ao terminar, crie `status/Jn.md` com: o que foi entregue, o que foi verificado, desvios e pendências.
4. **Commits.** Adicione só os seus caminhos (`git add <caminhos>`) e faça commit com mensagem curta em português. Se aparecer erro de `index.lock`, espere alguns segundos e tente de novo.
5. **Nada inventado.** Não crie entrevista, observação, citação, página de livro, resultado de teste ou print. Se faltar informação, escreva `[PENDENTE: o que falta]` no texto e cite em `status/Jn.md`.
6. **Simplicidade.** É um CRUD de projeto de faculdade. Não adicione camadas, bibliotecas ou recursos fora do contrato.
7. **Execução local.** Cada janela de código usa sua própria porta e seu próprio banco (`presenca.db`, fora do Git): J1 e J7 `http://localhost:5100`, J5 `5101`, J6 `5102`. Nunca rode dois `dotnet build` na mesma pasta ao mesmo tempo.
8. **Navegador.** Use o Playwright. Se acusar navegador em uso, outra janela está com ele: espere e tente de novo.
9. **Sem publicar nada.** Não crie repositório remoto nem faça push sem a Ana confirmar.

### Estilo dos textos

- Português formal e impessoal, com frases curtas e diretas, como em `C:\Users\anano\Desktop\Projetos Práticos Unic\Computação em Nuvem\Relatório de Aula Prática - Computação em Nuvem.docx`.
- Legendas: `Figura N — Título` e, abaixo, `Fonte: elaborado pelo autor (2026).` Tabelas: `Tabela N — Título`. Nos Markdown, use `Figura X`; a J8 numera.
- Citações no padrão ABNT (AUTOR, ano), sempre com a referência correspondente.
- Cabeçalho do plano de negócios (UFMT, Atitude Empreendedora) não entra no relatório. Só a ideia do sistema é reaproveitada.

---

## Etapa 0 — Ana: preparação

Faça antes de abrir as janelas. Nenhuma IA consegue fazer estes itens por você.

- [ ] **A1.** Baixar do AVA o arquivo **Modelo de entrega Projeto Integrado** e salvar como `relatorio/insumos/modelo-entrega.docx`. O PDF exige esse modelo, e ele não está nesta pasta.
- [ ] **A2.** Fazer o levantamento real exigido no Passo 0 e registrar em `relatorio/insumos/levantamento.md`: qual técnica foi usada (observação, conversa, pesquisa de sistemas parecidos ou experiência própria), quando, com quem ou em quais sistemas, e o que descobriu. Precisa ser verdadeiro. Conversar com alguém de uma escola e olhar dois sistemas parecidos já basta.
- [ ] **A3.** Conferir a regra de uso de IA na seção seguinte.
- [ ] **A4.** Para publicar no GitHub: rodar `! gh auth login` no prompt do Claude Code. Hoje o `gh` não está autenticado.

### Regra de IA do PDF (item "e" das normas)

> "Este trabalho interdisciplinar deve ser realizado integralmente pelo estudante, sem a utilização de ferramentas de Inteligência Artificial para substituir o processo de pesquisa, análise e escrita."

Este plano faz as janelas produzirem análise, código e texto, o que pode entrar em conflito com essa regra. Vale perguntar ao mediador pedagógico o que é aceito antes de postar. Reprovação é possível se o trabalho for invalidado, e o projeto é obrigatório. Além disso, os itens abaixo dependem da sua experiência real e não devem ser delegados:

- o levantamento (A2);
- as considerações finais, com o que você aprendeu e as dificuldades que teve;
- a revisão de todo o conteúdo, para conseguir explicar qualquer parte.

---

## Etapa 1 — Fundação

### J1 — Base do código

1. `git init` na raiz, com branch `main`. Criar `.gitignore` para .NET (`bin/`, `obj/`, `*.db*`, `.worktrees/`, `.playwright-mcp/`) e `.gitattributes` com `* text=auto`. O PDF original entra no repositório só se a Ana pedir; por padrão, coloque `Projeto Integrado.pdf` no `.gitignore`.
2. Criar solução e projeto em `src/PresencaPlus` com `dotnet new webapp`, alvo `net10.0`.
3. Adicionar os pacotes EF Core Sqlite e Design. Instalar a ferramenta `dotnet-ef` se não existir.
4. Criar `Models/`, `Data/AppDbContext.cs`, `Data/Seed.cs` e `Services/FrequenciaService.cs`, conforme as seções 5, 6 e 8 do contrato. Implementar por completo `FrequenciaService`, `LinhaFrequencia`, `Aula.RegistrarChamada` e `Aula.PodeRegistrarChamada`.
5. Criar a migration inicial e aplicá-la na inicialização (`Database.Migrate()` seguido do seed).
6. Em `Program.cs`, configurar cookie, políticas `Coordenacao` e `Professor`, e autorização por pasta: Turmas, Alunos e Aulas para Coordenação; Chamada para Professor; Relatorio para qualquer usuário autenticado.
7. Criar as páginas `/Login`, sair e `/AcessoNegado`. Em `_Layout`, montar menu por perfil já com os links finais (podem dar 404 até a J7 integrar). Remover as páginas de exemplo do template.
8. Definir `pt-BR` como cultura e criar `README.md` com instruções de execução e usuários de demonstração.
9. Verificar: `dotnet build` sem erro, e login, logout e bloqueio de acesso funcionando no navegador.
10. Commit. Criar os worktrees:

```powershell
git worktree add .worktrees/cadastros -b feat/cadastros
git worktree add .worktrees/chamada -b feat/chamada
```

11. Criar `status/J1.md`.

**Pronto quando:** o app sobe, o login funciona, o banco tem os dados da seção 8 e os worktrees existem.

---

## Etapa 2 — Paralelo

### J2 — Textos de análise

Trabalha na pasta principal.

1. `01-apresentacao.md`: sistema escolhido, motivo, como foi levantado (somente com base em `insumos/levantamento.md`) e os três critérios de validação do tema. Se o arquivo estiver vazio, deixe `[PENDENTE]`.
2. `02-fase1-requisitos.md`: RF, RNF, regras de negócio, referência à figura `uc.png` e descrição simplificada de UC01 a UC06 (ator, objetivo, passos principais).
3. `03-fase2-modelagem.md`: explicação de classes, MER e sequência, referência às figuras, e uma tabela de dicionário de dados por entidade (campo, tipo, descrição, obrigatório).
4. `08-referencias.md`: lista inicial em ABNT. Candidatas a conferir: Sommerville, Pressman e Maxim, Booch et al. (UML), Elmasri e Navathe, Lei n.º 9.394/1996, Lei n.º 13.709/2018, documentação do .NET e do EF Core. Inclua somente o que for citado no texto e tenha dados confirmados. Deixe `[PENDENTE]` para os materiais de aula que a Ana informar.

**Pronto quando:** todos os RF, RNF, UC e campos do contrato aparecem nos textos.

### J3 — Diagramas

Trabalha na pasta principal. Consulte também o código real em `src/PresencaPlus/Models`.

Produza quatro diagramas, cada um como arquivo HTML com SVG escrito à mão em `relatorio/diagramas/`, exportando para PNG com o Playwright em boa resolução:

| Diagrama | Conteúdo mínimo | PDF exige |
|---|---|---|
| `uc.png` | 2 atores, UC01 a UC06, include e fronteira do sistema | 3 casos de uso |
| `classes.png` | Classes da seção 6, com atributos, métodos e multiplicidades | 4 classes |
| `mer.png` | 5 entidades da seção 5, com PK, FK e cardinalidades | 3 entidades |
| `sequencia.png` | UC05: Professor, página `Registrar`, `AppDbContext`, `Aula`, banco. Inclui a validação RN05 | 1 diagrama |

Motivo do SVG: não exige instalar nada. A geração com PlantUML foi testada ao planejar e o download foi bloqueado pelo ambiente. Se a Ana autorizar, PlantUML ou draw.io também servem.

**Pronto quando:** os 4 PNGs estão legíveis em largura de página A4 e os nomes conferem com o contrato.

### J4 — Protótipos

Trabalha na pasta principal.

1. Criar wireframes simples em HTML estático em `relatorio/prototipos/`, em tons de cinza, com campos e botões. Podem seguir o layout do protótipo Presença+, sem copiá-lo.
2. Fazer as cinco telas marcadas na seção 7 do contrato: Login, Alunos, Minhas aulas (`tela-aulas.png`), Registrar chamada e Relatório.
3. Exportar para PNG com o Playwright.
4. Escrever `04-fase3-prototipagem.md`, com a figura e 1 ou 2 frases por tela, explicando para que serve e como o usuário a utiliza.

**Pronto quando:** existem 5 telas (o PDF pede 4), cada uma com descrição e coerente com os RF.

### J5 — Cadastros

Abra o terminal em `.worktrees/cadastros`. Porta 5101.

1. CRUD de Turmas, Alunos e Aulas, nas rotas da seção 7.
2. Validação em português e tratamento de RN03, RN04 e RN08, com mensagem clara em vez de erro 500.
3. Filtro de alunos por turma. Em Aulas, selects de turma e de professor vindos do banco.
4. Verificar no navegador criar, editar, excluir e os erros de CT04 a CT07.
5. Commit na branch `feat/cadastros`. Não faça merge.

**Pronto quando:** os três CRUDs funcionam e nenhum caminho de erro mostra exceção.

### J6 — Chamada e relatório

Abra o terminal em `.worktrees/chamada`. Porta 5102.

1. `/Chamada`: aulas do professor logado, com situação (pendente, registrada, futura), conforme RN07.
2. `/Chamada/Registrar/{id}`: lista da turma, todos começando presentes, e botão salvar. Usa `Aula.RegistrarChamada` e respeita RN05, RN06 e RN07, incluindo acesso direto pela URL à aula de outro professor.
3. `/Relatorio`: filtros de turma, data inicial e final. Tabela com aluno, matrícula, aulas, faltas, frequência e situação, destacando quem está abaixo de 75%. Botão de imprimir.
4. Verificar no navegador os cenários de CT08 a CT10.
5. Commit na branch `feat/chamada`. Não faça merge.

**Pronto quando:** salvar e reabrir a chamada preserva as faltas, e o relatório bate com um cálculo manual.

---

## Etapa 3 — Integração e testes

### J7 — Integração, testes e Fases 4 e 5

Só comece se `status/J2.md` a `status/J6.md` existirem.

1. Na pasta principal, fazer merge de `feat/cadastros` e `feat/chamada` em `main`. Remover os worktrees.
2. Ler `status/mudancas-contrato.md`, se existir, e ajustar `relatorio/secoes/` e diagramas afetados. Pedir à Ana para decidir se a mudança for de escopo.
3. Rodar com banco novo, na porta 5100.
4. Executar **CT01 a CT11** de verdade, salvando um print por caso (`img/ct01.png` em diante). Em caso de falha: registre, corrija, execute de novo e conte isso no texto. Não apague a falha.
5. Tirar os prints das funcionalidades em execução (`img/exec-*.png`).
6. Escrever `05-fase4-desenvolvimento.md`: ambiente, banco, as duas funcionalidades mais o relatório, trechos reais do código como texto (nunca imagem) e o link do repositório.
7. Escrever `06-fase5-testes.md`: tabela com ID, o que foi testado, resultado esperado e resultado obtido, seguida das evidências.
8. Perguntar à Ana se pode publicar. Com a confirmação, criar o repositório no GitHub e fazer push. Sem ela, deixar `[PENDENTE: link do GitHub]`.

**Pronto quando:** `main` compila, todos os CTs têm resultado real e print, e as Fases 4 e 5 estão escritas.

---

## Etapa 4 — Montagem

### J8 — Relatório final

Só comece se `status/J7.md` existir e `secoes/07-consideracoes-finais.md` tiver sido escrita pela Ana. Se esse arquivo não existir, avise e pare.

1. Usar a skill de docx. A máquina tem Node e Word, mas não tem Python, pandoc ou LibreOffice.
2. Montar sobre `insumos/modelo-entrega.docx`, mantendo capa e formatação do modelo. Se ele não existir, pare e peça à Ana. Só use a formatação do relatório de Computação em Nuvem como alternativa se ela autorizar. Essa formatação é: A4, margens 3 cm (superior e esquerda) e 2 cm (inferior e direita), Times New Roman 12, código em Courier New e número de página no cabeçalho à direita.
3. Seguir a ordem da seção 8 do PDF: capa e sumário, apresentação do tema, Fases 1 a 5, considerações finais e referências.
4. Numerar figuras e tabelas, gerar o sumário e confirmar que toda citação tem referência e vice-versa.
5. Salvar em `relatorio/entrega/` como `.docx` e `.pdf` (exportando pelo Word).
6. Conferir pelo PDF renderizado e preencher o checklist abaixo no `status/J8.md`, sem marcar o que não verificou.
7. Listar todo `[PENDENTE` que restar. Não o remova para esconder.

**Pronto quando:** arquivo único de até 10 MB, com todas as figuras legíveis e checklist conferido.

---

## Etapa 5 — Ana: revisão e postagem

- [ ] Escrever `relatorio/secoes/07-consideracoes-finais.md` (antes da J8): o que aprendeu e quais dificuldades enfrentou.
- [ ] Ler o relatório inteiro e rodar o sistema uma vez sozinha.
- [ ] Informar os materiais de aula usados, para citação nas referências.
- [ ] Conferir o prazo no AVA. O sistema considera só o último arquivo postado.
- [ ] Postar o arquivo único.

---

## Checklist de entrega (seção 9 do PDF)

| Item obrigatório | Atendido por |
|---|---|
| Tema: 3 funcionalidades, 1 consulta ao banco, viável | Contrato; J2 |
| Fase 1: 5 RF + 3 RNF | J2 (6 RF, 5 RNF) |
| Fase 1: casos de uso (mín. 3) + descrições | J2, J3 (6 UC) |
| Fase 2: diagrama de classes (mín. 4) | J3 |
| Fase 2: MER (mín. 3) + dicionário de dados | J2, J3 (5 entidades) |
| Fase 2: 1 diagrama de sequência | J3 |
| Fase 3: 4 telas com descrição | J4 (5 telas) |
| Fase 4: 2 funcionalidades com banco | J5, J6 |
| Fase 4: trechos do código | J7 |
| Fase 5: 5 casos de teste + prints | J7 (11 CTs) |
| Relatório na ordem da seção 8 | J8 |
| Citações e referências ABNT | J2, J7, J8 |
| Arquivo único, Word ou PDF, até 10 MB, no prazo | J8, Ana |

## Critérios de correção

| Bloco | Peso | Onde está o cuidado |
|---|---|---|
| Requisitos e casos de uso | 20% | J2: coerência com o tema |
| Classes, MER, dicionário e sequência consistentes | 25% | Contrato único, J3 confere com o código |
| Protótipos | 15% | J4: coerentes com os requisitos |
| Código com 2 funcionalidades e banco | 20% | J5, J6, J7 |
| Plano de testes com evidências | 10% | J7 |
| Normalização, citações e referências | 10% | J2, J8 |
