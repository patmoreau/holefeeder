import { componentSizes } from '@/types/theme/design-tokens';
import { minTouchTargetModifier } from './AppListItem';

describe('minTouchTargetModifier', () => {
  it('should floor the row at the platform minimum touch target', () => {
    expect(minTouchTargetModifier()).toMatchObject({ minHeight: componentSizes.minTouchTarget });
  });

  it('should floor the row at the 44pt iOS Human Interface Guidelines minimum', () => {
    expect(minTouchTargetModifier()).toMatchObject({ minHeight: 44 });
  });

  it('should not pin the row to a fixed height', () => {
    expect(minTouchTargetModifier()).not.toHaveProperty('height');
  });
});
