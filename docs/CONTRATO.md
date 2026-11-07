# Contrato do sistema — Presença+ (versão reduzida para o Projeto Integrado)

Este arquivo é a fonte única da verdade. Todas as janelas usam exatamente estes nomes, campos, regras e rotas, para que documentos, diagramas e código fiquem consistentes (a consistência vale 25% da nota).

**Se algo aqui precisar mudar**, não altere este arquivo. Registre em `status/mudancas-contrato.md` (o que mudou e por quê). A J7 reconcilia.

## 1. O sistema

Sistema web para uma escola controlar as faltas dos alunos. A coordenação cadastra turmas, alunos e aulas. O professor faz a chamada de cada aula. O relatório mostra a frequência de cada aluno e destaca quem está abaixo do mínimo de 75% exigido pela LDB (Lei n.º 9.394/1996).

Origem: plano de negócios Presença+ (`C:\Users\anano\Desktop\Tarefa da Unidade 4 — Criação do Modelo do Plano de Negócios`) e seu protótipo navegável. Escola fictícia do exemplo: Colégio Aurora.

**Fora do escopo** (existem no plano de negócios, mas não neste protótipo): SaaS com várias escolas, cobrança, perfil Setor de TI, cadastro de usuários pela tela, log de auditoria, importação de planilha, integração com ERP, aviso aos responsáveis.

## 2. Stack

| Item | Escolha |
|---|---|
| Linguagem e plataforma | C# com .NET 10 (SDK 10.0.400, já instalado) |
| Web | ASP.NET Core Razor Pages com Bootstrap do template |
| Banco | SQLite, via EF Core 10 (`Microsoft.EntityFrameworkCore.Sqlite`) |
| Login | Autenticação por cookie, sem ASP.NET Identity. Hash com `PasswordHasher<Usuario>` |
| Testes | Manuais e documentados, com prints via Playwright |

**Sem** camadas extras: nada de repositórios, CQRS, DDD, microsserviços, API separada, SPA ou Docker. O PageModel usa o `AppDbContext` direto. A única classe de serviço é `FrequenciaService`.

Linguagem e banco são permitidos pelo PDF ("outra que você se sinta confortável", "SQLite").

## 3. Requisitos

### Funcionais

| ID | Requisito | Perfil |
|---|---|---|
| RF01 | Autenticar usuário com login e senha, e encerrar a sessão | Todos |
| RF02 | Manter turmas (criar, listar, editar, excluir) | Coordenação |
| RF03 | Manter alunos (criar, listar, filtrar por turma, editar, excluir) | Coordenação |
| RF04 | Manter aulas (criar, listar, editar, excluir) | Coordenação |
| RF05 | Registrar a chamada de uma aula, marcando os alunos que faltaram | Professor |
| RF06 | Emitir relatório de frequência por turma e período, com destaque para alunos abaixo de 75% | Todos |

### Não funcionais

| ID | Requisito |
|---|---|
| RNF01 | Segurança: toda página, exceto a de login, exige autenticação; senhas guardadas apenas como hash |
| RNF02 | Autorização: cada perfil acessa somente as funções indicadas na tabela acima |
| RNF03 | Desempenho: páginas respondem em até 3 segundos com os dados de demonstração |
| RNF04 | Usabilidade: interface em português, datas dd/MM/aaaa, utilizável no navegador do celular (largura de 375 px) |
| RNF05 | Privacidade (LGPD): guardar só os dados necessários do aluno (nome, matrícula e turma), sem CPF, endereço ou telefone |

### Regras de negócio

| ID | Regra |
|---|---|
| RN01 | Frequência = (aulas com chamada − faltas) ÷ aulas com chamada × 100, contando apenas aulas da turma no período |
| RN02 | Situação: abaixo de 75% = "Abaixo de 75%"; de 75% até menos de 85% = "Atenção"; a partir de 85% = "Regular"; sem aula com chamada = "Sem aulas" |
| RN03 | Matrícula do aluno e nome da turma são únicos |
| RN04 | Não pode haver duas aulas na mesma turma, data e horário |
| RN05 | Chamada só pode ser registrada em aula com data igual ou anterior a hoje |
| RN06 | Na chamada, todos começam presentes. Salvar de novo substitui as faltas anteriores da aula |
| RN07 | O professor só vê e registra as próprias aulas |
| RN08 | Não se exclui turma com alunos ou aulas, aluno com faltas, nem aula com chamada registrada. O sistema avisa o motivo |

