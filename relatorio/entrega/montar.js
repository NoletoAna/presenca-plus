// Monta o relatório .docx a partir de relatorio/secoes/*.md.
// Uso: node relatorio/entrega/montar.js (exige o pacote npm "docx" no NODE_PATH).
const fs = require('fs'), path = require('path');
const d = require('docx');
const { Document, Packer, Paragraph, TextRun, ImageRun, Table, TableRow, TableCell, Header, PageNumber, TableOfContents,
  HeadingLevel, AlignmentType, WidthType, ShadingType, BorderStyle, LevelFormat, SectionType } = d;

const raiz = path.resolve(__dirname, '..');
const saida = path.join(__dirname, 'Relatorio-Projeto-Integrado.docx');
const FONTE = 'Times New Roman', MONO = 'Courier New';
const LARG = 9071; // largura útil em DXA: A4 menos margens de 3 cm e 2 cm
const NUMERADAS = ['01-apresentacao', '02-fase1-requisitos', '03-fase2-modelagem', '04-fase3-prototipagem',
  '05-fase4-desenvolvimento', '06-fase5-testes', '07-consideracoes-finais'];
const arquivos = [...NUMERADAS, '08-referencias'];

// Numeração: cada legenda consome um número; as menções no texto apontam para as próximas legendas.
function numerar(linhas) {
  let fig = 0, tab = 0;
  const log = [];
  for (let i = 0; i < linhas.length; i++) {
    let l = linhas[i];
    if (/^!\[/.test(l)) continue;
    if (/^Figura X — /.test(l)) { linhas[i] = l.replace('Figura X', `Figura ${++fig}`); continue; }
    if (/^Tabela X — /.test(l)) { linhas[i] = l.replace('Tabela X', `Tabela ${++tab}`); continue; }
    if (!/(Figuras?|Tabelas?) X/.test(l)) continue;
    let f = fig, t = tab;
    l = l.replace(/As Tabelas X a X/, () => {
      const dic = linhas.slice(i).filter(x => /^Tabela X — Dicionário/.test(x)).length;
      return `As Tabelas ${t + 1} a ${t + dic}`;
    });
    l = l.replace(/Figuras X, X e X/g, () => `Figuras ${++f}, ${++f} e ${++f}`);
    l = l.replace(/Figura X/g, () => `Figura ${++f}`).replace(/Tabela X/g, () => `Tabela ${++t}`);
    linhas[i] = l;
    log.push(l.match(/.{0,30}(Figuras?|Tabelas?) \d+.{0,30}/)[0]);
  }
  return { fig, tab, log };
}

function pngDim(b) { return { w: b.readUInt32BE(16), h: b.readUInt32BE(20) }; }

// Texto com **negrito**, `código` e [PENDENTE...] em amarelo.
function runs(txt, base = {}) {
  const out = [];
  const re = /(\[PENDENTE[^\]]*\])|(\*\*[^*]+\*\*)|(`[^`]+`)/g;
  let pos = 0, m;
  const add = (t, o = {}) => { if (t) out.push(new TextRun({ text: t, font: FONTE, ...base, ...o })); };
  while ((m = re.exec(txt))) {
    add(txt.slice(pos, m.index));
    if (m[1]) add(m[1].replace(/`/g, ''), { highlight: 'yellow' });
    else if (m[2]) add(m[2].slice(2, -2), { bold: true });
    else out.push(new TextRun({ text: m[3].slice(1, -1), ...base, font: MONO, size: (base.size || 24) - 2 }));
    pos = m.index + m[0].length;
  }
  add(txt.slice(pos));
  return out;
}

const SIMPLES = { line: 240 };
const corpo = (txt) => new Paragraph({ children: runs(txt), alignment: AlignmentType.JUSTIFIED, indent: { firstLine: 709 }, spacing: { line: 360, after: 120 } });
const legenda = (txt, opt = {}) => new Paragraph({ children: runs(txt, { size: 20 }), alignment: AlignmentType.CENTER, spacing: { ...SIMPLES, before: 60, after: 60 }, keepNext: true, keepLines: true, ...opt });
const fonte = (txt) => new Paragraph({ children: runs(txt, { size: 20 }), alignment: AlignmentType.CENTER, spacing: { ...SIMPLES, after: 240 } });

function imagem(rel, alt) {
  const dados = fs.readFileSync(path.resolve(path.join(raiz, 'secoes'), rel));
  const { w, h } = pngDim(dados);
  const maxW = 450, maxH = /celular/.test(rel) ? 380 : 520; // em pontos
  const k = Math.min(maxW / w, maxH / h);
  return new Paragraph({
    alignment: AlignmentType.CENTER, keepNext: true, spacing: { ...SIMPLES, before: 120 },
    children: [new ImageRun({ type: 'png', data: dados, transformation: { width: Math.round(w * k * 96 / 72), height: Math.round(h * k * 96 / 72) }, altText: { title: path.basename(rel), description: alt, name: path.basename(rel) } })],
  });
}

