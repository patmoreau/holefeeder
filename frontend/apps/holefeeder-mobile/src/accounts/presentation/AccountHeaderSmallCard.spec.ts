import { lightTheme } from '@/types/theme/light';
import { createStyles } from './AccountHeaderSmallCard';

describe('AccountHeaderSmallCard styles', () => {
  const styles = createStyles(lightTheme);

  it('should keep the name and balance on one line so the card fits the toolbar row', () => {
    expect(styles.row).not.toHaveProperty('flexWrap');
  });

  it('should shrink the name so a long name truncates', () => {
    expect(styles.name).toMatchObject({ flexShrink: 1 });
  });

  it('should never shrink the balance', () => {
    expect(styles.balance).toMatchObject({ flexShrink: 0 });
  });
});
