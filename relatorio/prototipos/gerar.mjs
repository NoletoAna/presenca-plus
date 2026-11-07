// Gera os wireframes das cinco telas em HTML estático. Uso: node gerar.mjs
import fs from 'fs';

const css = `*{box-sizing:border-box}body{margin:0;background:#fff;font:16px Arial,Helvetica,sans-serif;color:#222}
#tela{width:1000px;min-height:600px;border:2px solid #777;background:#fff}
.topo{display:flex;align-items:center;gap:22px;padding:14px 20px;border-bottom:2px solid #999;background:#eee}
.topo b{font-size:20px;margin-right:12px}.topo .fim{margin-left:auto}
main{padding:24px}h1{font-size:24px;margin:0 0 16px}
.linha{display:flex;gap:14px;align-items:flex-end;margin-bottom:18px}.entre{justify-content:space-between;align-items:center}
.campo{display:flex;flex-direction:column;gap:6px;flex:1}.campo span{font-size:14px}
.input{height:38px;border:1.5px solid #888;background:#fff;padding:9px 10px;color:#555}
.btn{display:inline-block;padding:9px 16px;border:1.5px solid #666;background:#ddd;white-space:nowrap}
.btn.p{background:#555;border-color:#555;color:#fff}.btn.m{padding:4px 9px;font-size:14px}
table{width:100%;border-collapse:collapse}th,td{border:1px solid #aaa;padding:8px 10px;text-align:left}th{background:#e6e6e6}
.alerta{background:#d0d0d0;font-weight:700}.c{text-align:center}.box{display:inline-block;width:20px;height:20px;border:1.5px solid #666;text-align:center;line-height:18px}
.login{width:400px;margin:70px auto}.login .campo{margin-bottom:16px}.login .btn{display:block;text-align:center}
.nota{color:#555;margin:0 0 14px}`;

const pagina = (nome, titulo, corpo) => fs.writeFileSync(new URL(`./${nome}.html`, import.meta.url),
  `<!doctype html><html lang="pt-BR"><head><meta charset="utf-8"><title>${titulo}</title><style>${css}</style></head><body><div id="tela">${corpo}</div></body></html>\n`);

const topo = (itens, nome) => `<div class="topo"><b>Presença+</b>${itens.map((i) => `<span>${i}</span>`).join('')}<span class="fim">${nome}</span><span class="btn m">Sair</span></div>`;
const coord = topo(['Turmas', 'Alunos', 'Aulas', 'Relatório'], 'Usuário da coordenação');
const prof = topo(['Minhas aulas', 'Relatório'], 'Professor');
const campo = (r, v) => `<label class="campo"><span>${r}</span><div class="input">${v}</div></label>`;
const tabela = (cab, linhas) => `<table><thead><tr>${cab.map((c) => `<th>${c}</th>`).join('')}</tr></thead><tbody>${linhas.join('')}</tbody></table>`;
const tr = (cels, cl = '') => `<tr class="${cl}">${cels.map((c) => `<td>${c}</td>`).join('')}</tr>`;

pagina('tela-login', 'Login', `<div class="topo"><b>Presença+</b></div><main><div class="login"><h1>Entrar</h1>
${campo('Login', 'usuário')}${campo('Senha', '••••••••')}<div class="btn p">Entrar</div></div></main>`);

const acoes = '<span class="btn m">Editar</span> <span class="btn m">Excluir</span>';
pagina('tela-alunos', 'Alunos', `${coord}<main><div class="linha entre"><h1 style="margin:0">Alunos</h1><span class="btn p">Novo aluno</span></div>
<div class="linha" style="width:460px">${campo('Filtrar por turma', 'Todas ▾')}<span class="btn">Filtrar</span></div>
${tabela(['Matrícula', 'Nome', 'Turma', ''], [['2026101', 'Aluno Exemplo 1', '7º ano A'], ['2026102', 'Aluno Exemplo 2', '7º ano A'], ['2026103', 'Aluno Exemplo 3', '7º ano A'], ['2026201', 'Aluno Exemplo 4', '8º ano B'], ['2026202', 'Aluno Exemplo 5', '8º ano B']].map((l) => tr([...l, acoes])))}</main>`);

pagina('tela-aulas', 'Minhas aulas', `${prof}<main><h1>Minhas aulas</h1>
${tabela(['Data', 'Horário', 'Turma', 'Disciplina', 'Situação', ''], [
    tr(['14/11/2026', '10:00', '7º ano A', 'Língua Portuguesa', 'Futura', '<span class="btn m">Abrir</span>']),
    tr(['07/11/2026', '10:00', '7º ano A', 'Língua Portuguesa', 'Pendente', '<span class="btn m">Fazer chamada</span>']),
    tr(['05/11/2026', '07:30', '7º ano A', 'Língua Portuguesa', 'Registrada', '<span class="btn m">Rever</span>']),
    tr(['05/11/2026', '09:30', '8º ano B', 'Língua Portuguesa', 'Registrada', '<span class="btn m">Rever</span>']),
    tr(['03/11/2026', '07:30', '7º ano A', 'Língua Portuguesa', 'Registrada', '<span class="btn m">Rever</span>']),
  ])}</main>`);

const box = (x) => `<span class="box">${x ? '✕' : ''}</span>`;
pagina('tela-chamada', 'Registrar chamada', `${prof}<main><h1>Registrar chamada</h1><p class="nota">7º ano A, Língua Portuguesa, 07/11/2026 às 10:00</p>
<p>Todos começam presentes. Marque somente os alunos que faltaram.</p>
${tabela(['Matrícula', 'Aluno', 'Faltou'], [['2026101', 'Aluno Exemplo 1', 1], ['2026102', 'Aluno Exemplo 2', 0], ['2026103', 'Aluno Exemplo 3', 0], ['2026104', 'Aluno Exemplo 4', 1], ['2026105', 'Aluno Exemplo 5', 0]].map(([m, n, f]) => tr([m, n, box(f)])))}
<div class="linha" style="margin-top:18px"><span class="btn p">Salvar chamada</span><span class="btn">Cancelar</span></div></main>`);

pagina('tela-relatorio', 'Relatório', `${coord}<main><h1>Relatório de frequência</h1>
<div class="linha">${campo('Turma', '7º ano A ▾')}${campo('Data inicial', '01/01/2026')}${campo('Data final', '07/11/2026')}<span class="btn p">Gerar</span></div>
${tabela(['Aluno', 'Matrícula', 'Aulas', 'Faltas', 'Frequência', 'Situação'], [
    tr(['Aluno Exemplo 1', '2026101', '20', '6', '70,0%', 'Abaixo de 75%'], 'alerta'),
    tr(['Aluno Exemplo 2', '2026102', '20', '4', '80,0%', 'Atenção']),
    tr(['Aluno Exemplo 3', '2026103', '20', '2', '90,0%', 'Regular']),
    tr(['Aluno Exemplo 4', '2026104', '20', '1', '95,0%', 'Regular']),
    tr(['Aluno Exemplo 5', '2026105', '20', '0', '100,0%', 'Regular']),
  ])}<div class="linha" style="margin-top:18px"><span class="btn">Imprimir</span></div></main>`);
