// Gera os quatro diagramas como HTML com SVG. Uso: node gerar.mjs
import fs from 'fs';

const esc = (s) => String(s).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
const FONT = 'font-family="Arial, Helvetica, sans-serif"';
const defs = `<defs>
<marker id="aberta" markerWidth="12" markerHeight="12" refX="10" refY="5" orient="auto"><path d="M0,0 L10,5 L0,10" fill="none" stroke="#111" stroke-width="1.4"/></marker>
<marker id="cheia" markerWidth="12" markerHeight="12" refX="10" refY="5" orient="auto"><path d="M0,0 L10,5 L0,10 z" fill="#111"/></marker>
</defs>`;
const salvar = (nome, w, h, corpo) => fs.writeFileSync(new URL(`./${nome}.html`, import.meta.url),
  `<!doctype html><html lang="pt-BR"><head><meta charset="utf-8"><title>${nome}</title><style>body{margin:0;background:#fff}</style></head><body>
<svg id="diagrama" xmlns="http://www.w3.org/2000/svg" width="${w}" height="${h}" viewBox="0 0 ${w} ${h}" ${FONT}><rect width="${w}" height="${h}" fill="#fff"/>${defs}
${corpo}
</svg></body></html>\n`);

const texto = (x, y, s, o = {}) => `<text x="${x}" y="${y}" font-size="${o.size ?? 18}" text-anchor="${o.anchor ?? 'middle'}" font-weight="${o.bold ? 700 : 400}" font-style="${o.italic ? 'italic' : 'normal'}" fill="#111">${esc(s)}</text>`;
const rotulo = (x, y, s, size = 16) => { const w = s.length * size * 0.47 + 6; return `<rect x="${x - w / 2}" y="${y - size}" width="${w}" height="${size + 7}" fill="#fff"/>` + texto(x, y, s, { size }); };
const linha = (x1, y1, x2, y2, o = {}) => `<line x1="${x1}" y1="${y1}" x2="${x2}" y2="${y2}" stroke="#111" stroke-width="${o.w ?? 1.6}" ${o.dash ? 'stroke-dasharray="8 6"' : ''} ${o.fim ? `marker-end="url(#${o.fim})"` : ''}/>`;

// ---------- Casos de uso ----------
function uc() {
  const elipse = (cx, cy, l1, l2) => `<ellipse cx="${cx}" cy="${cy}" rx="125" ry="46" fill="#fff" stroke="#111" stroke-width="1.8"/>` + texto(cx, cy - 4, l1, { bold: true }) + texto(cx, cy + 20, l2);
  const ator = (x, y, nome) => `<circle cx="${x}" cy="${y - 62}" r="18" fill="#fff" stroke="#111" stroke-width="2"/>` + linha(x, y - 44, x, y, { w: 2 }) + linha(x - 28, y - 26, x + 28, y - 26, { w: 2 }) + linha(x, y, x - 24, y + 38, { w: 2 }) + linha(x, y, x + 24, y + 38, { w: 2 }) + texto(x, y + 66, nome, { bold: true });
  const inc = (x1, y1, x2, y2, t = 0.5) => linha(x1, y1, x2, y2, { dash: true, fim: 'aberta' }) + rotulo(x1 + (x2 - x1) * t, y1 + (y2 - y1) * t + 5, '«include»', 15);
  const c = [
    `<rect x="240" y="30" width="760" height="770" fill="none" stroke="#111" stroke-width="2"/>`,
    texto(620, 60, 'Sistema Presença+', { bold: true, size: 20 }),
    // associações dos atores
    linha(135, 370, 262, 140), linha(135, 380, 262, 270), linha(135, 390, 262, 410), linha(135, 410, 480, 590), linha(135, 425, 486, 735),
    linha(1105, 380, 958, 230), linha(1105, 410, 748, 590), linha(1105, 425, 734, 735),
    // includes
    inc(495, 140, 580, 526, 0.2), inc(495, 280, 560, 530, 0.32), inc(470, 438, 530, 534, 0.45),
    inc(780, 266, 672, 533, 0.45), linha(610, 689, 610, 622, { dash: true, fim: 'aberta' }), rotulo(668, 662, '«include»', 15),
    elipse(380, 130, 'UC02', 'Manter turmas'),
    elipse(380, 270, 'UC03', 'Manter alunos'),
    elipse(380, 410, 'UC04', 'Manter aulas'),
    elipse(840, 230, 'UC05', 'Registrar chamada'),
    elipse(610, 574, 'UC01', 'Fazer login'),
    elipse(610, 735, 'UC06', 'Emitir relatório'),
    ator(90, 380, 'Coordenação'), ator(1150, 380, 'Professor'),
  ];
  salvar("uc", 1240, 830, c.join('\n'));
}

