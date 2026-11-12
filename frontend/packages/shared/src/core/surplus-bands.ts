export const SurplusBandKinds = {
  surplus: 'surplus',
  deficit: 'deficit',
} as const;

export type SurplusBandKind = (typeof SurplusBandKinds)[keyof typeof SurplusBandKinds];

export type BandPoint = { x: number; low: number; high: number };

export type SurplusBand = { kind: SurplusBandKind; points: BandPoint[] };

type Vertex = BandPoint & { difference: number };

const vertexAt = (x: number, income: number, expenses: number): Vertex => ({
  x,
  low: Math.min(income, expenses),
  high: Math.max(income, expenses),
  difference: income - expenses,
});

const withCrossings = (points: { income: number; expenses: number }[]): Vertex[] =>
  points.flatMap((point, index) => {
    const vertex = vertexAt(index, point.income, point.expenses);
    const next = points[index + 1];
    if (!next) return [vertex];

    const nextDifference = next.income - next.expenses;
    if (vertex.difference * nextDifference >= 0) return [vertex];

    const t = vertex.difference / (vertex.difference - nextDifference);
    const income = point.income + (next.income - point.income) * t;
    return [vertex, vertexAt(index + t, income, income)];
  });

const kindOf = (from: Vertex, to: Vertex): SurplusBandKind | null => {
  if (from.difference > 0 || to.difference > 0) return SurplusBandKinds.surplus;
  if (from.difference < 0 || to.difference < 0) return SurplusBandKinds.deficit;
  return null;
};

const toPoint = ({ x, low, high }: Vertex): BandPoint => ({ x, low, high });

const of = (points: { income: number; expenses: number }[]): SurplusBand[] => {
  const vertices = withCrossings(points);
  const bands: SurplusBand[] = [];

  vertices.slice(1).forEach((to, index) => {
    const from = vertices[index];
    const kind = kindOf(from, to);
    if (!kind) return;

    const current = bands[bands.length - 1];
    const lastPoint = current?.points[current.points.length - 1];
    if (current && current.kind === kind && lastPoint?.x === from.x) {
      current.points.push(toPoint(to));
    } else {
      bands.push({ kind, points: [toPoint(from), toPoint(to)] });
    }
  });

  return bands;
};

export const SurplusBands = {
  of: of,
};