## 4. Casos de uso

Atores: **Coordenação** e **Professor**.

| ID | Caso de uso | Ator | RF |
|---|---|---|---|
| UC01 | Fazer login | Coordenação, Professor | RF01 |
| UC02 | Manter turmas | Coordenação | RF02 |
| UC03 | Manter alunos | Coordenação | RF03 |
| UC04 | Manter aulas | Coordenação | RF04 |
| UC05 | Registrar chamada (principal) | Professor | RF05 |
| UC06 | Emitir relatório de frequência | Coordenação, Professor | RF06 |

UC02 a UC06 têm `<<include>>` para UC01.

## 5. Modelo de dados

| Tabela | Campo | Tipo | Obrig. | Regra |
|---|---|---|---|---|
| Usuarios | Id | int | Sim | PK |
| | Nome | texto(100) | Sim | |
| | Login | texto(50) | Sim | único |
| | SenhaHash | texto(200) | Sim | hash, nunca a senha |
| | Perfil | texto(15) | Sim | `Coordenacao` ou `Professor` |
| Turmas | Id | int | Sim | PK |
| | Nome | texto(30) | Sim | único |
| | AnoLetivo | int | Sim | |
| | Turno | texto(10) | Sim | `Matutino`, `Vespertino` ou `Noturno` |
| Alunos | Id | int | Sim | PK |
| | Matricula | texto(10) | Sim | único |
| | Nome | texto(100) | Sim | |
| | TurmaId | int | Sim | FK → Turmas |
| Aulas | Id | int | Sim | PK |
| | TurmaId | int | Sim | FK → Turmas |
| | ProfessorId | int | Sim | FK → Usuarios |
| | Disciplina | texto(50) | Sim | |
| | Data | data | Sim | único com TurmaId e Horario |
| | Horario | hora | Sim | |
| | ChamadaRegistradaEm | data e hora | Não | nulo = chamada pendente |
| Faltas | Id | int | Sim | PK |
| | AulaId | int | Sim | FK → Aulas |
| | AlunoId | int | Sim | FK → Alunos; único com AulaId |

Relacionamentos (todas as FKs com `DeleteBehavior.Restrict`): Turma 1:N Aluno; Turma 1:N Aula; Usuario (professor) 1:N Aula; Aula 1:N Falta; Aluno 1:N Falta.

## 6. Classes (namespace `PresencaPlus`)

Os diagramas devem mostrar estes nomes e o código deve ter exatamente estes membros.

```csharp
class Usuario { Id; Nome; Login; SenhaHash; Perfil;
    void DefinirSenha(string senha);
    bool VerificarSenha(string senha); }

class Turma { Id; Nome; AnoLetivo; Turno; List<Aluno> Alunos; List<Aula> Aulas; }

class Aluno { Id; Matricula; Nome; TurmaId; Turma; List<Falta> Faltas; }

class Aula { Id; TurmaId; ProfessorId; Disciplina; DateOnly Data; TimeOnly Horario;
    DateTime? ChamadaRegistradaEm; Turma; Professor; List<Falta> Faltas;
    bool PodeRegistrarChamada(DateOnly hoje);                 // RN05
    void RegistrarChamada(IEnumerable<int> alunosFaltantes, DateTime agora); } // RN06

class Falta { Id; AulaId; AlunoId; Aula; Aluno; }

record LinhaFrequencia(Aluno Aluno, int Aulas, int Faltas) {
    double? Percentual { get; }  // RN01
    string Situacao { get; } }   // RN02

class FrequenciaService {
    Task<List<LinhaFrequencia>> GerarRelatorioAsync(int turmaId, DateOnly de, DateOnly ate); }

class AppDbContext : DbContext { Usuarios; Turmas; Alunos; Aulas; Faltas; }
```

## 7. Telas e rotas

Protótipo = entra na Fase 3 do relatório.