// ---------- Classes ----------
function caixa(x, y, w, nome, atrs, mets = [], este = '') {
  const lh = 24, h1 = este ? 54 : 36, h2 = atrs.length * lh + 10, h3 = mets.length ? mets.length * lh + 10 : 0;
  let s = `<rect x="${x}" y="${y}" width="${w}" height="${h1 + h2 + h3}" fill="#fff" stroke="#111" stroke-width="1.8"/>`;
  s += `<rect x="${x}" y="${y}" width="${w}" height="${h1}" fill="#e8e8e8" stroke="#111" stroke-width="1.8"/>`;
  if (este) s += texto(x + w / 2, y + 20, este, { size: 15 });
  s += texto(x + w / 2, y + h1 - 12, nome, { bold: true, size: 19 });
  atrs.forEach((a, i) => { s += texto(x + 12, y + h1 + 23 + i * lh, a, { anchor: 'start', size: 16 }); });
  if (mets.length) {
    s += linha(x, y + h1 + h2, x + w, y + h1 + h2);
    mets.forEach((m, i) => { s += texto(x + 12, y + h1 + h2 + 23 + i * lh, m, { anchor: 'start', size: 16 }); });
  }
  return { s, h: h1 + h2 + h3 };
}

function classes() {
  const W = 410;
  const t = caixa(40, 40, W, 'Turma', ['+ Id: int', '+ Nome: string', '+ AnoLetivo: int', '+ Turno: string', '+ Alunos: List<Aluno>', '+ Aulas: List<Aula>']);
  const a = caixa(600, 40, 520, 'Aula', ['+ Id: int', '+ TurmaId: int', '+ ProfessorId: int', '+ Disciplina: string', '+ Data: DateOnly', '+ Horario: TimeOnly', '+ ChamadaRegistradaEm: DateTime?', '+ Turma: Turma', '+ Professor: Usuario', '+ Faltas: List<Falta>'],
    ['+ PodeRegistrarChamada(hoje: DateOnly): bool', '+ RegistrarChamada(alunosFaltantes:', '     IEnumerable<int>, agora: DateTime): void']);
  const u = caixa(1270, 40, W, 'Usuario', ['+ Id: int', '+ Nome: string', '+ Login: string', '+ SenhaHash: string', '+ Perfil: string'], ['+ DefinirSenha(senha: string): void', '+ VerificarSenha(senha: string): bool']);
  const al = caixa(40, 470, W, 'Aluno', ['+ Id: int', '+ Matricula: string', '+ Nome: string', '+ TurmaId: int', '+ Turma: Turma', '+ Faltas: List<Falta>']);
  const f = caixa(650, 530, 420, 'Falta', ['+ Id: int', '+ AulaId: int', '+ AlunoId: int', '+ Aula: Aula', '+ Aluno: Aluno']);
  const lf = caixa(40, 810, W, 'LinhaFrequencia', ['+ Aluno: Aluno', '+ Aulas: int', '+ Faltas: int', '+ Percentual: double?', '+ Situacao: string'], [], '«record»');
  const fs_ = caixa(600, 840, 520, 'FrequenciaService', [], ['+ GerarRelatorioAsync(turmaId: int, de: DateOnly,', '     ate: DateOnly): Task<List<LinhaFrequencia>>']);
  const ctx = caixa(1270, 790, W, 'AppDbContext', ['+ Usuarios: DbSet<Usuario>', '+ Turmas: DbSet<Turma>', '+ Alunos: DbSet<Aluno>', '+ Aulas: DbSet<Aula>', '+ Faltas: DbSet<Falta>'], [], '«DbContext»');
  const rel = [
    linha(450, 170, 600, 170), texto(466, 160, '1'), texto(584, 160, '*'),           // Turma - Aula
    linha(245, 40 + t.h, 245, 470), texto(262, 40 + t.h + 22, '1'), texto(262, 462, '*'), // Turma - Aluno
    linha(1270, 170, 1120, 170), texto(1254, 160, '1'), texto(1136, 160, '*'), rotulo(1195, 196, 'Professor', 15), // Usuario - Aula
    linha(860, 40 + a.h, 860, 530), texto(877, 40 + a.h + 22, '1'), texto(877, 522, '*'), // Aula - Falta
    linha(450, 610, 650, 610), texto(466, 600, '1'), texto(634, 600, '*'),           // Aluno - Falta
    linha(245, 810, 245, 470 + al.h, { fim: 'aberta' }), texto(262, 800, '1'),      // LinhaFrequencia -> Aluno
    linha(600, 880, 450, 880, { dash: true, fim: 'aberta' }), rotulo(525, 870, '«cria»', 15),
    linha(1120, 880, 1270, 880, { dash: true, fim: 'aberta' }), rotulo(1195, 870, '«usa»', 15),
  ];
  salvar('classes', 1720, 1040, [...rel, t.s, a.s, u.s, al.s, f.s, lf.s, fs_.s, ctx.s].join('\n'));
}

