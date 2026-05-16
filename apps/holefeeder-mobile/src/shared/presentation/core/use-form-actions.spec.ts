import { renderHook, waitFor } from '@testing-library/react-native';
import { FormContext, useFormActions } from '@/shared/presentation/core/use-form-actions';
import * as navigation from '@/shared/presentation/navigation';

// Mock dependencies
jest.mock('@/shared/presentation/navigation', () => ({
  goBack: jest.fn(),
}));

jest.mock('react-i18next', () => ({
  useTranslation: () => ({
    t: (key: string) => key,
  }),
}));

const mockShowDiscardAlert = jest.fn();
const mockShowFormErrorAlert = jest.fn();

jest.mock('@/shared/presentation/show-alert', () => ({
  showAlert: jest.fn(() => ({
    showDiscardAlert: mockShowDiscardAlert,
    showFormErrorAlert: mockShowFormErrorAlert,
  })),
}));

describe('useFormActions', () => {
  const mockSaveForm = jest.fn();
  const mockGoBack = navigation.goBack as jest.Mock;

  beforeEach(() => {
    jest.clearAllMocks();
  });

  describe('handleSave', () => {
    it('should call goBack immediately when form is not dirty', async () => {
      const { result } = renderHook(() =>
        useFormActions({
          saveForm: mockSaveForm,
          isDirty: false,
          errors: {},
        })
      );

      await result.current.handleSave();

      expect(mockSaveForm).not.toHaveBeenCalled();
      expect(mockGoBack).toHaveBeenCalledTimes(1);
    });

    it('should save and navigate back when form is dirty and save succeeds', async () => {
      mockSaveForm.mockResolvedValue(true);

      const { result } = renderHook(() =>
        useFormActions({
          saveForm: mockSaveForm,
          isDirty: true,
          errors: {},
        })
      );

      await result.current.handleSave();

      await waitFor(() => {
        expect(mockSaveForm).toHaveBeenCalledTimes(1);
        expect(mockGoBack).toHaveBeenCalledTimes(1);
      });
    });

    it('should show error alert when save fails with validation errors', async () => {
      mockSaveForm.mockResolvedValue(false);
      const errors = { name: 'Name is required', email: 'Email is invalid' };

      const { result } = renderHook(() =>
        useFormActions({
          saveForm: mockSaveForm,
          isDirty: true,
          errors,
        })
      );

      await result.current.handleSave();

      await waitFor(() => {
        expect(mockSaveForm).toHaveBeenCalledTimes(1);
        expect(mockShowFormErrorAlert).toHaveBeenCalledWith({
          errors,
          errorCount: 2,
        });
        expect(mockGoBack).not.toHaveBeenCalled();
      });
    });

    it('should not show error alert when save fails but no validation errors exist', async () => {
      mockSaveForm.mockResolvedValue(false);

      const { result } = renderHook(() =>
        useFormActions({
          saveForm: mockSaveForm,
          isDirty: true,
          errors: {},
        })
      );

      await result.current.handleSave();

      await waitFor(() => {
        expect(mockSaveForm).toHaveBeenCalledTimes(1);
        expect(mockShowFormErrorAlert).not.toHaveBeenCalled();
        expect(mockGoBack).not.toHaveBeenCalled();
      });
    });
  });

  describe('handleCancel', () => {
    it('should call goBack immediately when form is not dirty', () => {
      const { result } = renderHook(() =>
        useFormActions({
          saveForm: mockSaveForm,
          isDirty: false,
          errors: {},
        })
      );

      result.current.handleCancel();

      expect(mockShowDiscardAlert).not.toHaveBeenCalled();
      expect(mockGoBack).toHaveBeenCalledTimes(1);
    });

    it('should show discard alert when form is dirty', () => {
      const { result } = renderHook(() =>
        useFormActions({
          saveForm: mockSaveForm,
          isDirty: true,
          errors: {},
        })
      );

      result.current.handleCancel();

      expect(mockShowDiscardAlert).toHaveBeenCalledWith({
        onConfirm: mockGoBack,
      });
      expect(mockGoBack).not.toHaveBeenCalled();
    });

    it('should call goBack when user confirms discard', () => {
      const { result } = renderHook(() =>
        useFormActions({
          saveForm: mockSaveForm,
          isDirty: true,
          errors: {},
        })
      );

      result.current.handleCancel();

      // Simulate user confirming the discard alert
      const onConfirm = mockShowDiscardAlert.mock.calls[0][0].onConfirm;
      onConfirm();

      expect(mockGoBack).toHaveBeenCalledTimes(1);
    });
  });

  describe('dependency updates', () => {
    it('should update handleSave when dependencies change', async () => {
      const { result, rerender } = renderHook((props: FormContext) => useFormActions(props), {
        initialProps: {
          saveForm: mockSaveForm,
          isDirty: false,
          errors: {},
        },
      });

      await result.current.handleSave();
      expect(mockSaveForm).not.toHaveBeenCalled();

      // Change isDirty to true
      mockSaveForm.mockResolvedValue(true);
      rerender({
        saveForm: mockSaveForm,
        isDirty: true,
        errors: {},
      });

      await result.current.handleSave();
      await waitFor(() => {
        expect(mockSaveForm).toHaveBeenCalledTimes(1);
      });
    });
  });
});