function tabela(linhas) {
  const cel = (l) => l.trim().replace(/^\|/, '').replace(/\|$/, '').split('|').map(c => c.trim());
  const cab = cel(linhas[0]);
  const lin = linhas.slice(2).map(cel);
  // Largura proporcional ao texto mais longo de cada coluna, com mínimo para IDs.
  const peso = cab.map((c, i) => Math.max(5, Math.sqrt(Math.max(...[c, ...lin.map(r => r[i])].map(x => x.length)))));
  const soma = peso.reduce((a, b) => a + b, 0);
  const col = peso.map(p => Math.round(LARG * p / soma));
  col[col.length - 1] += LARG - col.reduce((a, b) => a + b, 0);
  const borda = { style: BorderStyle.SINGLE, size: 4, color: '000000' };
  const bordas = { top: borda, bottom: borda, left: borda, right: borda };
  const junto = lin.length <= 8; // tabelas curtas ficam inteiras na mesma página
  const celula = (t, i, cabeca) => new TableCell({
    width: { size: col[i], type: WidthType.DXA }, borders: bordas,
    margins: { top: 40, bottom: 40, left: 80, right: 80 },
    shading: cabeca ? { type: ShadingType.CLEAR, fill: 'D9D9D9' } : undefined,
    children: [new Paragraph({ children: runs(t, { size: 20, bold: cabeca }), spacing: SIMPLES, keepNext: junto || cabeca })],
  });
  return new Table({
    width: { size: LARG, type: WidthType.DXA }, columnWidths: col,
    rows: [new TableRow({ tableHeader: true, cantSplit: true, children: cab.map((t, i) => celula(t, i, true)) }),
      ...lin.map(r => new TableRow({ cantSplit: true, children: r.map((t, i) => celula(t, i, false)) }))],
  });
}

function codigo(linhas) {
  return linhas.map((l, i) => new Paragraph({
    children: [new TextRun({ text: l || ' ', font: MONO, size: 17 })],
    spacing: { ...SIMPLES, after: i === linhas.length - 1 ? 200 : 0 }, keepLines: true, keepNext: i < linhas.length - 1,
    indent: { left: 280, hanging: 280 },
  }));
}

function blocos(linhas, nome) {
  const num = NUMERADAS.indexOf(nome) + 1;
  const ref = nome === '08-referencias';
  const out = [];
  let i = 0, sub = 0;
  while (i < linhas.length) {
    const l = linhas[i];
    let m;
    if (!l.trim()) { i++; continue; }
    if (l.startsWith('# ')) {
      const t = l.slice(2).toUpperCase();
      out.push(new Paragraph({ heading: HeadingLevel.HEADING_1, pageBreakBefore: true, children: [new TextRun({ text: ref ? t : `${num} ${t}` })] }));
    } else if (l.startsWith('## ')) {
      out.push(new Paragraph({ heading: HeadingLevel.HEADING_2, children: [new TextRun({ text: `${num}.${++sub} ${l.slice(3).toUpperCase()}` })] }));
    } else if (l.startsWith('```')) {
      const c = []; i++;
      while (!linhas[i].startsWith('```')) c.push(linhas[i++]);
      out.push(...codigo(c));
    } else if (l.startsWith('|')) {
      const t = [];
      while (i < linhas.length && linhas[i].startsWith('|')) t.push(linhas[i++]);
      out.push(tabela(t));
      continue;
    } else if ((m = l.match(/^!\[(.*)\]\((.*)\)$/))) {
      out.push(imagem(m[2], m[1]));
    } else if (/^Figura \d+ — /.test(l)) {
      out.push(legenda(l));
    } else if (/^Tabela \d+ — /.test(l)) {
      out.push(legenda(l, { alignment: AlignmentType.LEFT }));
    } else if (/^Fonte: /.test(l)) {
      out.push(fonte(l));
    } else if (l.startsWith('- ')) {
      out.push(new Paragraph({ numbering: { reference: 'topicos', level: 0 }, children: runs(l.slice(2)), alignment: AlignmentType.JUSTIFIED, spacing: { line: 360 } }));
    } else if (/^\*\*[^*]+\*\*$/.test(l)) {
      out.push(new Paragraph({ children: runs(l), keepNext: true, spacing: { line: 360, before: 160 } }));
    } else if (ref) {
      out.push(new Paragraph({ children: runs(l), spacing: { ...SIMPLES, after: 240 } }));
    } else {
      out.push(corpo(l));
    }
    i++;
  }
  return out;
}

