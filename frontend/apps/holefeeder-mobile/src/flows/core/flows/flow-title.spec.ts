import { FlowTitle } from '@/flows/core/flows/flow-title';

describe('FlowTitle', () => {
  describe('title', () => {
    it('uses the description when there is one', () => {
      expect(FlowTitle.title({ description: 'Café Olimpico', categoryName: 'Food' })).toBe('Café Olimpico');
    });

    it('falls back to the category name when the description is empty', () => {
      expect(FlowTitle.title({ description: '', categoryName: 'Food' })).toBe('Food');
    });

    it('falls back to the category name when the description is only spaces', () => {
      expect(FlowTitle.title({ description: '   ', categoryName: 'Food' })).toBe('Food');
    });
  });

  describe('showsCategory', () => {
    it('shows the category under a real description', () => {
      expect(FlowTitle.showsCategory({ description: 'Café Olimpico' })).toBe(true);
    });

    it('hides the category when it is already the title', () => {
      expect(FlowTitle.showsCategory({ description: '  ' })).toBe(false);
    });
  });
});
