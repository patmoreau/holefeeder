import { act, renderHook } from '@testing-library/react-native';
import { useAmountInput } from '@/shared/presentation/components/fields/use-amount-input';

const NO_BREAK_SPACE = '\u00a0';

describe('useAmountInput', () => {
  const createHook = async ({ amount = 0, currentLocale = 'en-CA', onAmountChange = jest.fn() } = {}) =>
    renderHook(() =>
      useAmountInput({
        amount: amount,
        onAmountChange: onAmountChange,
        currentLocale: currentLocale,
        currencyCode: 'CAD',
      })
    );

  describe('initial display', () => {
    it('formats the amount with grouping and two decimals for en-CA', async () => {
      const { result } = await createHook({ amount: 1000 });

      expect(result.current.textAmount.value).toBe('1,000.00');
    });

    it('formats the amount with the locale separators for fr-CA', async () => {
      const { result } = await createHook({
        amount: 1000,
        currentLocale: 'fr-CA',
      });

      expect(result.current.textAmount.value).toBe(`1${NO_BREAK_SPACE}000,00`);
    });

    it('shows zero as 0.00', async () => {
      const { result } = await createHook({ amount: 0 });

      expect(result.current.textAmount.value).toBe('0.00');
    });
  });

  describe('typing', () => {
    it('shifts typed digits in as cents and reports the new amount', async () => {
      const onAmountChange = jest.fn();
      const { result } = await createHook({ onAmountChange: onAmountChange });

      await act(async () => result.current.handleChangeText('12345'));

      expect(result.current.textAmount.value).toBe('123.45');
      expect(onAmountChange).toHaveBeenCalledWith(123.45);
    });

    it('ignores characters that are not digits', async () => {
      const onAmountChange = jest.fn();
      const { result } = await createHook({ onAmountChange: onAmountChange });

      await act(async () => result.current.handleChangeText('1a2'));

      expect(result.current.textAmount.value).toBe('0.12');
      expect(onAmountChange).toHaveBeenCalledWith(0.12);
    });

    it('falls back to 0.00 when the field is cleared', async () => {
      const onAmountChange = jest.fn();
      const { result } = await createHook({
        amount: 12.5,
        onAmountChange: onAmountChange,
      });

      await act(async () => result.current.handleChangeText(''));

      expect(result.current.textAmount.value).toBe('0.00');
      expect(onAmountChange).toHaveBeenCalledWith(0);
    });

    it('does not report a change when the input is already formatted', async () => {
      const onAmountChange = jest.fn();
      const { result } = await createHook({
        amount: 1000,
        onAmountChange: onAmountChange,
      });

      await act(async () => result.current.handleChangeText('1,000.00'));

      expect(onAmountChange).not.toHaveBeenCalled();
    });

    it('moves the caret to the end after reformatting', async () => {
      const { result } = await createHook();

      await act(async () => result.current.handleChangeText('123456'));

      expect(result.current.selection.value).toEqual({ start: 8, end: 8 });
    });
  });
});