// ---------- MER ----------
function mer() {
  const W = 400;
  const tab = (x, y, nome, cols) => caixa(x, y, W, nome, cols);
  const t = tab(40, 40, 'Turmas', ['PK  Id: int', '       Nome: texto(30), único', '       AnoLetivo: int', '       Turno: texto(10)']);
  const a = tab(620, 40, 'Aulas', ['PK  Id: int', 'FK  TurmaId: int', 'FK  ProfessorId: int', '       Disciplina: texto(50)', '       Data: data', '       Horario: hora', '       ChamadaRegistradaEm: data e hora']);
  const u = tab(1200, 40, 'Usuarios', ['PK  Id: int', '       Nome: texto(100)', '       Login: texto(50), único', '       SenhaHash: texto(200)', '       Perfil: texto(15)']);
  const al = tab(40, 430, 'Alunos', ['PK  Id: int', '       Matricula: texto(10), único', '       Nome: texto(100)', 'FK  TurmaId: int']);
  const f = tab(620, 430, 'Faltas', ['PK  Id: int', 'FK  AulaId: int', 'FK  AlunoId: int']);
  const rel = [
    linha(440, 140, 620, 140), texto(458, 130, '1'), texto(602, 130, 'N'), rotulo(530, 134, 'tem', 16),
    linha(1200, 140, 1020, 140), texto(1182, 130, '1'), texto(1038, 130, 'N'), rotulo(1110, 134, 'ministra', 16),
    linha(240, 40 + t.h, 240, 430), texto(258, 40 + t.h + 22, '1'), texto(258, 422, 'N'), rotulo(240, 330, 'tem', 16),
    linha(820, 40 + a.h, 820, 430), texto(838, 40 + a.h + 22, '1'), texto(838, 422, 'N'), rotulo(820, 345, 'gera', 16),
    linha(440, 520, 620, 520), texto(458, 510, '1'), texto(602, 510, 'N'), rotulo(530, 514, 'recebe', 16),
  ];
  salvar('mer', 1640, 640, [...rel, t.s, a.s, u.s, al.s, f.s].join('\n'));
}

