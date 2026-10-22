import { SurplusBands } from '@/summary/core/surplus-bands';

const p = (income: number, expenses: number) => ({ income, expenses });

describe('SurplusBands', () => {
  describe('of', () => {
    it('makes one surplus band when income stays above expenses', () => {
      expect(SurplusBands.of([p(4200, 3650), p(4350, 3900), p(4400, 3260)])).toEqual([
        {
          kind: 'surplus',
          points: [
            { x: 0, low: 3650, high: 4200 },
            { x: 1, low: 3900, high: 4350 },
            { x: 2, low: 3260, high: 4400 },
          ],
        },
      ]);
    });

    it('makes one deficit band when expenses stay above income', () => {
      expect(SurplusBands.of([p(100, 300), p(200, 250)])).toEqual([
        {
          kind: 'deficit',
          points: [
            { x: 0, low: 100, high: 300 },
            { x: 1, low: 200, high: 250 },
          ],
        },
      ]);
    });

    it('splits the band where the lines cross between two periods', () => {
      const bands = SurplusBands.of([p(4200, 3650), p(4200, 4520)]);

      expect(bands.map((band) => band.kind)).toEqual(['surplus', 'deficit']);
      const crossing = bands[0].points[1];
      expect(crossing.x).toBeCloseTo(550 / 870);
      expect(crossing.low).toBeCloseTo(4200);
      expect(crossing.high).toBeCloseTo(4200);
      expect(bands[1].points[0]).toEqual(crossing);
      expect(bands[1].points[1]).toEqual({ x: 1, low: 4200, high: 4520 });
    });

    it('alternates bands in order over several crossings', () => {
      const months = [p(4200, 3650), p(4200, 4520), p(4350, 3900), p(4200, 3700), p(4200, 4780), p(4400, 3260)];

      expect(SurplusBands.of(months).map((band) => band.kind)).toEqual(['surplus', 'deficit', 'surplus', 'deficit', 'surplus']);
    });

    it('closes a band where income equals expenses, without a zero-height band', () => {
      expect(SurplusBands.of([p(200, 100), p(150, 150), p(150, 150), p(100, 200)])).toEqual([
        {
          kind: 'surplus',
          points: [
            { x: 0, low: 100, high: 200 },
            { x: 1, low: 150, high: 150 },
          ],
        },
        {
          kind: 'deficit',
          points: [
            { x: 2, low: 150, high: 150 },
            { x: 3, low: 100, high: 200 },
          ],
        },
      ]);
    });

    it('has no bands for a single period or none', () => {
      expect(SurplusBands.of([p(4200, 3650)])).toEqual([]);
      expect(SurplusBands.of([])).toEqual([]);
    });

    it('keeps one band when the lines touch without crossing', () => {
      const bands = SurplusBands.of([p(200, 100), p(150, 150), p(200, 100)]);

      expect(bands).toHaveLength(1);
      expect(bands[0].kind).toBe('surplus');
      expect(bands[0].points.map((point) => point.x)).toEqual([0, 1, 2]);
    });
  });
});