| Tela | Rota | Perfil | Protótipo |
|---|---|---|---|
| Login | `/Login` | Público | Sim |
| Turmas | `/Turmas`, `/Turmas/Criar`, `/Turmas/Editar/{id}`, `/Turmas/Excluir/{id}` | Coordenação | Não |
| Alunos | `/Alunos`, `/Alunos/Criar`, `/Alunos/Editar/{id}`, `/Alunos/Excluir/{id}` | Coordenação | Sim |
| Aulas | `/Aulas`, `/Aulas/Criar`, `/Aulas/Editar/{id}`, `/Aulas/Excluir/{id}` | Coordenação | Não |
| Minhas aulas | `/Chamada` | Professor | Sim |
| Registrar chamada | `/Chamada/Registrar/{id}` | Professor | Sim |
| Relatório de frequência | `/Relatorio` | Todos | Sim |
| Acesso negado | `/AcessoNegado` | Todos | Não |

Depois do login, Coordenação vai para `/Relatorio` e Professor para `/Chamada`.

## 8. Dados de demonstração

Todos fictícios, criados na primeira execução e sempre iguais (sem aleatoriedade).

- Usuários, com senha `demo1234`: `beatriz` (Coordenação), `marina` e `carlos` (Professor).
- Três turmas (7º ano A, 8º ano B, 9º ano A), cada uma com 10 alunos.
- Pelo menos 20 aulas por turma nas semanas anteriores à data da execução, com chamada registrada.
- Pelo menos 1 aluno abaixo de 75%, 1 em "Atenção", e 1 aula de hoje com chamada pendente para `marina`.
- 1 aula futura para `marina` (para o CT09).

## 9. Casos de teste

Resultado esperado definido antes da execução, como o PDF pede. O resultado obtido só pode vir de execução real.

| ID | O que testar | Resultado esperado |
|---|---|---|
| CT01 | Login com dados válidos (`beatriz`) | Entra e vai para o relatório |
| CT02 | Login com senha errada | Mensagem de erro e continua no login |
| CT03 | Abrir `/Alunos` sem login | Redireciona para o login |
| CT04 | Cadastrar aluno com dados válidos | Aluno salvo e exibido na lista |
| CT05 | Cadastrar aluno sem nome | Mensagem de validação e nada é salvo |
| CT06 | Cadastrar aluno com matrícula repetida | Sistema impede e avisa |
| CT07 | Cadastrar aula em turma, data e horário já ocupados | Sistema impede e avisa |
| CT08 | Registrar chamada com 2 faltas e reabrir a aula | As 2 faltas continuam marcadas e os dados estão no banco |
| CT09 | Tentar registrar chamada em aula futura | Sistema impede e avisa |
| CT10 | Conferir no relatório a frequência de um aluno, calculando à mão | Valor igual ao cálculo e situação conforme a RN02 |
| CT11 | Entrar como professor e abrir `/Alunos` | Página de acesso negado |

## 10. Funcionalidades descritas na Fase 4

As duas exigidas pelo PDF, com trecho de código e prints: **Cadastro de alunos (RF03)** e **Registro de chamada (RF05)**. O relatório (RF06) entra como terceira funcionalidade, para mostrar a consulta ao banco.

## 11. Arquivos do relatório

Nomes fixos, para uma janela poder referenciar o que outra ainda vai gerar.

| Arquivo em `relatorio/` | Autor |
|---|---|
| `insumos/levantamento.md` | Ana |
| `insumos/modelo-entrega.docx` | Ana (baixar do AVA) |
| `secoes/01-apresentacao.md` | J2 |
| `secoes/02-fase1-requisitos.md` | J2 |
| `secoes/03-fase2-modelagem.md` | J2 |
| `secoes/04-fase3-prototipagem.md` | J4 |
| `secoes/05-fase4-desenvolvimento.md` | J7 |
| `secoes/06-fase5-testes.md` | J7 |
| `secoes/07-consideracoes-finais.md` | Ana |
| `secoes/08-referencias.md` | J2 inicia; J7 e J8 completam |
| `img/uc.png`, `img/classes.png`, `img/mer.png`, `img/sequencia.png` | J3 |
| `img/tela-login.png`, `img/tela-alunos.png`, `img/tela-aulas.png`, `img/tela-chamada.png`, `img/tela-relatorio.png` | J4 |
| `img/exec-*.png`, `img/ct01.png` a `img/ct11.png` | J7 |