const centro = (t, o = {}) => new Paragraph({ alignment: AlignmentType.CENTER, spacing: { line: 360, ...(o.spacing || {}) }, pageBreakBefore: o.quebra, children: [new TextRun({ text: t, font: FONTE, bold: true, size: 24 })] });

function capa() {
  const TITULO = 'PRESENÇA+: SISTEMA WEB PARA CONTROLE DE FALTAS DE ALUNOS';
  const AUTORA = 'ANA CAROLINA NOLETO DOS SANTOS';
  return [
    centro('UNIVERSIDADE DE CUIABÁ – UNIC'),
    centro('CURSO: ANÁLISE E DESENVOLVIMENTO DE SISTEMAS'),
    centro('DISCIPLINA: PROJETO INTEGRADO'),
    centro(AUTORA, { spacing: { before: 2400 } }),
    centro(TITULO, { spacing: { before: 2400 } }),
    centro('CUIABÁ-MT', { spacing: { before: 3600 } }),
    centro('2026'),
    centro(AUTORA, { quebra: true }),
    centro(TITULO, { spacing: { before: 2400 } }),
    new Paragraph({ alignment: AlignmentType.JUSTIFIED, indent: { left: 4252 }, spacing: { ...SIMPLES, before: 1600 }, children: [new TextRun({ text: 'Relatório técnico apresentado à disciplina de Projeto Integrado da Universidade de Cuiabá – UNIC, como requisito parcial para avaliação.', font: FONTE, size: 22 })] }),
    centro('CUIABÁ-MT', { spacing: { before: 3200 } }),
    centro('2026'),
    centro('SUMÁRIO', { quebra: true, spacing: { after: 240 } }),
    new TableOfContents('Sumário', { hyperlink: true, headingStyleRange: '1-2' }),
  ];
}

async function main() {
  // Junta tudo para numerar figuras e tabelas em sequência no documento inteiro.
  const partes = arquivos.map(n => ({ n, l: fs.readFileSync(path.join(raiz, 'secoes', n + '.md'), 'utf8').replace(/\r/g, '').split('\n') }));
  const todas = partes.flatMap(p => p.l);
  const r = numerar(todas);
  console.log(`Figuras: ${r.fig}, Tabelas: ${r.tab}`);
  r.log.forEach(x => console.log('  ' + x));
  let pos = 0;
  const texto = partes.flatMap(p => {
    const l = todas.slice(pos, pos + p.l.length);
    pos += p.l.length;
    return blocos(l, p.n);
  });

  const titulo = { font: FONTE, size: 24, bold: true, color: '000000' };
  const pagina = { size: { width: 11906, height: 16838 }, margin: { top: 1701, left: 1701, bottom: 1134, right: 1134, header: 1134, footer: 567 } };
  const doc = new Document({
    creator: 'Ana Carolina Noleto dos Santos', title: 'Presença+: sistema web para controle de faltas de alunos',
    styles: {
      default: {
        document: { run: { font: FONTE, size: 24, language: { value: 'pt-BR' } } },
        heading1: { run: titulo, paragraph: { spacing: { before: 0, after: 360, line: 360 }, keepNext: true } },
        heading2: { run: titulo, paragraph: { spacing: { before: 360, after: 240, line: 360 }, keepNext: true } },
      },
      paragraphStyles: [
        { id: 'TOC1', name: 'toc 1', basedOn: 'Normal', next: 'Normal', run: { font: FONTE, size: 24, bold: true }, paragraph: { spacing: { line: 300, before: 60 } } },
        { id: 'TOC2', name: 'toc 2', basedOn: 'Normal', next: 'Normal', run: { font: FONTE, size: 24 }, paragraph: { spacing: { line: 300 }, indent: { left: 280 } } },
      ],
    },
    numbering: { config: [{ reference: 'topicos', levels: [{ level: 0, format: LevelFormat.BULLET, text: '•', alignment: AlignmentType.LEFT, style: { paragraph: { indent: { left: 709, hanging: 284 } } } }] }] },
    sections: [
      { properties: { page: pagina }, children: capa() },
      {
        properties: { type: SectionType.NEXT_PAGE, page: pagina },
        headers: { default: new Header({ children: [new Paragraph({ alignment: AlignmentType.RIGHT, spacing: SIMPLES, children: [new TextRun({ children: [PageNumber.CURRENT], font: FONTE, size: 20 })] })] }) },
        children: texto,
      },
    ],
  });
  fs.writeFileSync(saida, await Packer.toBuffer(doc));
  console.log('OK', saida);
}
main();