// ---------- Sequência (UC05) ----------
function sequencia() {
  const xs = { prof: 110, pag: 430, ctx: 760, aula: 1060, bd: 1340 };
  const nomes = [['prof', 'Professor'], ['pag', 'Registrar (página)'], ['ctx', 'AppDbContext'], ['aula', 'Aula'], ['bd', 'Banco (SQLite)']];
  const H = 1120, topo = 90;
  const c = [];
  for (const [k, n] of nomes) {
    c.push(linha(xs[k], topo, xs[k], H - 30, { dash: true, w: 1.2 }));
    c.push(`<rect x="${xs[k] - 110}" y="30" width="220" height="${topo - 30}" fill="#e8e8e8" stroke="#111" stroke-width="1.8"/>`);
    c.push(texto(xs[k], 67, n, { bold: true, size: 19 }));
  }
  let y = 135;
  const msg = (de, para, t, volta = false) => {
    const x1 = xs[de], x2 = xs[para];
    c.push(linha(x1, y, x2, y, { dash: volta, fim: volta ? 'aberta' : 'cheia' }));
    c.push(rotulo((x1 + x2) / 2, y - 12, t, 16));
    y += 48;
  };
  const moldura = (y1, y2, tag, cond) => {
    c.push(`<rect x="24" y="${y1}" width="1440" height="${y2 - y1}" fill="none" stroke="#555" stroke-width="1.4"/>`);
    c.push(`<rect x="24" y="${y1}" width="60" height="26" fill="#e8e8e8" stroke="#555"/>`);
    c.push(texto(54, y1 + 19, tag, { bold: true, size: 15 }));
    c.push(texto(96, y1 + 19, cond, { anchor: 'start', size: 15, italic: true }));
  };
  msg('prof', 'pag', 'abre a aula: GET /Chamada/Registrar/{id}');
  msg('pag', 'ctx', 'consulta aula, faltas e alunos');
  msg('ctx', 'bd', 'SELECT em Aulas, Faltas e Alunos');
  msg('bd', 'ctx', 'registros', true);
  msg('ctx', 'pag', 'aula e alunos da turma', true);
  msg('pag', 'aula', 'PodeRegistrarChamada(hoje)');
  msg('aula', 'pag', 'verdadeiro ou falso', true);
  const a0 = y - 12; y += 46;
  msg('pag', 'prof', 'avisa que a data é futura (RN05)', true);
  const a1 = y - 18;
  c.push(`<line x1="24" y1="${a1}" x2="1464" y2="${a1}" stroke="#555" stroke-dasharray="8 6"/>`);
  c.push(texto(96, a1 + 19, '[data igual ou anterior a hoje]', { anchor: 'start', size: 15, italic: true }));
  y += 46;
  msg('pag', 'prof', 'lista de alunos, todos presentes (RN06)', true);
  msg('prof', 'pag', 'marca as faltas e salva: POST');
  msg('pag', 'ctx', 'consulta a aula e confere RN05 e RN07');
  msg('pag', 'ctx', 'Faltas.RemoveRange(faltas antigas)');
  msg('pag', 'aula', 'RegistrarChamada(alunosFaltantes, agora)');
  msg('pag', 'ctx', 'SaveChangesAsync()');
  msg('ctx', 'bd', 'DELETE e INSERT em Faltas, UPDATE em Aulas');
  msg('bd', 'ctx', 'confirmação', true);
  msg('ctx', 'pag', 'alterações gravadas', true);
  msg('pag', 'prof', 'mensagem "Chamada registrada."', true);
  moldura(a0, y - 18, 'alt', '[data futura]');
  salvar('sequencia', 1490, y + 30, c.join('\n').replaceAll(`y2="${H - 30}"`, `y2="${y + 4}"`));
}

uc(); classes(); mer(); sequencia();
